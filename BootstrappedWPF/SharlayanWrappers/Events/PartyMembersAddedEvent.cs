namespace BootstrappedWPF.SharlayanWrappers.Events {
    using System.Collections.Concurrent;

    using Sharlayan;
    using Sharlayan.Core;

    public class PartyMembersAddedEvent : SharlayanDataEvent<ConcurrentDictionary<uint, PartyMember>> {
        public PartyMembersAddedEvent(object sender, MemoryHandler memoryHandler, ConcurrentDictionary<uint, PartyMember> eventData) : base(sender, memoryHandler, eventData) { }
    }
}