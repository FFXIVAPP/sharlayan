// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Target.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.Target.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Diagnostics;

    using Sharlayan.Core;
    using Sharlayan.Core.Enums;
    using Sharlayan.Models.ReadResults;
    using Sharlayan.Utilities;

    public partial class Reader {
        public bool CanGetTargetInfo() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.CHARMAP_KEY) && this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.TARGET_KEY);
            if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        public TargetResult GetTargetInfo() {
            TargetResult result = new TargetResult();

            if (!this.CanGetTargetInfo() || !this._memoryHandler.IsAttached) {
                return result;
            }

            byte[] targetInfoMap = this._memoryHandler.BufferPool.Rent(this._memoryHandler.Structures.TargetInfo.SourceSize);

            try {
                IntPtr targetAddress = (IntPtr) this._memoryHandler.Scanner.Locations[Signatures.TARGET_KEY];

                if (targetAddress.ToInt64() > 0) {
                    this._memoryHandler.GetByteArray(targetAddress, targetInfoMap);

                    long currentTargetAddress = this._memoryHandler.GetInt64FromBytes(targetInfoMap, this._memoryHandler.Structures.TargetInfo.Current);
                    long mouseOverTargetAddress = this._memoryHandler.GetInt64FromBytes(targetInfoMap, this._memoryHandler.Structures.TargetInfo.MouseOver);
                    long focusTargetAddress = this._memoryHandler.GetInt64FromBytes(targetInfoMap, this._memoryHandler.Structures.TargetInfo.Focus);
                    long previousTargetAddress = this._memoryHandler.GetInt64FromBytes(targetInfoMap, this._memoryHandler.Structures.TargetInfo.Previous);

                    uint currentTargetID = SharlayanBitConverter.TryToUInt32(targetInfoMap, this._memoryHandler.Structures.TargetInfo.CurrentID);

                    if (currentTargetAddress > 0) {
                        try {
                            ActorItem entry = this.GetTargetActorItemFromSource(currentTargetAddress);
                            currentTargetID = entry.ID;
                            if (entry.IsValid) {
                                result.TargetsFound = true;
                                result.TargetInfo.CurrentTarget = entry;
                            }
                        }
                        catch (Exception ex) {
                            this._memoryHandler.RaiseException(Logger, ex);
                        }
                    }

                    if (mouseOverTargetAddress > 0) {
                        try {
                            ActorItem entry = this.GetTargetActorItemFromSource(mouseOverTargetAddress);
                            if (entry.IsValid) {
                                result.TargetsFound = true;
                                result.TargetInfo.MouseOverTarget = entry;
                            }
                        }
                        catch (Exception ex) {
                            this._memoryHandler.RaiseException(Logger, ex);
                        }
                    }

                    if (focusTargetAddress > 0) {
                        try {
                            ActorItem entry = this.GetTargetActorItemFromSource(focusTargetAddress);
                            if (entry.IsValid) {
                                result.TargetsFound = true;
                                result.TargetInfo.FocusTarget = entry;
                            }
                        }
                        catch (Exception ex) {
                            this._memoryHandler.RaiseException(Logger, ex);
                        }
                    }

                    if (previousTargetAddress > 0) {
                        try {
                            ActorItem entry = this.GetTargetActorItemFromSource(previousTargetAddress);
                            if (entry.IsValid) {
                                result.TargetsFound = true;
                                result.TargetInfo.PreviousTarget = entry;
                            }
                        }
                        catch (Exception ex) {
                            this._memoryHandler.RaiseException(Logger, ex);
                        }
                    }

                    if (currentTargetID > 0) {
                        result.TargetsFound = true;
                        result.TargetInfo.CurrentTargetID = currentTargetID;
                    }
                }

                if (result.TargetInfo.CurrentTargetID > 0) {
                    try {
                        if (this.CanGetEnmityEntities()) {
                            IntPtr counter = (IntPtr) this._memoryHandler.Scanner.Locations[Signatures.ENMITY_COUNT_KEY] + this._memoryHandler.Structures.EnmityItem.EnmityCount;
                            short enmityCount = this._memoryHandler.GetInt16(counter);
                            IntPtr enmityStructure = (IntPtr) this._memoryHandler.Scanner.Locations[Signatures.ENMITYMAP_KEY];

                            if (enmityCount > 0 && enmityCount < 16 && enmityStructure.ToInt64() > 0) {
                                int enmitySourceSize = this._memoryHandler.Structures.EnmityItem.SourceSize;
                                for (uint i = 0; i < enmityCount; i++) {
                                    try {
                                        IntPtr address = new IntPtr(enmityStructure.ToInt64() + i * enmitySourceSize);
                                        
                                        EnmityItem enmityEntry = new EnmityItem {
                                            ID = this._memoryHandler.GetUInt32(address, this._memoryHandler.Structures.EnmityItem.ID),
                                            Name = this._memoryHandler.GetString(address + this._memoryHandler.Structures.EnmityItem.Name),
                                            Enmity = this._memoryHandler.GetUInt32(address + this._memoryHandler.Structures.EnmityItem.Enmity),
                                        };

                                        if (enmityEntry.ID <= 0) {
                                            continue;
                                        }

                                        if (string.IsNullOrWhiteSpace(enmityEntry.Name)) {
                                            ActorItem pc = this._pcWorkerDelegate.GetActorItem(enmityEntry.ID);
                                            ActorItem npc = this._npcWorkerDelegate.GetActorItem(enmityEntry.ID);
                                            ActorItem monster = this._monsterWorkerDelegate.GetActorItem(enmityEntry.ID);
                                            try {
                                                enmityEntry.Name = (pc ?? npc).Name ?? monster.Name;
                                            }
                                            catch (Exception ex) {
                                                this._memoryHandler.RaiseException(Logger, ex);
                                            }
                                        }

                                        result.TargetInfo.EnmityItems.Add(enmityEntry);
                                    }
                                    catch (Exception ex) {
                                        this._memoryHandler.RaiseException(Logger, ex);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex) {
                        this._memoryHandler.RaiseException(Logger, ex);
                    }
                }
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }
            finally {
                this._memoryHandler.BufferPool.Return(targetInfoMap);
            }

            return result;
        }

        private ActorItem GetTargetActorItemFromSource(long address) {
            ActorItem entry;

            IntPtr targetAddress = new IntPtr(address);

            byte[] targetMap = this._memoryHandler.BufferPool.Rent(this._memoryHandler.Structures.TargetInfo.Size);

            try {
                this._memoryHandler.GetByteArray(targetAddress, targetMap);

                entry = this._actorItemResolver.ResolveActorFromBytes(targetMap);

                if (entry.Type == Actor.Type.EventObject) {
                    (ushort EventObjectTypeID, Actor.EventObjectType EventObjectType) = this.GetEventObjectType(targetAddress);
                    entry.EventObjectTypeID = EventObjectTypeID;
                    entry.EventObjectType = EventObjectType;
                }

                (uint mapID, uint mapIndex, uint mapTerritory) = this.GetMapInfo();

                entry.MapID = mapID;
                entry.MapIndex = mapIndex;
                entry.MapTerritory = mapTerritory;
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
                entry = new ActorItem();
            }
            finally {
                this._memoryHandler.BufferPool.Return(targetMap);
            }

            return entry;
        }
    }
}