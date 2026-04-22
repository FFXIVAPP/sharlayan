// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartyMemberMapperTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Common.Math;

    using Sharlayan.Models.Structures;
    using Sharlayan.Resources.Mappers;

    using Xunit;

    using NativePartyMember = FFXIVClientStructs.FFXIV.Client.Game.Group.PartyMember;

    public class PartyMemberMapperTests {
        [Fact]
        public void Build_HPCurrent_MatchesCurrentHPOffset() {
            PartyMember partyMember = PartyMemberMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<NativePartyMember>(nameof(NativePartyMember.CurrentHP)), partyMember.HPCurrent);
        }

        [Fact]
        public void Build_ID_MatchesEntityIdOffset() {
            PartyMember partyMember = PartyMemberMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<NativePartyMember>(nameof(NativePartyMember.EntityId)), partyMember.ID);
        }

        [Fact]
        public void Build_Position_XYZ_SwappedRelativeToCsVector3() {
            // Same swap as ActorItemMapper — see its header for rationale.
            PartyMember partyMember = PartyMemberMapper.Build();
            int positionOffset = (int)Marshal.OffsetOf<NativePartyMember>(nameof(NativePartyMember.Position));

            Assert.Equal(positionOffset + (int)Marshal.OffsetOf<Vector3>(nameof(Vector3.X)), partyMember.X);
            Assert.Equal(positionOffset + (int)Marshal.OffsetOf<Vector3>(nameof(Vector3.Y)), partyMember.Z);
            Assert.Equal(positionOffset + (int)Marshal.OffsetOf<Vector3>(nameof(Vector3.Z)), partyMember.Y);
        }

        [Fact]
        public void Build_SourceSize_MatchesNativePartyMemberSize() {
            PartyMember partyMember = PartyMemberMapper.Build();
            Assert.Equal(Marshal.SizeOf<NativePartyMember>(), partyMember.SourceSize);
        }

        [Fact]
        public void Build_MappedFields_AreAllReasonable() {
            PartyMember partyMember = PartyMemberMapper.Build();

            Assert.True(partyMember.HPCurrent > 0, nameof(PartyMember.HPCurrent));
            Assert.True(partyMember.HPMax > 0, nameof(PartyMember.HPMax));
            Assert.True(partyMember.MPCurrent > 0, nameof(PartyMember.MPCurrent));
            Assert.True(partyMember.ID > 0, nameof(PartyMember.ID));
            Assert.True(partyMember.Name > 0, nameof(PartyMember.Name));
            Assert.True(partyMember.Job > 0, nameof(PartyMember.Job));
            Assert.True(partyMember.Level > 0, nameof(PartyMember.Level));
            Assert.True(partyMember.X > 0, nameof(PartyMember.X));
            Assert.True(partyMember.Y > 0, nameof(PartyMember.Y));
            Assert.True(partyMember.Z > 0, nameof(PartyMember.Z));
            Assert.True(partyMember.SourceSize > 0, nameof(PartyMember.SourceSize));
            // DefaultStatusEffectOffset is explicitly 0 (StatusManager is at offset 0),
            // so it is a "mapped but zero" field. Verified separately below.
        }

        [Fact]
        public void Build_DefaultStatusEffectOffset_IsStatusManagerOffset() {
            // StatusManager sits at native offset 0 inside PartyMember, so this field
            // legitimately maps to zero. Pin that so a regression to "DefaultStatusEffectOffset
            // should obviously be non-zero" doesn't accidentally bust the mapping.
            PartyMember partyMember = PartyMemberMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<NativePartyMember>(nameof(NativePartyMember.StatusManager)), partyMember.DefaultStatusEffectOffset);
            Assert.Equal(0, partyMember.DefaultStatusEffectOffset);
        }
    }
}
