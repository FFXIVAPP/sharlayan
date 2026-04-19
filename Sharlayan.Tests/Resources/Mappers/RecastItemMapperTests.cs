// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecastItemMapperTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game;

    using Sharlayan.Models.Structures;
    using Sharlayan.Resources.Mappers;

    using Xunit;

    public class RecastItemMapperTests {
        [Fact]
        public void Build_ID_MatchesActionIdOffset() {
            RecastItem item = RecastItemMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<RecastDetail>(nameof(RecastDetail.ActionId)), item.ID);
        }

        [Fact]
        public void Build_IsAvailable_MatchesIsActiveOffset() {
            // IsActive sits at offset 0 on RecastDetail.
            RecastItem item = RecastItemMapper.Build();
            Assert.Equal(0, item.IsAvailable);
        }

        [Fact]
        public void Build_ItemSize_MatchesRecastDetailSize() {
            RecastItem item = RecastItemMapper.Build();
            Assert.Equal(Marshal.SizeOf<RecastDetail>(), item.ItemSize);
        }

        [Fact]
        public void Build_ContainerSize_Is80TimesItemSize() {
            // ActionManager._cooldowns is FixedSizeArray80<RecastDetail>.
            RecastItem item = RecastItemMapper.Build();
            Assert.Equal(Marshal.SizeOf<RecastDetail>() * 80, item.ContainerSize);
        }

        [Fact]
        public void Build_UnmappedFields_StayZero() {
            // These Sharlayan fields correspond to UI-level concepts that live on HotbarSlot,
            // not RecastDetail, and stay unmapped until a consumer proves they are needed.
            RecastItem item = RecastItemMapper.Build();
            Assert.Equal(0, item.ActionProc);
            Assert.Equal(0, item.Amount);
            Assert.Equal(0, item.Category);
            Assert.Equal(0, item.Icon);
            Assert.Equal(0, item.InRange);
            Assert.Equal(0, item.RemainingCost);
            Assert.Equal(0, item.Type);
            Assert.Equal(0, item.ChargeReady);
            Assert.Equal(0, item.ChargesRemaining);
        }
    }
}
