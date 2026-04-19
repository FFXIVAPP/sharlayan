// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotBarItemMapper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Maps Sharlayan's HotBarItem against a single hotbar slot in
//   FFXIVClientStructs.FFXIV.Client.UI.Misc.RaptureHotbarModule.HotbarSlot. ContainerSize
//   reports the full 16-slot hotbar size; ItemSize is the per-slot struct size.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.UI.Misc;

    using Sharlayan.Models.Structures;

    internal static class HotBarItemMapper {
        public static HotBarItem Build() {
            return new HotBarItem {
                ContainerSize = Marshal.SizeOf<RaptureHotbarModule.Hotbar>(),
                ItemSize = Marshal.SizeOf<RaptureHotbarModule.HotbarSlot>(),
                ID = (int)Marshal.OffsetOf<RaptureHotbarModule.HotbarSlot>(nameof(RaptureHotbarModule.HotbarSlot.CommandId)),

                // Closest equivalent to "Name" is PopUpHelp — the tooltip string shown when
                // hovering a slot.
                Name = (int)Marshal.OffsetOf<RaptureHotbarModule.HotbarSlot>(nameof(RaptureHotbarModule.HotbarSlot.PopUpHelp)),

                // KeyBinds maps to the internal _keybindHint FixedSizeArray (terse variant
                // without the leading space/brackets); consumer code reads from that offset.
                KeyBinds = FieldOffsetReader.OffsetOf<RaptureHotbarModule.HotbarSlot>("_keybindHint"),
            };
        }
    }
}
