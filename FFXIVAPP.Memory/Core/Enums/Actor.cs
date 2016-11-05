// FFXIVAPP.Memory
// FFXIVAPP & Related Plugins/Modules
// Copyright © 2007 - 2016 Ryan Wilson - All Rights Reserved
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
using System.IO;
using System.Net;
using System.Text;
using FFXIVAPP.Memory.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FFXIVAPP.Memory.Core.Enums
{
    public static class Actor
    {
        public static Enumeration ActionStatus { get; set; }
        public static Enumeration Icon { get; set; }
        public static Enumeration Job { get; set; }
        public static Enumeration Sex { get; set; }
        public static Enumeration Status { get; set; }
        public static Enumeration TargetType { get; set; }
        public static Enumeration Type { get; set; }

        private static void LoadJson(string json)
        {
            ActionStatus.Load(JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte>>(JObject.Parse(json)["ActionStatus"].ToString()));
            Icon.Load(JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte>>(JObject.Parse(json)["Icon"].ToString()));
            Job.Load(JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte>>(JObject.Parse(json)["Job"].ToString()));
            Sex.Load(JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte>>(JObject.Parse(json)["Sex"].ToString()));
            Status.Load(JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte>>(JObject.Parse(json)["Status"].ToString()));
            TargetType.Load(JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte>>(JObject.Parse(json)["TargetType"].ToString()));
            Type.Load(JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte>>(JObject.Parse(json)["Type"].ToString()));
        }

        public static void Initialize(ProcessModel processModel, string patchVersion = "latest")
        {
            ActionStatus = new Enumeration();
            Icon = new Enumeration();
            Job = new Enumeration();
            Sex = new Enumeration();
            Status = new Enumeration();
            TargetType = new Enumeration();
            Type = new Enumeration();

            var file = Path.Combine(Directory.GetCurrentDirectory(), $"enums-{(processModel.IsWin64 ? "x64" : "x86")}.json");
            if (File.Exists(file))
            {
                using (var streamReader = new StreamReader(file))
                {
                    LoadJson(streamReader.ReadToEnd());
                }
            }
            else
            {
                using (var webClient = new WebClient { Encoding = Encoding.UTF8})
                {
                    var json = webClient.DownloadString($"http://xivapp.com/api/enums?patchVersion={patchVersion}&platform={(processModel.IsWin64 ? "x64" : "x86")}");
                    LoadJson(json);
                    File.WriteAllText(file, JObject.Parse(json).ToString(Formatting.Indented), Encoding.GetEncoding(932));
                }
            }
        }
    }
}