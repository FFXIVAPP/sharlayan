// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnmityItemMapper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Maps Sharlayan's EnmityItem against FFXIVClientStructs' Client::Game::UI::HaterInfo —
//   the per-entry struct inside Hater._haters. Includes a Name field so this is the
//   richer variant; for simple hate-only lists see HateInfo (no Name).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game.UI;

    using Sharlayan.Models.Structures;

    internal static class EnmityItemMapper {
        public static EnmityItem Build() {
            return new EnmityItem {
                Name = FieldOffsetReader.OffsetOf<HaterInfo>("_name"),
                ID = (int)Marshal.OffsetOf<HaterInfo>(nameof(HaterInfo.EntityId)),
                Enmity = (int)Marshal.OffsetOf<HaterInfo>(nameof(HaterInfo.Enmity)),
                SourceSize = Marshal.SizeOf<HaterInfo>(),

                // EnmityCount was historically the offset of the parent list's count field
                // (Hater.HaterCount at 0x900 or Hate.HateArrayLength at 0x100). It does not
                // sit inside HaterInfo itself. Leaving at 0 preserves the "missing-from-JSON"
                // baseline; consumers that need the count should read from Hater directly.
            };
        }
    }
}
