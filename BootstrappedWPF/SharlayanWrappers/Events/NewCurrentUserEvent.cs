namespace BootstrappedWPF.SharlayanWrappers.Events {
    using Sharlayan;
    using Sharlayan.Core;

    public class NewCurrentUserEvent : SharlayanDataEvent<ActorItem> {
        public NewCurrentUserEvent(object sender, MemoryHandler memoryHandler, ActorItem eventData) : base(sender, memoryHandler, eventData) { }
    }
}