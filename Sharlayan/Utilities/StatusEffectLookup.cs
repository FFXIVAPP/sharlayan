// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusEffectLookup.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   StatusEffectLookup.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System.Collections.Concurrent;
    using System.Linq;

    using Sharlayan.Models;
    using Sharlayan.Models.XIVDatabase;

    public static class StatusEffectLookup {
        private static StatusItem DefaultStatusInfo = new StatusItem {
            Name = new Localization {
                Chinese = "???",
                English = "???",
                French = "???",
                German = "???",
                Japanese = "???",
                Korean = "???",
            },
            CompanyAction = false,
        };

        private static bool Loading;

        private static ConcurrentDictionary<uint, StatusItem> StatusEffects = new ConcurrentDictionary<uint, StatusItem>();

        public static StatusItem GetStatusInfo(uint id, string patchVersion = "latest", bool useLocalCache = true) {
            if (Loading) {
                return DefaultStatusInfo;
            }

            lock (StatusEffects) {
                if (StatusEffects.Any()) {
                    return StatusEffects.ContainsKey(id)
                               ? StatusEffects[id]
                               : DefaultStatusInfo;
                }

                Resolve(patchVersion, useLocalCache);
                return DefaultStatusInfo;
            }
        }

        internal static void Resolve(string patchVersion = "latest", bool useLocalCache = true) {
            if (Loading) {
                return;
            }

            Loading = true;
            APIHelper.GetStatusEffects(StatusEffects, patchVersion, useLocalCache);
            Loading = false;
        }
    }
}