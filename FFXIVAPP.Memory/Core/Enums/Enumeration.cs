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

using System.Collections.Concurrent;
using System.Linq;

namespace FFXIVAPP.Memory.Core.Enums
{
    public class Enumeration
    {
        private readonly ConcurrentDictionary<byte, string> _byte = new ConcurrentDictionary<byte, string>();
        private readonly ConcurrentDictionary<string, byte> _string = new ConcurrentDictionary<string, byte>();

        public Enumeration(ConcurrentDictionary<string, byte> dictionary)
        {
            _string = dictionary;
            _byte = new ConcurrentDictionary<byte, string>(dictionary.ToDictionary(kvp => kvp.Value, kvp => kvp.Key));
        }

        public string this[byte key]
        {
            get
            {
                string result;
                if (!_byte.TryGetValue(key, out result))
                {
                    result = "UNKNOWN";
                }
                return result;
            }
        }

        public byte this[string key]
        {
            get
            {
                byte result;
                if (!_string.TryGetValue(key, out result))
                {
                    result = 0;
                }
                return result;
            }
        }
    }
}
