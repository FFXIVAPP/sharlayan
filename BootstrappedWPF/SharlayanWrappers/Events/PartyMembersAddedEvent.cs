// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartyMembersAddedEvent.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   PartyMembersAddedEvent.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.SharlayanWrappers.Events {
    using System.Collections.Concurrent;

    using Sharlayan;
    using Sharlayan.Core;

    public class PartyMembersAddedEvent : SharlayanDataEvent<ConcurrentDictionary<uint, PartyMember>> {
        public PartyMembersAddedEvent(object sender, MemoryHandler memoryHandler, ConcurrentDictionary<uint, PartyMember> eventData) : base(sender, memoryHandler, eventData) { }
    }
}