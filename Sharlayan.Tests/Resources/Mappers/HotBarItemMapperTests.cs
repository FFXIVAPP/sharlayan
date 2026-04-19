// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotBarItemMapperTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.UI.Misc;

    using Sharlayan.Models.Structures;
    using Sharlayan.Resources.Mappers;

    using Xunit;

    public class HotBarItemMapperTests {
        [Fact]
        public void Build_ID_MatchesCommandIdOffset() {
            HotBarItem item = HotBarItemMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<RaptureHotbarModule.HotbarSlot>(nameof(RaptureHotbarModule.HotbarSlot.CommandId)), item.ID);
        }

        [Fact]
        public void Build_ContainerSize_MatchesHotbarSize() {
            HotBarItem item = HotBarItemMapper.Build();
            Assert.Equal(Marshal.SizeOf<RaptureHotbarModule.Hotbar>(), item.ContainerSize);
        }

        [Fact]
        public void Build_ItemSize_MatchesHotbarSlotSize() {
            HotBarItem item = HotBarItemMapper.Build();
            Assert.Equal(Marshal.SizeOf<RaptureHotbarModule.HotbarSlot>(), item.ItemSize);
        }

        [Fact]
        public void Build_Name_AtOffsetZero_PopUpHelpIsFirstField() {
            // PopUpHelp is at native offset 0 on HotbarSlot.
            HotBarItem item = HotBarItemMapper.Build();
            Assert.Equal(0, item.Name);
        }

        [Fact]
        public void Build_KeyBinds_NonZero() {
            HotBarItem item = HotBarItemMapper.Build();
            Assert.True(item.KeyBinds > 0, nameof(item.KeyBinds));
        }
    }
}
