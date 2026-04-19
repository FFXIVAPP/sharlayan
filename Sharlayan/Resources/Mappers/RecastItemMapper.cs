// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecastItemMapper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Maps Sharlayan's RecastItem against FFXIVClientStructs'
//   Client::Game::ActionManager::RecastDetail (each entry of ActionManager._cooldowns).
//   RecastDetail is a lean 0x14 struct with only IsActive / ActionId / Elapsed / Total —
//   most of Sharlayan's UI-flavoured RecastItem fields (Icon, InRange, ChargeReady, etc.)
//   live on the HotbarSlot side, not here. They stay unmapped.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game;

    using Sharlayan.Models.Structures;

    internal static class RecastItemMapper {
        public static RecastItem Build() {
            return new RecastItem {
                // ActionManager._cooldowns is a FixedSizeArray80<RecastDetail>.
                ContainerSize = Marshal.SizeOf<RecastDetail>() * 80,
                ItemSize = Marshal.SizeOf<RecastDetail>(),
                ID = (int)Marshal.OffsetOf<RecastDetail>(nameof(RecastDetail.ActionId)),
                IsAvailable = (int)Marshal.OffsetOf<RecastDetail>(nameof(RecastDetail.IsActive)),

                // Elapsed is a float. Sharlayan's historical CoolDownPercent was not a
                // literal percent either — consumers computed 100 * Elapsed / Total.
                CoolDownPercent = (int)Marshal.OffsetOf<RecastDetail>(nameof(RecastDetail.Elapsed)),

                // ActionProc, Amount, Category, Icon, InRange, RemainingCost, Type,
                // ChargeReady, ChargesRemaining have no direct RecastDetail equivalents —
                // those concepts live on HotbarSlot. Stay at default(int)=0.
            };
        }
    }
}
