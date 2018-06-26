// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartyMember.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   PartyMember.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core {
    using Sharlayan.Core.Interfaces;

    public class PartyMember : ActorItemBase, IPartyMember {
        public bool IsValid => this.ID > 0 && !string.IsNullOrWhiteSpace(this.Name);
    }
}