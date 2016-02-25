// FFXIVAPP.Memory ~ ItemInfo.cs
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

using System;
using FFXIVAPP.Memory.Core.Interfaces;

namespace FFXIVAPP.Memory.Core
{
    public class ItemInfo : IItemInfo
    {
        public bool IsHQ { get; set; }
        public int Slot { get; set; }
        public uint ID { get; set; }
        public uint SB { get; set; }
        public uint GlamourID { get; set; }
        public uint Amount { get; set; }

        public double SBPercent
        {
            get { return (double) Decimal.Divide(SB, 10000); }
        }

        public uint Durability { get; set; }

        public double DurabilityPercent
        {
            get { return (double) Decimal.Divide(Durability, 30000); }
        }
    }
}
