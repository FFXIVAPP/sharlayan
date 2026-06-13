// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   StatusItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.XIVDatabase {
    public class StatusItem {
        public bool CompanyAction { get; set; }

        public Localization Name { get; set; }

        /// <summary>
        /// Max simultaneous stacks per Lumina's Status sheet (<c>MaxStacks</c> column).
        /// 0 or 1 means the status doesn't stack — its in-memory <c>Status.Param</c> field
        /// encodes something other than a stack count (food/potion id, or unrelated meta
        /// for ordinary buffs), so resolvers should leave <c>StatusItem.Stacks</c> at 0
        /// rather than surfacing the Param byte as a misleading "stacks" value.
        /// </summary>
        public byte MaxStacks { get; set; }

        /// <summary>
        /// Lumina Status sheet <c>StatusCategory</c> column: 1 = beneficial (enhancement —
        /// green/up-arrow border in the game UI), 2 = detrimental (enfeeblement — red/down-arrow),
        /// 0 = neither (system statuses). This is the same discriminator the game's own
        /// party-list / status HUD uses to colour the icon border.
        /// </summary>
        public byte StatusCategory { get; set; }

        /// <summary>
        /// Lumina Status sheet <c>CanDispel</c> column — true for detrimental effects
        /// removable with Esuna (the ones drawn with a white bar above the icon in-game).
        /// </summary>
        public bool CanDispel { get; set; }
    }
}