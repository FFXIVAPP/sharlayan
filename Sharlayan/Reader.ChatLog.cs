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

            IntPtr chatPointerAddress = this._memoryHandler.Scanner.Locations[Signatures.ChatLogKey];

            if (chatPointerAddress.ToInt64() <= 20) {
                return result;
            }

            List<List<byte>> buffered = new List<List<byte>>();

            try {
                this._chatLogReader.ChatLogPointers = new ChatLogPointers {
                    LineCount = this._memoryHandler.GetUInt32(chatPointerAddress),
                    OffsetArrayStart = this._memoryHandler.GetInt64(chatPointerAddress, this._memoryHandler.Structures.ChatLogPointers.OffsetArrayStart),
                    OffsetArrayPos = this._memoryHandler.GetInt64(chatPointerAddress, this._memoryHandler.Structures.ChatLogPointers.OffsetArrayPos),
                    OffsetArrayEnd = this._memoryHandler.GetInt64(chatPointerAddress, this._memoryHandler.Structures.ChatLogPointers.OffsetArrayEnd),
                    LogStart = this._memoryHandler.GetInt64(chatPointerAddress, this._memoryHandler.Structures.ChatLogPointers.LogStart),
                    LogNext = this._memoryHandler.GetInt64(chatPointerAddress, this._memoryHandler.Structures.ChatLogPointers.LogNext),
                    LogEnd = this._memoryHandler.GetInt64(chatPointerAddress, this._memoryHandler.Structures.ChatLogPointers.LogEnd),
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
                this._memoryHandler.RaiseException(Logger, ex);
            }

            foreach (List<byte> bytes in buffered) {
                if (!bytes.Any()) {
                    continue;
                }

                try {
                    ChatLogItem chatLogEntry = ChatEntry.Process(bytes.ToArray());

                    // assign logged user for this instance to chatLogEntry
                    chatLogEntry.PlayerCharacterName = this._pcWorkerDelegate.CurrentUser.Name;

                    if (Regex.IsMatch(chatLogEntry.Combined, @"[\w\d]{4}::?.+")) {
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
        }
    }
}