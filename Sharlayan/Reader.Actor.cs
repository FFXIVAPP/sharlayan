// Sharlayan ~ Reader.Actor.cs
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
using System.Linq;
using Sharlayan.Core;
using Sharlayan.Core.Enums;
using Sharlayan.Delegates;
using Sharlayan.Helpers;
using Sharlayan.Models;
using BitConverter = Sharlayan.Helpers.BitConverter;

namespace Sharlayan
{
    public static partial class Reader
    {
        public static bool CanGetActors()
        {
            var canRead = Scanner.Instance.Locations.ContainsKey(Signatures.CharacterMapKey);
            if (canRead)
            {
                // OTHER STUFF?
            }
            return canRead;
        }

        public static ActorReadResult GetActors()
        {
            var result = new ActorReadResult();

            if (!CanGetActors() || !MemoryHandler.Instance.IsAttached)
            {
                return result;
            }

            try
            {
                #region Ensure Target

                var targetAddress = IntPtr.Zero;

                #endregion

                var endianSize = MemoryHandler.Instance.ProcessModel.IsWin64 ? 8 : 4;

                var limit = MemoryHandler.Instance.Structures.ActorInfo.Size;

                var characterAddressMap = MemoryHandler.Instance.GetByteArray(Scanner.Instance.Locations[Signatures.CharacterMapKey], endianSize * limit);
                var uniqueAddresses = new Dictionary<IntPtr, IntPtr>();
                var firstAddress = IntPtr.Zero;

                var firstTime = true;

                for (var i = 0; i < limit; i++)
                {
                    IntPtr characterAddress;

                    if (MemoryHandler.Instance.ProcessModel.IsWin64)
                    {
                        characterAddress = new IntPtr(BitConverter.TryToInt64(characterAddressMap, i * endianSize));
                    }
                    else
                    {
                        characterAddress = new IntPtr(BitConverter.TryToInt32(characterAddressMap, i * endianSize));
                    }
                    if (characterAddress == IntPtr.Zero)
                    {
                        continue;
                    }

                    if (firstTime)
                    {
                        firstAddress = characterAddress;
                        firstTime = false;
                    }
                    uniqueAddresses[characterAddress] = characterAddress;
                }

                #region ActorEntity Handlers

                result.RemovedMonster = MonsterWorkerDelegate.EntitiesDictionary.Keys.ToDictionary(key => key);
                result.RemovedNPC = NPCWorkerDelegate.EntitiesDictionary.Keys.ToDictionary(key => key);
                result.RemovedPC = PCWorkerDelegate.EntitiesDictionary.Keys.ToDictionary(key => key);

                foreach (var kvp in uniqueAddresses)
                {
                    try
                    {
                        var source = MemoryHandler.Instance.GetByteArray(new IntPtr(kvp.Value.ToInt64()), 0x23F0);
                        //var source = MemoryHandler.Instance.GetByteArray(characterAddress, 0x3F40);

                        var ID = BitConverter.TryToUInt32(source, MemoryHandler.Instance.Structures.ActorEntity.ID);
                        var NPCID2 = BitConverter.TryToUInt32(source, MemoryHandler.Instance.Structures.ActorEntity.NPCID2);
                        var Type = (Actor.Type) source[MemoryHandler.Instance.Structures.ActorEntity.Type];
                        ActorEntity existing = null;
                        var newEntry = false;

                        switch (Type)
                        {
                            case Actor.Type.Monster:
                                if (result.RemovedMonster.ContainsKey(ID))
                                {
                                    result.RemovedMonster.Remove(ID);
                                    existing = MonsterWorkerDelegate.GetEntity(ID);
                                }
                                else
                                {
                                    result.NewMonster.Add(ID);
                                    newEntry = true;
                                }
                                break;
                            case Actor.Type.PC:
                                if (result.RemovedPC.ContainsKey(ID))
                                {
                                    result.RemovedPC.Remove(ID);
                                    existing = PCWorkerDelegate.GetEntity(ID);
                                }
                                else
                                {
                                    result.NewPC.Add(ID);
                                    newEntry = true;
                                }
                                break;
                            case Actor.Type.NPC:
                            case Actor.Type.Aetheryte:
                            case Actor.Type.EObj:
                                if (result.RemovedNPC.ContainsKey(NPCID2))
                                {
                                    result.RemovedNPC.Remove(NPCID2);
                                    existing = NPCWorkerDelegate.GetEntity(NPCID2);
                                }
                                else
                                {
                                    result.NewNPC.Add(NPCID2);
                                    newEntry = true;
                                }
                                break;
                            default:
                                if (result.RemovedNPC.ContainsKey(ID))
                                {
                                    result.RemovedNPC.Remove(ID);
                                    existing = NPCWorkerDelegate.GetEntity(ID);
                                }
                                else
                                {
                                    result.NewNPC.Add(ID);
                                    newEntry = true;
                                }
                                break;
                        }

                        var isFirstEntry = kvp.Value.ToInt64() == firstAddress.ToInt64();

                        var entry = ActorEntityHelper.ResolveActorFromBytes(source, isFirstEntry, existing);

                        #region Ensure Map & Zone

                        EnsureMapAndZone(entry);

                        #endregion

                        if (isFirstEntry)
                        {
                            if (targetAddress.ToInt64() > 0)
                            {
                                var targetInfoSource = MemoryHandler.Instance.GetByteArray(targetAddress, 128);
                                entry.TargetID = (int) BitConverter.TryToUInt32(targetInfoSource, MemoryHandler.Instance.Structures.ActorEntity.ID);
                            }
                        }
                        if (!entry.IsValid)
                        {
                            result.NewMonster.Remove(entry.ID);
                            result.NewMonster.Remove(entry.NPCID2);
                            result.NewNPC.Remove(entry.ID);
                            result.NewNPC.Remove(entry.NPCID2);
                            result.NewPC.Remove(entry.ID);
                            result.NewPC.Remove(entry.NPCID2);
                            continue;
                        }
                        if (existing != null)
                        {
                            continue;
                        }

                        if (newEntry)
                        {
                            switch (entry.Type)
                            {
                                case Actor.Type.Monster:
                                    MonsterWorkerDelegate.EnsureEntity(entry.ID, entry);
                                    break;
                                case Actor.Type.PC:
                                    PCWorkerDelegate.EnsureEntity(entry.ID, entry);
                                    break;
                                case Actor.Type.Aetheryte:
                                case Actor.Type.EObj:
                                case Actor.Type.NPC:
                                    NPCWorkerDelegate.EnsureEntity(entry.NPCID2, entry);
                                    break;
                                default:
                                    NPCWorkerDelegate.EnsureEntity(entry.ID, entry);
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MemoryHandler.Instance.RaiseException(Logger, ex, true);
                    }
                }

                try
                {
                    // REMOVE OLD MONSTERS FROM LIVE CURRENT DICTIONARY
                    foreach (var kvp in result.RemovedMonster)
                    {
                        MonsterWorkerDelegate.RemoveEntity(kvp.Key);
                    }

                    // REMOVE OLD NPC'S FROM LIVE CURRENT DICTIONARY
                    foreach (var kvp in result.RemovedNPC)
                    {
                        NPCWorkerDelegate.RemoveEntity(kvp.Key);
                    }

                    // REMOVE OLD PC'S FROM LIVE CURRENT DICTIONARY
                    foreach (var kvp in result.RemovedPC)
                    {
                        PCWorkerDelegate.RemoveEntity(kvp.Key);
                    }
                }
                catch (Exception ex)
                {
                    MemoryHandler.Instance.RaiseException(Logger, ex, true);
                }

                MemoryHandler.Instance.ScanCount++;

                #endregion
            }
            catch (Exception ex)
            {
                MemoryHandler.Instance.RaiseException(Logger, ex, true);
            }

            return result;
        }
    }
}
