// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActorItemMapperTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game.Character;
    using FFXIVClientStructs.FFXIV.Client.Game.Object;
    using FFXIVClientStructs.FFXIV.Common.Math;

    using Sharlayan.Models.Structures;
    using Sharlayan.Resources.Mappers;

    using Xunit;

    public class ActorItemMapperTests {
        [Fact]
        public void Build_HPCurrent_MatchesCharacterHealthOffset() {
            ActorItem actorItem = ActorItemMapper.Build();
            int expected = (int)Marshal.OffsetOf<Character>(nameof(Character.Health));
            Assert.Equal(expected, actorItem.HPCurrent);
        }

        [Fact]
        public void Build_HPMax_MatchesCharacterMaxHealthOffset() {
            ActorItem actorItem = ActorItemMapper.Build();
            int expected = (int)Marshal.OffsetOf<Character>(nameof(Character.MaxHealth));
            Assert.Equal(expected, actorItem.HPMax);
        }

        [Fact]
        public void Build_Position_XYZ_SwappedRelativeToCsVector3_AsHistoricalConvention() {
            // Sharlayan's public API exposes ActorItem.X as game-X, but ActorItem.Y and Z
            // are swapped vs the native Vector3. This test pins that swap so a refactor
            // can't accidentally "fix" it.
            ActorItem actorItem = ActorItemMapper.Build();
            int positionOffset = (int)Marshal.OffsetOf<Character>(nameof(Character.Position));

            Assert.Equal(positionOffset + (int)Marshal.OffsetOf<Vector3>(nameof(Vector3.X)), actorItem.X);
            Assert.Equal(positionOffset + (int)Marshal.OffsetOf<Vector3>(nameof(Vector3.Y)), actorItem.Z);
            Assert.Equal(positionOffset + (int)Marshal.OffsetOf<Vector3>(nameof(Vector3.Z)), actorItem.Y);
        }

        [Fact]
        public void Build_SourceSize_MatchesCharacterStructSize() {
            ActorItem actorItem = ActorItemMapper.Build();
            Assert.Equal(Marshal.SizeOf<Character>(), actorItem.SourceSize);
        }

        [Fact]
        public void Build_MappedFields_AreAllNonZero() {
            // Sanity: any field the mapper explicitly populates must be a positive byte
            // offset. Zero indicates either an unmapped-on-purpose field (handled by the
            // list below) or a silent mapping failure (the bug we are guarding against).
            ActorItem actorItem = ActorItemMapper.Build();

            Assert.True(actorItem.Name > 0, nameof(ActorItem.Name));
            Assert.True(actorItem.ID > 0, nameof(ActorItem.ID));
            Assert.True(actorItem.OwnerID > 0, nameof(ActorItem.OwnerID));
            Assert.True(actorItem.Type > 0, nameof(ActorItem.Type));
            Assert.True(actorItem.Heading > 0, nameof(ActorItem.Heading));
            Assert.True(actorItem.HitBoxRadius > 0, nameof(ActorItem.HitBoxRadius));
            Assert.True(actorItem.Fate > 0, nameof(ActorItem.Fate));
            Assert.True(actorItem.NPCID1 > 0, nameof(ActorItem.NPCID1));
            Assert.True(actorItem.NPCID2 > 0, nameof(ActorItem.NPCID2));
            Assert.True(actorItem.Distance > 0, nameof(ActorItem.Distance));
            Assert.True(actorItem.X > 0, nameof(ActorItem.X));
            Assert.True(actorItem.Y > 0, nameof(ActorItem.Y));
            Assert.True(actorItem.Z > 0, nameof(ActorItem.Z));
            Assert.True(actorItem.HPCurrent > 0, nameof(ActorItem.HPCurrent));
            Assert.True(actorItem.HPMax > 0, nameof(ActorItem.HPMax));
            Assert.True(actorItem.MPCurrent > 0, nameof(ActorItem.MPCurrent));
            Assert.True(actorItem.GPCurrent > 0, nameof(ActorItem.GPCurrent));
            Assert.True(actorItem.GPMax > 0, nameof(ActorItem.GPMax));
            Assert.True(actorItem.CPCurrent > 0, nameof(ActorItem.CPCurrent));
            Assert.True(actorItem.CPMax > 0, nameof(ActorItem.CPMax));
            Assert.True(actorItem.Title > 0, nameof(ActorItem.Title));
            Assert.True(actorItem.Job > 0, nameof(ActorItem.Job));
            Assert.True(actorItem.Level > 0, nameof(ActorItem.Level));
            Assert.True(actorItem.Icon > 0, nameof(ActorItem.Icon));
            Assert.True(actorItem.IsGM > 0, nameof(ActorItem.IsGM));
            Assert.True(actorItem.TargetID > 0, nameof(ActorItem.TargetID));
            Assert.True(actorItem.SourceSize > 0, nameof(ActorItem.SourceSize));
        }

        [Fact]
        public void Build_NowMappedFields_AreNonZero() {
            // These were unmapped in the initial P3 pass; P3-B12 and P3-B17 wired them
            // via BattleChara.CastInfo (casting group), CharacterData.Flags (agro/combat),
            // GameObject.RenderFlags (cutscene), and CharacterData.CombatTaggerId (claim).
            ActorItem actorItem = ActorItemMapper.Build();

            Assert.True(actorItem.CastingID > 0, nameof(actorItem.CastingID));
            Assert.True(actorItem.CastingProgress > 0, nameof(actorItem.CastingProgress));
            Assert.True(actorItem.CastingTargetID > 0, nameof(actorItem.CastingTargetID));
            Assert.True(actorItem.CastingTime > 0, nameof(actorItem.CastingTime));
            Assert.True(actorItem.IsCasting1 > 0, nameof(actorItem.IsCasting1));
            Assert.True(actorItem.IsCasting2 > 0, nameof(actorItem.IsCasting2));
            Assert.True(actorItem.Status > 0, nameof(actorItem.Status));
            Assert.True(actorItem.ClaimedByID > 0, nameof(actorItem.ClaimedByID));
            Assert.True(actorItem.AgroFlags > 0, nameof(actorItem.AgroFlags));
            Assert.True(actorItem.CombatFlags > 0, nameof(actorItem.CombatFlags));
            Assert.True(actorItem.InCutscene > 0, nameof(actorItem.InCutscene));
            Assert.True(actorItem.TargetFlags > 0, nameof(actorItem.TargetFlags));
            Assert.True(actorItem.EventObjectType > 0, nameof(actorItem.EventObjectType));
            Assert.True(actorItem.EntityCount > 0, nameof(actorItem.EntityCount));
        }

        [Fact]
        public void Build_StillUnmappedFields_StayAtZero() {
            // No clean FCS equivalent — PlayerState-sourced data for the local player only
            // (GrandCompany), computed values (DifficultyRank, ActionStatus), or nested
            // structs that haven't been wired yet. If any of these become non-zero via a
            // future edit, this test fails and forces the author to update intentionally.
            ActorItem actorItem = ActorItemMapper.Build();

            Assert.Equal(0, actorItem.ActionStatus);
            Assert.Equal(0, actorItem.ModelID);
            Assert.Equal(0, actorItem.DifficultyRank);
            Assert.Equal(0, actorItem.GatheringInvisible);
            Assert.Equal(0, actorItem.GatheringStatus);
            Assert.Equal(0, actorItem.GrandCompany);
            Assert.Equal(0, actorItem.GrandCompanyRank);
        }
    }
}
