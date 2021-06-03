// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartyResult.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   PartyResult.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System.Collections.Concurrent;

    using Sharlayan.Core;

    public class PartyResult {
        public ConcurrentDictionary<uint, PartyMember> NewPartyMembers { get; internal set; } = new ConcurrentDictionary<uint, PartyMember>();

        public ConcurrentDictionary<uint, PartyMember> PartyMembers { get; internal set; } = new ConcurrentDictionary<uint, PartyMember>();

        public ConcurrentDictionary<uint, PartyMember> RemovedPartyMembers { get; internal set; } = new ConcurrentDictionary<uint, PartyMember>();
    }
}