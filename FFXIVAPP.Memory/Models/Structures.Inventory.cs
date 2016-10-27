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

namespace FFXIVAPP.Memory.Models
{
    public partial class Structures
    {
        public class Inventory {
            public int ContainerAmount { get; set; }
            public int ID { get; set; }
            public int Slot { get; set; }
            public int ItemAmount { get; set; }
            public int SB { get; set; }
            public int Durability { get; set; }
            public int GlamourID { get; set; }
            public int IsHQ { get; set; }
        }
    }
}
