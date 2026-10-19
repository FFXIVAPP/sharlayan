// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BeastmasterGaugeMapperTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2026 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Beastmaster's gauge arrived in game patch 7.57 (BST shipped as a limited job in 7.56
//   with no gauge struct at all). Pins the mapper against FCS' BeastmasterGauge.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game.Gauge;

    using Sharlayan.Models.Structures;
    using Sharlayan.Resources.Mappers;

    using Xunit;

    public class BeastmasterGaugeMapperTests {
        [Fact]
        public void Build_AllFields_MatchFcsOffsets() {
            JobResources.BeastmasterResources gauge = JobResourcesMapper.Build().Beastmaster;

            Assert.Equal(Off(nameof(BeastmasterGauge.TPGauge)), gauge.TPGauge);
            Assert.Equal(Off(nameof(BeastmasterGauge.PetTPGauge)), gauge.PetTPGauge);
            Assert.Equal(Off(nameof(BeastmasterGauge.PetTPAtLastUse)), gauge.PetTPAtLastUse);
            Assert.Equal(Off(nameof(BeastmasterGauge.ActiveBattlehorn)), gauge.ActiveBattlehorn);
            Assert.Equal(Off(nameof(BeastmasterGauge.InstinctualComboState)), gauge.InstinctualComboState);
            Assert.Equal(Off(nameof(BeastmasterGauge.CurrentAffinity)), gauge.CurrentAffinity);
            Assert.Equal(Off(nameof(BeastmasterGauge.ChainCount)), gauge.ChainCount);
            Assert.Equal(Off(nameof(BeastmasterGauge.KinshipState)), gauge.KinshipState);
            Assert.Equal(Off(nameof(BeastmasterGauge.InstinctState)), gauge.InstinctState);
        }

        [Fact]
        public void Build_Offsets_MatchKnownGoodLayout() {
            // Document the 7.57 layout. Every field is a distinct byte from 0x08 to 0x10;
            // if upstream shifts them this fails rather than silently relocating reads.
            JobResources.BeastmasterResources gauge = JobResourcesMapper.Build().Beastmaster;

            Assert.Equal(0x08, gauge.TPGauge);
            Assert.Equal(0x09, gauge.PetTPGauge);
            Assert.Equal(0x0A, gauge.PetTPAtLastUse);
            Assert.Equal(0x0B, gauge.ActiveBattlehorn);
            Assert.Equal(0x0C, gauge.InstinctualComboState);
            Assert.Equal(0x0D, gauge.CurrentAffinity);
            Assert.Equal(0x0E, gauge.ChainCount);
            Assert.Equal(0x0F, gauge.KinshipState);
            Assert.Equal(0x10, gauge.InstinctState);
        }

        [Fact]
        public void Build_AllOffsets_FitInsideTheGaugeStruct() {
            // JobResources.SourceSize sizes the read buffer from JobGaugeManager, and the
            // gauge is a union member at 0x08 — every offset must land inside it.
            int gaugeSize = FieldOffsetReader.SizeOf<BeastmasterGauge>();
            JobResources.BeastmasterResources gauge = JobResourcesMapper.Build().Beastmaster;

            Assert.Equal(0x18, gaugeSize); // known-good 7.57 value
            Assert.True(gauge.InstinctState < gaugeSize, "highest BST offset must sit inside BeastmasterGauge");
        }

        [Fact]
        public void BeastmasterAffinity_MirrorsFcsEnum() {
            // Sharlayan redeclares the enum, so the values must track FCS exactly or the
            // resolver's cast would mislabel the pet's affinity.
            Assert.Equal((byte)BeastmasterAffinity.None, (byte)Sharlayan.Core.JobResources.Enums.BeastmasterAffinity.None);
            Assert.Equal((byte)BeastmasterAffinity.Volant, (byte)Sharlayan.Core.JobResources.Enums.BeastmasterAffinity.Volant);
            Assert.Equal((byte)BeastmasterAffinity.Rampant, (byte)Sharlayan.Core.JobResources.Enums.BeastmasterAffinity.Rampant);
            Assert.Equal((byte)BeastmasterAffinity.Durant, (byte)Sharlayan.Core.JobResources.Enums.BeastmasterAffinity.Durant);
            Assert.Equal((byte)BeastmasterAffinity.Eldritch, (byte)Sharlayan.Core.JobResources.Enums.BeastmasterAffinity.Eldritch);
            Assert.Equal((byte)BeastmasterAffinity.Sunstrider, (byte)Sharlayan.Core.JobResources.Enums.BeastmasterAffinity.Sunstrider);
            Assert.Equal((byte)BeastmasterAffinity.Moonstalker, (byte)Sharlayan.Core.JobResources.Enums.BeastmasterAffinity.Moonstalker);

            // Guards against upstream adding an affinity Sharlayan hasn't mirrored.
            Assert.Equal(
                System.Enum.GetValues<BeastmasterAffinity>().Length,
                System.Enum.GetValues<Sharlayan.Core.JobResources.Enums.BeastmasterAffinity>().Length);
        }

        private static int Off(string fieldName) {
            return (int)Marshal.OffsetOf<BeastmasterGauge>(fieldName);
        }
    }
}
