// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Inventory.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.Inventory.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;

    using Sharlayan.Core;
    using Sharlayan.Core.Enums;
    using Sharlayan.Models.ReadResults;

    public static partial class Reader {
        public static bool CanGetInventory() {
            var canRead = Scanner.Instance.Locations.ContainsKey(Signatures.InventoryKey);
            if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        public static InventoryResult GetInventory() {
            var result = new InventoryResult();

            if (!CanGetInventory() || !MemoryHandler.Instance.IsAttached)
                return result;

            try {
                const int inventoryCount = 74;
                const int inventoryByteCount = 24;
                var inventoryMap = new IntPtr(MemoryHandler.Instance.GetPlatformUInt(Scanner.Instance.Locations[Signatures.InventoryKey]));
                var inventoryBytes = MemoryHandler.Instance.GetByteArray(inventoryMap, inventoryCount * 24);
                for (var i = 0; i < inventoryCount; i++) {
                    var bagOffset = i * inventoryByteCount;
                    var bagid = BitConverter.ToUInt32(inventoryBytes, bagOffset + MemoryHandler.Instance.Structures.InventoryContainer.ID);
                    if(!Enum.IsDefined(typeof(Inventory.Container), bagid))
                        continue;
                    var container = new InventoryContainer {
                        Amount = BitConverter.ToUInt32(inventoryBytes, bagOffset + MemoryHandler.Instance.Structures.InventoryContainer.Amount),
                        TypeID = bagid,
                        ContainerType = (Inventory.Container)bagid
                    };

                    const int itemByteCount = 56;
                    var bagSlotsAddress = new IntPtr(MemoryHandler.Instance.GetPlatformUIntFromBytes(inventoryBytes, bagOffset));
                    var bagSlotsBytes = MemoryHandler.Instance.GetByteArray(bagSlotsAddress, (int)container.Amount * itemByteCount);
                    for (var j = 0; j < container.Amount; j++) {
                        var slotOffset = j * itemByteCount;
                        var itemId = BitConverter.ToUInt32(bagSlotsBytes, slotOffset + MemoryHandler.Instance.Structures.InventoryItem.ID);
                        if(itemId <= 0) {
                            container.InventoryItems.Add(new InventoryItem());
                            continue;
                        }
                        container.InventoryItems.Add(new InventoryItem {
                            Slot = BitConverter.ToUInt16(bagSlotsBytes, slotOffset + MemoryHandler.Instance.Structures.InventoryItem.Slot),
                            ID = itemId,
                            Amount = BitConverter.ToUInt32(bagSlotsBytes, slotOffset + MemoryHandler.Instance.Structures.InventoryItem.Amount),
                            Spiritbond = BitConverter.ToUInt16(bagSlotsBytes, slotOffset + MemoryHandler.Instance.Structures.InventoryItem.SB),
                            Condition = BitConverter.ToUInt16(bagSlotsBytes, slotOffset + MemoryHandler.Instance.Structures.InventoryItem.Durability),
                            IsHQ = (bagSlotsBytes[slotOffset + MemoryHandler.Instance.Structures.InventoryItem.IsHQ] & 1) == 1,
                            MateriaTypes = new [] {
                                (Inventory.MateriaType)bagSlotsBytes[slotOffset + MemoryHandler.Instance.Structures.InventoryItem.MateriaType],
                                (Inventory.MateriaType)bagSlotsBytes[slotOffset + 2 + MemoryHandler.Instance.Structures.InventoryItem.MateriaType],
                                (Inventory.MateriaType)bagSlotsBytes[slotOffset + 4 + MemoryHandler.Instance.Structures.InventoryItem.MateriaType],
                                (Inventory.MateriaType)bagSlotsBytes[slotOffset + 6 + MemoryHandler.Instance.Structures.InventoryItem.MateriaType],
                                (Inventory.MateriaType)bagSlotsBytes[slotOffset + 8 + MemoryHandler.Instance.Structures.InventoryItem.MateriaType],
                            },
                            MateriaRanks = new[] {
                                bagSlotsBytes[slotOffset + MemoryHandler.Instance.Structures.InventoryItem.MateriaRank],
                                bagSlotsBytes[slotOffset + 1 + MemoryHandler.Instance.Structures.InventoryItem.MateriaType],
                                bagSlotsBytes[slotOffset + 2 + MemoryHandler.Instance.Structures.InventoryItem.MateriaType],
                                bagSlotsBytes[slotOffset + 3 + MemoryHandler.Instance.Structures.InventoryItem.MateriaType],
                                bagSlotsBytes[slotOffset + 4 + MemoryHandler.Instance.Structures.InventoryItem.MateriaType]
                            },
                            DyeID = bagSlotsBytes[slotOffset + MemoryHandler.Instance.Structures.InventoryItem.DyeID],
                            GlamourID = BitConverter.ToUInt32(bagSlotsBytes, slotOffset + MemoryHandler.Instance.Structures.InventoryItem.GlamourID)
                        });
                    }
                    result.InventoryContainers.Add(container);
                }
            } catch (Exception ex) {
                MemoryHandler.Instance.RaiseException(Logger, ex, true);
            }

            return result;
        }
    }
}