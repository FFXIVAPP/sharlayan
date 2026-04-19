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

    using FFXIVClientStructs.FFXIV.Client.System.String;
    using FFXIVClientStructs.FFXIV.Client.UI.Misc;

    using Sharlayan.Models.Structures;

    internal static class HotBarItemMapper {
        public static HotBarItem Build() {
            // ContainerSize: FCS's Hotbar struct declares size 0x08 (vtable only); the actual
            // in-memory stride is 16 slots × HotbarSlot size. Compute from slot size so this
            // stays correct if HotbarSlot grows.
            int slotSize = Marshal.SizeOf<RaptureHotbarModule.HotbarSlot>();

            // Name: HotbarSlot.PopUpHelp is a Utf8String (pointer-based C++ object) at slot+0x00.
            // Its first field is a pointer (StringPtr), so reading bytes at offset 0 yields the
            // pointer's raw bytes, not the string. The inline char buffer lives at
            // Utf8String._inlineBuffer @ +0x22 — reading from there gives the null-terminated
            // action name for short strings (<= 64 chars, which covers every action name).
            int popUpHelpOff = (int)Marshal.OffsetOf<RaptureHotbarModule.HotbarSlot>(nameof(RaptureHotbarModule.HotbarSlot.PopUpHelp));
            int inlineBufOff = FieldOffsetReader.OffsetOf<Utf8String>("_inlineBuffer");

            return new HotBarItem {
                ContainerSize = 16 * slotSize,
                ItemSize = slotSize,
                ID = (int)Marshal.OffsetOf<RaptureHotbarModule.HotbarSlot>(nameof(RaptureHotbarModule.HotbarSlot.CommandId)),
                Name = popUpHelpOff + inlineBufOff,

                // KeyBinds maps to _keybindHint — an inline 16-byte FixedSizeArray of bytes
                // (not behind a Utf8String pointer), so direct byte reads work.
                KeyBinds = FieldOffsetReader.OffsetOf<RaptureHotbarModule.HotbarSlot>("_keybindHint"),
            };
        }
    }
}
