// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusItemMapper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Maps Sharlayan's Models.Structures.StatusItem against FFXIVClientStructs'
//   Client::Game::Status struct — each entry in StatusManager._status[60].
//   StatusItem.Stacks is filled from the Param field (for debuffs, Param is the stack
//   count; for food/potions it's the item ID — caller discriminates).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game;

    using Sharlayan.Models.Structures;

    internal static class StatusItemMapper {
        public static StatusItem Build() {
            return new StatusItem {
                StatusID = (int)Marshal.OffsetOf<Status>(nameof(Status.StatusId)),
                Stacks = (int)Marshal.OffsetOf<Status>(nameof(Status.Param)),
                Duration = (int)Marshal.OffsetOf<Status>(nameof(Status.RemainingTime)),
                CasterID = (int)Marshal.OffsetOf<Status>(nameof(Status.SourceObject)),
                SourceSize = Marshal.SizeOf<Status>(),
            };
        }
    }
}
