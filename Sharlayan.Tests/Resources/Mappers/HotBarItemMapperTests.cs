// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotBarItemMapperTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.System.String;
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
        public void Build_ContainerSize_Is16SlotsOfHotbarSlotSize() {
            // FCS's Hotbar struct declares size 0x08 (vtable only); the actual in-memory
            // stride is 16 × HotbarSlot. ContainerSize must be computed from slot size so
            // bar indexing (RECAST_KEY + type × ContainerSize) lands on the right bar.
            HotBarItem item = HotBarItemMapper.Build();
            Assert.Equal(16 * Marshal.SizeOf<RaptureHotbarModule.HotbarSlot>(), item.ContainerSize);
        }

        [Fact]
        public void Build_ItemSize_MatchesHotbarSlotSize() {
            HotBarItem item = HotBarItemMapper.Build();
            Assert.Equal(Marshal.SizeOf<RaptureHotbarModule.HotbarSlot>(), item.ItemSize);
        }

        [Fact]
        public void Build_Name_PointsAtPopUpHelpInlineBuffer() {
            // PopUpHelp is a Utf8String at slot+0x00; reading bytes there returns the
            // StringPtr bytes, not the action name. Name must point at _inlineBuffer inside
            // the Utf8String (+0x22 into it) so GetStringFromBytes reads the inline chars.
            HotBarItem item = HotBarItemMapper.Build();
            int expected = (int)Marshal.OffsetOf<RaptureHotbarModule.HotbarSlot>(nameof(RaptureHotbarModule.HotbarSlot.PopUpHelp))
                           + Sharlayan.Resources.Mappers.FieldOffsetReader.OffsetOf<Utf8String>("_inlineBuffer");
            Assert.Equal(expected, item.Name);
        }

        [Fact]
        public void Build_KeyBinds_PointsAtPopUpKeybindHint() {
            // _popUpKeybindHint is the human-readable " [Ctrl+Alt+0]" variant; _keybindHint is
            // a packed-binary encoding that yields garbage when read as a string. The Reader
            // splits on '+' so we need the readable one.
            HotBarItem item = HotBarItemMapper.Build();
            int expected = Sharlayan.Resources.Mappers.FieldOffsetReader.OffsetOf<RaptureHotbarModule.HotbarSlot>("_popUpKeybindHint");
            Assert.Equal(expected, item.KeyBinds);
        }
    }
}
