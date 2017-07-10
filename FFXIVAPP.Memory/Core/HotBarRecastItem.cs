// FFXIVAPP.Memory ~ HotBarRecastItem.cs
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

using System.Collections.Generic;
using FFXIVAPP.Memory.Core.Interfaces;

namespace FFXIVAPP.Memory.Core
{
    public class HotBarRecastItem : IHotBarRecastItem
    {
        public HotBarRecastItem()
        {
            Modifiers = new List<string>();
        }

        public string Name { get; set; }
        public int ID { get; set; }
        public string KeyBinds { get; set; }
        public List<string> Modifiers { get; set; }
        public string ActionKey { get; set; }
        public int Slot { get; set; }
        public int Category { get; set; }
        public int Type { get; set; }
        public int Icon { get; set; }
        public int CoolDownPercent { get; set; }
        public bool IsAvailable { get; set; }
        public int RemainingCost { get; set; }
        public int Amount { get; set; }
        public bool InRange { get; set; }
        public bool IsProcOrCombo { get; set; }
    }
}
