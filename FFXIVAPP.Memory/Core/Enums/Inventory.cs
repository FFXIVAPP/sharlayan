// FFXIVAPP.Memory ~ Inventory.cs
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

namespace FFXIVAPP.Memory.Core.Enums
{
    public class Inventory
    {
        public enum Container : byte
        {
            INVENTORY_1 = 0,
            INVENTORY_2 = 1,
            INVENTORY_3 = 2,
            INVENTORY_4 = 3,
            CURRENT_EQ = 4,
            EXTRA_EQ = 5,
            CRYSTALS = 6,
            QUESTS_KI = 9,
            HIRE_1 = 18,
            HIRE_2 = 19,
            HIRE_3 = 20,
            HIRE_4 = 21,
            HIRE_5 = 22,
            HIRE_6 = 23,
            HIRE_7 = 24,

            AC_MH = 29,
            AC_OH = 30,
            AC_HEAD = 31,
            AC_BODY = 32,
            AC_HANDS = 33,
            AC_BELT = 34,
            AC_LEGS = 35,
            AC_FEET = 36,
            AC_EARRINGS = 37,
            AC_NECK = 38,
            AC_WRISTS = 39,
            AC_RINGS = 40,
            AC_SOULS = 41,
            COMPANY_1 = 42,
            COMPANY_2 = 43,
            COMPANY_3 = 44,
            COMPANY_CRYSTALS = 45
        }
    }
}
