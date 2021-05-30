// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatLogReader.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ChatLogReader.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

        public IEnumerable<List<byte>> ResolveEntries(int offset, int length) {
            List<List<byte>> entries = new List<List<byte>>();

            this.EnsureArrayIndexes();

            for (int i = offset; i < length; i++) {
                int currentOffset = this.Indexes[i];

                List<byte> entry = this.ResolveEntry(this.PreviousOffset, currentOffset);
                if (entry.Any()) {
                    entries.Add(entry);
                }

                this.PreviousOffset = currentOffset;
            }

            return entries;
        }

        private List<byte> ResolveEntry(int offset, int length) {
            List<byte> result = new List<byte>();

            int size = length - offset;

            if (size == 0) {
                return result;
            }

            byte[] buffer = this._memoryHandler.BufferPool.Rent(size);

            try {
                this._memoryHandler.GetByteArray(new IntPtr(this.ChatLogPointers.LogStart + offset), buffer);
                for (int i = 0; i < size; i++) {
                    result.Add(buffer[i]);
                }
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }
            finally {
                this._memoryHandler.BufferPool.Return(buffer);
            }

            return result;
        }
    }
}