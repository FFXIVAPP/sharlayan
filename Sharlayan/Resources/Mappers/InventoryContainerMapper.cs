// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InventoryContainerMapper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Maps Sharlayan's InventoryContainer against FFXIVClientStructs' InventoryContainer.
//   Sharlayan's "ID" holds the container's type enum offset; "Amount" holds its Size.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game;

    using Sharlayan.Models.Structures;

    using NativeInventoryContainer = FFXIVClientStructs.FFXIV.Client.Game.InventoryContainer;
    using SharlayanInventoryContainer = Sharlayan.Models.Structures.InventoryContainer;

    internal static class InventoryContainerMapper {
        public static SharlayanInventoryContainer Build() {
            return new SharlayanInventoryContainer {
                ID = (int)Marshal.OffsetOf<NativeInventoryContainer>(nameof(NativeInventoryContainer.Type)),
                Amount = (int)Marshal.OffsetOf<NativeInventoryContainer>(nameof(NativeInventoryContainer.Size)),
            };
        }
    }
}
