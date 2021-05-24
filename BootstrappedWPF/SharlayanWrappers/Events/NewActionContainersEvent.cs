namespace BootstrappedWPF.SharlayanWrappers.Events {
    using System.Collections.Concurrent;

    using Sharlayan;
    using Sharlayan.Core;

    public class NewActionContainersEvent : SharlayanDataEvent<ConcurrentBag<ActionContainer>> {
        public NewActionContainersEvent(object sender, MemoryHandler memoryHandler, ConcurrentBag<ActionContainer> eventData) : base(sender, memoryHandler, eventData) { }
    }
}