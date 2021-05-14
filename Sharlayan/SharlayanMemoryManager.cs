// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharlayanMemoryManager.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
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
    using System.Linq;

    using Sharlayan.Models;

    public class SharlayanMemoryManager {
        private static Lazy<SharlayanMemoryManager> _instance = new Lazy<SharlayanMemoryManager>(() => new SharlayanMemoryManager());

        internal ConcurrentDictionary<int, MemoryHandler> _memoryHandlers = new ConcurrentDictionary<int, MemoryHandler>();

        public static SharlayanMemoryManager Instance => _instance.Value;

        public MemoryHandler AddHandler(MemoryHandlerConfiguration configuration) {
            MemoryHandler memoryHandler = new MemoryHandler(configuration);
            return this._memoryHandlers.AddOrUpdate(configuration.ProcessModel.ProcessID, memoryHandler, (k, v) => memoryHandler);
        }

        public MemoryHandler GetHandler(ProcessModel processModel) {
            if (this._memoryHandlers.TryGetValue(processModel.ProcessID, out MemoryHandler memoryHandler)) {
                // FOUND
            }

            return memoryHandler;
        }

        public List<MemoryHandler> GetHandlers() {
            return Instance._memoryHandlers.Values.ToList();
        }

        public bool RemoveHandler(ProcessModel processModel) {
            return this._memoryHandlers.TryRemove(processModel.ProcessID, out MemoryHandler removedHandler);
        }
    }
}