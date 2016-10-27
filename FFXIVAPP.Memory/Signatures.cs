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
using FFXIVAPP.Memory.Models;
using Newtonsoft.Json;

namespace FFXIVAPP.Memory
{
    public static class Signatures
    {
        public static IEnumerable<Signature> Resolve(bool IsWin64, string patchVersion = "1.0")
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "offsets.json");
            if (File.Exists(file))
            {
                using (var streamReader = new StreamReader(file))
                {
                    var json = streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject<IEnumerable<Signature>>(json);
                }
            }
            else
            {
                using (var webClient = new WebClient())
                {
                    var json = webClient.DownloadString($"http://xivapp.com/api/signatures?patchVersion={patchVersion}&platform={(IsWin64 ? "x64" : "x86")}");
                    return JsonConvert.DeserializeObject<IEnumerable<Signature>>(json);
                }
            }
        }
    }
}
