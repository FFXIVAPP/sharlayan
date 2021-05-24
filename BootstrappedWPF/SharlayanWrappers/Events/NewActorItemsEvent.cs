namespace BootstrappedWPF.SharlayanWrappers.Events {
    using System.Collections.Concurrent;

    using Sharlayan;
    using Sharlayan.Core;

    public class NewActorItemsEvent : SharlayanDataEvent<ConcurrentDictionary<uint, ActorItem>> {
        public NewActorItemsEvent(object sender, MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) : base(sender, memoryHandler, eventData) { }
    }
}