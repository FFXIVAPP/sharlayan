// FFXIVAPP.Memory
// Reader.Inventory.cs
// 
// Copyright © 2007 - 2015 Ryan Wilson - All Rights Reserved
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions are met: 
// 
//  * Redistributions of source code must retain the above copyright notice, 
//    this list of conditions and the following disclaimer. 
//  * Redistributions in binary form must reproduce the above copyright 
//    notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution. 
//  * Neither the name of SyndicatedLife nor the names of its contributors may 
//    be used to endorse or promote products derived from this software 
//    without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE. 

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

            if (MemoryHandler.Instance.SigScanner.Locations.ContainsKey("INVENTORY"))
            {
                try
                {
                    InventoryPointerMap = new IntPtr(MemoryHandler.Instance.GetPlatformUInt(MemoryHandler.Instance.SigScanner.Locations["INVENTORY"]));

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
