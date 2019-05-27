// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoneLookup.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ZoneLookup.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;

    using Sharlayan.Models;
    using Sharlayan.Models.XIVDatabase;

    public static class ZoneLookup {
        private static MapItem DefaultZoneInfo = new MapItem {
            Name = new Localization {
                Chinese = "???",
                English = "???",
                French = "???",
                German = "???",
                Japanese = "???",
                Korean = "???"
            },
            Index = 0,
            IsDungeonInstance = false
        };

        private static bool Loading;

        private static ConcurrentDictionary<uint, MapItem> Zones = new ConcurrentDictionary<uint, MapItem>();

        public static async Task<MapItem> GetZoneInfo(uint id, string patchVersion = "latest") {
            if (Loading) {
                return DefaultZoneInfo;
            }

            if (Zones.Any()) {
                return Zones.ContainsKey(id)
                           ? Zones[id]
                           : DefaultZoneInfo;
            }

            await Resolve(patchVersion);
            return DefaultZoneInfo;
        }

        internal static async Task Resolve(string patchVersion = "latest") {
            if (Loading) {
                return;
            }

            Loading = true;
            await APIHelper.GetZones(Zones, patchVersion);
            Loading = false;
        }
    }
}