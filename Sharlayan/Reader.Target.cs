// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Target.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.Target.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;

    using Sharlayan.Core;
    using Sharlayan.Core.Enums;
    using Sharlayan.Models.ReadResults;
    using Sharlayan.Utilities;

    public partial class Reader {
        private byte[] _targetInfoMap;

        private byte[] _targetMap;

        public bool CanGetTargetInfo() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.CharacterMapKey) && this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.TargetKey);
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

            if (this._targetInfoMap == null) {
                this._targetInfoMap = new byte[this._memoryHandler.Structures.TargetInfo.SourceSize];
            }

            try {
                IntPtr targetAddress = (IntPtr) this._memoryHandler.Scanner.Locations[Signatures.TargetKey];

                if (targetAddress.ToInt64() > 0) {
                    this._memoryHandler.GetByteArray(targetAddress, this._targetInfoMap);

                    long currentTarget = this._memoryHandler.GetInt64FromBytes(this._targetInfoMap, this._memoryHandler.Structures.TargetInfo.Current);
                    long mouseOverTarget = this._memoryHandler.GetInt64FromBytes(this._targetInfoMap, this._memoryHandler.Structures.TargetInfo.MouseOver);
                    long focusTarget = this._memoryHandler.GetInt64FromBytes(this._targetInfoMap, this._memoryHandler.Structures.TargetInfo.Focus);
                    long previousTarget = this._memoryHandler.GetInt64FromBytes(this._targetInfoMap, this._memoryHandler.Structures.TargetInfo.Previous);

                    uint currentTargetID = SharlayanBitConverter.TryToUInt32(this._targetInfoMap, this._memoryHandler.Structures.TargetInfo.CurrentID);

                    if (currentTarget > 0) {
                        try {
                            ActorItem entry = this.GetTargetActorItemFromSource(currentTarget);
                            currentTargetID = entry.ID;
                            if (entry.IsValid) {
                                result.TargetsFound = true;
                                result.TargetInfo.CurrentTarget = entry;
                            }
                        }
                        catch (Exception ex) {
                            this._memoryHandler.RaiseException(Logger, ex, true);
                        }
                    }

                    if (mouseOverTarget > 0) {
                        try {
                            ActorItem entry = this.GetTargetActorItemFromSource(mouseOverTarget);
                            if (entry.IsValid) {
                                result.TargetsFound = true;
                                result.TargetInfo.MouseOverTarget = entry;
                            }
                        }
                        catch (Exception ex) {
                            this._memoryHandler.RaiseException(Logger, ex, true);
                        }
                    }

                    if (focusTarget > 0) {
                        try {
                            ActorItem entry = this.GetTargetActorItemFromSource(focusTarget);
                            if (entry.IsValid) {
                                result.TargetsFound = true;
                                result.TargetInfo.FocusTarget = entry;
                            }
                        }
                        catch (Exception ex) {
                            this._memoryHandler.RaiseException(Logger, ex, true);
                        }
                    }

                    if (previousTarget > 0) {
                        try {
                            ActorItem entry = this.GetTargetActorItemFromSource(previousTarget);
                            if (entry.IsValid) {
                                result.TargetsFound = true;
                                result.TargetInfo.PreviousTarget = entry;
                            }
                        }
                        catch (Exception ex) {
                            this._memoryHandler.RaiseException(Logger, ex, true);
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
                            short enmityCount = this._memoryHandler.GetInt16(this._memoryHandler.Scanner.Locations[Signatures.EnmityCountKey]);
                            IntPtr enmityStructure = (IntPtr) this._memoryHandler.Scanner.Locations[Signatures.EnmityMapKey];

                            if (enmityCount > 0 && enmityCount < 16 && enmityStructure.ToInt64() > 0) {
                                int enmitySourceSize = this._memoryHandler.Structures.EnmityItem.SourceSize;
                                for (uint i = 0; i < enmityCount; i++) {
                                    try {
                                        IntPtr address = new IntPtr(enmityStructure.ToInt64() + i * enmitySourceSize);
                                        EnmityItem enmityEntry = new EnmityItem {
                                            ID = this._memoryHandler.GetUInt32(address, this._memoryHandler.Structures.EnmityItem.ID),
                                            // Name = this._memoryHandler.GetString(address + this._memoryHandler.Structures.EnmityItem.Name),
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
                                                this._memoryHandler.RaiseException(Logger, ex, true);
                                            }
                                        }

                                        result.TargetInfo.EnmityItems.Add(enmityEntry);
                                    }
                                    catch (Exception ex) {
                                        this._memoryHandler.RaiseException(Logger, ex, true);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex) {
                        this._memoryHandler.RaiseException(Logger, ex, true);
                    }
                }
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex, true);
            }

            return result;
        }

        private ActorItem GetTargetActorItemFromSource(long address) {
            IntPtr targetAddress = new IntPtr(address);

            if (this._targetMap == null) {
                this._targetMap = new byte[this._memoryHandler.Structures.TargetInfo.Size];
            }

            this._memoryHandler.GetByteArray(targetAddress, this._targetMap);
            ActorItem entry = this._actorItemResolver.ResolveActorFromBytes(this._targetMap);

            if (entry.Type == Actor.Type.EventObject) {
                (ushort EventObjectTypeID, Actor.EventObjectType EventObjectType) = this.GetEventObjectType(targetAddress);
                entry.EventObjectTypeID = EventObjectTypeID;
                entry.EventObjectType = EventObjectType;
            }

            this.EnsureMapAndZone(entry);

            return entry;
        }
    }
}