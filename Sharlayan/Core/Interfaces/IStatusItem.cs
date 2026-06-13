// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStatusItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   IStatusItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.Interfaces {
    public interface IStatusItem {
        uint CasterID { get; set; }

        /// <summary>True for detrimental effects removable with Esuna (Lumina Status sheet <c>CanDispel</c>).</summary>
        bool CanDispel { get; set; }

        float Duration { get; set; }

        /// <summary>True when <see cref="StatusCategory"/> == 1 — beneficial (enhancement).</summary>
        bool IsBeneficial { get; }

        bool IsCompanyAction { get; set; }

        /// <summary>True when <see cref="StatusCategory"/> == 2 — detrimental (enfeeblement).</summary>
        bool IsDetrimental { get; }

        ActorItem SourceEntity { get; set; }

        byte Stacks { get; set; }

        /// <summary>Raw Lumina <c>StatusCategory</c> byte: 1 = beneficial, 2 = detrimental, 0 = neither/unresolved.</summary>
        byte StatusCategory { get; set; }

        short StatusID { get; set; }

        string StatusName { get; set; }

        /// <summary>
        /// Always-English name regardless of <see cref="SharlayanConfiguration.GameLanguage"/>.
        /// Use for programmatic comparisons against hardcoded English literals (e.g.
        /// <c>statuses.Find(s =&gt; s.StatusNameEnglish == "Iron Will")</c>); use
        /// <see cref="StatusName"/> for display.
        /// </summary>
        string StatusNameEnglish { get; set; }

        ActorItem TargetEntity { get; set; }

        string TargetName { get; set; }

        bool IsValid();
    }
}