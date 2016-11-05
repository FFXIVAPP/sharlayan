// FFXIVAPP.Memory ~ Enumeration.cs
// 
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
using System.Linq;

namespace FFXIVAPP.Memory.Core.Enums
{
    public class Enumeration
    {
        private ConcurrentDictionary<byte, string> _byte = new ConcurrentDictionary<byte, string>();
        private ConcurrentDictionary<string, byte> _string = new ConcurrentDictionary<string, byte>();
        public string this[byte key] => _byte[key];
        public byte this[string key] => _string[key];

        public Enumeration(ConcurrentDictionary<string, byte> dictionary)
        {
            _string = dictionary;
            _byte = new ConcurrentDictionary<byte, string>(dictionary.ToDictionary(kvp => kvp.Value, kvp => kvp.Key));
        }
    }
}
