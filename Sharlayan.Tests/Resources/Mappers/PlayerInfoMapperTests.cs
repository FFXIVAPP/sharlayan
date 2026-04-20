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
            // Derived attributes — now wired to _attributes array.
            Assert.True(info.Strength > 0, nameof(info.Strength));
            Assert.True(info.HPMax > 0, nameof(info.HPMax));
            Assert.True(info.CriticalHitRate > 0, nameof(info.CriticalHitRate));
            Assert.True(info.Determination > 0, nameof(info.Determination));
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
        public void Build_DerivedAttributes_MatchAttributesArrayOffsets() {
            // PlayerState._attributes is FixedSizeArray74<int> at [FieldOffset(0x1A8)].
            // Each slot = attribBase + PlayerAttribute_value * 4.
            PlayerInfo info = PlayerInfoMapper.Build();
            int attribBase = (int)Marshal.OffsetOf<PlayerState>("_attributes");
            int Attr(PlayerAttribute a) => attribBase + (int)a * sizeof(int);

            Assert.Equal(Attr(PlayerAttribute.Strength),          info.Strength);
            Assert.Equal(Attr(PlayerAttribute.Dexterity),         info.Dexterity);
            Assert.Equal(Attr(PlayerAttribute.Vitality),          info.Vitality);
            Assert.Equal(Attr(PlayerAttribute.Intelligence),      info.Intelligence);
            Assert.Equal(Attr(PlayerAttribute.Mind),              info.Mind);
            Assert.Equal(Attr(PlayerAttribute.Piety),             info.Piety);
            Assert.Equal(Attr(PlayerAttribute.HealthPoints),      info.HPMax);
            Assert.Equal(Attr(PlayerAttribute.GatheringPoints),   info.GPMax);
            Assert.Equal(Attr(PlayerAttribute.CraftingPoints),    info.CPMax);
            Assert.Equal(Attr(PlayerAttribute.Tenacity),          info.Tenacity);
            Assert.Equal(Attr(PlayerAttribute.AttackPower),       info.AttackPower);
            Assert.Equal(Attr(PlayerAttribute.Defense),           info.Defense);
            Assert.Equal(Attr(PlayerAttribute.DirectHitRate),     info.DirectHit);
            Assert.Equal(Attr(PlayerAttribute.MagicDefense),      info.MagicDefense);
            Assert.Equal(Attr(PlayerAttribute.CriticalHit),       info.CriticalHitRate);
            Assert.Equal(Attr(PlayerAttribute.Determination),     info.Determination);
            Assert.Equal(Attr(PlayerAttribute.SkillSpeed),        info.SkillSpeed);
            Assert.Equal(Attr(PlayerAttribute.SpellSpeed),        info.SpellSpeed);
            Assert.Equal(Attr(PlayerAttribute.AttackMagicPotency),   info.AttackMagicPotency);
            Assert.Equal(Attr(PlayerAttribute.HealingMagicPotency),  info.HealingMagicPotency);
            Assert.Equal(Attr(PlayerAttribute.FireResistance),    info.FireResistance);
            Assert.Equal(Attr(PlayerAttribute.IceResistance),     info.IceResistance);
            Assert.Equal(Attr(PlayerAttribute.WindResistance),    info.WindResistance);
            Assert.Equal(Attr(PlayerAttribute.EarthResistance),   info.EarthResistance);
            Assert.Equal(Attr(PlayerAttribute.LightningResistance), info.LightningResistance);
            Assert.Equal(Attr(PlayerAttribute.WaterResistance),   info.WaterResistance);
            Assert.Equal(Attr(PlayerAttribute.SlashingResistance), info.SlashingResistance);
            Assert.Equal(Attr(PlayerAttribute.PiercingResistance), info.PiercingResistance);
            Assert.Equal(Attr(PlayerAttribute.BluntResistance),   info.BluntResistance);
            Assert.Equal(Attr(PlayerAttribute.Craftsmanship),     info.Craftmanship);
            Assert.Equal(Attr(PlayerAttribute.Control),           info.Control);
            Assert.Equal(Attr(PlayerAttribute.Gathering),         info.Gathering);
            Assert.Equal(Attr(PlayerAttribute.Perception),        info.Perception);
        }
    }
}
