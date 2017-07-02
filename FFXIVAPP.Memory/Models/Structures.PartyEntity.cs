// FFXIVAPP.Memory ~ Structures.PartyEntity.cs
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

namespace FFXIVAPP.Memory.Models
{
    public partial class Structures
    {
        public class PartyEntityStructure
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
            public int ID { get; set; }
            public int Name { get; set; }
            public int Job { get; set; }
            public int Level { get; set; }
            public int HPCurrent { get; set; }
            public int HPMax { get; set; }
            public int MPCurrent { get; set; }
            public int MPMax { get; set; }
            public int DefaultStatusEffectOffset { get; set; }
        }
    }
}
