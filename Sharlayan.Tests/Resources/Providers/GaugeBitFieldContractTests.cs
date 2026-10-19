// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GaugeBitFieldContractTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2026 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Several gauges pack multiple values into one byte. Those positions are declared once in
//   Sharlayan.Utilities.GaugeBitField and consumed by JobResourceResolver; FFXIVClientStructs
//   declares the same layout via [BitField] attributes.
//
//   These tests close the loop in both directions:
//     1. every GaugeBitField entry matches FCS' attribute for the same field, so an upstream
//        renumber fails here instead of silently changing what consumers see;
//     2. GaugeBitField.Extract actually performs the arithmetic those positions describe, so a
//        broken shift or mask cannot pass just because FCS is unchanged.
//
//   A field MOVING is invisible to this project by design (offsets derive at runtime). A field
//   changing MEANING is exactly what this file is for.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Providers {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using FFXIVClientStructs.FFXIV.Client.Game.Gauge;

    using Sharlayan.Utilities;

    using Xunit;

    public class GaugeBitFieldContractTests {
        // ------------------------------------------------------------------------------
        // 1. Sharlayan's declared positions vs FCS' [BitField] attributes.
        // ------------------------------------------------------------------------------

        [Fact]
        public void BlackMageGauge_EnochianBits_MatchFcs() {
            AssertMatchesFcs<BlackMageGauge>(
                "BitFieldD",
                ("EnochianActive", GaugeBitField.EnochianActive),
                ("ParadoxActive", GaugeBitField.ParadoxActive),
                ("AstralSoulStacks", GaugeBitField.AstralSoulStacks));
        }

        [Fact]
        public void SummonerGauge_AttunementBits_MatchFcs() {
            AssertMatchesFcs<SummonerGauge>(
                "BitFieldE",
                ("AttunementType", GaugeBitField.AttunementType),
                ("AttunementCount", GaugeBitField.AttunementCount));
        }

        [Fact]
        public void AstrologianGauge_CardBits_MatchFcs() {
            AssertMatchesFcs<AstrologianGauge>(
                "CardsBacking",
                ("Card1", GaugeBitField.Card1),
                ("Card2", GaugeBitField.Card2),
                ("Card3", GaugeBitField.Card3),
                ("CurrentArcana", GaugeBitField.CurrentArcana));
        }

        [Fact]
        public void BeastmasterGauge_KinshipBits_MatchFcs() {
            AssertMatchesFcs<BeastmasterGauge>(
                "KinshipState",
                ("KinshipBattlehorn", GaugeBitField.KinshipBattlehorn),
                ("KinshipClassification", GaugeBitField.KinshipClassification));
        }

        [Fact]
        public void BeastmasterGauge_InstinctBits_MatchFcs() {
            AssertMatchesFcs<BeastmasterGauge>(
                "InstinctState",
                ("NaturalInstinct", GaugeBitField.NaturalInstinct),
                ("MasteredInstinct", GaugeBitField.MasteredInstinct));
        }

        // ------------------------------------------------------------------------------
        // 2. The extraction arithmetic itself, on synthetic values.
        // ------------------------------------------------------------------------------

        [Fact]
        public void Extract_PullsTheRightNibbles() {
            // 0xA5 = 1010 0101 -> low nibble 5, high nibble 0xA (10).
            Assert.Equal(0x5, GaugeBitField.Extract(0xA5, GaugeBitField.KinshipBattlehorn));
            Assert.Equal(0xA, GaugeBitField.Extract(0xA5, GaugeBitField.KinshipClassification));
        }

        [Fact]
        public void Extract_PullsTheRightTwoBitPairs() {
            // 0x0E = 0000 1110 -> bits 0-1 = 2, bits 2-3 = 3.
            Assert.Equal(2, GaugeBitField.Extract(0x0E, GaugeBitField.NaturalInstinct));
            Assert.Equal(3, GaugeBitField.Extract(0x0E, GaugeBitField.MasteredInstinct));
        }

        [Fact]
        public void Extract_HandlesEnochianByteLayout() {
            // 0x1D = 0001 1101 -> Enochian set, Paradox clear, AstralSoul = 0b111 = 7.
            Assert.True(GaugeBitField.Flag(0x1D, GaugeBitField.EnochianActive));
            Assert.False(GaugeBitField.Flag(0x1D, GaugeBitField.ParadoxActive));
            Assert.Equal(7, GaugeBitField.Extract(0x1D, GaugeBitField.AstralSoulStacks));
        }

        [Fact]
        public void Extract_HandlesAstrologianUShortLayout() {
            // Cards is 16-bit: four nibbles, highest is CurrentArcana.
            const int cards = 0x4321;
            Assert.Equal(1, GaugeBitField.Extract(cards, GaugeBitField.Card1));
            Assert.Equal(2, GaugeBitField.Extract(cards, GaugeBitField.Card2));
            Assert.Equal(3, GaugeBitField.Extract(cards, GaugeBitField.Card3));
            Assert.Equal(4, GaugeBitField.Extract(cards, GaugeBitField.CurrentArcana));
        }

        [Fact]
        public void Extract_MasksToFieldWidth_IgnoringHigherBits() {
            // A 6-bit field must not bleed in bits above its width.
            Assert.Equal(0b111111, GaugeBitField.Extract(0xFF, GaugeBitField.AttunementCount));
            Assert.Equal(0b11, GaugeBitField.Extract(0xFF, GaugeBitField.AttunementType));
        }

        // ------------------------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------------------------

        private static void AssertMatchesFcs<T>(string backingFieldName, params (string Name, GaugeBits Ours)[] expected) {
            FieldInfo field = typeof(T).GetField(backingFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.NotNull(field);

            (string Name, int Index, int Length)[] fromFcs = BitFieldsOn(field);

            Assert.Equal(expected.Length, fromFcs.Length);

            foreach ((string name, GaugeBits ours) in expected) {
                (string Name, int Index, int Length) theirs = Assert.Single(fromFcs.Where(f => f.Name == name));
                Assert.Equal(theirs.Index, ours.Index);
                Assert.Equal(theirs.Length, ours.Length);
            }
        }

        private static (string Name, int Index, int Length)[] BitFieldsOn(FieldInfo field) {
            List<(string Name, int Index, int Length)> found = new();

            foreach (Attribute attribute in field.GetCustomAttributes()) {
                Type attributeType = attribute.GetType();
                if (!attributeType.IsGenericType || !attributeType.Name.StartsWith("BitFieldAttribute", StringComparison.Ordinal)) {
                    continue;
                }

                string name = (string)attributeType.GetProperty("Name")!.GetValue(attribute)!;
                int index = (int)attributeType.GetProperty("Index")!.GetValue(attribute)!;
                int length = (int)attributeType.GetProperty("Length")!.GetValue(attribute)!;
                found.Add((name, index, length));
            }

            return found.OrderBy(f => f.Index).ToArray();
        }
    }
}
