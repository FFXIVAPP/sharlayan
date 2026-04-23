// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LuminaGameDataCache.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Process-wide cache of Lumina <see cref="Lumina.GameData"/> instances keyed on sqpack path.
//   Opening a GameData loads the sqpack indexes (tens of MB) — every consumer that wants
//   Excel data (Reader.GameState's per-frame weather/BGM lookups, Reader.Lumina's name
//   helpers, LuminaXivDatabaseProvider's bulk actions/statuses/zones load) must share a
//   single instance per sqpack directory.
//
//   The cached GameData is constructed with tuned <see cref="Lumina.LuminaOptions"/>:
//     - CacheFileResources = false   : Sharlayan only reads Excel sheets; disable Lumina's
//                                       raw-file blob cache which otherwise grows unbounded.
//     - PanicOnSheetChecksumMismatch = false : tolerate patch-day checksum drift rather than
//                                       throwing and killing a whole call chain.
//     - LoadMultithreaded = true     : parallelise the initial sqpack index load.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Providers {
    using System;
    using System.Collections.Concurrent;
    using System.IO;

    using Lumina;

    internal static class LuminaGameDataCache {
        private static readonly ConcurrentDictionary<string, GameData> _cache = new ConcurrentDictionary<string, GameData>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Returns the cached <see cref="GameData"/> for the configuration's sqpack path,
        /// or <c>null</c> if the path can't be resolved or GameData construction fails.
        /// Used by per-frame / best-effort callers that must degrade gracefully.
        /// </summary>
        public static GameData GetOrNull(SharlayanConfiguration configuration) {
            string sqpack = TryResolveSqpackPath(configuration);
            if (sqpack == null) {
                return null;
            }
            try {
                return _cache.GetOrAdd(sqpack, CreateGameData);
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// Returns the cached <see cref="GameData"/> for the configuration's sqpack path,
        /// throwing if the path can't be resolved. Used by the xivdatabase provider's bulk
        /// loaders where a missing sqpack is a genuine configuration error.
        /// </summary>
        public static GameData Get(SharlayanConfiguration configuration) {
            string sqpack = ResolveSqpackPathStrict(configuration);
            return _cache.GetOrAdd(sqpack, CreateGameData);
        }

        private static GameData CreateGameData(string sqpack) {
            return new GameData(sqpack, new LuminaOptions {
                CacheFileResources = false,
                PanicOnSheetChecksumMismatch = false,
                LoadMultithreaded = true,
            });
        }

        // Lenient resolver — returns null on any missing input. Mirrors what
        // Reader.GameState used to do inline before this cache was introduced.
        private static string TryResolveSqpackPath(SharlayanConfiguration configuration) {
            if (!string.IsNullOrWhiteSpace(configuration?.GameInstallPath)) {
                string gameSqpack = Path.Combine(configuration.GameInstallPath, "game", "sqpack");
                if (Directory.Exists(gameSqpack)) return gameSqpack;
                string rootSqpack = Path.Combine(configuration.GameInstallPath, "sqpack");
                if (Directory.Exists(rootSqpack)) return rootSqpack;
            }
            string exePath = configuration?.ProcessModel?.Process?.MainModule?.FileName;
            if (string.IsNullOrWhiteSpace(exePath)) return null;
            string gameDir = Path.GetDirectoryName(exePath);
            if (string.IsNullOrWhiteSpace(gameDir)) return null;
            string guess = Path.Combine(gameDir, "sqpack");
            return Directory.Exists(guess) ? guess : null;
        }

        // Strict resolver — throws on missing input. Mirrors what
        // LuminaXivDatabaseProvider used to do inline before this cache was introduced.
        private static string ResolveSqpackPathStrict(SharlayanConfiguration configuration) {
            if (!string.IsNullOrWhiteSpace(configuration?.GameInstallPath)) {
                string explicitSqpack = Path.Combine(configuration.GameInstallPath, "game", "sqpack");
                if (Directory.Exists(explicitSqpack)) {
                    return explicitSqpack;
                }
                explicitSqpack = Path.Combine(configuration.GameInstallPath, "sqpack");
                if (Directory.Exists(explicitSqpack)) {
                    return explicitSqpack;
                }
                throw new DirectoryNotFoundException($"sqpack directory not found under configured GameInstallPath '{configuration.GameInstallPath}'.");
            }

            string exePath = configuration?.ProcessModel?.Process?.MainModule?.FileName
                             ?? throw new InvalidOperationException("Cannot locate game sqpack: no GameInstallPath set and no running ProcessModel available.");

            string gameDir = Path.GetDirectoryName(exePath)
                             ?? throw new InvalidOperationException($"Cannot derive game directory from exe path '{exePath}'.");

            string sqpack = Path.Combine(gameDir, "sqpack");
            if (!Directory.Exists(sqpack)) {
                throw new DirectoryNotFoundException($"sqpack directory not found at '{sqpack}'.");
            }

            return sqpack;
        }
    }
}
