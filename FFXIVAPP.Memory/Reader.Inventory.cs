// FFXIVAPP.Memory ~ Reader.Inventory.cs
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
using System.Collections.Generic;
using FFXIVAPP.Memory.Core;
using FFXIVAPP.Memory.Core.Enums;

namespace FFXIVAPP.Memory
{
    public class InventoryReadResult
    {
        public InventoryReadResult()
        {
            InventoryEntities = new List<InventoryEntity>();
        }

        public List<InventoryEntity> InventoryEntities { get; set; }
    }

    public static partial class Reader
    {
        public static IntPtr InventoryPointerMap { get; set; }

        public static InventoryReadResult GetInventoryItems()
        {
            var result = new InventoryReadResult();

            if (Scanner.Instance.Locations.ContainsKey("INVENTORY"))
            {
                try
                {
                    InventoryPointerMap = new IntPtr(MemoryHandler.Instance.GetPlatformUInt(Scanner.Instance.Locations["INVENTORY"]));

                    result.InventoryEntities = new List<InventoryEntity>
                    {
                        GetItems(InventoryPointerMap, Inventory.Container.INVENTORY_1),
                        GetItems(InventoryPointerMap, Inventory.Container.INVENTORY_2),
                        GetItems(InventoryPointerMap, Inventory.Container.INVENTORY_3),
                        GetItems(InventoryPointerMap, Inventory.Container.INVENTORY_4),
                        GetItems(InventoryPointerMap, Inventory.Container.CURRENT_EQ),
                        GetItems(InventoryPointerMap, Inventory.Container.EXTRA_EQ),
                        GetItems(InventoryPointerMap, Inventory.Container.CRYSTALS),
                        GetItems(InventoryPointerMap, Inventory.Container.QUESTS_KI),
                        GetItems(InventoryPointerMap, Inventory.Container.HIRE_1),
                        GetItems(InventoryPointerMap, Inventory.Container.HIRE_2),
                        GetItems(InventoryPointerMap, Inventory.Container.HIRE_3),
                        GetItems(InventoryPointerMap, Inventory.Container.HIRE_4),
                        GetItems(InventoryPointerMap, Inventory.Container.HIRE_5),
                        GetItems(InventoryPointerMap, Inventory.Container.HIRE_6),
                        GetItems(InventoryPointerMap, Inventory.Container.HIRE_7),
                        GetItems(InventoryPointerMap, Inventory.Container.COMPANY_1),
                        GetItems(InventoryPointerMap, Inventory.Container.COMPANY_2),
                        GetItems(InventoryPointerMap, Inventory.Container.COMPANY_3),
                        GetItems(InventoryPointerMap, Inventory.Container.COMPANY_CRYSTALS),
                        GetItems(InventoryPointerMap, Inventory.Container.AC_MH),
                        GetItems(InventoryPointerMap, Inventory.Container.AC_OH),
                        GetItems(InventoryPointerMap, Inventory.Container.AC_HEAD),
                        GetItems(InventoryPointerMap, Inventory.Container.AC_BODY),
                        GetItems(InventoryPointerMap, Inventory.Container.AC_HANDS),
                        GetItems(InventoryPointerMap, Inventory.Container.AC_BELT),
                        GetItems(InventoryPointerMap, Inventory.Container.AC_LEGS),
                        GetItems(InventoryPointerMap, Inventory.Container.AC_FEET),
                        GetItems(InventoryPointerMap, Inventory.Container.AC_EARRINGS),
                        GetItems(InventoryPointerMap, Inventory.Container.AC_NECK),
                        GetItems(InventoryPointerMap, Inventory.Container.AC_WRISTS),
                        GetItems(InventoryPointerMap, Inventory.Container.AC_RINGS),
                        GetItems(InventoryPointerMap, Inventory.Container.AC_SOULS)
                    };
                }
                catch (Exception ex)
                {
                }
            }

            return result;
        }

        private static InventoryEntity GetItems(IntPtr address, Inventory.Container type)
        {
            var offset = (uint) ((int) type * 24);
            var containerAddress = MemoryHandler.Instance.GetPlatformUInt(address, offset);

            var container = new InventoryEntity
            {
                Amount = MemoryHandler.Instance.GetByte(address, offset + 0x8),
                Items = new List<ItemInfo>(),
                Type = type
            };
            // The number of item is 50 in COMPANY's locker
            int limit;
            switch (type)
            {
                case Inventory.Container.COMPANY_1:
                case Inventory.Container.COMPANY_2:
                case Inventory.Container.COMPANY_3:
                    limit = 3200;
                    break;
                default:
                    limit = 1600;
                    break;
            }

            for (var ci = 0; ci < limit; ci += 64)
            {
                var itemOffset = new IntPtr(containerAddress + ci);
                var id = MemoryHandler.Instance.GetPlatformUInt(itemOffset, 0x8);
                if (id > 0)
                {
                    container.Items.Add(new ItemInfo
                    {
                        ID = (uint) id,
                        Slot = MemoryHandler.Instance.GetByte(itemOffset, 0x4),
                        Amount = MemoryHandler.Instance.GetByte(itemOffset, 0xC),
                        SB = MemoryHandler.Instance.GetUInt16(itemOffset, 0x10),
                        Durability = MemoryHandler.Instance.GetUInt16(itemOffset, 0x12),
                        GlamourID = (uint) MemoryHandler.Instance.GetPlatformUInt(itemOffset, 0x30),
                        //get the flag that show if the item is hq or not
                        IsHQ = (MemoryHandler.Instance.GetByte(itemOffset, 0x14) == 0x01)
                    });
                }
            }

            return container;
        }
    }
}
