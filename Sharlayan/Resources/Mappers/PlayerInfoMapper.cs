// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerInfoMapper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Partial PlayerInfo mapper. Sharlayan's PlayerInfo has ~100 fields spanning job
//   levels, job experience, raw base stats, derived attributes (Strength, Attack Power,
//   Crit Rate, etc.), and HP/MP/CP/GP caps. We map only what has a clean, stable
//   FFXIVClientStructs equivalent:
//
//     - The six BaseX fields (BaseStrength/Dexterity/Vitality/Intelligence/Mind/Piety)
//       live directly on PlayerState.
//     - JobID comes from PlayerState.CurrentClassJobId.
//
//   The individual per-job level fields (ACN, ALC, ...) and *_CurrentEXP fields require
//   the ExpArrayIndex from each job's ClassJob sheet row — that's dynamic Lumina data,
//   not a compile-time struct layout. They stay unmapped here; consumers that need them
//   should read from PlayerState._classJobLevels / _classJobExperience with the correct
//   ExpArrayIndex. Same for derived attributes which live at per-BaseParam indices
//   inside PlayerState._attributes.
//
//   SourceSize = sizeof(PlayerState), so consumers who just want "read the whole
//   PlayerState buffer" still get the right byte count.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game.UI;

    using Sharlayan.Models.Structures;

    internal static class PlayerInfoMapper {
        public static PlayerInfo Build() {
            return new PlayerInfo {
                JobID = (int)Marshal.OffsetOf<PlayerState>(nameof(PlayerState.CurrentClassJobId)),
                BaseStrength = (int)Marshal.OffsetOf<PlayerState>(nameof(PlayerState.BaseStrength)),
                BaseDexterity = (int)Marshal.OffsetOf<PlayerState>(nameof(PlayerState.BaseDexterity)),
                BaseVitality = (int)Marshal.OffsetOf<PlayerState>(nameof(PlayerState.BaseVitality)),
                BaseIntelligence = (int)Marshal.OffsetOf<PlayerState>(nameof(PlayerState.BaseIntelligence)),
                BaseMind = (int)Marshal.OffsetOf<PlayerState>(nameof(PlayerState.BaseMind)),
                BasePiety = (int)Marshal.OffsetOf<PlayerState>(nameof(PlayerState.BasePiety)),
                SourceSize = Marshal.SizeOf<PlayerState>(),
            };
        }
    }
}
