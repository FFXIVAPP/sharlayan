// FFXIVAPP.Memory
// FFXIVAPP & Related Plugins/Modules
// Copyright © 2007 - 2016 Ryan Wilson - All Rights Reserved
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
using FFXIVAPP.Memory.Core;
using FFXIVAPP.Memory.Core.Enums;
using FFXIVAPP.Memory.Delegates;
using FFXIVAPP.Memory.Helpers;
using FFXIVAPP.Memory.Models;

namespace FFXIVAPP.Memory
{
    public static partial class Reader
    {
        public static ActorReadResult GetActors()
        {
            var result = new ActorReadResult();

            if (Scanner.Instance.Locations.ContainsKey("CHARMAP"))
            {
                try
                {
                    #region Ensure Target

                    var targetAddress = IntPtr.Zero;

                    #endregion

                    var endianSize = MemoryHandler.Instance.ProcessModel.IsWin64 ? 8 : 4;

                    const int limit = 1372;

                    var characterAddressMap = MemoryHandler.Instance.GetByteArray(Scanner.Instance.Locations["CHARMAP"], endianSize * limit);
                    var uniqueAddresses = new Dictionary<IntPtr, IntPtr>();
                    var firstAddress = IntPtr.Zero;

                    var firstTime = true;

                    for (var i = 0; i < limit; i++)
                    {
                        IntPtr characterAddress;
                        if (MemoryHandler.Instance.ProcessModel.IsWin64)
                        {
                            characterAddress = new IntPtr(BitConverter.ToInt64(characterAddressMap, i * endianSize));
                        }
                        else
                        {
                            characterAddress = new IntPtr(BitConverter.ToInt32(characterAddressMap, i * endianSize));
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

                    result.PreviousMonster = MonsterWorkerDelegate.EntitiesDictionary.Keys.ToDictionary(key => key);
                    result.PreviousNPC = NPCWorkerDelegate.EntitiesDictionary.Keys.ToDictionary(key => key);
                    result.PreviousPC = PCWorkerDelegate.EntitiesDictionary.Keys.ToDictionary(key => key);

                    foreach (var kvp in uniqueAddresses)
                    {
                        try
                        {
                            var source = MemoryHandler.Instance.GetByteArray(new IntPtr(kvp.Value.ToInt64()), 0x23F0);
                            //var source = MemoryHandler.Instance.GetByteArray(characterAddress, 0x3F40);

                            var ID = BitConverter.ToUInt32(source, MemoryHandler.Instance.Structures.ActorEntity.ID);
                            var NPCID2 = BitConverter.ToUInt32(source, MemoryHandler.Instance.Structures.ActorEntity.NPCID2);
                            var Type = Entity.Type[source[MemoryHandler.Instance.Structures.ActorEntity.Type]];
                            ActorEntity existing = null;
                            var newEntry = false;

                            switch (Type)
                            {
                                case "Monster":
                                    if (result.PreviousMonster.ContainsKey(ID))
                                    {
                                        result.PreviousMonster.Remove(ID);
                                        existing = MonsterWorkerDelegate.GetEntity(ID);
                                    }
                                    else
                                    {
                                        result.NewMonster.Add(ID);
                                        newEntry = true;
                                    }
                                    break;
                                case "PC":
                                    if (result.PreviousPC.ContainsKey(ID))
                                    {
                                        result.PreviousPC.Remove(ID);
                                        existing = PCWorkerDelegate.GetEntity(ID);
                                    }
                                    else
                                    {
                                        result.NewPC.Add(ID);
                                        newEntry = true;
                                    }
                                    break;
                                case "NPC":
                                    if (result.PreviousNPC.ContainsKey(NPCID2))
                                    {
                                        result.PreviousNPC.Remove(NPCID2);
                                        existing = NPCWorkerDelegate.GetEntity(NPCID2);
                                    }
                                    else
                                    {
                                        result.NewNPC.Add(NPCID2);
                                        newEntry = true;
                                    }
                                    break;
                                default:
                                    if (result.PreviousNPC.ContainsKey(ID))
                                    {
                                        result.PreviousNPC.Remove(ID);
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
                                    entry.TargetID = (int) BitConverter.ToUInt32(targetInfoSource, MemoryHandler.Instance.Structures.ActorEntity.ID);
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
                                    case "Monster":
                                        MonsterWorkerDelegate.EnsureEntity(entry.ID, entry);
                                        break;
                                    case "PC":
                                        PCWorkerDelegate.EnsureEntity(entry.ID, entry);
                                        break;
                                    case "NPC":
                                        NPCWorkerDelegate.EnsureEntity(entry.NPCID2, entry);
                                        break;
                                    default:
                                        NPCWorkerDelegate.EnsureEntity(entry.ID, entry);
                                        break;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }

                    // REMOVE OLD MONSTERS FROM LIVE CURRENT DICTIONARY
                    foreach (var kvp in result.PreviousMonster)
                    {
                        MonsterWorkerDelegate.RemoveEntity(kvp.Key);
                    }

                    // REMOVE OLD NPC'S FROM LIVE CURRENT DICTIONARY
                    foreach (var kvp in result.PreviousNPC)
                    {
                        NPCWorkerDelegate.RemoveEntity(kvp.Key);
                    }

                    // REMOVE OLD PC'S FROM LIVE CURRENT DICTIONARY
                    foreach (var kvp in result.PreviousPC)
                    {
                        PCWorkerDelegate.RemoveEntity(kvp.Key);
                    }

                    MemoryHandler.Instance.ScanCount++;

                    #endregion
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return result;
        }
    }
}
