// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Inventory.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
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

            if (!CanGetInventory() || !MemoryHandler.Instance.IsAttached) {
                return result;
            }

            try {
                const int inventoryCount = 74;
                const int inventoryByteCount = 24;

                var inventoryMap = new IntPtr(MemoryHandler.Instance.GetInt64(Scanner.Instance.Locations[Signatures.InventoryKey]));
                var inventoryBytes = MemoryHandler.Instance.GetByteArray(inventoryMap, inventoryCount * 24);

                for (var i = 0; i < inventoryCount; i++) {
                    var bagIndex = i * inventoryByteCount;
                    var bagID = BitConverter.ToUInt32(inventoryBytes, bagIndex + MemoryHandler.Instance.Structures.InventoryContainer.ID);

                    if (!Enum.IsDefined(typeof(Inventory.Container), bagID)) {
                        continue;
                    }

                    var container = new InventoryContainer {
                        Amount = BitConverter.ToUInt32(inventoryBytes, bagIndex + MemoryHandler.Instance.Structures.InventoryContainer.Amount),
                        TypeID = bagID,
                        ContainerType = (Inventory.Container) bagID,
                    };

                    var itemByteCount = 56;

                    var slotMap = new IntPtr(MemoryHandler.Instance.GetInt64FromBytes(inventoryBytes, bagIndex));
                    var slotBytes = MemoryHandler.Instance.GetByteArray(slotMap, (int) container.Amount * itemByteCount);

                    for (var j = 0; j < container.Amount; j++) {
                        var slotIndex = j * itemByteCount;
                        var itemId = BitConverter.ToUInt32(slotBytes, slotIndex + MemoryHandler.Instance.Structures.InventoryItem.ID);

                        if (itemId <= 0) {
                            container.InventoryItems.Add(new InventoryItem());
                            continue;
                        }

                        container.InventoryItems.Add(
                            new InventoryItem {
                                Slot = BitConverter.ToUInt16(slotBytes, slotIndex + MemoryHandler.Instance.Structures.InventoryItem.Slot),
                                ID = itemId,
                                Amount = BitConverter.ToUInt32(slotBytes, slotIndex + MemoryHandler.Instance.Structures.InventoryItem.Amount),
                                SB = BitConverter.ToUInt16(slotBytes, slotIndex + MemoryHandler.Instance.Structures.InventoryItem.SB),
                                Condition = BitConverter.ToUInt16(slotBytes, slotIndex + MemoryHandler.Instance.Structures.InventoryItem.Durability),
                                IsHQ = (slotBytes[slotIndex + MemoryHandler.Instance.Structures.InventoryItem.IsHQ] & 1) == 1,
                                MateriaTypes = new[] {
                                    (Inventory.MateriaType) slotBytes[slotIndex + MemoryHandler.Instance.Structures.InventoryItem.MateriaType],
                                    (Inventory.MateriaType) slotBytes[slotIndex + 2 + MemoryHandler.Instance.Structures.InventoryItem.MateriaType],
                                    (Inventory.MateriaType) slotBytes[slotIndex + 4 + MemoryHandler.Instance.Structures.InventoryItem.MateriaType],
                                    (Inventory.MateriaType) slotBytes[slotIndex + 6 + MemoryHandler.Instance.Structures.InventoryItem.MateriaType],
                                    (Inventory.MateriaType) slotBytes[slotIndex + 8 + MemoryHandler.Instance.Structures.InventoryItem.MateriaType],
                                },
                                MateriaRanks = new[] {
                                    slotBytes[slotIndex + MemoryHandler.Instance.Structures.InventoryItem.MateriaRank],
                                    slotBytes[slotIndex + 1 + MemoryHandler.Instance.Structures.InventoryItem.MateriaRank],
                                    slotBytes[slotIndex + 2 + MemoryHandler.Instance.Structures.InventoryItem.MateriaRank],
                                    slotBytes[slotIndex + 3 + MemoryHandler.Instance.Structures.InventoryItem.MateriaRank],
                                    slotBytes[slotIndex + 4 + MemoryHandler.Instance.Structures.InventoryItem.MateriaRank],
                                },
                                DyeID = slotBytes[slotIndex + MemoryHandler.Instance.Structures.InventoryItem.DyeID],
                                GlamourID = BitConverter.ToUInt32(slotBytes, slotIndex + MemoryHandler.Instance.Structures.InventoryItem.GlamourID),
                            });
                    }

                    result.InventoryContainers.Add(container);
                }
            }
            catch (Exception ex) {
                MemoryHandler.Instance.RaiseException(Logger, ex, true);
            }

            return result;
        }
    }
}