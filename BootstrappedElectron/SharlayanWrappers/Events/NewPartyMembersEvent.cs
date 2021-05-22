// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewPartyMembersEvent.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   NewPartyMembersEvent.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedElectron.SharlayanWrappers.Events {
    using System.Collections.Concurrent;

    using Sharlayan;
    using Sharlayan.Core;

    public class NewPartyMembersEvent : SharlayanDataEvent<ConcurrentDictionary<uint, PartyMember>> {
        public NewPartyMembersEvent(object sender, MemoryHandler memoryHandler, ConcurrentDictionary<uint, PartyMember> eventData) : base(sender, memoryHandler, eventData) { }
    }
}