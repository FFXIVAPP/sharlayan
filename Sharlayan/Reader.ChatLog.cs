// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.ChatLog.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
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
        private const string UNRESOLVED = "UNRESOLVED";

        private readonly object _chatLogLock = new object();

        // F91: precompiled like KeyBindsRegex — previously an inline static
        // Regex.IsMatch per chat line.
        private static readonly Regex ChatCodeRegex = new Regex(@"[\w\d]{4}::?.+", RegexOptions.Compiled);

        private List<byte[]> _chatLogBufferList = new List<byte[]>();

        public bool CanGetChatLog() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.CHATLOG_KEY);
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

            lock (this._chatLogLock) {
            this._chatLogReader.PreviousArrayIndex = previousArrayIndex;
            this._chatLogReader.PreviousOffset = previousOffset;

            IntPtr chatPointerAddress = this._memoryHandler.Scanner.Locations[Signatures.CHATLOG_KEY];

            if (chatPointerAddress.ToInt64() <= 20) {
                return result;
            }

            this._chatLogBufferList.Clear();

            try {
                // F92 (was 7 scalar syscalls): LineCount sits at offset 0 and both
                // StdVector triplets are small fixed offsets into the same LogModule —
                // one bulk read covers the whole span, and the pointer set is captured
                // from a single moment instead of 7 reads of mutating memory.
                Models.Structures.ChatLogPointers pointerOffsets = this._memoryHandler.Structures.ChatLogPointers;
                int pointersSpan = (int) Math.Max(pointerOffsets.OffsetArrayEnd, pointerOffsets.LogEnd) + 8;
                byte[] pointersMap = this._memoryHandler.BufferPool.Rent(pointersSpan);
                try {
                    if (!this._memoryHandler.Peek(chatPointerAddress, pointersMap, pointersSpan)) {
                        // Failed read: zeroed pointers -> currentArrayIndex 0 -> no entries this poll.
                        this._chatLogReader.ChatLogPointers = new ChatLogPointers();
                    }
                    else {
                        this._chatLogReader.ChatLogPointers = new ChatLogPointers {
                            LineCount = SharlayanBitConverter.TryToUInt32(pointersMap, 0),
                            OffsetArrayStart = SharlayanBitConverter.TryToInt64(pointersMap, (int) pointerOffsets.OffsetArrayStart),
                            OffsetArrayPos = SharlayanBitConverter.TryToInt64(pointersMap, (int) pointerOffsets.OffsetArrayPos),
                            OffsetArrayEnd = SharlayanBitConverter.TryToInt64(pointersMap, (int) pointerOffsets.OffsetArrayEnd),
                            LogStart = SharlayanBitConverter.TryToInt64(pointersMap, (int) pointerOffsets.LogStart),
                            LogNext = SharlayanBitConverter.TryToInt64(pointersMap, (int) pointerOffsets.LogNext),
                            LogEnd = SharlayanBitConverter.TryToInt64(pointersMap, (int) pointerOffsets.LogEnd),
                        };
                    }
                }
                finally {
                    this._memoryHandler.BufferPool.Return(pointersMap);
                }

                long currentArrayIndex = (this._chatLogReader.ChatLogPointers.OffsetArrayPos - this._chatLogReader.ChatLogPointers.OffsetArrayStart) / 4;
                if (currentArrayIndex > 0) {
                    if (this._chatLogReader.ChatLogFirstRun) {
                        this._chatLogReader.EnsureArrayIndexes();
                        this._chatLogReader.ChatLogFirstRun = false;
                        // F90: the two pointer fields are read non-atomically from live
                        // memory — clamp before indexing the fixed 1000-entry table (an
                        // out-of-range index previously threw AND left ChatLogFirstRun
                        // false, so first-run initialization never retried properly).
                        if (currentArrayIndex > this._chatLogReader.Indexes.Count) {
                            currentArrayIndex = this._chatLogReader.Indexes.Count;
                        }

                        this._chatLogReader.PreviousOffset = this._chatLogReader.Indexes[(int) currentArrayIndex - 1];
                        this._chatLogReader.PreviousArrayIndex = (int) currentArrayIndex - 1;
                    }
                    else {
                        this._chatLogReader.EnsureArrayIndexes();
                        if (currentArrayIndex < this._chatLogReader.PreviousArrayIndex) {
                            IEnumerable<byte[]> bufferEntries = this._chatLogReader.ResolveEntries(this._chatLogReader.PreviousArrayIndex, 1000);
                            this._chatLogBufferList.AddRange(bufferEntries);
                            this._chatLogReader.PreviousOffset = 0;
                            this._chatLogReader.PreviousArrayIndex = 0;
                        }

                        if (this._chatLogReader.PreviousArrayIndex < currentArrayIndex) {
                            IEnumerable<byte[]> bufferEntries = this._chatLogReader.ResolveEntries(this._chatLogReader.PreviousArrayIndex, (int) currentArrayIndex);
                            this._chatLogBufferList.AddRange(bufferEntries);
                        }

                        this._chatLogReader.PreviousArrayIndex = (int) currentArrayIndex;
                    }
                }


            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }

            foreach (byte[] bytes in this._chatLogBufferList) {
                if (bytes.Length == 0) {
                    continue;
                }

                try {
                    ChatLogItem chatLogEntry = ChatEntry.Process(bytes);

                    // assign logged user for this instance to chatLogEntry
                    chatLogEntry.PlayerCharacterName = this._pcWorkerDelegate.CurrentUser?.Name ?? UNRESOLVED;

                    if (ChatCodeRegex.IsMatch(chatLogEntry.Combined)) {
                        result.ChatLogItems.Enqueue(chatLogEntry);
                    }
                }
                catch (Exception ex) {
                    this._memoryHandler.RaiseException(Logger, ex);
                }
            }

            result.PreviousArrayIndex = this._chatLogReader.PreviousArrayIndex;
            result.PreviousOffset = this._chatLogReader.PreviousOffset;

            return result;
            } // end lock
        }
    }
}