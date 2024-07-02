// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusEffectLookup.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   StatusEffectLookup.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    using Sharlayan.Models;
    using Sharlayan.Models.XIVDatabase;

    public static class StatusEffectLookup {
        private static bool _loading;

        private static ConcurrentDictionary<uint, StatusItem> _statusEffects = new ConcurrentDictionary<uint, StatusItem>();

        private static StatusItem DefaultStatusInfo = new StatusItem {
            Name = new Localization {
                Chinese = Constants.UNKNOWN_LOCALIZED_NAME,
                English = Constants.UNKNOWN_LOCALIZED_NAME,
                French = Constants.UNKNOWN_LOCALIZED_NAME,
                German = Constants.UNKNOWN_LOCALIZED_NAME,
                Japanese = Constants.UNKNOWN_LOCALIZED_NAME,
                Korean = Constants.UNKNOWN_LOCALIZED_NAME,
            },
            CompanyAction = false,
        };

        public static StatusItem GetStatusInfo(uint id) {
            return _statusEffects.ContainsKey(id)
                       ? _statusEffects[id]
                       : DefaultStatusInfo;
        }

        internal static async Task Resolve(SharlayanConfiguration configuration) {
            if (_loading) {
                return;
            }

            _loading = true;
            await APIHelper.GetStatusEffects(_statusEffects, configuration);
            _loading = false;
        }
    }
}