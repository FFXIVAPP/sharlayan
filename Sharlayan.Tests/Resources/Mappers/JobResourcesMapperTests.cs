// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourcesMapperTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game;
    using FFXIVClientStructs.FFXIV.Client.Game.Gauge;

    using Sharlayan.Models.Structures;
    using Sharlayan.Resources.Mappers;

    using Xunit;

    public class JobResourcesMapperTests {
        [Fact]
        public void Build_SourceSize_MatchesJobGaugeManagerSize() {
            JobResources resources = JobResourcesMapper.Build();
            Assert.Equal(Marshal.SizeOf<JobGaugeManager>(), resources.SourceSize);
        }

        [Fact]
        public void Build_Bard_Timer_MatchesSongTimerOffset() {
            JobResources resources = JobResourcesMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<BardGauge>(nameof(BardGauge.SongTimer)), resources.Bard.Timer);
        }

        [Fact]
        public void Build_BlackMage_Timer_MatchesEnochianTimerOffset() {
            JobResources resources = JobResourcesMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<BlackMageGauge>(nameof(BlackMageGauge.EnochianTimer)), resources.BlackMage.Timer);
        }

        [Fact]
        public void Build_Astrologian_Cards_MatchesCardsOffset() {
            JobResources resources = JobResourcesMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<AstrologianGauge>(nameof(AstrologianGauge.Cards)), resources.Astrologian.Seals);
        }

        [Fact]
        public void Build_Dancer_StepsAreContiguous() {
            JobResources resources = JobResourcesMapper.Build();
            Assert.Equal(resources.Dancer.Step1 + 1, resources.Dancer.Step2);
            Assert.Equal(resources.Dancer.Step2 + 1, resources.Dancer.Step3);
            Assert.Equal(resources.Dancer.Step3 + 1, resources.Dancer.Step4);
        }

        [Fact]
        public void Build_AllJobs_HaveAtLeastOneNonZeroMappedField() {
            JobResources r = JobResourcesMapper.Build();
            Assert.True(r.Astrologian.Seals > 0, "Astrologian");
            Assert.True(r.Bard.Timer > 0, "Bard");
            Assert.True(r.BlackMage.Timer > 0, "BlackMage");
            Assert.True(r.Dancer.Esprit > 0, "Dancer");
            Assert.True(r.DarkKnight.Timer > 0, "DarkKnight");
            Assert.True(r.Dragoon.Timer > 0, "Dragoon");
            Assert.True(r.GunBreaker.Cartridge > 0, "GunBreaker");
            Assert.True(r.Machinist.Battery > 0, "Machinist");
            Assert.True(r.Monk.Chakra > 0, "Monk");
            Assert.True(r.Ninja.NinkiGauge > 0, "Ninja");
            Assert.True(r.Paladin.OathGauge > 0, "Paladin");
            Assert.True(r.Pictomancer.PalleteGauge > 0, "Pictomancer");
            Assert.True(r.Reaper.Soul > 0, "Reaper");
            Assert.True(r.RedMage.BlackMana > 0, "RedMage");
            Assert.True(r.Sage.Addersgall > 0, "Sage");
            Assert.True(r.Samurai.Kenki > 0, "Samurai");
            Assert.True(r.Scholar.Aetherflow > 0, "Scholar");
            Assert.True(r.Summoner.SummonTimer > 0, "Summoner");
            Assert.True(r.Viper.Timer > 0, "Viper");
            Assert.True(r.Warrior.BeastGauge > 0, "Warrior");
            Assert.True(r.WhiteMage.Lily > 0, "WhiteMage");
        }
    }
}
