// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InventoryItemMapper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Maps Sharlayan's InventoryItem against FFXIVClientStructs' Client::Game::InventoryItem.
//   Sharlayan's DyeID/MateriaType/MateriaRank carry the offset of the FIRST element of
//   the corresponding FixedSizeArray — consumer code walks from there.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game;

    using Sharlayan.Models.Structures;

    using NativeInventoryItem = FFXIVClientStructs.FFXIV.Client.Game.InventoryItem;
    using SharlayanInventoryItem = Sharlayan.Models.Structures.InventoryItem;

    internal static class InventoryItemMapper {
        public static SharlayanInventoryItem Build() {
            return new SharlayanInventoryItem {
                Slot = (int)Marshal.OffsetOf<NativeInventoryItem>(nameof(NativeInventoryItem.Slot)),
                ID = (int)Marshal.OffsetOf<NativeInventoryItem>(nameof(NativeInventoryItem.ItemId)),
                Amount = (int)Marshal.OffsetOf<NativeInventoryItem>(nameof(NativeInventoryItem.Quantity)),
                SB = (int)Marshal.OffsetOf<NativeInventoryItem>(nameof(NativeInventoryItem.SpiritbondOrCollectability)),
                Durability = (int)Marshal.OffsetOf<NativeInventoryItem>(nameof(NativeInventoryItem.Condition)),

                // Flags is a byte bitfield; bit 0 = HighQuality. Sharlayan historically
                // stored a single int offset here and bit-checked the byte.
                IsHQ = (int)Marshal.OffsetOf<NativeInventoryItem>(nameof(NativeInventoryItem.Flags)),

                // FixedSizeArray backing fields — offsets point at the first element.
                MateriaType = FieldOffsetReader.OffsetOf<NativeInventoryItem>("_materia"),
                MateriaRank = FieldOffsetReader.OffsetOf<NativeInventoryItem>("_materiaGrades"),
                DyeID = FieldOffsetReader.OffsetOf<NativeInventoryItem>("_stains"),

                GlamourID = (int)Marshal.OffsetOf<NativeInventoryItem>(nameof(NativeInventoryItem.GlamourId)),
            };
        }
    }
}
