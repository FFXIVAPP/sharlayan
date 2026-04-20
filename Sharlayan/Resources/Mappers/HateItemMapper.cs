// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HateItemMapper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Maps Sharlayan's HateItem against FFXIVClientStructs' Client::Game::UI::HateInfo —
//   the per-entry struct inside Hate._hateInfo (target enmity list). No Name field;
//   Reader.Target resolves names via actor lookup after reading ID + Enmity.
//   Compare EnmityItemMapper which maps HaterInfo (player aggro list, has Name).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game.UI;

    using Sharlayan.Models.Structures;

    internal static class HateItemMapper {
        public static HateItem Build() {
            return new HateItem {
                ID       = (int)Marshal.OffsetOf<HateInfo>(nameof(HateInfo.EntityId)),
                Enmity   = (int)Marshal.OffsetOf<HateInfo>(nameof(HateInfo.Enmity)),
                SourceSize = Marshal.SizeOf<HateInfo>(),
            };
        }
    }
}
