// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnmityItemMapperTests.cs" company="SyndicatedLife">
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

    public class EnmityItemMapperTests {
        [Fact]
        public void Build_Name_AtOffsetZero() {
            // HaterInfo._name is the first field of the native struct (offset 0).
            EnmityItem item = EnmityItemMapper.Build();
            Assert.Equal(0, item.Name);
        }

        [Fact]
        public void Build_ID_MatchesEntityIdOffset() {
            EnmityItem item = EnmityItemMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<HaterInfo>(nameof(HaterInfo.EntityId)), item.ID);
        }

        [Fact]
        public void Build_Enmity_MatchesEnmityOffset() {
            EnmityItem item = EnmityItemMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<HaterInfo>(nameof(HaterInfo.Enmity)), item.Enmity);
        }

        [Fact]
        public void Build_SourceSize_MatchesHaterInfoSize() {
            EnmityItem item = EnmityItemMapper.Build();
            Assert.Equal(Marshal.SizeOf<HaterInfo>(), item.SourceSize);
        }

        [Fact]
        public void Build_UnmappedFields_StayZero() {
            EnmityItem item = EnmityItemMapper.Build();
            Assert.Equal(0, item.EnmityCount);
        }
    }
}
