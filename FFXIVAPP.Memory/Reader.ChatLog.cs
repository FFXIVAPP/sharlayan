// FFXIVAPP.Memory
// Reader.ChatLog.cs
// 
// Copyright © 2007 - 2015 Ryan Wilson - All Rights Reserved
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions are met: 
// 
//  * Redistributions of source code must retain the above copyright notice, 
//    this list of conditions and the following disclaimer. 
//  * Redistributions in binary form must reproduce the above copyright 
//    notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution. 
//  * Neither the name of SyndicatedLife nor the names of its contributors may 
//    be used to endorse or promote products derived from this software 
//    without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FFXIVAPP.Memory.Core;
using FFXIVAPP.Memory.Models;

namespace FFXIVAPP.Memory
{
    public class ChatLogReadResult
    {
        public ChatLogReadResult()
        {
            ChatLogEntries = new List<ChatLogEntry>();
        }

        public List<ChatLogEntry> ChatLogEntries { get; set; }
        public int PreviousArrayIndex { get; set; }
        public int PreviousOffset { get; set; }
    }

    public static partial class Reader
    {
        private static ChatLogPointers ChatLogPointers;
        private static int PreviousArrayIndex;
        private static int PreviousOffset;
        private static readonly List<int> Indexes = new List<int>();
        private static bool ChatLogFirstRun = true;

        private static void EnsureArrayIndexes()
        {
            Indexes.Clear();
            for (var i = 0; i < 1000; i++)
            {
                Indexes.Add((int) MemoryHandler.Instance.GetPlatformUInt(new IntPtr(ChatLogPointers.OffsetArrayStart + (i * 4))));
            }
        }

        private static IEnumerable<List<byte>> ResolveEntries(int offset, int length)
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

        public static ChatLogReadResult GetChatLog(int previousArrayIndex = 0, int previousOffset = 0)
        {
            var result = new ChatLogReadResult();

            PreviousArrayIndex = previousArrayIndex;
            PreviousOffset = previousOffset;

            if (Scanner.Instance.Locations.ContainsKey("CHATLOG"))
            {
                var chatPointerMap = (IntPtr) Scanner.Instance.Locations["CHATLOG"];

                if (chatPointerMap.ToInt64() <= 20)
                {
                    return result;
                }

                var buffered = new List<List<byte>>();

                try
                {
                    Indexes.Clear();
                    switch (MemoryHandler.Instance.GameLanguage)
                    {
                        case "Korean":
                            ChatLogPointers = new ChatLogPointers
                            {
                                LineCount = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap),
                                OffsetArrayStart = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x20),
                                OffsetArrayPos = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x24),
                                OffsetArrayEnd = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x28),
                                LogStart = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x30),
                                LogNext = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x34),
                                LogEnd = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x38)
                            };
                        default:
                            if (MemoryHandler.Instance.ProcessModel.IsWin64)
                            {
                                ChatLogPointers = new ChatLogPointers
                                {
                                    LineCount = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap),
                                    OffsetArrayStart = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x3C),
                                    OffsetArrayPos = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x44),
                                    OffsetArrayEnd = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x4C),
                                    LogStart = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x5C),
                                    LogNext = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x64),
                                    LogEnd = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x6C)
                                };
                            }
                            else
                            {
                                ChatLogPointers = new ChatLogPointers
                                {
                                    LineCount = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap),
                                    OffsetArrayStart = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x24),
                                    OffsetArrayPos = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x28),
                                    OffsetArrayEnd = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x2C),
                                    LogStart = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x34),
                                    LogNext = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x38),
                                    LogEnd = (uint)MemoryHandler.Instance.GetPlatformUInt(chatPointerMap, 0x3C)
                                };
                            }
                            break;
                    }
                    

                    EnsureArrayIndexes();

                    var currentArrayIndex = (ChatLogPointers.OffsetArrayPos - ChatLogPointers.OffsetArrayStart) / 4;

                    if (ChatLogFirstRun)
                    {
                        ChatLogFirstRun = false;
                        PreviousOffset = Indexes[(int) currentArrayIndex - 1];
                        PreviousArrayIndex = (int) currentArrayIndex - 1;
                    }
                    else
                    {
                        if (currentArrayIndex < PreviousArrayIndex)
                        {
                            buffered.AddRange(ResolveEntries(PreviousArrayIndex, 1000));
                            PreviousOffset = 0;
                            PreviousArrayIndex = 0;
                        }
                        if (PreviousArrayIndex < currentArrayIndex)
                        {
                            buffered.AddRange(ResolveEntries(PreviousArrayIndex, (int) currentArrayIndex));
                        }
                        PreviousArrayIndex = (int) currentArrayIndex;
                    }
                }
                catch (Exception ex)
                {
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
                    }
                }
            }

            result.PreviousArrayIndex = PreviousArrayIndex;
            result.PreviousOffset = PreviousOffset;

            return result;
        }
    }
}
