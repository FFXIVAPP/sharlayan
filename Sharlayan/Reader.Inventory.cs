// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Inventory.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
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

    public partial class Reader {
        // Maximum realistic container slot count — guards against torn reads of container.Amount.
        // The largest real bag (player inventory) is 140 slots; saddle bags 70; FC chests ~500.
        // Torn reads can return arbitrarily large values, so cap at 600 to stay safe.
        private const int _inventoryMaxSlots = 600;

        const int _inventoryCount = 74;

        public bool CanGetInventory() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.INVENTORY_KEY);
            if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        public InventoryResult GetInventory() {
            InventoryResult result = new InventoryResult();

            if (!this.CanGetInventory() || !this._memoryHandler.IsAttached) {
                return result;
            }

            // F51: use the FCS-derived struct size as the per-container stride.
            int inventoryByteCount = this._memoryHandler.Structures.InventoryContainer.SourceSize;
            if (inventoryByteCount <= 0) {
                return result;
            }

            byte[] inventoryMap = this._memoryHandler.BufferPool.Rent(_inventoryCount * inventoryByteCount);
            byte[] slotBuffer = null;

            try {
                IntPtr inventoryAddress = new IntPtr(this._memoryHandler.GetInt64(this._memoryHandler.Scanner.Locations[Signatures.INVENTORY_KEY]));
                this._memoryHandler.GetByteArray(inventoryAddress, inventoryMap, _inventoryCount * inventoryByteCount);

                int itemByteCount = 56;

                for (int i = 0; i < _inventoryCount; i++) {
                    int bagIndex = i * inventoryByteCount;
                    uint bagID = BitConverter.ToUInt32(inventoryMap, bagIndex + this._memoryHandler.Structures.InventoryContainer.ID);

                    if (!Enum.IsDefined(typeof(Inventory.Container), bagID)) {
                        continue;
                    }

                    InventoryContainer container = new InventoryContainer {
                        Amount = BitConverter.ToUInt32(inventoryMap, bagIndex + this._memoryHandler.Structures.InventoryContainer.Amount),
                        TypeID = bagID,
                        ContainerType = (Inventory.Container) bagID,
                    };

                    // F17: clamp Amount before any allocation to guard against torn reads.
                    if (container.Amount > _inventoryMaxSlots) {
                        continue;
                    }

                    if (container.Amount == 0) {
                        result.InventoryContainers.Add(container);
                        continue;
                    }

                    IntPtr inventorySlotAddress = new IntPtr(this._memoryHandler.GetInt64FromBytes(inventoryMap, bagIndex));

                    // F19: reuse one pooled buffer for every container's slot data.
                    // A failed read previously yielded a zeroed fresh array (empty items);
                    // clear the reused buffer on failure to preserve that behavior.
                    int slotByteCount = (int) container.Amount * itemByteCount;
                    slotBuffer ??= this._memoryHandler.BufferPool.Rent(_inventoryMaxSlots * itemByteCount);
                    if (!this._memoryHandler.Peek(inventorySlotAddress, slotBuffer, slotByteCount)) {
                        Array.Clear(slotBuffer, 0, slotByteCount);
                    }

                    byte[] slotBytes = slotBuffer;

                    for (int j = 0; j < container.Amount; j++) {
                        int slotIndex = j * itemByteCount;
                        uint itemId = BitConverter.ToUInt32(slotBytes, slotIndex + this._memoryHandler.Structures.InventoryItem.ID);

                        // F18: uint can never be < 0; use == 0 to express empty-slot intent.
                        if (itemId == 0) {
                            container.InventoryItems.Add(new InventoryItem());
                            continue;
                        }

                        int mType = this._memoryHandler.Structures.InventoryItem.MateriaType;
                        int mRank = this._memoryHandler.Structures.InventoryItem.MateriaRank;

                        container.InventoryItems.Add(
                            new InventoryItem {
                                Slot = BitConverter.ToUInt16(slotBytes, slotIndex + this._memoryHandler.Structures.InventoryItem.Slot),
                                ID = itemId,
                                Amount = BitConverter.ToUInt32(slotBytes, slotIndex + this._memoryHandler.Structures.InventoryItem.Amount),
                                SB = BitConverter.ToUInt16(slotBytes, slotIndex + this._memoryHandler.Structures.InventoryItem.SB),
                                Condition = BitConverter.ToUInt16(slotBytes, slotIndex + this._memoryHandler.Structures.InventoryItem.Durability),
                                IsHQ = (slotBytes[slotIndex + this._memoryHandler.Structures.InventoryItem.IsHQ] & 1) == 1,
                                // F56: _materia is FixedSizeArray5<ushort>; each element is 2 bytes.
                                // Read as ushort so materia IDs > 255 (Dawntrail grade VIII/IX/X) are
                                // preserved correctly. Stride of +2 between elements was already correct.
                                MateriaTypes = new[] {
                                    (Inventory.MateriaType) BitConverter.ToUInt16(slotBytes, slotIndex + mType),
                                    (Inventory.MateriaType) BitConverter.ToUInt16(slotBytes, slotIndex + 2 + mType),
                                    (Inventory.MateriaType) BitConverter.ToUInt16(slotBytes, slotIndex + 4 + mType),
                                    (Inventory.MateriaType) BitConverter.ToUInt16(slotBytes, slotIndex + 6 + mType),
                                    (Inventory.MateriaType) BitConverter.ToUInt16(slotBytes, slotIndex + 8 + mType),
                                },
                                MateriaRanks = new[] {
                                    slotBytes[slotIndex + mRank],
                                    slotBytes[slotIndex + 1 + mRank],
                                    slotBytes[slotIndex + 2 + mRank],
                                    slotBytes[slotIndex + 3 + mRank],
                                    slotBytes[slotIndex + 4 + mRank],
                                },
                                DyeID = slotBytes[slotIndex + this._memoryHandler.Structures.InventoryItem.DyeID],
                                GlamourID = BitConverter.ToUInt32(slotBytes, slotIndex + this._memoryHandler.Structures.InventoryItem.GlamourID),
                            });
                    }

                    result.InventoryContainers.Add(container);
                }
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }
            finally {
                this._memoryHandler.BufferPool.Return(inventoryMap);
                if (slotBuffer != null) {
                    this._memoryHandler.BufferPool.Return(slotBuffer);
                }
            }

            return result;
        }
    }
}
