// FFXIVAPP.Memory ~ APIHelper.cs
// 
// Copyright © 2007 - 2017 Ryan Wilson - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using FFXIVAPP.Memory.Models;
using Newtonsoft.Json;

namespace FFXIVAPP.Memory.Helpers
{
    public static class APIHelper
    {
        private static WebClient _webClient = new WebClient
        {
            Encoding = Encoding.UTF8
        };

        public static void GetActions(ConcurrentDictionary<uint, ActionItem> actions)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "actions.json");
            if (File.Exists(file) && !MemoryHandler.Instance.IgnoreJSONCache)
            {
                EnsureDictionaryValues(actions, file);
            }
            else
            {
                APIResponseToDictionary(actions, file, "http://xivapp.com/api/actions");
            }
        }

        public static void GetStatusEffects(ConcurrentDictionary<uint, StatusItem> statusEffects)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "statuses.json");
            if (File.Exists(file) && !MemoryHandler.Instance.IgnoreJSONCache)
            {
                EnsureDictionaryValues(statusEffects, file);
            }
            else
            {
                APIResponseToDictionary(statusEffects, file, "http://xivapp.com/api/statuses");
            }
        }

        public static void GetZones(ConcurrentDictionary<uint, MapItem> mapInfos)
        {
            // These ID's link to offset 7 in the old JSON values.
            // eg: "map id = 4" would be 148 in offset 7.
            // This is known as the TerritoryType value
            // - It maps directly to SaintCoins map.csv against TerritoryType ID
            var file = Path.Combine(Directory.GetCurrentDirectory(), "zones.json");
            if (File.Exists(file) && !MemoryHandler.Instance.IgnoreJSONCache)
            {
                EnsureDictionaryValues(mapInfos, file);
            }
            else
            {
                APIResponseToDictionary(mapInfos, file, "http://xivapp.com/api/zones");
            }
        }

        public static Structures GetStructures(ProcessModel processModel, string patchVersion = "latest")
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), $"structures-{(processModel.IsWin64 ? "x64" : "x86")}.json");
            if (File.Exists(file) && !MemoryHandler.Instance.IgnoreJSONCache)
            {
                return EnsureClassValues<Structures>(file);
            }
            return APIResponseToClass<Structures>(file, $"http://xivapp.com/api/structures?patchVersion={patchVersion}&platform={(processModel.IsWin64 ? "x64" : "x86")}");
        }

        public static IEnumerable<Signature> GetSignatures(ProcessModel processModel, string patchVersion = "latest")
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), $"signatures-{(processModel.IsWin64 ? "x64" : "x86")}.json");
            if (File.Exists(file) && !MemoryHandler.Instance.IgnoreJSONCache)
            {
                var json = FileResponseToJSON(file);
                return JsonConvert.DeserializeObject<IEnumerable<Signature>>(json, Constants.SerializerSettings);
            }
            else
            {
                var json = APIResponseToJSON($"http://xivapp.com/api/signatures?patchVersion={patchVersion}&platform={(processModel.IsWin64 ? "x64" : "x86")}");
                var resolved = JsonConvert.DeserializeObject<IEnumerable<Signature>>(json, Constants.SerializerSettings);

                File.WriteAllText(file, JsonConvert.SerializeObject(resolved, Formatting.Indented, Constants.SerializerSettings), Encoding.GetEncoding(932));

                return resolved;
            }
        }

        #region Private Methods

        private static void EnsureDictionaryValues<T>(ConcurrentDictionary<uint, T> dictionary, string file)
        {
            var json = FileResponseToJSON(file);
            var resolved = JsonConvert.DeserializeObject<ConcurrentDictionary<uint, T>>(json, Constants.SerializerSettings);

            foreach (var kvp in resolved)
            {
                dictionary.AddOrUpdate(kvp.Key, kvp.Value, (k, v) => kvp.Value);
            }
        }

        private static void APIResponseToDictionary<T>(ConcurrentDictionary<uint, T> dictionary, string file, string uri)
        {
            var json = APIResponseToJSON(uri);
            var resolved = JsonConvert.DeserializeObject<ConcurrentDictionary<uint, T>>(json, Constants.SerializerSettings);

            foreach (var kvp in resolved)
            {
                dictionary.AddOrUpdate(kvp.Key, kvp.Value, (k, v) => kvp.Value);
            }

            File.WriteAllText(file, JsonConvert.SerializeObject(dictionary, Formatting.Indented, Constants.SerializerSettings), Encoding.UTF8);
        }

        private static T EnsureClassValues<T>(string file)
        {
            var json = FileResponseToJSON(file);
            return JsonConvert.DeserializeObject<T>(json, Constants.SerializerSettings);
        }

        private static T APIResponseToClass<T>(string file, string uri)
        {
            var json = APIResponseToJSON(uri);
            var resolved = JsonConvert.DeserializeObject<T>(json, Constants.SerializerSettings);

            File.WriteAllText(file, JsonConvert.SerializeObject(resolved, Formatting.Indented, Constants.SerializerSettings), Encoding.UTF8);

            return resolved;
        }

        private static string FileResponseToJSON(string file)
        {
            using (var streamReader = new StreamReader(file))
            {
                return streamReader.ReadToEnd();
            }
        }

        private static string APIResponseToJSON(string uri)
        {
            return _webClient.DownloadString(uri);
        }

        #endregion
    }
}
