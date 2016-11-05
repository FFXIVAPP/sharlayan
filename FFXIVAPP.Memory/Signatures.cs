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

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using FFXIVAPP.Memory.Models;
using Newtonsoft.Json;

namespace FFXIVAPP.Memory
{
    public static class Signatures
    {
        public static IEnumerable<Signature> Resolve(bool IsWin64, string patchVersion = "latest")
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), $"signatures-{(IsWin64 ? "x64" : "x86")}.json");
            if (File.Exists(file))
            {
                using (var streamReader = new StreamReader(file))
                {
                    var json = streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject<IEnumerable<Signature>>(json, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }
            }
            else
            {
                using (var webClient = new WebClient { Encoding = Encoding.UTF8 })
                {
                    var json = webClient.DownloadString($"http://xivapp.com/api/signatures?patchVersion={patchVersion}&platform={(IsWin64 ? "x64" : "x86")}");
                    var signatures = JsonConvert.DeserializeObject<IEnumerable<Signature>>(json);
                    File.WriteAllText(file, JsonConvert.SerializeObject(signatures, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }), Encoding.GetEncoding(932));
                    return signatures;
                }
            }
        }
    }
}
