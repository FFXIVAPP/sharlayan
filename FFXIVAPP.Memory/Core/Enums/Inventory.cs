// FFXIVAPP.Memory ~ Inventory.cs
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

namespace FFXIVAPP.Memory.Core.Enums
{
    public class Inventory
    {
        public enum Container : byte
        {
            INVENTORY_1 = 0x0,
            INVENTORY_2 = 0x1,
            INVENTORY_3 = 0x2,
            INVENTORY_4 = 0x3,
            CURRENT_EQ = 0x4,
            EXTRA_EQ = 0x5,
            CRYSTALS = 0x6,
            QUESTS_KI = 0x9,

            HIRE_1 = 0x12,
            HIRE_2 = 0x13,
            HIRE_3 = 0x14,
            HIRE_4 = 0x15,
            HIRE_5 = 0x16,
            HIRE_6 = 0x17,
            HIRE_7 = 0x18,

            AC_MH = 0x1D,
            AC_OH = 0x1E,
            AC_HEAD = 0x1F,
            AC_BODY = 0x20,
            AC_HANDS = 0x21,
            AC_BELT = 0x22,
            AC_LEGS = 0x23,
            AC_FEET = 0x24,
            AC_EARRINGS = 0x25,
            AC_NECK = 0x26,
            AC_WRISTS = 0x27,
            AC_RINGS = 0x28,
            AC_SOULS = 0x29,

            COMPANY_1 = 0x2A,
            COMPANY_2 = 0x2B,
            COMPANY_3 = 0x2C,
            COMPANY_CRYSTALS = 0x2D
        }
    }
}
