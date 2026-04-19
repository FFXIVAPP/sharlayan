// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetInfoMapperTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game.Control;

    using Sharlayan.Models.Structures;
    using Sharlayan.Resources.Mappers;

    using Xunit;

    public class TargetInfoMapperTests {
        [Fact]
        public void Build_Current_MatchesTargetOffset() {
            TargetInfo info = TargetInfoMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<TargetSystem>(nameof(TargetSystem.Target)), info.Current);
        }

        [Fact]
        public void Build_CurrentID_MatchesTargetObjectIdOffset() {
            TargetInfo info = TargetInfoMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<TargetSystem>(nameof(TargetSystem.TargetObjectId)), info.CurrentID);
        }

        [Fact]
        public void Build_Focus_MatchesFocusTargetOffset() {
            TargetInfo info = TargetInfoMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<TargetSystem>(nameof(TargetSystem.FocusTarget)), info.Focus);
        }

        [Fact]
        public void Build_SourceSize_MatchesTargetSystemSize() {
            TargetInfo info = TargetInfoMapper.Build();
            Assert.Equal(Marshal.SizeOf<TargetSystem>(), info.SourceSize);
        }

        [Fact]
        public void Build_MappedFields_NonZero() {
            TargetInfo info = TargetInfoMapper.Build();
            Assert.True(info.Current > 0, nameof(info.Current));
            Assert.True(info.CurrentID > 0, nameof(info.CurrentID));
            Assert.True(info.MouseOver > 0, nameof(info.MouseOver));
            Assert.True(info.Focus > 0, nameof(info.Focus));
            Assert.True(info.Previous > 0, nameof(info.Previous));
            Assert.True(info.SourceSize > 0, nameof(info.SourceSize));
        }

        [Fact]
        public void Build_UnmappedFields_StayZero() {
            TargetInfo info = TargetInfoMapper.Build();
            Assert.Equal(0, info.Size);
        }
    }
}
