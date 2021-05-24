namespace BootstrappedWPF.SharlayanWrappers.Events {
    using Sharlayan;
    using Sharlayan.Core;

    public class NewTargetInfoEvent : SharlayanDataEvent<TargetInfo> {
        public NewTargetInfoEvent(object sender, MemoryHandler memoryHandler, TargetInfo eventData) : base(sender, memoryHandler, eventData) { }
    }
}