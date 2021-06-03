// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartyWorkerDelegate.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   PartyWorkerDelegate.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Delegates {
    using System.Collections.Concurrent;

    using Sharlayan.Core;

    internal class PartyWorkerDelegate {
        public ConcurrentDictionary<uint, PartyMember> PartyMembers = new ConcurrentDictionary<uint, PartyMember>();

        public void EnsurePartyMember(uint key, PartyMember entity) {
            this.PartyMembers.AddOrUpdate(key, entity, (k, v) => entity);
        }

        public PartyMember GetPartyMember(uint key) {
            PartyMember entity;
            this.PartyMembers.TryGetValue(key, out entity);
            return entity;
        }

        public bool RemovePartyMember(uint key) {
            PartyMember entity;
            return this.PartyMembers.TryRemove(key, out entity);
        }
    }
}