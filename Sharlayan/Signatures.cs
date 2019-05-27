// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Signatures.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Signatures.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Sharlayan.Models;
    using Sharlayan.Utilities;

    public static class Signatures {
        public const string AgroCountKey = "AGRO_COUNT";

        public const string AgroMapKey = "AGROMAP";

        public const string CharacterMapKey = "CHARMAP";

        public const string ChatLogKey = "CHATLOG";

        public const string EnmityCountKey = "ENMITY_COUNT";

        public const string EnmityMapKey = "ENMITYMAP";

        public const string GameMainKey = "GAMEMAIN";

        public const string HotBarKey = "HOTBAR";

        public const string InventoryKey = "INVENTORY";

        public const string MapInformationKey = "MAPINFO";

        public const string PartyCountKey = "PARTYCOUNT";

        public const string PartyMapKey = "PARTYMAP";

        public const string PlayerInformationKey = "PLAYERINFO";

        public const string RecastKey = "RECAST";

        public const string TargetKey = "TARGET";

        public const string ZoneInformationKey = "ZONEINFO";

        public static async Task<IEnumerable<Signature>> Resolve(ProcessModel processModel, string patchVersion = "latest") {
            return await APIHelper.GetSignatures(processModel, patchVersion);
        }
    }
}