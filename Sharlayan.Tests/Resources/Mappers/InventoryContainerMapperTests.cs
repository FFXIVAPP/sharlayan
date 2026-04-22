// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InventoryContainerMapperTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game;

    using Sharlayan.Resources.Mappers;

    using Xunit;

    using NativeInventoryContainer = FFXIVClientStructs.FFXIV.Client.Game.InventoryContainer;
    using SharlayanInventoryContainer = Sharlayan.Models.Structures.InventoryContainer;

    public class InventoryContainerMapperTests {
        [Fact]
        public void Build_ID_MatchesTypeOffset() {
            SharlayanInventoryContainer container = InventoryContainerMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<NativeInventoryContainer>(nameof(NativeInventoryContainer.Type)), container.ID);
        }

        [Fact]
        public void Build_Amount_MatchesSizeOffset() {
            SharlayanInventoryContainer container = InventoryContainerMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<NativeInventoryContainer>(nameof(NativeInventoryContainer.Size)), container.Amount);
        }

        [Fact]
        public void Build_AllFields_NonZero() {
            SharlayanInventoryContainer container = InventoryContainerMapper.Build();
            Assert.True(container.ID > 0, nameof(container.ID));
            Assert.True(container.Amount > 0, nameof(container.Amount));
        }
    }
}
