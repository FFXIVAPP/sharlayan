namespace BootstrappedWPF.SharlayanWrappers.Events {
    using System.Collections.Concurrent;

    using Sharlayan;
    using Sharlayan.Core;

    public class PartyMembersRemovedEvent : SharlayanDataEvent<ConcurrentDictionary<uint, PartyMember>> {
        public PartyMembersRemovedEvent(object sender, MemoryHandler memoryHandler, ConcurrentDictionary<uint, PartyMember> eventData) : base(sender, memoryHandler, eventData) { }
    }
}