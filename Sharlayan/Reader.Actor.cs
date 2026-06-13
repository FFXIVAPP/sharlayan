// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Actor.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.Actor.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using Sharlayan.Core;
    using Sharlayan.Core.Enums;
    using Sharlayan.Models.ReadResults;
    using Sharlayan.Utilities;

    public partial class Reader {
        private ConcurrentDictionary<uint, DateTime> _expiringActors = new ConcurrentDictionary<uint, DateTime>();

        private ConcurrentDictionary<IntPtr, IntPtr> _uniqueCharacterAddresses = new ConcurrentDictionary<IntPtr, IntPtr>();

        public bool CanGetActors() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.CHARMAP_KEY);
            if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        public ActorResult GetActors() {
            ActorResult result = new ActorResult();

            if (!this.CanGetActors() || !this._memoryHandler.IsAttached) {
                return result;
            }

            int limit = this._memoryHandler.Structures.ActorItem.EntityCount;
            int sourceSize = this._memoryHandler.Structures.ActorItem.SourceSize;

            byte[] characterAddressMap = this._memoryHandler.BufferPool.Rent(8 * limit);
            byte[] sourceMap = this._memoryHandler.BufferPool.Rent(sourceSize);

            try {
                this._memoryHandler.GetByteArray(this._memoryHandler.Scanner.Locations[Signatures.CHARMAP_KEY], characterAddressMap);

                IntPtr firstAddress = IntPtr.Zero;

                DateTime now = DateTime.Now;

                TimeSpan staleActorRemovalTime = TimeSpan.FromSeconds(0.25);

                bool firstTime = true;

                for (int i = 0; i < limit; i++) {
                    IntPtr characterAddress = new IntPtr(SharlayanBitConverter.TryToInt64(characterAddressMap, i * 8));

                    if (characterAddress == IntPtr.Zero) {
                        continue;
                    }

                    if (firstTime) {
                        firstAddress = characterAddress;
                        firstTime = false;
                    }

                    this._uniqueCharacterAddresses[characterAddress] = characterAddress;
                }

                // Track IDs seen this scan to determine removals lazily after the loop.
                HashSet<uint> presentMonsterIDs = new HashSet<uint>();
                HashSet<uint> presentNPCIDs = new HashSet<uint>();
                HashSet<uint> presentPCIDs = new HashSet<uint>();

                (uint mapID, uint mapIndex, uint mapTerritory) = this.GetMapInfo();

                foreach (KeyValuePair<IntPtr, IntPtr> kvp in this._uniqueCharacterAddresses) {
                    try {
                        IntPtr characterAddress = new IntPtr(kvp.Value.ToInt64());
                        this._memoryHandler.GetByteArray(characterAddress, sourceMap);

                        uint ID = SharlayanBitConverter.TryToUInt32(sourceMap, this._memoryHandler.Structures.ActorItem.ID);
                        uint NPCID2 = SharlayanBitConverter.TryToUInt32(sourceMap, this._memoryHandler.Structures.ActorItem.NPCID2);
                        Actor.Type Type = (Actor.Type) sourceMap[this._memoryHandler.Structures.ActorItem.Type];

                        ActorItem existing = null;
                        bool newEntry = false;

                        switch (Type) {
                            case Actor.Type.Monster:
                                presentMonsterIDs.Add(ID);
                                existing = this._monsterWorkerDelegate.GetActorItem(ID);
                                if (existing == null) {
                                    newEntry = true;
                                }

                                break;
                            case Actor.Type.PC:
                                presentPCIDs.Add(ID);
                                existing = this._pcWorkerDelegate.GetActorItem(ID);
                                if (existing == null) {
                                    newEntry = true;
                                }

                                break;
                            case Actor.Type.NPC:
                            case Actor.Type.Aetheryte:
                            case Actor.Type.EventObject:
                                presentNPCIDs.Add(NPCID2);
                                existing = this._npcWorkerDelegate.GetActorItem(NPCID2);
                                if (existing == null) {
                                    newEntry = true;
                                }

                                break;
                            default:
                                presentNPCIDs.Add(ID);
                                existing = this._npcWorkerDelegate.GetActorItem(ID);
                                if (existing == null) {
                                    newEntry = true;
                                }

                                break;
                        }

                        bool isFirstEntry = kvp.Value.ToInt64() == firstAddress.ToInt64();

                        ActorItem entry = this._actorItemResolver.ResolveActorFromBytes(sourceMap, isFirstEntry, existing);

                        if (entry != null && entry.IsValid) {
                            if (this._expiringActors.ContainsKey(ID)) {
                                this._expiringActors.TryRemove(ID, out DateTime removedDateTime);
                            }
                        }

                        if (entry.Type == Actor.Type.EventObject) {
                            (ushort EventObjectTypeID, Actor.EventObjectType EventObjectType) = this.GetEventObjectType(characterAddress);
                            entry.EventObjectTypeID = EventObjectTypeID;
                            entry.EventObjectType = EventObjectType;
                        }

                        entry.MapID = mapID;
                        entry.MapIndex = mapIndex;
                        entry.MapTerritory = mapTerritory;

                        if (!entry.IsValid) {
                            result.NewMonsters.TryRemove(entry.ID, out ActorItem _);
                            result.NewMonsters.TryRemove(entry.NPCID2, out ActorItem _);
                            result.NewNPCs.TryRemove(entry.ID, out ActorItem _);
                            result.NewNPCs.TryRemove(entry.NPCID2, out ActorItem _);
                            result.NewPCs.TryRemove(entry.ID, out ActorItem _);
                            result.NewPCs.TryRemove(entry.NPCID2, out ActorItem _);
                            continue;
                        }

                        if (existing != null) {
                            continue;
                        }

                        if (newEntry) {
                            switch (entry.Type) {
                                case Actor.Type.Monster:
                                    this._monsterWorkerDelegate.EnsureActorItem(entry.ID, entry);
                                    result.NewMonsters.TryAdd(entry.ID, entry.Clone());
                                    break;
                                case Actor.Type.PC:
                                    this._pcWorkerDelegate.EnsureActorItem(entry.ID, entry);
                                    result.NewPCs.TryAdd(entry.ID, entry.Clone());
                                    break;
                                case Actor.Type.Aetheryte:
                                case Actor.Type.EventObject:
                                case Actor.Type.NPC:
                                    this._npcWorkerDelegate.EnsureActorItem(entry.NPCID2, entry);
                                    result.NewNPCs.TryAdd(entry.NPCID2, entry.Clone());
                                    break;
                                default:
                                    this._npcWorkerDelegate.EnsureActorItem(entry.ID, entry);
                                    result.NewNPCs.TryAdd(entry.ID, entry.Clone());
                                    break;
                            }
                        }
                    }
                    catch (Exception ex) {
                        this._memoryHandler.RaiseException(Logger, ex);
                    }
                }

                // Clone only the actors genuinely absent from this scan into the Removed sets.
                foreach (KeyValuePair<uint, ActorItem> kvp in this._monsterWorkerDelegate.ActorItems) {
                    if (!presentMonsterIDs.Contains(kvp.Key)) {
                        result.RemovedMonsters.TryAdd(kvp.Key, kvp.Value.Clone());
                    }
                }

                foreach (KeyValuePair<uint, ActorItem> kvp in this._npcWorkerDelegate.ActorItems) {
                    if (!presentNPCIDs.Contains(kvp.Key)) {
                        result.RemovedNPCs.TryAdd(kvp.Key, kvp.Value.Clone());
                    }
                }

                foreach (KeyValuePair<uint, ActorItem> kvp in this._pcWorkerDelegate.ActorItems) {
                    if (!presentPCIDs.Contains(kvp.Key)) {
                        result.RemovedPCs.TryAdd(kvp.Key, kvp.Value.Clone());
                    }
                }

                try {
                    // add the "removed" actors to the expiring list
                    foreach (KeyValuePair<uint, ActorItem> kvp in result.RemovedMonsters) {
                        if (!this._expiringActors.ContainsKey(kvp.Key)) {
                            this._expiringActors[kvp.Key] = now + staleActorRemovalTime;
                        }
                    }

                    foreach (KeyValuePair<uint, ActorItem> kvp in result.RemovedNPCs) {
                        if (!this._expiringActors.ContainsKey(kvp.Key)) {
                            this._expiringActors[kvp.Key] = now + staleActorRemovalTime;
                        }
                    }

                    foreach (KeyValuePair<uint, ActorItem> kvp in result.RemovedPCs) {
                        if (!this._expiringActors.ContainsKey(kvp.Key)) {
                            this._expiringActors[kvp.Key] = now + staleActorRemovalTime;
                        }
                    }

                    // check expiring list for stale actors
                    foreach (KeyValuePair<uint, DateTime> kvp in this._expiringActors) {
                        if (now > kvp.Value) {
                            // Stale actor. Remove it.
                            this._monsterWorkerDelegate.RemoveActorItem(kvp.Key);
                            this._npcWorkerDelegate.RemoveActorItem(kvp.Key);
                            this._pcWorkerDelegate.RemoveActorItem(kvp.Key);

                            this._expiringActors.TryRemove(kvp.Key, out DateTime removedDateTime);
                        }
                        else {
                            // Not stale enough yet. We're not actually removing it.
                            result.RemovedMonsters.TryRemove(kvp.Key, out ActorItem _);
                            result.RemovedNPCs.TryRemove(kvp.Key, out ActorItem _);
                            result.RemovedPCs.TryRemove(kvp.Key, out ActorItem _);
                        }
                    }
                }
                catch (Exception ex) {
                    this._memoryHandler.RaiseException(Logger, ex);
                }

                this._memoryHandler.ScanCount++;
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }
            finally {
                this._memoryHandler.BufferPool.Return(characterAddressMap);
                this._memoryHandler.BufferPool.Return(sourceMap);
            }

            result.CurrentMonsters = this._monsterWorkerDelegate.ActorItems;
            result.CurrentNPCs = this._npcWorkerDelegate.ActorItems;
            result.CurrentPCs = this._pcWorkerDelegate.ActorItems;

            this._uniqueCharacterAddresses.Clear();

            return result;
        }
    }
}