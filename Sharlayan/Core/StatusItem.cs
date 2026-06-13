// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   StatusItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core {
    using Sharlayan.Core.Interfaces;

    public class StatusItem : IStatusItem {
        public uint CasterID { get; set; }

        /// <summary>
        /// True for detrimental effects removable with Esuna (Lumina Status sheet
        /// <c>CanDispel</c> column — the statuses drawn with a white bar above the
        /// icon in-game). Always false when <see cref="IsDetrimental"/> is false.
        /// </summary>
        public bool CanDispel { get; set; }

        public float Duration { get; set; }

        /// <summary>
        /// True when <see cref="StatusCategory"/> == 1 — a beneficial effect
        /// (enhancement; green/up-arrow icon border in the game UI).
        /// </summary>
        public bool IsBeneficial => this.StatusCategory == 1;

        public bool IsCompanyAction { get; set; }

        /// <summary>
        /// True when <see cref="StatusCategory"/> == 2 — a detrimental effect
        /// (enfeeblement; red/down-arrow icon border in the game UI).
        /// </summary>
        public bool IsDetrimental => this.StatusCategory == 2;

        public ActorItem SourceEntity { get; set; }

        public byte Stacks { get; set; }

        /// <summary>
        /// Raw Lumina Status sheet <c>StatusCategory</c> byte: 1 = beneficial,
        /// 2 = detrimental, 0 = neither (system statuses, or the status id didn't
        /// resolve against XIVDatabase). Prefer the <see cref="IsBeneficial"/> /
        /// <see cref="IsDetrimental"/> conveniences unless you need the raw value.
        /// </summary>
        public byte StatusCategory { get; set; }

        public short StatusID { get; set; }

        public string StatusName { get; set; }

        /// <summary>
        /// Always-English name regardless of <see cref="SharlayanConfiguration.GameLanguage"/>.
        /// Use for programmatic comparisons against hardcoded English literals (e.g.
        /// <c>statuses.Find(s =&gt; s.StatusNameEnglish == "Iron Will")</c>); use
        /// <see cref="StatusName"/> for display.
        /// </summary>
        public string StatusNameEnglish { get; set; }

        public ActorItem TargetEntity { get; set; }

        public string TargetName { get; set; }

        public bool IsValid() {
            return this.StatusID > 0 && this.Duration <= 86400 && this.CasterID > 0;
        }
    }
}