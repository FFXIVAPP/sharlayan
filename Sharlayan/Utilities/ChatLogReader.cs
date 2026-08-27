// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatLogReader.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ChatLogReader.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System;
    using System.Collections.Generic;

    using NLog;

    using Sharlayan.Models;

    internal class ChatLogReader {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public readonly List<int> Indexes = new List<int>();

        private int BUFFER_SIZE = 4000;

        public bool ChatLogFirstRun = true;

        public ChatLogPointers ChatLogPointers;

        public int PreviousArrayIndex;

        public int PreviousOffset;

        public ChatLogReader(MemoryHandler memoryHandler) {
            this._memoryHandler = memoryHandler;
        }

        private MemoryHandler _memoryHandler { get; }

        public void EnsureArrayIndexes() {
            this.Indexes.Clear();

            byte[] buffer = this._memoryHandler.BufferPool.Rent(this.BUFFER_SIZE);

            try {
                this._memoryHandler.GetByteArray(new IntPtr(this.ChatLogPointers.OffsetArrayStart), buffer);
                for (int i = 0; i < this.BUFFER_SIZE; i += 4) {
                    this.Indexes.Add(BitConverter.ToInt32(buffer, i));
                }
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }
            finally {
                this._memoryHandler.BufferPool.Return(buffer);
            }
        }

        // F90: hard ceiling on a single chat entry. offset/length both come from int32s
        // read out of live game memory, so a torn read can make (length - offset) huge —
        // this runs in an elevated process, and the unclamped `new byte[size]` could
        // allocate up to ~2GB from one bad frame. Real entries are far below 16KB.
        private const int MaxEntrySize = 0x4000;

        private static readonly byte[] EmptyEntry = new byte[0];

        public IEnumerable<byte[]> ResolveEntries(int offset, int length) {
            List<byte[]> entries = new List<byte[]>();

            // F90: clamp to the actual index window — callers derive both bounds from
            // non-atomic reads of mutating memory, and a torn pair previously indexed
            // past Indexes.Count and aborted the whole batch mid-walk.
            if (offset < 0) {
                offset = 0;
            }

            if (length > this.Indexes.Count) {
                length = this.Indexes.Count;
            }

            for (int i = offset; i < length; i++) {
                int currentOffset = this.Indexes[i];

                byte[] entry = this.ResolveEntry(this.PreviousOffset, currentOffset);
                if (entry.Length > 0) {
                    entries.Add(entry);
                }

                this.PreviousOffset = currentOffset;
            }

            return entries;
        }

        private byte[] ResolveEntry(int offset, int length) {
            int size = length - offset;

            // F90: negative (ring wraparound / stale index) or absurd sizes are torn
            // data, not entries — skip rather than throw or allocate huge.
            if (size <= 0 || size > MaxEntrySize) {
                return EmptyEntry;
            }

            byte[] result = new byte[size];

            try {
                this._memoryHandler.GetByteArray(new IntPtr(this.ChatLogPointers.LogStart + offset), result);
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }

            return result;
        }
    }
}