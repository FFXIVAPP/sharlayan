// --------------------------------------------------------------------------------------------------------------------
// <copyright file="APIHelper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   APIHelper.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Cache;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using Sharlayan.Models;
    using Sharlayan.Models.Structures;
    using Sharlayan.Models.XIVDatabase;

    using StatusItem = Sharlayan.Models.XIVDatabase.StatusItem;

    public static class APIHelper {
        private static Encoding _webClientEncoding = Encoding.UTF8;

        private static RequestCachePolicy _webClientRequestCachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

        public static async Task GetActions(ConcurrentDictionary<uint, ActionItem> actions, SharlayanConfiguration configuration) {

            string file = Path.Combine(configuration.JSONCacheDirectory, $"actions-latest.json");

            if (File.Exists(file) && configuration.UseLocalCache) {
                EnsureDictionaryValues(actions, file);
            }
            else {
                //await APIResponseToDictionary(actions, $"{configuration.APIBaseURL}/xivdatabase/{configuration.PatchVersion}/actions.json", configuration);
                await APIResponseToDictionary(actions, $"{configuration.APIBaseURL}/xivdatabase/latest/actions.json", configuration);

            }

            if (configuration.UseLocalCache) {
                File.WriteAllText(file, JsonConvert.SerializeObject(actions, Formatting.Indented, Constants.SerializerSettings), Encoding.UTF8);
            }
        }

        public static async Task<Signature[]> GetSignatures(SharlayanConfiguration configuration) {
            string region = configuration.GameRegion.ToString().ToLowerInvariant();
            string patchVersion = configuration.PatchVersion;
            string file = Path.Combine(configuration.JSONCacheDirectory, $"signatures-{region}-latest.json");


            if (File.Exists(file) && configuration.UseLocalCache) {
                return FileResponseToJSON<Signature[]>(file);
            }

            //string json = await APIResponseToJSON($"{configuration.APIBaseURL}/signatures/{patchVersion}/x64.json");
            string json = await APIResponseToJSON($"{configuration.APIBaseURL}/signatures/latest/x64.json");

            Signature[] resolved = JsonConvert.DeserializeObject<Signature[]>(json, Constants.SerializerSettings);

            if (configuration.UseLocalCache) {
                File.WriteAllText(file, JsonConvert.SerializeObject(resolved, Formatting.Indented, Constants.SerializerSettings), Encoding.UTF8);
            }

            return resolved;
        }

        public static async Task GetStatusEffects(ConcurrentDictionary<uint, StatusItem> statusEffects, SharlayanConfiguration configuration) {

            string file = Path.Combine(configuration.JSONCacheDirectory, $"statuses-latest.json");

            if (File.Exists(file) && configuration.UseLocalCache) {
                EnsureDictionaryValues(statusEffects, file);
            }
            else {
                //await APIResponseToDictionary(statusEffects, $"{configuration.APIBaseURL}/xivdatabase/{configuration.PatchVersion}/statuses.json", configuration);
                await APIResponseToDictionary(statusEffects, $"{configuration.APIBaseURL}/xivdatabase/latest/statuses.json", configuration);

            }

            if (configuration.UseLocalCache) {
                File.WriteAllText(file, JsonConvert.SerializeObject(statusEffects, Formatting.Indented, Constants.SerializerSettings), Encoding.UTF8);
            }
        }

        public static async Task<StructuresContainer> GetStructures(SharlayanConfiguration configuration) {
            string region = configuration.GameRegion.ToString().ToLowerInvariant();
            string patchVersion = configuration.PatchVersion;
            string file = Path.Combine(configuration.JSONCacheDirectory, $"structures-{region}-latest.json");

            if (File.Exists(file) && configuration.UseLocalCache) {
                return EnsureClassValues<StructuresContainer>(file);
            }

            // StructuresContainer structuresContainer = await APIResponseTo<StructuresContainer>($"{configuration.APIBaseURL}/structures/{patchVersion}/x64.json");
            StructuresContainer structuresContainer = await APIResponseTo<StructuresContainer>($"{configuration.APIBaseURL}/structures/latest/x64.json");

            if (configuration.UseLocalCache) {
                File.WriteAllText(file, JsonConvert.SerializeObject(structuresContainer, Formatting.Indented, Constants.SerializerSettings), Encoding.UTF8);
            }

            return structuresContainer;
        }

        public static async Task GetZones(ConcurrentDictionary<uint, MapItem> mapInfos, SharlayanConfiguration configuration) {
            // These ID's link to offset 7 in the old JSON values.
            // eg: "map id = 4" would be 148 in offset 7.
            // This is known as the TerritoryType value
            // - It maps directly to SaintCoins map.csv against TerritoryType ID

            string file = Path.Combine(configuration.JSONCacheDirectory, $"zones-latest.json");

            if (File.Exists(file) && configuration.UseLocalCache) {
                EnsureDictionaryValues(mapInfos, file);
            }
            else {
                //await APIResponseToDictionary(mapInfos, $"{configuration.APIBaseURL}/xivdatabase/{configuration.PatchVersion}/zones.json", configuration);
                await APIResponseToDictionary(mapInfos, $"{configuration.APIBaseURL}/xivdatabase/latest/zones.json", configuration);

            }

            if (configuration.UseLocalCache) {
                File.WriteAllText(file, JsonConvert.SerializeObject(mapInfos, Formatting.Indented, Constants.SerializerSettings), Encoding.UTF8);
            }
        }

        private static async Task APIResponseToDictionary<T>(ConcurrentDictionary<uint, T> dictionary, string uri, SharlayanConfiguration configuration) {
            ConcurrentDictionary<uint, T> resolved = await APIResponseTo<ConcurrentDictionary<uint, T>>(uri);

            foreach (KeyValuePair<uint, T> kvp in resolved) {
                dictionary.AddOrUpdate(kvp.Key, kvp.Value, (k, v) => kvp.Value);
            }
        }

        private static async Task<string> APIResponseToJSON(string uri) {
            using HttpRequestMessage request = new HttpRequestMessage {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri),
            };
            using HttpClient httpClient = new HttpClient();
            using HttpResponseMessage response = await httpClient.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        private static async Task<T> APIResponseTo<T>(string uri) {
            string json = await APIResponseToJSON(uri);

            return JsonConvert.DeserializeObject<T>(json, Constants.SerializerSettings);
        }

        private static T EnsureClassValues<T>(string file) {
            return FileResponseToJSON<T>(file);
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
            return (T) serializer.Deserialize(streamReader, typeof(T));
        }
    }
}