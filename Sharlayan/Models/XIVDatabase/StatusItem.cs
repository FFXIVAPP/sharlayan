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
    }
}