// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GaugeBitFieldContractTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2026 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Several gauges pack multiple values into one byte. JobResourceResolver masks those bits
//   by hand, and FFXIVClientStructs declares the same layout via [BitField] attributes — but
//   nothing connected the two, so an upstream bit reshuffle would silently change what
//   Sharlayan reports while every other test stayed green (a field MOVING is invisible to
//   this project by design; a field changing MEANING is not).
//
//   These tests read the attributes straight off the FCS types and pin the (name, start bit,
//   width) of every bitfield the resolver masks. If upstream renumbers anything, the matching
//   assertion fails and points at the resolver line that needs updating.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Providers {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using FFXIVClientStructs.FFXIV.Client.Game.Gauge;

    using Xunit;

    public class GaugeBitFieldContractTests {
        [Fact]
        public void BlackMageGauge_EnochianBits_MatchResolverMasks() {
            // JobResourceResolver: & 1, & 2, (>> 2) & 7
            AssertBitFields<BlackMageGauge>(
                "BitFieldD",
                ("EnochianActive", 0, 1),
                ("ParadoxActive", 1, 1),
                ("AstralSoulStacks", 2, 3));
        }

        [Fact]
        public void SummonerGauge_AttunementBits_MatchFcsDeclaration() {
            // Sharlayan surfaces the raw byte; pin the split so a consumer masking it
            // (or a future resolver change) has an enforced reference.
            AssertBitFields<SummonerGauge>(
                "BitFieldE",
                ("AttunementType", 0, 2),
                ("AttunementCount", 2, 6));
        }

        [Fact]
        public void AstrologianGauge_CardBits_MatchResolverMasks() {
            // JobResourceResolver: 0xF & (cards >> 0 / >> 4 / >> 8), CurrentArcana >> 12
            AssertBitFields<AstrologianGauge>(
                "CardsBacking",
                ("Card1", 0, 4),
                ("Card2", 4, 4),
                ("Card3", 8, 4),
                ("CurrentArcana", 12, 4));
        }

        [Fact]
        public void BeastmasterGauge_KinshipBits_MatchResolverMasks() {
            // JobResourceResolver: & 0xF, (>> 4) & 0xF
            AssertBitFields<BeastmasterGauge>(
                "KinshipState",
                ("KinshipBattlehorn", 0, 4),
                ("KinshipClassification", 4, 4));
        }

        [Fact]
        public void BeastmasterGauge_InstinctBits_MatchResolverMasks() {
            // JobResourceResolver: & 0x3, (>> 2) & 0x3
            AssertBitFields<BeastmasterGauge>(
                "InstinctState",
                ("NaturalInstinct", 0, 2),
                ("MasteredInstinct", 2, 2));
        }

        private static void AssertBitFields<T>(string backingFieldName, params (string Name, int Index, int Length)[] expected) {
            FieldInfo field = typeof(T).GetField(backingFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.NotNull(field);

            (string Name, int Index, int Length)[] actual = BitFieldsOn(field);

            Assert.Equal(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++) {
                Assert.Equal(expected[i].Name, actual[i].Name);
                Assert.Equal(expected[i].Index, actual[i].Index);
                Assert.Equal(expected[i].Length, actual[i].Length);
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
