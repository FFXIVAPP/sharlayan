// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharlayanMemoryManager.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   SharlayanMemoryManager.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public class SharlayanMemoryManager {
        private static Lazy<SharlayanMemoryManager> _instance = new Lazy<SharlayanMemoryManager>(() => new SharlayanMemoryManager());

        internal ConcurrentDictionary<int, MemoryHandler> _memoryHandlers = new ConcurrentDictionary<int, MemoryHandler>();

        public static SharlayanMemoryManager Instance => _instance.Value;

        public MemoryHandler AddHandler(SharlayanConfiguration configuration) {
            MemoryHandler memoryHandler = new MemoryHandler(configuration);
            return this._memoryHandlers.AddOrUpdate(configuration.ProcessModel.ProcessID, memoryHandler, (k, v) => memoryHandler);
        }

        public MemoryHandler GetHandler(int processID) {
            if (this._memoryHandlers.TryGetValue(processID, out MemoryHandler memoryHandler)) {
                // FOUND
            }

            return memoryHandler;
        }

        public ICollection<MemoryHandler> GetHandlers() {
            return Instance._memoryHandlers.Values;
        }

        public bool RemoveHandler(int processID) {
            if (!this._memoryHandlers.TryRemove(processID, out MemoryHandler removedHandler)) {
                return false;
            }

            removedHandler.Dispose();
            return true;
        }
    }
}