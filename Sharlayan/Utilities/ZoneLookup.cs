// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoneLookup.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ZoneLookup.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    using Sharlayan.Models;
    using Sharlayan.Models.XIVDatabase;
    using Sharlayan.Resources;

    public static class ZoneLookup {
        // F81: 0 = idle, 1 = in flight. Interlocked so concurrent callers can't
        // double-load; try/finally so a provider exception (e.g. Lumina throwing
        // during the patch/Lumina-release window) can't wedge the flag and
        // permanently no-op every future resolve.
        private static int _loading;

        private static ConcurrentDictionary<uint, MapItem> _zones = new ConcurrentDictionary<uint, MapItem>();

        private static MapItem DefaultZoneInfo = new MapItem {
            Name = new Localization {
                Chinese = Constants.UNKNOWN_LOCALIZED_NAME,
                English = Constants.UNKNOWN_LOCALIZED_NAME,
                French = Constants.UNKNOWN_LOCALIZED_NAME,
                German = Constants.UNKNOWN_LOCALIZED_NAME,
                Japanese = Constants.UNKNOWN_LOCALIZED_NAME,
                Korean = Constants.UNKNOWN_LOCALIZED_NAME,
            },
            Index = 0,
            IsDungeonInstance = false,
        };

        public static MapItem GetZoneInfo(uint id) {
            return _zones.ContainsKey(id)
                       ? _zones[id]
                       : DefaultZoneInfo;
        }

        internal static async Task Resolve(SharlayanConfiguration configuration) {
            if (Interlocked.CompareExchange(ref _loading, 1, 0) != 0) {
                return;
            }

            try {
                IResourceProvider provider = ResourceProviderFactory.Create(configuration);
                await provider.GetZonesAsync(_zones, configuration);
            }
            finally {
                Interlocked.Exchange(ref _loading, 0);
            }
        }
    }
}