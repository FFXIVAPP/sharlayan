// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Actor.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.Actor.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Sharlayan.Core;
    using Sharlayan.Core.Enums;
    using Sharlayan.Models.ReadResults;
    using Sharlayan.Utilities;

    public partial class Reader {
        private byte[] _actorTargetInfoMap = new byte[128];

        private byte[] _characterAddressMap;

        private Dictionary<uint, DateTime> _expiringActors = new Dictionary<uint, DateTime>();

        private byte[] _sourceMap;

        private Dictionary<IntPtr, IntPtr> _uniqueCharacterAddresses = new Dictionary<IntPtr, IntPtr>();

        public bool CanGetActors() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.CharacterMapKey);
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

            try {
                IntPtr targetAddress = IntPtr.Zero;

                int limit = this._memoryHandler.Structures.ActorItem.EntityCount;
                if (this._characterAddressMap == null) {
                    this._characterAddressMap = new byte[8 * limit];
                }

                int sourceSize = this._memoryHandler.Structures.ActorItem.SourceSize;
                if (this._sourceMap == null) {
                    this._sourceMap = new byte[sourceSize];
                }

                this._memoryHandler.GetByteArray(this._memoryHandler.Scanner.Locations[Signatures.CharacterMapKey], this._characterAddressMap);

                IntPtr firstAddress = IntPtr.Zero;

                DateTime now = DateTime.Now;

                TimeSpan staleActorRemovalTime = TimeSpan.FromSeconds(0.25);

                bool firstTime = true;

                for (int i = 0; i < limit; i++) {
                    IntPtr characterAddress = new IntPtr(SharlayanBitConverter.TryToInt64(this._characterAddressMap, i * 8));

                    if (characterAddress == IntPtr.Zero) {
                        continue;
                    }

                    if (firstTime) {
                        firstAddress = characterAddress;
                        firstTime = false;
                    }

                    this._uniqueCharacterAddresses[characterAddress] = characterAddress;
                }

                foreach (KeyValuePair<uint, ActorItem> kvp in this._monsterWorkerDelegate.ActorItems) {
                    result.RemovedMonsters.TryAdd(kvp.Key, kvp.Value.Clone());
                }

                foreach (KeyValuePair<uint, ActorItem> kvp in this._npcWorkerDelegate.ActorItems) {
                    result.RemovedNPCs.TryAdd(kvp.Key, kvp.Value.Clone());
                }

                foreach (KeyValuePair<uint, ActorItem> kvp in this._pcWorkerDelegate.ActorItems) {
                    result.RemovedPCs.TryAdd(kvp.Key, kvp.Value.Clone());
                }

                (uint mapID, uint mapIndex, uint mapTerritory) = this.GetMapInfo();

                foreach (KeyValuePair<IntPtr, IntPtr> kvp in this._uniqueCharacterAddresses) {
                    try {
                        IntPtr characterAddress = new IntPtr(kvp.Value.ToInt64());
                        this._memoryHandler.GetByteArray(characterAddress, this._sourceMap);

                        uint ID = SharlayanBitConverter.TryToUInt32(this._sourceMap, this._memoryHandler.Structures.ActorItem.ID);
                        uint NPCID2 = SharlayanBitConverter.TryToUInt32(this._sourceMap, this._memoryHandler.Structures.ActorItem.NPCID2);
                        Actor.Type Type = (Actor.Type) this._sourceMap[this._memoryHandler.Structures.ActorItem.Type];

                        ActorItem existing = null;
                        bool newEntry = false;

                        switch (Type) {
                            case Actor.Type.Monster:
                                if (result.RemovedMonsters.ContainsKey(ID)) {
                                    result.RemovedMonsters.TryRemove(ID, out ActorItem removedMonster);
                                    existing = this._monsterWorkerDelegate.GetActorItem(ID);
                                }
                                else {
                                    newEntry = true;
                                }

                                break;
                            case Actor.Type.PC:
                                if (result.RemovedPCs.ContainsKey(ID)) {
                                    result.RemovedPCs.TryRemove(ID, out ActorItem removedPC);
                                    existing = this._pcWorkerDelegate.GetActorItem(ID);
                                }
                                else {
                                    newEntry = true;
                                }

                                break;
                            case Actor.Type.NPC:
                            case Actor.Type.Aetheryte:
                            case Actor.Type.EventObject:
                                if (result.RemovedNPCs.ContainsKey(NPCID2)) {
                                    result.RemovedNPCs.TryRemove(NPCID2, out ActorItem removedNPC);
                                    existing = this._npcWorkerDelegate.GetActorItem(NPCID2);
                                }
                                else {
                                    newEntry = true;
                                }

                                break;
                            default:
                                if (result.RemovedNPCs.ContainsKey(ID)) {
                                    result.RemovedNPCs.TryRemove(ID, out ActorItem removedNPC);
                                    existing = this._npcWorkerDelegate.GetActorItem(ID);
                                }
                                else {
                                    newEntry = true;
                                }

                                break;
                        }

                        bool isFirstEntry = kvp.Value.ToInt64() == firstAddress.ToInt64();

                        ActorItem entry = this._actorItemResolver.ResolveActorFromBytes(this._sourceMap, isFirstEntry, existing);

                        if (entry != null && entry.IsValid) {
                            if (this._expiringActors.ContainsKey(ID)) {
                                this._expiringActors.Remove(ID);
                            }
                        }

                        if (entry.Type == Actor.Type.EventObject) {
                            (ushort EventObjectTypeID, Actor.EventObjectType EventObjectType) = this.GetEventObjectType(targetAddress);
                            entry.EventObjectTypeID = EventObjectTypeID;
                            entry.EventObjectType = EventObjectType;
                        }

                        entry.MapID = mapID;
                        entry.MapIndex = mapIndex;
                        entry.MapTerritory = mapTerritory;

                        if (isFirstEntry) {
                            if (targetAddress.ToInt64() > 0) {
                                this._memoryHandler.GetByteArray(targetAddress, this._actorTargetInfoMap);
                                entry.TargetID = (int) SharlayanBitConverter.TryToUInt32(this._actorTargetInfoMap, this._memoryHandler.Structures.ActorItem.ID);
                            }
                        }

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
                        this._memoryHandler.RaiseException(ex, true);
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
                    foreach (KeyValuePair<uint, DateTime> kvp in this._expiringActors.ToList()) {
                        if (now > kvp.Value) {
                            // Stale actor. Remove it.
                            this._monsterWorkerDelegate.RemoveActorItem(kvp.Key);
                            this._npcWorkerDelegate.RemoveActorItem(kvp.Key);
                            this._pcWorkerDelegate.RemoveActorItem(kvp.Key);

                            this._expiringActors.Remove(kvp.Key);
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
                    this._memoryHandler.RaiseException(ex, true);
                }

                this._memoryHandler.ScanCount++;
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(ex, true);
            }

            result.CurrentMonsters = this._monsterWorkerDelegate.ActorItems;
            result.CurrentNPCs = this._npcWorkerDelegate.ActorItems;
            result.CurrentPCs = this._pcWorkerDelegate.ActorItems;

            this._uniqueCharacterAddresses.Clear();

            return result;
        }
    }
}