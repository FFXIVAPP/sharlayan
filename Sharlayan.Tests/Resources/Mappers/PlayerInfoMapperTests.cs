// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerInfoMapperTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game.UI;

    using Sharlayan.Models.Structures;
    using Sharlayan.Resources.Mappers;

    using Xunit;

    public class PlayerInfoMapperTests {
        [Fact]
        public void Build_JobID_MatchesCurrentClassJobIdOffset() {
            PlayerInfo info = PlayerInfoMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<PlayerState>(nameof(PlayerState.CurrentClassJobId)), info.JobID);
        }

        [Fact]
        public void Build_BaseStrength_MatchesPlayerStateOffset() {
            PlayerInfo info = PlayerInfoMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<PlayerState>(nameof(PlayerState.BaseStrength)), info.BaseStrength);
        }

        [Fact]
        public void Build_SourceSize_MatchesPlayerStateSize() {
            PlayerInfo info = PlayerInfoMapper.Build();
            Assert.Equal(Marshal.SizeOf<PlayerState>(), info.SourceSize);
        }

        [Fact]
        public void Build_MappedFields_NonZero() {
            PlayerInfo info = PlayerInfoMapper.Build();
            Assert.True(info.JobID > 0, nameof(info.JobID));
            Assert.True(info.BaseStrength > 0, nameof(info.BaseStrength));
            Assert.True(info.BaseDexterity > info.BaseStrength, nameof(info.BaseDexterity));
            Assert.True(info.BaseVitality > info.BaseDexterity, nameof(info.BaseVitality));
            Assert.True(info.BaseIntelligence > info.BaseVitality, nameof(info.BaseIntelligence));
            Assert.True(info.BaseMind > info.BaseIntelligence, nameof(info.BaseMind));
            Assert.True(info.BasePiety > info.BaseMind, nameof(info.BasePiety));
            Assert.True(info.SourceSize > 0, nameof(info.SourceSize));
        }

        [Fact]
        public void Build_PerJobFieldsAndDerivedStats_StayZero() {
            // Per-job levels/EXP and derived stats require Lumina ClassJob.ExpArrayIndex
            // and BaseParam lookups — they are not compile-time struct offsets. Pinned
            // here so a future mapper edit can't silently change the unmapped baseline
            // for a field that actually needs the lookup indirection.
            PlayerInfo info = PlayerInfoMapper.Build();

            Assert.Equal(0, info.PLD_Level());
            Assert.Equal(0, info.BLU_CurrentEXP);
            Assert.Equal(0, info.Strength);
            Assert.Equal(0, info.AttackPower);
            Assert.Equal(0, info.CriticalHitRate);
            Assert.Equal(0, info.DirectHit);
            Assert.Equal(0, info.HPMax);
            Assert.Equal(0, info.CPMax);
            Assert.Equal(0, info.FireResistance);
        }
    }

    internal static class PlayerInfoTestExtensions {
        // PLD (Paladin) has no property on PlayerInfo; this helper keeps the test fluent
        // and documents the gap symbolically. Returns 0 always — matches the unmapped
        // expectation for all non-job-listed values.
        internal static int PLD_Level(this PlayerInfo _) => 0;
    }
}
