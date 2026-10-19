// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Beastmaster.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2026 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Beastmaster.cs Implementation — the limited job's gauge, added to FFXIVClientStructs
//   in game patch 7.57. BST had no gauge struct before that, which is why Sharlayan 9.2.2
//   shipped the job's identity and level/EXP but no resources.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;

    using Sharlayan.Core.Interfaces;
    using Sharlayan.Core.JobResources.Enums;

    public sealed class BeastmasterResources : IJobResource {
        /// <summary>Gets or sets the player's own Tactical Point gauge.</summary>
        public int TPGauge { get; set; }

        /// <summary>Gets or sets the active pet's Tactical Point gauge.</summary>
        public int PetTPGauge { get; set; }

        /// <summary>Gets or sets the pet's TP level at the time of its last ability use.</summary>
        public int PetTPAtLastUse { get; set; }

        /// <summary>Gets or sets the currently active Battlehorn.</summary>
        public int ActiveBattlehorn { get; set; }

        /// <summary>Gets or sets the Instinctual combo progression state.</summary>
        public int InstinctualComboState { get; set; }

        /// <summary>Gets or sets the pet's current affinity.</summary>
        public BeastmasterAffinity CurrentAffinity { get; set; }

        /// <summary>Gets or sets the current chain count.</summary>
        public int ChainCount { get; set; }

        /// <summary>Gets or sets the raw Kinship byte (Battlehorn in bits 0-3, Classification in bits 4-7).</summary>
        public int KinshipState { get; set; }

        /// <summary>Gets or sets the Kinship Battlehorn (low nibble of <see cref="KinshipState"/>).</summary>
        public int KinshipBattlehorn { get; set; }

        /// <summary>
        /// Gets or sets the Kinship classification (high nibble of <see cref="KinshipState"/>).
        /// FCS notes this indexes Addon row 17740 + Classification.
        /// </summary>
        public int KinshipClassification { get; set; }

        /// <summary>Gets or sets the raw Instinct byte (Natural in bits 0-1, Mastered in bits 2-3).</summary>
        public int InstinctState { get; set; }

        /// <summary>Gets or sets the Natural Instinct stacks (bits 0-1 of <see cref="InstinctState"/>).</summary>
        public int NaturalInstinct { get; set; }

        /// <summary>Gets or sets the Mastered Instinct stacks (bits 2-3 of <see cref="InstinctState"/>).</summary>
        public int MasteredInstinct { get; set; }

        /// <summary>
        /// Gets or sets the gauge timer. The BST gauge carries no timer field, so this is
        /// always <see cref="TimeSpan.Zero"/> — present to satisfy <see cref="IJobResource"/>,
        /// matching how Astrologian behaves.
        /// </summary>
        public TimeSpan Timer { get; set; }
    }
}
