namespace BootstrappedWPF.SharlayanWrappers.Events {
    using System.Collections.Concurrent;

    using Sharlayan;
    using Sharlayan.Core;

    public class NewInventoryContainersEvent : SharlayanDataEvent<ConcurrentBag<InventoryContainer>> {
        public NewInventoryContainersEvent(object sender, MemoryHandler memoryHandler, ConcurrentBag<InventoryContainer> eventData) : base(sender, memoryHandler, eventData) { }
    }
}