﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartyWorkerDelegate.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   PartyWorkerDelegate.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Delegates {
    using System.Collections.Concurrent;

    using Sharlayan.Core;

    internal static class PartyWorkerDelegate {
        public static ConcurrentDictionary<uint, PartyMember> PartyMembers = new ConcurrentDictionary<uint, PartyMember>();

        public static void EnsurePartyMember(uint key, PartyMember entity) {
            PartyMembers.AddOrUpdate(key, entity, (k, v) => entity);
        }

        public static PartyMember GetPartyMember(uint key) {
            PartyMember entity;
            PartyMembers.TryGetValue(key, out entity);
            return entity;
        }

        public static bool RemovePartyMember(uint key) {
            PartyMember entity;
            return PartyMembers.TryRemove(key, out entity);
        }
    }
}