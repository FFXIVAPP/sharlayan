namespace BootstrappedWPF.SharlayanWrappers.Events {
    using Sharlayan;
    using Sharlayan.Core;

    public class NewPlayerInfoEvent : SharlayanDataEvent<PlayerInfo> {
        public NewPlayerInfoEvent(object sender, MemoryHandler memoryHandler, PlayerInfo eventData) : base(sender, memoryHandler, eventData) { }
    }
}