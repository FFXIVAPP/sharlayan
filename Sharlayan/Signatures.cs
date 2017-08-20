// Sharlayan ~ Signatures.cs
// 
// Copyright © 2007 - 2017 Ryan Wilson - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using Sharlayan.Helpers;
using Sharlayan.Models;

namespace Sharlayan
{
    public static class Signatures
    {
        #region Locations Keys
        
        public static string AgroCountKey = "AGRO_COUNT";
        public static string AgroMapKey = "AGROMAP";
        public static string CharacterMapKey = "CHARMAP";
        public static string ChatLogKey = "CHATLOG";
        public static string EnmityCountKey = "ENMITY_COUNT";
        public static string EnmityMapKey = "ENMITYMAP";
        public static string GameMainKey = "GAMEMAIN";
        public static string HotBarKey = "HOTBAR";
        public static string InventoryKey = "INVENTORY";
        public static string MapInformationKey = "MAPINFO";
        public static string PartyCountKey = "PARTYCOUNT";
        public static string PartyMapKey = "PARTYMAP";
        public static string PlayerInformationKey = "PLAYERINFO";
        public static string RecastKey = "RECAST";
        public static string TargetKey = "TARGET";
        public static string ZoneInformationKey = "ZONEINFO";
        
        #endregion

        public static IEnumerable<Signature> Resolve(ProcessModel processModel, string patchVersion = "latest")
        {
            return APIHelper.GetSignatures(processModel, patchVersion);
        }
    }
}
