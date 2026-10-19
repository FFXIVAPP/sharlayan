// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GaugeBitField.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2026 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Single source of truth for every gauge value FFXIV packs into a shared byte.
//
//   These layouts used to live as hand-written shifts and masks inside JobResourceResolver,
//   which left two things unverifiable: the arithmetic itself, and whether the bit positions
//   still agreed with FFXIVClientStructs. A field MOVING is invisible to this project by
//   design (offsets derive at runtime); a field changing MEANING is not, and a silently
//   wrong mask reports a plausible-but-wrong gauge value.
//
//   Declaring the positions here once means the resolver and the tests read the same
//   numbers, and GaugeBitFieldContractTests asserts those numbers against FCS' own
//   [BitField] attributes — so an upstream renumber fails a test instead of quietly
//   changing what consumers see.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    /// <summary>
    /// A packed field: <paramref name="Index"/> is the start bit, <paramref name="Length"/> the
    /// width in bits. Mirrors FCS' <c>BitField&lt;T&gt;(name, index, length)</c>.
    /// </summary>
    internal readonly record struct GaugeBits(int Index, int Length);

    internal static class GaugeBitField {
        // BlackMageGauge.BitFieldD @ 0x0D
        internal static readonly GaugeBits EnochianActive = new(0, 1);
        internal static readonly GaugeBits ParadoxActive = new(1, 1);
        internal static readonly GaugeBits AstralSoulStacks = new(2, 3);

        // SummonerGauge.BitFieldE @ 0x0E
        internal static readonly GaugeBits AttunementType = new(0, 2);
        internal static readonly GaugeBits AttunementCount = new(2, 6);

        // AstrologianGauge.CardsBacking @ 0x08 (a ushort, hence bits past 7)
        internal static readonly GaugeBits Card1 = new(0, 4);
        internal static readonly GaugeBits Card2 = new(4, 4);
        internal static readonly GaugeBits Card3 = new(8, 4);
        internal static readonly GaugeBits CurrentArcana = new(12, 4);

        // BeastmasterGauge.KinshipState @ 0x0F
        internal static readonly GaugeBits KinshipBattlehorn = new(0, 4);
        internal static readonly GaugeBits KinshipClassification = new(4, 4);

        // BeastmasterGauge.InstinctState @ 0x10
        internal static readonly GaugeBits NaturalInstinct = new(0, 2);
        internal static readonly GaugeBits MasteredInstinct = new(2, 2);

        /// <summary>Extracts a packed field from the raw gauge value.</summary>
        internal static int Extract(int raw, GaugeBits field) {
            return (raw >> field.Index) & ((1 << field.Length) - 1);
        }

        /// <summary>Extracts a packed single-bit field as a bool.</summary>
        internal static bool Flag(int raw, GaugeBits field) {
            return Extract(raw, field) != 0;
        }
    }
}
