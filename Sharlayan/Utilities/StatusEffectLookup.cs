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
    using System.Threading;
    using System.Threading.Tasks;

    using Sharlayan.Models;
    using Sharlayan.Models.XIVDatabase;
    using Sharlayan.Resources;

    public static class StatusEffectLookup {
        // F81: 0 = idle, 1 = in flight. Interlocked so concurrent callers can't
        // double-load; try/finally so a provider exception (e.g. Lumina throwing
        // during the patch/Lumina-release window) can't wedge the flag and
        // permanently no-op every future resolve.
        private static int _loading;

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
            if (Interlocked.CompareExchange(ref _loading, 1, 0) != 0) {
                return;
            }

            try {
                IResourceProvider provider = ResourceProviderFactory.Create(configuration);
                await provider.GetStatusEffectsAsync(_statusEffects, configuration);
            }
            finally {
                Interlocked.Exchange(ref _loading, 0);
            }
        }
    }
}