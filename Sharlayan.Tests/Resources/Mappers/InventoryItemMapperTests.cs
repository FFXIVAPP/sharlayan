// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InventoryItemMapperTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game;

    using Sharlayan.Resources.Mappers;

    using Xunit;

    using NativeInventoryItem = FFXIVClientStructs.FFXIV.Client.Game.InventoryItem;
    using SharlayanInventoryItem = Sharlayan.Models.Structures.InventoryItem;

    public class InventoryItemMapperTests {
        [Fact]
        public void Build_ID_MatchesItemIdOffset() {
            SharlayanInventoryItem item = InventoryItemMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<NativeInventoryItem>(nameof(NativeInventoryItem.ItemId)), item.ID);
        }

        [Fact]
        public void Build_Amount_MatchesQuantityOffset() {
            SharlayanInventoryItem item = InventoryItemMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<NativeInventoryItem>(nameof(NativeInventoryItem.Quantity)), item.Amount);
        }

        [Fact]
        public void Build_GlamourID_MatchesGlamourIdOffset() {
            SharlayanInventoryItem item = InventoryItemMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<NativeInventoryItem>(nameof(NativeInventoryItem.GlamourId)), item.GlamourID);
        }

        [Fact]
        public void Build_AllFields_NonZero() {
            SharlayanInventoryItem item = InventoryItemMapper.Build();
            Assert.True(item.Slot > 0, nameof(item.Slot));
            Assert.True(item.ID > 0, nameof(item.ID));
            Assert.True(item.Amount > 0, nameof(item.Amount));
            Assert.True(item.SB > 0, nameof(item.SB));
            Assert.True(item.Durability > 0, nameof(item.Durability));
            Assert.True(item.IsHQ > 0, nameof(item.IsHQ));
            Assert.True(item.MateriaType > 0, nameof(item.MateriaType));
            Assert.True(item.MateriaRank > 0, nameof(item.MateriaRank));
            Assert.True(item.DyeID > 0, nameof(item.DyeID));
            Assert.True(item.GlamourID > 0, nameof(item.GlamourID));
        }
    }
}
