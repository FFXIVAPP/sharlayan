// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Actor.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.Actor.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Collections.Generic;

    using Sharlayan.Core;
    using Sharlayan.Core.Enums;
    using Sharlayan.Delegates;
    using Sharlayan.Models.ReadResults;
    using Sharlayan.Utilities;

    using BitConverter = Sharlayan.Utilities.BitConverter;

    public static partial class Reader {
        public static bool CanGetActors() {
            var canRead = Scanner.Instance.Locations.ContainsKey(Signatures.CharacterMapKey);
            if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        public static ActorResult GetActors() {
            var result = new ActorResult();

            if (!CanGetActors() || !MemoryHandler.Instance.IsAttached) {
                return result;
            }

            try {
                IntPtr targetAddress = IntPtr.Zero;

                var endianSize = MemoryHandler.Instance.ProcessModel.IsWin64
                                     ? 8
                                     : 4;

                var sourceSize = MemoryHandler.Instance.Structures.ActorItem.SourceSize;
                var limit = MemoryHandler.Instance.Structures.ActorItem.EntityCount;
                byte[] characterAddressMap = MemoryHandler.Instance.GetByteArray(Scanner.Instance.Locations[Signatures.CharacterMapKey], endianSize * limit);
                Dictionary<IntPtr, IntPtr> uniqueAddresses = new Dictionary<IntPtr, IntPtr>();
                IntPtr firstAddress = IntPtr.Zero;

                var firstTime = true;

                for (var i = 0; i < limit; i++) {
                    IntPtr characterAddress;

                    if (MemoryHandler.Instance.ProcessModel.IsWin64) {
                        characterAddress = new IntPtr(BitConverter.TryToInt64(characterAddressMap, i * endianSize));
                    }
                    else {
                        characterAddress = new IntPtr(BitConverter.TryToInt32(characterAddressMap, i * endianSize));
                    }

                    if (characterAddress == IntPtr.Zero) {
                        continue;
                    }

                    if (firstTime) {
                        firstAddress = characterAddress;
                        firstTime = false;
                    }

                    uniqueAddresses[characterAddress] = characterAddress;
                }

                foreach (KeyValuePair<uint, ActorItem> kvp in MonsterWorkerDelegate.ActorItems) {
                    result.RemovedMonsters.TryAdd(kvp.Key, kvp.Value.Clone());
                }

                foreach (KeyValuePair<uint, ActorItem> kvp in NPCWorkerDelegate.ActorItems) {
                    result.RemovedNPCs.TryAdd(kvp.Key, kvp.Value.Clone());
                }

                foreach (KeyValuePair<uint, ActorItem> kvp in PCWorkerDelegate.ActorItems) {
                    result.RemovedPCs.TryAdd(kvp.Key, kvp.Value.Clone());
                }

                foreach (KeyValuePair<IntPtr, IntPtr> kvp in uniqueAddresses) {
                    try {
                        var characterAddress = new IntPtr(kvp.Value.ToInt64());
                        byte[] source = MemoryHandler.Instance.GetByteArray(characterAddress, sourceSize);

                        // var source = MemoryHandler.Instance.GetByteArray(characterAddress, 0x3F40);
                        var ID = BitConverter.TryToUInt32(source, MemoryHandler.Instance.Structures.ActorItem.ID);
                        var NPCID2 = BitConverter.TryToUInt32(source, MemoryHandler.Instance.Structures.ActorItem.NPCID2);
                        var TypeID = source[MemoryHandler.Instance.Structures.ActorItem.Type];
                        var Type = Actor.Type.Unknown;
                        if (Enum.IsDefined(typeof(Actor.Type), TypeID)) {
                            Type = (Actor.Type) TypeID;
                        }

                        ActorItem existing = null;
                        var newEntry = false;

                        switch (Type) {
                            case Actor.Type.Monster:
                                if (result.RemovedMonsters.ContainsKey(ID)) {
                                    result.RemovedMonsters.TryRemove(ID, out ActorItem removedMonster);
                                    existing = MonsterWorkerDelegate.GetActorItem(ID);
                                }
                                else {
                                    newEntry = true;
                                }

                                break;
                            case Actor.Type.PC:
                                if (result.RemovedPCs.ContainsKey(ID)) {
                                    result.RemovedPCs.TryRemove(ID, out ActorItem removedPC);
                                    existing = PCWorkerDelegate.GetActorItem(ID);
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
                                    existing = NPCWorkerDelegate.GetActorItem(NPCID2);
                                }
                                else {
                                    newEntry = true;
                                }

                                break;
                            default:
                                if (result.RemovedNPCs.ContainsKey(ID)) {
                                    result.RemovedNPCs.TryRemove(ID, out ActorItem removedNPC);
                                    existing = NPCWorkerDelegate.GetActorItem(ID);
                                }
                                else {
                                    newEntry = true;
                                }

                                break;
                        }

                        var isFirstEntry = kvp.Value.ToInt64() == firstAddress.ToInt64();

                        ActorItem entry = ActorItemResolver.ResolveActorFromBytes(source, isFirstEntry, existing);

                        if (entry.Type == Actor.Type.EventObject) {
                            var (EventObjectTypeID, EventObjectType) = GetEventObjectType(targetAddress);
                            entry.EventObjectTypeID = EventObjectTypeID;
                            entry.EventObjectType = EventObjectType;
                        }

                        EnsureMapAndZone(entry);

                        if (isFirstEntry) {
                            if (targetAddress.ToInt64() > 0) {
                                byte[] targetInfoSource = MemoryHandler.Instance.GetByteArray(targetAddress, 128);
                                entry.TargetID = (int) BitConverter.TryToUInt32(targetInfoSource, MemoryHandler.Instance.Structures.ActorItem.ID);
                            }
                        }

                        // it doesn't matter what this is set to; it won't be used in code below
                        ActorItem removed;

                        if (!entry.IsValid) {
                            result.NewMonsters.TryRemove(entry.ID, out removed);
                            result.NewMonsters.TryRemove(entry.NPCID2, out removed);
                            result.NewNPCs.TryRemove(entry.ID, out removed);
                            result.NewNPCs.TryRemove(entry.NPCID2, out removed);
                            result.NewPCs.TryRemove(entry.ID, out removed);
                            result.NewPCs.TryRemove(entry.NPCID2, out removed);
                            continue;
                        }

                        if (existing != null) {
                            continue;
                        }

                        if (newEntry) {
                            switch (entry.Type) {
                                case Actor.Type.Monster:
                                    MonsterWorkerDelegate.EnsureActorItem(entry.ID, entry);
                                    result.NewMonsters.TryAdd(entry.ID, entry.Clone());
                                    break;
                                case Actor.Type.PC:
                                    PCWorkerDelegate.EnsureActorItem(entry.ID, entry);
                                    result.NewPCs.TryAdd(entry.ID, entry.Clone());
                                    break;
                                case Actor.Type.Aetheryte:
                                case Actor.Type.EventObject:
                                case Actor.Type.NPC:
                                    NPCWorkerDelegate.EnsureActorItem(entry.NPCID2, entry);
                                    result.NewNPCs.TryAdd(entry.NPCID2, entry.Clone());
                                    break;
                                default:
                                    NPCWorkerDelegate.EnsureActorItem(entry.ID, entry);
                                    result.NewNPCs.TryAdd(entry.ID, entry.Clone());
                                    break;
                            }
                        }
                    }
                    catch (Exception ex) {
                        MemoryHandler.Instance.RaiseException(Logger, ex, true);
                    }
                }

                try {
                    // REMOVE OLD MONSTERS FROM LIVE CURRENT DICTIONARY
                    foreach (KeyValuePair<uint, ActorItem> kvp in result.RemovedMonsters) {
                        MonsterWorkerDelegate.RemoveActorItem(kvp.Key);
                    }

                    // REMOVE OLD NPC'S FROM LIVE CURRENT DICTIONARY
                    foreach (KeyValuePair<uint, ActorItem> kvp in result.RemovedNPCs) {
                        NPCWorkerDelegate.RemoveActorItem(kvp.Key);
                    }

                    // REMOVE OLD PC'S FROM LIVE CURRENT DICTIONARY
                    foreach (KeyValuePair<uint, ActorItem> kvp in result.RemovedPCs) {
                        PCWorkerDelegate.RemoveActorItem(kvp.Key);
                    }
                }
                catch (Exception ex) {
                    MemoryHandler.Instance.RaiseException(Logger, ex, true);
                }

                MemoryHandler.Instance.ScanCount++;
            }
            catch (Exception ex) {
                MemoryHandler.Instance.RaiseException(Logger, ex, true);
            }

            return result;
        }
    }
}