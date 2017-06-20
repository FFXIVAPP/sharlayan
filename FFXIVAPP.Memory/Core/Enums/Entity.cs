// FFXIVAPP.Memory
// FFXIVAPP & Related Plugins/Modules
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

using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FFXIVAPP.Memory.Core.Enums
{
    public static class Entity
    {
        private static ConcurrentDictionary<string, byte> DefaultDictionary = new ConcurrentDictionary<string, byte>();
        public static Enumeration Container { get; private set; }
        public static Enumeration ActionStatus { get; private set; }
        public static Enumeration Icon { get; private set; }
        public static Enumeration Job { get; private set; }
        public static Enumeration Sex { get; private set; }
        public static Enumeration Status { get; private set; }
        public static Enumeration TargetType { get; private set; }
        public static Enumeration Type { get; private set; }

        public static void Initialize(string json)
        {
            Container = new Enumeration(JsonToDictionary(json, "Container"));
            ActionStatus = new Enumeration(JsonToDictionary(json, "ActionStatus"));
            Icon = new Enumeration(JsonToDictionary(json, "Icon"));
            Job = new Enumeration(JsonToDictionary(json, "Job"));
            Sex = new Enumeration(JsonToDictionary(json, "Sex"));
            Status = new Enumeration(JsonToDictionary(json, "Status"));
            TargetType = new Enumeration(JsonToDictionary(json, "TargetType"));
            Type = new Enumeration(JsonToDictionary(json, "Type"));
        }

        private static ConcurrentDictionary<string, byte> JsonToDictionary(string json, string key)
        {
            try
            {
                var node = JObject.Parse(json)[key]
                                  .ToString();
                if (string.IsNullOrWhiteSpace(node))
                {
                    return DefaultDictionary;
                }
                return JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte>>(node, Constants.SerializerSettings);
            }
            catch (Exception)
            {
                return DefaultDictionary;
            }
        }
    }
}
