// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FFXIVClientStructsDirectProviderTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Providers {
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    using FFXIVClientStructs.FFXIV.Client.Game.Character;

    using Sharlayan;
    using Sharlayan.Resources.Providers;

    using Xunit;

    public class FFXIVClientStructsDirectProviderTests {
        [Fact]
        public async Task GetStructuresAsync_ActorItem_HPCurrentMatchesCharacterDataHealthOffset() {
            // The Sharlayan ActorItem.HPCurrent int offset must equal the raw byte offset of
            // CharacterData.Health inside CharacterData. If FFXIVClientStructs renames or
            // relocates the Health field, this test fails — by design, because downstream
            // consumers reading HP would break.
            FFXIVClientStructsDirectProvider provider = new FFXIVClientStructsDirectProvider();
            SharlayanConfiguration configuration = new SharlayanConfiguration();

            Models.Structures.StructuresContainer container = await provider.GetStructuresAsync(configuration);

            int expectedHealthOffset = (int)Marshal.OffsetOf<CharacterData>(nameof(CharacterData.Health));

            Assert.NotNull(container);
            Assert.NotNull(container.ActorItem);
            Assert.Equal(expectedHealthOffset, container.ActorItem.HPCurrent);
        }

        [Fact]
        public async Task GetStructuresAsync_ActorItem_HPMaxMatchesCharacterDataMaxHealthOffset() {
            FFXIVClientStructsDirectProvider provider = new FFXIVClientStructsDirectProvider();
            SharlayanConfiguration configuration = new SharlayanConfiguration();

            Models.Structures.StructuresContainer container = await provider.GetStructuresAsync(configuration);

            int expectedMaxHealthOffset = (int)Marshal.OffsetOf<CharacterData>(nameof(CharacterData.MaxHealth));

            Assert.Equal(expectedMaxHealthOffset, container.ActorItem.HPMax);
        }

        [Fact]
        public async Task GetStructuresAsync_ActorItem_MPCurrentMatchesCharacterDataManaOffset() {
            FFXIVClientStructsDirectProvider provider = new FFXIVClientStructsDirectProvider();
            SharlayanConfiguration configuration = new SharlayanConfiguration();

            Models.Structures.StructuresContainer container = await provider.GetStructuresAsync(configuration);

            int expectedManaOffset = (int)Marshal.OffsetOf<CharacterData>(nameof(CharacterData.Mana));

            Assert.Equal(expectedManaOffset, container.ActorItem.MPCurrent);
        }

        [Fact]
        public async Task GetStructuresAsync_ActorItem_OffsetsAreNonZero() {
            // Sanity: CharacterData fields live at non-zero offsets — if the mapper returned
            // zeros something silently broke in the FFXIVClientStructs reference or layout.
            FFXIVClientStructsDirectProvider provider = new FFXIVClientStructsDirectProvider();
            SharlayanConfiguration configuration = new SharlayanConfiguration();

            Models.Structures.StructuresContainer container = await provider.GetStructuresAsync(configuration);

            Assert.True(container.ActorItem.HPCurrent > 0, "HPCurrent must be a positive byte offset.");
            Assert.True(container.ActorItem.HPMax > 0, "HPMax must be a positive byte offset.");
            Assert.True(container.ActorItem.MPCurrent > 0, "MPCurrent must be a positive byte offset.");
        }
    }
}
