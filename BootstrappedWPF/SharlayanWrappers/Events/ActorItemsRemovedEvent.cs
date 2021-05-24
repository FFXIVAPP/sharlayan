namespace BootstrappedWPF.SharlayanWrappers.Events {
    using System.Collections.Concurrent;

    using Sharlayan;
    using Sharlayan.Core;

    public class ActorItemsRemovedEvent : SharlayanDataEvent<ConcurrentDictionary<uint, ActorItem>> {
        public ActorItemsRemovedEvent(object sender, MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) : base(sender, memoryHandler, eventData) { }
    }
}