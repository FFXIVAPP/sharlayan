﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoneLookup.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ZoneLookup.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System.Collections.Concurrent;
    using System.Linq;

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
                Korean = "???",
            },
            Index = 0,
            IsDungeonInstance = false,
        };

        private static bool Loading;

        private static ConcurrentDictionary<uint, MapItem> Zones = new ConcurrentDictionary<uint, MapItem>();

        public static MapItem GetZoneInfo(uint id, string patchVersion = "latest") {
            if (Loading) {
                return DefaultZoneInfo;
            }

            lock (Zones) {
                if (Zones.Any()) {
                    return Zones.ContainsKey(id)
                               ? Zones[id]
                               : DefaultZoneInfo;
                }

                Resolve(patchVersion);
                return DefaultZoneInfo;
            }
        }

        internal static void Resolve(string patchVersion = "latest") {
            if (Loading) {
                return;
            }

            Loading = true;
            APIHelper.GetZones(Zones, patchVersion);
            Loading = false;
        }
    }
}