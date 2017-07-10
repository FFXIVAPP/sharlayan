// FFXIVAPP.Memory ~ IHotBarRecastItem.cs
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

namespace FFXIVAPP.Memory.Core.Interfaces
{
    public interface IHotBarRecastItem
    {
        string Name { get; set; }
        int ID { get; set; }
        string KeyBinds { get; set; }
        List<string> Modifiers { get; set; }
        string ActionKey { get; set; }
        int Slot { get; set; }
        int Category { get; set; }
        int Type { get; set; }
        int Icon { get; set; }
        int ReadyPercent { get; set; }
        bool IsAvailable { get; set; }
        int RemainingCost { get; set; }
        int Amount { get; set; }
        bool InRange { get; set; }
        bool IsProcOrCombo { get; set; }
    }
}
