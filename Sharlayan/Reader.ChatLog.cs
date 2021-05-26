// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.ChatLog.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.ChatLog.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Sharlayan.Core;
    using Sharlayan.Models;
    using Sharlayan.Models.ReadResults;
    using Sharlayan.Utilities;

    public partial class Reader {
        public bool CanGetChatLog() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.ChatLogKey);
            if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        public ChatLogResult GetChatLog(int previousArrayIndex = 0, int previousOffset = 0) {
            ChatLogResult result = new ChatLogResult();

            if (!this.CanGetChatLog() || !this._memoryHandler.IsAttached) {
                return result;
            }

            this._chatLogReader.PreviousArrayIndex = previousArrayIndex;
            this._chatLogReader.PreviousOffset = previousOffset;

            IntPtr chatPointerMap = this._memoryHandler.Scanner.Locations[Signatures.ChatLogKey];

            if (chatPointerMap.ToInt64() <= 20) {
                return result;
            }

            List<List<byte>> buffered = new List<List<byte>>();

            try {
                this._chatLogReader.ChatLogPointers = new ChatLogPointers {
                    LineCount = this._memoryHandler.GetUInt32(chatPointerMap),
                    OffsetArrayStart = this._memoryHandler.GetInt64(chatPointerMap, this._memoryHandler.Structures.ChatLogPointers.OffsetArrayStart),
                    OffsetArrayPos = this._memoryHandler.GetInt64(chatPointerMap, this._memoryHandler.Structures.ChatLogPointers.OffsetArrayPos),
                    OffsetArrayEnd = this._memoryHandler.GetInt64(chatPointerMap, this._memoryHandler.Structures.ChatLogPointers.OffsetArrayEnd),
                    LogStart = this._memoryHandler.GetInt64(chatPointerMap, this._memoryHandler.Structures.ChatLogPointers.LogStart),
                    LogNext = this._memoryHandler.GetInt64(chatPointerMap, this._memoryHandler.Structures.ChatLogPointers.LogNext),
                    LogEnd = this._memoryHandler.GetInt64(chatPointerMap, this._memoryHandler.Structures.ChatLogPointers.LogEnd),
                };

                long currentArrayIndex = (this._chatLogReader.ChatLogPointers.OffsetArrayPos - this._chatLogReader.ChatLogPointers.OffsetArrayStart) / 4;
                if (this._chatLogReader.ChatLogFirstRun) {
                    this._chatLogReader.EnsureArrayIndexes();
                    this._chatLogReader.ChatLogFirstRun = false;
                    this._chatLogReader.PreviousOffset = this._chatLogReader.Indexes[(int) currentArrayIndex - 1];
                    this._chatLogReader.PreviousArrayIndex = (int) currentArrayIndex - 1;
                }
                else {
                    if (currentArrayIndex < this._chatLogReader.PreviousArrayIndex) {
                        buffered.AddRange(this._chatLogReader.ResolveEntries(this._chatLogReader.PreviousArrayIndex, 1000));
                        this._chatLogReader.PreviousOffset = 0;
                        this._chatLogReader.PreviousArrayIndex = 0;
                    }

                    if (this._chatLogReader.PreviousArrayIndex < currentArrayIndex) {
                        buffered.AddRange(this._chatLogReader.ResolveEntries(this._chatLogReader.PreviousArrayIndex, (int) currentArrayIndex));
                    }

                    this._chatLogReader.PreviousArrayIndex = (int) currentArrayIndex;
                }
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex, true);
            }

            foreach (List<byte> bytes in buffered.Where(b => b.Count > 0)) {
                try {
                    ChatLogItem chatLogEntry = ChatEntry.Process(bytes.ToArray());

                    // assign logged user for this instance to chatLogEntry
                    chatLogEntry.PlayerCharacterName = this._pcWorkerDelegate.CurrentUser.Name;

                    if (Regex.IsMatch(chatLogEntry.Combined, @"[\w\d]{4}::?.+")) {
                        result.ChatLogItems.Enqueue(chatLogEntry);
                    }
                }
                catch (Exception ex) {
                    this._memoryHandler.RaiseException(Logger, ex, true);
                }
            }

            result.PreviousArrayIndex = this._chatLogReader.PreviousArrayIndex;
            result.PreviousOffset = this._chatLogReader.PreviousOffset;

            return result;
        }

        private class ChatLogReader {
            public readonly List<int> Indexes = new List<int>();

            private byte[] _indexes;

            private int BUFFER_SIZE = 4000;

            public bool ChatLogFirstRun = true;

            public ChatLogPointers ChatLogPointers;

            public int PreviousArrayIndex;

            public int PreviousOffset;

            public ChatLogReader(MemoryHandler memoryHandler) {
                this._memoryHandler = memoryHandler;
                this._indexes = new byte[this.BUFFER_SIZE];
            }

            private MemoryHandler _memoryHandler { get; }

            public void EnsureArrayIndexes() {
                this.Indexes.Clear();
                this._memoryHandler.GetByteArray(new IntPtr(this.ChatLogPointers.OffsetArrayStart), this._indexes);
                for (int i = 0; i < this.BUFFER_SIZE; i += 4) {
                    this.Indexes.Add(BitConverter.ToInt32(this._indexes, i));
                }
            }

            public IEnumerable<List<byte>> ResolveEntries(int offset, int length) {
                List<List<byte>> entries = new List<List<byte>>();
                this.EnsureArrayIndexes();
                for (int i = offset; i < length; i++) {
                    int currentOffset = this.Indexes[i];
                    entries.Add(this.ResolveEntry(this.PreviousOffset, currentOffset));
                    this.PreviousOffset = currentOffset;
                }

                return entries;
            }

            private List<byte> ResolveEntry(int offset, int length) {
                return new List<byte>(this._memoryHandler.GetByteArray(new IntPtr(this.ChatLogPointers.LogStart + offset), length - offset));
            }
        }
    }
}