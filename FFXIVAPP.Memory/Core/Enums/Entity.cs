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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FFXIVAPP.Memory.Core.Enums
{
    public static class Entity
    {
        public static Enumeration Container { get; set; }
        public static Enumeration ActionStatus { get; set; }
        public static Enumeration Icon { get; set; }
        public static Enumeration Job { get; set; }
        public static Enumeration Sex { get; set; }
        public static Enumeration Status { get; set; }
        public static Enumeration TargetType { get; set; }
        public static Enumeration Type { get; set; }

        public static void Initialize(string json)
        {
            Container = new Enumeration(JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte>>(JObject.Parse(json)["Container"]?.ToString() ?? "{}"));
            ActionStatus = new Enumeration(JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte>>(JObject.Parse(json)["ActionStatus"]?.ToString() ?? "{}"));
            Icon = new Enumeration(JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte>>(JObject.Parse(json)["Icon"]?.ToString() ?? "{}"));
            Job = new Enumeration(JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte>>(JObject.Parse(json)["Job"]?.ToString() ?? "{}"));
            Sex = new Enumeration(JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte>>(JObject.Parse(json)["Sex"]?.ToString() ?? "{}"));
            Status = new Enumeration(JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte>>(JObject.Parse(json)["Status"]?.ToString() ?? "{}"));
            TargetType = new Enumeration(JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte>>(JObject.Parse(json)["TargetType"]?.ToString() ?? "{}"));
            Type = new Enumeration(JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte>>(JObject.Parse(json)["Type"]?.ToString() ?? "{}"));
        }
    }
}
