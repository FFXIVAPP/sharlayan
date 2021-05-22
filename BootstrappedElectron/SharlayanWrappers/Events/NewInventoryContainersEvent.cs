// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewInventoryContainersEvent.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   NewInventoryContainersEvent.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedElectron.SharlayanWrappers.Events {
    using System.Collections.Concurrent;

    using Sharlayan;
    using Sharlayan.Core;

    public class NewInventoryContainersEvent : SharlayanDataEvent<ConcurrentBag<InventoryContainer>> {
        public NewInventoryContainersEvent(object sender, MemoryHandler memoryHandler, ConcurrentBag<InventoryContainer> eventData) : base(sender, memoryHandler, eventData) { }
    }
}