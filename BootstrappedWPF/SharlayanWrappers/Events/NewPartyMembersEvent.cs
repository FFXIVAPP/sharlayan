namespace BootstrappedWPF.SharlayanWrappers.Events {
    using System.Collections.Concurrent;

    using Sharlayan;
    using Sharlayan.Core;

    public class NewPartyMembersEvent : SharlayanDataEvent<ConcurrentDictionary<uint, PartyMember>> {
        public NewPartyMembersEvent(object sender, MemoryHandler memoryHandler, ConcurrentDictionary<uint, PartyMember> eventData) : base(sender, memoryHandler, eventData) { }
    }
}