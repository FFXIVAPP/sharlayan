// --------------------------------------------------------------------------------------------------------------------
// <copyright file="APIHelper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Local-JSON-only bootstrap for the legacy resource path. The HTTP fetch against the
//   sharlayan-resources repository has been removed — that repo is no longer maintained
//   and its signatures/offsets drift with every FFXIV patch. New code should go through
//   Sharlayan.Resources.IResourceProvider and ResourceProviderFactory.Create(configuration),
//   which defaults to FFXIVClientStructsDirect. This class remains only so the harness's
//   [3] A/B diff against snapshotted legacy JSON keeps working.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using Sharlayan.Models;
    using Sharlayan.Models.Structures;
    using Sharlayan.Models.XIVDatabase;

    using StatusItem = Sharlayan.Models.XIVDatabase.StatusItem;

    public static class APIHelper {
        [Obsolete("Use Sharlayan.Resources.IResourceProvider.GetActionsAsync via ResourceProviderFactory.")]
        public static Task GetActions(ConcurrentDictionary<uint, ActionItem> actions, SharlayanConfiguration configuration) {
            string file = Path.Combine(configuration.JSONCacheDirectory, "actions-latest.json");
            RequireLocalFile(file, "actions");
            EnsureDictionaryValues(actions, file);
            return Task.CompletedTask;
        }

        [Obsolete("Use Sharlayan.Resources.IResourceProvider.GetSignaturesAsync via ResourceProviderFactory.")]
        public static Task<Signature[]> GetSignatures(SharlayanConfiguration configuration) {
            string region = configuration.GameRegion.ToString().ToLowerInvariant();
            string file = Path.Combine(configuration.JSONCacheDirectory, $"signatures-{region}-latest.json");
            RequireLocalFile(file, "signatures");
            return Task.FromResult(FileResponseToJSON<Signature[]>(file));
        }

        [Obsolete("Use Sharlayan.Resources.IResourceProvider.GetStatusEffectsAsync via ResourceProviderFactory.")]
        public static Task GetStatusEffects(ConcurrentDictionary<uint, StatusItem> statusEffects, SharlayanConfiguration configuration) {
            string file = Path.Combine(configuration.JSONCacheDirectory, "statuses-latest.json");
            RequireLocalFile(file, "status effects");
            EnsureDictionaryValues(statusEffects, file);
            return Task.CompletedTask;
        }

        [Obsolete("Use Sharlayan.Resources.IResourceProvider.GetStructuresAsync via ResourceProviderFactory.")]
        public static Task<StructuresContainer> GetStructures(SharlayanConfiguration configuration) {
            string region = configuration.GameRegion.ToString().ToLowerInvariant();
            string file = Path.Combine(configuration.JSONCacheDirectory, $"structures-{region}-latest.json");
            RequireLocalFile(file, "structures");
            return Task.FromResult(FileResponseToJSON<StructuresContainer>(file));
        }

        [Obsolete("Use Sharlayan.Resources.IResourceProvider.GetZonesAsync via ResourceProviderFactory.")]
        public static Task GetZones(ConcurrentDictionary<uint, MapItem> mapInfos, SharlayanConfiguration configuration) {
            string file = Path.Combine(configuration.JSONCacheDirectory, "zones-latest.json");
            RequireLocalFile(file, "zones");
            EnsureDictionaryValues(mapInfos, file);
            return Task.CompletedTask;
        }

        private static void RequireLocalFile(string path, string kind) {
            if (!File.Exists(path)) {
                throw new FileNotFoundException(
                    $"Legacy {kind} JSON not found at '{path}'. The sharlayan-resources HTTP fallback has been removed; " +
                    "place a snapshotted JSON file at the configured JSONCacheDirectory or switch SharlayanConfiguration.ResourceProvider " +
                    "to FFXIVClientStructsDirect (now the default).", path);
            }
        }

        private static void EnsureDictionaryValues<T>(ConcurrentDictionary<uint, T> dictionary, string file) {
            ConcurrentDictionary<uint, T> resolved = FileResponseToJSON<ConcurrentDictionary<uint, T>>(file);
            foreach (KeyValuePair<uint, T> kvp in resolved) {
                dictionary.AddOrUpdate(kvp.Key, kvp.Value, (k, v) => kvp.Value);
            }
        }

        private static T FileResponseToJSON<T>(string file) {
            using StreamReader streamReader = File.OpenText(file);
            JsonSerializer serializer = new JsonSerializer {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Populate,
            };
            return (T)serializer.Deserialize(streamReader, typeof(T));
        }
    }
}
