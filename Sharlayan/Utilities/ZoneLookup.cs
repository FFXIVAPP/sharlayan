// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoneLookup.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ZoneLookup.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System.Collections.Concurrent;

    using Sharlayan.Models;
    using Sharlayan.Models.XIVDatabase;

    public static class ZoneLookup {
        private static bool _loading;

        private static ConcurrentDictionary<uint, MapItem> _zones = new ConcurrentDictionary<uint, MapItem>();

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

        public static MapItem GetZoneInfo(uint id) {
            return _zones.ContainsKey(id)
                       ? _zones[id]
                       : DefaultZoneInfo;
        }

        internal static void Resolve(SharlayanConfiguration configuration) {
            if (_loading) {
                return;
            }

            _loading = true;
            APIHelper.GetZones(_zones, configuration);
            _loading = false;
        }
    }
}