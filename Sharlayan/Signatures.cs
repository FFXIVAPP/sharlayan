// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Signatures.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Signatures.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System.Threading.Tasks;

    using Sharlayan.Models;
    using Sharlayan.Utilities;

    public static class Signatures {
        public const string AGRO_COUNT_KEY = "AGRO_COUNT";

        public const string AGROMAP_KEY = "AGROMAP";

        public const string CHARMAP_KEY = "CHARMAP";

        public const string CHATLOG_KEY = "CHATLOG";

        public const string ENMITY_COUNT_KEY = "ENMITY_COUNT";

        public const string ENMITYMAP_KEY = "ENMITYMAP";

        public const string HOTBAR_KEY = "HOTBAR";

        public const string INVENTORY_KEY = "INVENTORY";

        public const string JOBRESOURCES_KEY = "JOBRESOURCES";

        public const string MAPINFO_KEY = "MAPINFO";

        public const string PARTYCOUNT_KEY = "PARTYCOUNT";

        public const string PARTYMAP_KEY = "PARTYMAP";

        public const string PLAYERINFO_KEY = "PLAYERINFO";

        public const string RECAST_KEY = "RECAST";

        public const string TARGET_KEY = "TARGET";

        public const string ZONEINFO_KEY = "ZONEINFO";

        public static async Task<Signature[]> Resolve(SharlayanConfiguration configuration) {
            Signature[] signatures = await APIHelper.GetSignatures(configuration);
            return signatures;
        }
    }
}