// Sharlayan ~ Reader.ChatLog.cs
// 
// Copyright © 2007 - 2017 Ryan Wilson - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using Sharlayan.Core;
using Sharlayan.Models;

namespace Sharlayan
{
    public static partial class Reader
    {
        private static class ChatLogReader
        {
            public static ChatLogPointers ChatLogPointers;
            public static int PreviousArrayIndex;
            public static int PreviousOffset;
            public static readonly List<int> Indexes = new List<int>();
            public static bool ChatLogFirstRun = true;

            public static void EnsureArrayIndexes()
            {
                Indexes.Clear();
                for (var i = 0; i < 1000; i++)
                {
                    Indexes.Add((int)MemoryHandler.Instance.GetPlatformUInt(new IntPtr(ChatLogPointers.OffsetArrayStart + i * 4)));
                }
            }

            public static IEnumerable<List<byte>> ResolveEntries(int offset, int length)
            {
                var entries = new List<List<byte>>();
                for (var i = offset; i < length; i++)
                {
                    EnsureArrayIndexes();
                    var currentOffset = Indexes[i];
                    entries.Add(ResolveEntry(PreviousOffset, currentOffset));
                    PreviousOffset = currentOffset;
                }
                return entries;
            }

            private static List<byte> ResolveEntry(int offset, int length)
            {
                return new List<byte>(MemoryHandler.Instance.GetByteArray(new IntPtr(ChatLogPointers.LogStart + offset), length - offset));
            }
        }

        public static bool CanGetChatLog()
        {
            var canRead = Scanner.Instance.Locations.ContainsKey(Signatures.ChatLogKey);
            if (canRead)
            {
                // OTHER STUFF?
            }
            return canRead;
        }

        public static ChatLogReadResult GetChatLog(int previousArrayIndex = 0, int previousOffset = 0)
        {
            var result = new ChatLogReadResult();

            if (!CanGetChatLog())
            {
                return result;
            }

            ChatLogReader.PreviousArrayIndex = previousArrayIndex;
            ChatLogReader.PreviousOffset = previousOffset;

            var chatPointerMap = (IntPtr) Scanner.Instance.Locations[Signatures.ChatLogKey];

            if (chatPointerMap.ToInt64() <= 20)
            {
                return result;
            }

            var buffered = new List<List<byte>>();

            try
            {
                ChatLogReader.Indexes.Clear();
                ChatLogReader.ChatLogPointers = new ChatLogPointers
                {
                    LineCount = (uint) MemoryHandler.Instance.GetPlatformUInt(chatPointerMap),
                    OffsetArrayStart = MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, MemoryHandler.Instance.Structures.ChatLogPointers.OffsetArrayStart),
                    OffsetArrayPos = MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, MemoryHandler.Instance.Structures.ChatLogPointers.OffsetArrayPos),
                    OffsetArrayEnd = MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, MemoryHandler.Instance.Structures.ChatLogPointers.OffsetArrayEnd),
                    LogStart = MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, MemoryHandler.Instance.Structures.ChatLogPointers.LogStart),
                    LogNext = MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, MemoryHandler.Instance.Structures.ChatLogPointers.LogNext),
                    LogEnd = MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, MemoryHandler.Instance.Structures.ChatLogPointers.LogEnd)
                };

                ChatLogReader.EnsureArrayIndexes();

                var currentArrayIndex = (ChatLogReader.ChatLogPointers.OffsetArrayPos - ChatLogReader.ChatLogPointers.OffsetArrayStart) / 4;
                if (ChatLogReader.ChatLogFirstRun)
                {
                    ChatLogReader.ChatLogFirstRun = false;
                    ChatLogReader.PreviousOffset = ChatLogReader.Indexes[(int) currentArrayIndex - 1];
                    ChatLogReader.PreviousArrayIndex = (int) currentArrayIndex - 1;
                }
                else
                {
                    if (currentArrayIndex < ChatLogReader.PreviousArrayIndex)
                    {
                        buffered.AddRange(ChatLogReader.ResolveEntries(ChatLogReader.PreviousArrayIndex, 1000));
                        ChatLogReader.PreviousOffset = 0;
                        ChatLogReader.PreviousArrayIndex = 0;
                    }
                    if (ChatLogReader.PreviousArrayIndex < currentArrayIndex)
                    {
                        buffered.AddRange(ChatLogReader.ResolveEntries(ChatLogReader.PreviousArrayIndex, (int) currentArrayIndex));
                    }
                    ChatLogReader.PreviousArrayIndex = (int) currentArrayIndex;
                }
            }
            catch (Exception ex)
            {
                MemoryHandler.Instance.RaiseException(Logger, ex, true);
            }

            foreach (var bytes in buffered.Where(b => b.Count > 0))
            {
                try
                {
                    var chatLogEntry = ChatEntry.Process(bytes.ToArray());
                    if (Regex.IsMatch(chatLogEntry.Combined, @"[\w\d]{4}::?.+"))
                    {
                        result.ChatLogEntries.Add(chatLogEntry);
                    }
                }
                catch (Exception ex)
                {
                    MemoryHandler.Instance.RaiseException(Logger, ex, true);
                }
            }

            result.PreviousArrayIndex = ChatLogReader.PreviousArrayIndex;
            result.PreviousOffset = ChatLogReader.PreviousOffset;

            return result;
        }
    }
}
