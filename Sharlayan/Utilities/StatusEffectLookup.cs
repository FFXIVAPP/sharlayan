// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusEffectLookup.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   StatusEffectLookup.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;

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
                Korean = "???"
            },
            CompanyAction = false
        };

        private static bool Loading;

        private static ConcurrentDictionary<uint, StatusItem> StatusEffects = new ConcurrentDictionary<uint, StatusItem>();

        public static async Task<StatusItem> GetStatusInfo(uint id, string patchVersion = "latest") {
            if (Loading) {
                return DefaultStatusInfo;
            }

            if (StatusEffects.Any()) {
                return StatusEffects.ContainsKey(id)
                           ? StatusEffects[id]
                           : DefaultStatusInfo;
            }

            await Resolve(patchVersion);
            return DefaultStatusInfo;
        }

        internal static async Task Resolve(string patchVersion = "latest") {
            if (Loading) {
                return;
            }

            Loading = true;
            await APIHelper.GetStatusEffects(StatusEffects, patchVersion);
            Loading = false;
        }
    }
}