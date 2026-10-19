// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResources.Beastmaster.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2026 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResources.Beastmaster.cs Implementation — byte offsets into FFXIVClientStructs'
//   BeastmasterGauge (added in game patch 7.57).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public partial class JobResources {
        public sealed class BeastmasterResources {
            public int ActiveBattlehorn { get; set; }

            public int ChainCount { get; set; }

            public int CurrentAffinity { get; set; }

            public int InstinctState { get; set; }

            public int InstinctualComboState { get; set; }

            public int KinshipState { get; set; }

            public int PetTPAtLastUse { get; set; }

            public int PetTPGauge { get; set; }

            public int TPGauge { get; set; }
        }
    }
}
