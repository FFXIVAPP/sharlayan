// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LuminaLookup.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Public name → row-id helpers backed by Lumina's Excel sheets. Inverse of Reader.Lumina's
//   id-to-name lookups: given a localised resource name from any client language, return the
//   universal numeric row id (Weather "Moon Dust" → 148, Action "リフルジェントアロー" → 98).
//
//   Each lookup is built lazily on first call by walking all four client languages and
//   unioning every row's display name into a single case-insensitive dictionary keyed by
//   sheet type. Subsequent calls hit the cache directly — no repeat sqpack scans.
//
//   Per-language failures (e.g. MismatchedColumnHashException during the gap between an
//   FFXIV patch and a matching Lumina.Excel package release) silently degrade: partial
//   coverage from the languages that did resolve is better than nothing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using Lumina;
    using Lumina.Data;
    using Lumina.Excel;
    using Lumina.Excel.Sheets;

    using Sharlayan.Resources.Providers;

    public static class LuminaLookup {
        // One name→row-id dictionary per sheet type. Process-wide cache (not Reader-scoped)
        // because the underlying Lumina GameData is also process-wide via LuminaGameDataCache.
        // Keyed on the typeof(T) Excel row struct.
        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, uint>> _cache =
            new ConcurrentDictionary<Type, IReadOnlyDictionary<string, uint>>();

        // FFXIV's four primary client languages. We probe each in turn and union the results
        // so callers can pass a name from any language and get the same row id back.
        private static readonly Language[] _languages = new[] {
            Language.English,
            Language.Japanese,
            Language.German,
            Language.French,
        };

        /// <summary>Translate a Weather row's localised name (e.g. "Moon Dust") to its universal RowId.</summary>
        public static uint? WeatherIdFromName(SharlayanConfiguration configuration, string name)
            => FindRowId<Weather>(configuration, name, r => r.Name.ExtractText());

        /// <summary>Translate an Action row's localised name (e.g. "リフルジェントアロー") to its universal RowId.</summary>
        public static uint? ActionIdFromName(SharlayanConfiguration configuration, string name)
            => FindRowId<Lumina.Excel.Sheets.Action>(configuration, name, r => r.Name.ExtractText());

        /// <summary>Translate a Status (status effect) row's localised name to its universal RowId.</summary>
        public static uint? StatusIdFromName(SharlayanConfiguration configuration, string name)
            => FindRowId<Status>(configuration, name, r => r.Name.ExtractText());

        /// <summary>Translate a PlaceName row's localised name (zones / fields / districts) to its universal RowId.</summary>
        public static uint? PlaceNameIdFromName(SharlayanConfiguration configuration, string name)
            => FindRowId<PlaceName>(configuration, name, r => r.Name.ExtractText());

        /// <summary>Translate a ContentFinderCondition (duty / instance / raid) localised name to its universal RowId.</summary>
        public static uint? ContentFinderConditionIdFromName(SharlayanConfiguration configuration, string name)
            => FindRowId<ContentFinderCondition>(configuration, name, r => r.Name.ExtractText());

        /// <summary>Translate an Item row's localised name to its universal RowId.</summary>
        public static uint? ItemIdFromName(SharlayanConfiguration configuration, string name)
            => FindRowId<Item>(configuration, name, r => r.Name.ExtractText());

        /// <summary>Translate a BNpcName (battle NPC / mob) row's Singular localised name to its universal RowId.</summary>
        public static uint? BNpcNameIdFromName(SharlayanConfiguration configuration, string name)
            => FindRowId<BNpcName>(configuration, name, r => r.Singular.ExtractText());

        /// <summary>Translate an ENpcResident (event NPC / friendly NPC) Singular localised name to its universal RowId.</summary>
        public static uint? ENpcResidentIdFromName(SharlayanConfiguration configuration, string name)
            => FindRowId<ENpcResident>(configuration, name, r => r.Singular.ExtractText());

        /// <summary>Translate a Mount row's Singular localised name to its universal RowId.</summary>
        public static uint? MountIdFromName(SharlayanConfiguration configuration, string name)
            => FindRowId<Mount>(configuration, name, r => r.Singular.ExtractText());

        /// <summary>Translate a Companion (minion) row's Singular localised name to its universal RowId.</summary>
        public static uint? CompanionIdFromName(SharlayanConfiguration configuration, string name)
            => FindRowId<Companion>(configuration, name, r => r.Singular.ExtractText());

        /// <summary>
        /// Lower-level entry point: given a sheet row type and a name selector, return the
        /// row id whose name matches. Useful for callers that need a sheet not exposed by
        /// the dedicated helpers above. The cache is keyed on <typeparamref name="T"/> so
        /// each unique row type gets one lookup table built across all 4 languages.
        /// </summary>
        public static uint? FindRowId<T>(SharlayanConfiguration configuration, string name, Func<T, string> nameSelector)
            where T : struct, IExcelRow<T> {
            if (string.IsNullOrEmpty(name) || configuration == null || nameSelector == null) {
                return null;
            }
            IReadOnlyDictionary<string, uint> dict = _cache.GetOrAdd(typeof(T), _ => BuildLookup(configuration, nameSelector));
            return dict.TryGetValue(name, out uint id) ? id : (uint?)null;
        }

        /// <summary>
        /// Drops the cached lookups so the next call will rebuild them. Useful if the sqpack
        /// path or game install changes mid-process. Rare in practice — most callers can
        /// ignore this.
        /// </summary>
        public static void ClearCache() => _cache.Clear();

        private static IReadOnlyDictionary<string, uint> BuildLookup<T>(SharlayanConfiguration configuration, Func<T, string> nameSelector)
            where T : struct, IExcelRow<T> {
            // OrdinalIgnoreCase — case-insensitive for Latin scripts; no-op for CJK characters
            // (they don't have case), so Japanese / Chinese names still match exactly.
            Dictionary<string, uint> dict = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
            GameData lumina = LuminaGameDataCache.GetOrNull(configuration);
            if (lumina == null) {
                return dict;
            }

            foreach (Language lang in _languages) {
                ExcelSheet<T> sheet;
                try {
                    sheet = lumina.Excel.GetSheet<T>(lang);
                }
                catch {
                    // Per-language failure (hash mismatch / schema drift / language not
                    // available in this sqpack). Skip and try the next language.
                    continue;
                }
                foreach (T row in sheet) {
                    string nm;
                    try { nm = nameSelector(row); }
                    catch { continue; }
                    if (string.IsNullOrEmpty(nm)) {
                        continue;
                    }
                    // First occurrence wins. In practice the same display name across
                    // languages points at the same RowId for almost every sheet, so this is
                    // a stable choice; rare cross-language collisions get the earliest
                    // language's row.
                    if (!dict.ContainsKey(nm)) {
                        dict[nm] = row.RowId;
                    }
                }
            }
            return dict;
        }
    }
}
