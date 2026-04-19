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
        public void Build_PerJobFieldsAndEXP_AreMapped() {
            // P3-B11 wired all 29 base-class per-job Level and CurrentEXP offsets using hard-coded
            // ExpArrayIndex constants from the ClassJob sheet. Each level offset sits at
            // _classJobLevels + idx*2; each EXP offset at _classJobExperience + idx*4.
            PlayerInfo info = PlayerInfoMapper.Build();
            int levelsBase = (int)Marshal.OffsetOf<PlayerState>("_classJobLevels");
            int expBase    = (int)Marshal.OffsetOf<PlayerState>("_classJobExperience");

            // PGL = ExpArrayIndex 0 (first slot in both arrays).
            Assert.Equal(levelsBase, info.PGL);
            Assert.Equal(expBase, info.PGL_CurrentEXP);

            // PCT = ExpArrayIndex 31 (newest job at end of the array).
            Assert.Equal(levelsBase + 31 * sizeof(short), info.PCT);
            Assert.Equal(expBase    + 31 * sizeof(int),   info.PCT_CurrentEXP);
        }

        [Fact]
        public void Build_DerivedStats_StayZero() {
            // Derived attributes (Strength/AttackPower/CriticalHitRate/etc.) and resistances
            // live in PlayerState._attributes indexed by BaseParam — that's dynamic Lumina
            // data, not a compile-time struct offset. Same for HPMax/MPMax/etc which live on
            // Character, not PlayerState. Pinned here so a future edit can't silently "map"
            // them to a stale offset.
            PlayerInfo info = PlayerInfoMapper.Build();

            Assert.Equal(0, info.Strength);
            Assert.Equal(0, info.AttackPower);
            Assert.Equal(0, info.CriticalHitRate);
            Assert.Equal(0, info.DirectHit);
            Assert.Equal(0, info.HPMax);
            Assert.Equal(0, info.CPMax);
            Assert.Equal(0, info.FireResistance);
        }
    }
}
