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

    using Sharlayan.Models;
    using Sharlayan.Models.XIVDatabase;

    public static class StatusEffectLookup {
        private static bool _loading;

        private static ConcurrentDictionary<uint, StatusItem> _statusEffects = new ConcurrentDictionary<uint, StatusItem>();

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

        public static StatusItem GetStatusInfo(uint id) {
            return _statusEffects.ContainsKey(id)
                       ? _statusEffects[id]
                       : DefaultStatusInfo;
        }

        internal static void Resolve(MemoryHandlerConfiguration configuration) {
            if (_loading) {
                return;
            }

            _loading = true;
            APIHelper.GetStatusEffects(_statusEffects, configuration.PatchVersion, configuration.UseLocalCache, configuration.APIBaseURL);
            _loading = false;
        }
    }
}