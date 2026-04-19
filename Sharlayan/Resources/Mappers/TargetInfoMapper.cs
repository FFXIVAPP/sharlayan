// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetInfoMapper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Maps Sharlayan's TargetInfo to FFXIVClientStructs' Client::Game::Control::TargetSystem.
//   Each "target" field on Sharlayan.TargetInfo holds the offset of the corresponding
//   GameObject* pointer within TargetSystem; consumer code dereferences the pointer to
//   read the actor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game.Control;

    using Sharlayan.Models.Structures;

    internal static class TargetInfoMapper {
        public static TargetInfo Build() {
            return new TargetInfo {
                Current = (int)Marshal.OffsetOf<TargetSystem>(nameof(TargetSystem.Target)),
                CurrentID = (int)Marshal.OffsetOf<TargetSystem>(nameof(TargetSystem.TargetObjectId)),
                MouseOver = (int)Marshal.OffsetOf<TargetSystem>(nameof(TargetSystem.MouseOverTarget)),
                Focus = (int)Marshal.OffsetOf<TargetSystem>(nameof(TargetSystem.FocusTarget)),
                Previous = (int)Marshal.OffsetOf<TargetSystem>(nameof(TargetSystem.PreviousTarget)),
                SourceSize = Marshal.SizeOf<TargetSystem>(),

                // "Size" has no clean equivalent in TargetSystem — unclear what Sharlayan's
                // historical value referred to. Leaving at 0 preserves the unmapped default.
            };
        }
    }
}
