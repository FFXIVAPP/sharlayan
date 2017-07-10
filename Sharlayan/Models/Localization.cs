// Sharlayan ~ Localization.cs
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

using System;

namespace Sharlayan.Models
{
    public class Localization
    {
        public string English { get; set; }
        public string French { get; set; }
        public string Japanese { get; set; }
        public string German { get; set; }
        public string Chinese { get; set; }
        public string Korean { get; set; }

        public bool Matches(string name)
        {
            return string.Equals(English, name, StringComparison.InvariantCultureIgnoreCase) || string.Equals(French, name, StringComparison.InvariantCultureIgnoreCase) || string.Equals(Japanese, name, StringComparison.InvariantCultureIgnoreCase) || string.Equals(German, name, StringComparison.InvariantCultureIgnoreCase) || string.Equals(Chinese, name, StringComparison.InvariantCultureIgnoreCase) || string.Equals(Korean, name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
