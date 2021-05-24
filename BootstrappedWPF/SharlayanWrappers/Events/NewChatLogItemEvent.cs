namespace BootstrappedWPF.SharlayanWrappers.Events {
    using Sharlayan;
    using Sharlayan.Core;

    public class NewChatLogItemEvent : SharlayanDataEvent<ChatLogItem> {
        public NewChatLogItemEvent(object sender, MemoryHandler memoryHandler, ChatLogItem eventData) : base(sender, memoryHandler, eventData) { }
    }
}