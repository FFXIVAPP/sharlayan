namespace BootstrappedWPF.SharlayanWrappers.Events {
    using System.Collections.Concurrent;

    using Sharlayan;
    using Sharlayan.Core;

    public class ActorItemsAddedEvent : SharlayanDataEvent<ConcurrentDictionary<uint, ActorItem>> {
        public ActorItemsAddedEvent(object sender, MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) : base(sender, memoryHandler, eventData) { }
    }
}