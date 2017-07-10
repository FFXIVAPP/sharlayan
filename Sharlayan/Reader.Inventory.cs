// Sharlayan ~ Reader.Inventory.cs
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
using System.Collections.Generic;
using Sharlayan.Core;
using Sharlayan.Core.Enums;
using Sharlayan.Models;

namespace Sharlayan
{
    public static partial class Reader
    {
        public static IntPtr InventoryPointerMap { get; set; }

        public static InventoryReadResult GetInventoryItems()
        {
            var result = new InventoryReadResult();

            if (!Scanner.Instance.Locations.ContainsKey("INVENTORY"))
            {
                return result;
            }

            try
            {
                InventoryPointerMap = new IntPtr(MemoryHandler.Instance.GetPlatformUInt(Scanner.Instance.Locations["INVENTORY"]));

                result.InventoryEntities = new List<InventoryEntity>
                {
                    GetItems(Inventory.Container.INVENTORY_1),
                    GetItems(Inventory.Container.INVENTORY_2),
                    GetItems(Inventory.Container.INVENTORY_3),
                    GetItems(Inventory.Container.INVENTORY_4),
                    GetItems(Inventory.Container.CURRENT_EQ),
                    GetItems(Inventory.Container.EXTRA_EQ),
                    GetItems(Inventory.Container.CRYSTALS),
                    GetItems(Inventory.Container.QUESTS_KI),
                    GetItems(Inventory.Container.HIRE_1),
                    GetItems(Inventory.Container.HIRE_2),
                    GetItems(Inventory.Container.HIRE_3),
                    GetItems(Inventory.Container.HIRE_4),
                    GetItems(Inventory.Container.HIRE_5),
                    GetItems(Inventory.Container.HIRE_6),
                    GetItems(Inventory.Container.HIRE_7),
                    GetItems(Inventory.Container.COMPANY_1),
                    GetItems(Inventory.Container.COMPANY_2),
                    GetItems(Inventory.Container.COMPANY_3),
                    GetItems(Inventory.Container.COMPANY_CRYSTALS),
                    GetItems(Inventory.Container.AC_MH),
                    GetItems(Inventory.Container.AC_OH),
                    GetItems(Inventory.Container.AC_HEAD),
                    GetItems(Inventory.Container.AC_BODY),
                    GetItems(Inventory.Container.AC_HANDS),
                    GetItems(Inventory.Container.AC_BELT),
                    GetItems(Inventory.Container.AC_LEGS),
                    GetItems(Inventory.Container.AC_FEET),
                    GetItems(Inventory.Container.AC_EARRINGS),
                    GetItems(Inventory.Container.AC_NECK),
                    GetItems(Inventory.Container.AC_WRISTS),
                    GetItems(Inventory.Container.AC_RINGS),
                    GetItems(Inventory.Container.AC_SOULS)
                };
            }
            catch (Exception ex)
            {
                MemoryHandler.Instance.RaiseException(Logger, ex, true);
            }

            return result;
        }

        private static InventoryEntity GetItems(Inventory.Container type)
        {
            var offset = (uint) ((int) type * 24);
            var containerAddress = MemoryHandler.Instance.GetPlatformUInt(InventoryPointerMap, offset);

            var container = new InventoryEntity
            {
                Amount = MemoryHandler.Instance.GetByte(InventoryPointerMap, offset + MemoryHandler.Instance.Structures.InventoryEntity.Amount),
                Items = new List<ItemInfo>(),
                TypeID = (byte) type,
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

            for (var i = 0; i < limit; i += 64)
            {
                var itemOffset = new IntPtr(containerAddress + i);
                var id = MemoryHandler.Instance.GetPlatformUInt(itemOffset, MemoryHandler.Instance.Structures.ItemInfo.ID);
                if (id > 0)
                {
                    container.Items.Add(new ItemInfo
                    {
                        ID = (uint) id,
                        Slot = MemoryHandler.Instance.GetByte(itemOffset, MemoryHandler.Instance.Structures.ItemInfo.Slot),
                        Amount = MemoryHandler.Instance.GetByte(itemOffset, MemoryHandler.Instance.Structures.ItemInfo.Amount),
                        SB = MemoryHandler.Instance.GetUInt16(itemOffset, MemoryHandler.Instance.Structures.ItemInfo.SB),
                        Durability = MemoryHandler.Instance.GetUInt16(itemOffset, MemoryHandler.Instance.Structures.ItemInfo.ID),
                        GlamourID = (uint) MemoryHandler.Instance.GetPlatformUInt(itemOffset, MemoryHandler.Instance.Structures.ItemInfo.GlamourID),
                        //get the flag that show if the item is hq or not
                        IsHQ = MemoryHandler.Instance.GetByte(itemOffset, MemoryHandler.Instance.Structures.ItemInfo.IsHQ) == 0x01
                    });
                }
            }

            return container;
        }
    }
}
