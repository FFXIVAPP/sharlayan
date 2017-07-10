// Sharlayan ~ Structures.RecastEntity.cs
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

namespace Sharlayan.Models
{
    public partial class Structures
    {
        public class RecastEntityStructure
        {
            public int Category { get; set; }
            public int Type { get; set; }
            public int ID { get; set; }
            public int Icon { get; set; }
            public int CoolDownPercent { get; set; }
            public int ActionProc { get; set; }
            public int IsAvailable { get; set; }
            public int RemainingCost { get; set; }
            public int Amount { get; set; }
            public int InRange { get; set; }
        }
    }
}
