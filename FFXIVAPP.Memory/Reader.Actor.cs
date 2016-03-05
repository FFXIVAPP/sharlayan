// FFXIVAPP.Memory ~ Reader.Actor.cs
// 
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FFXIVAPP.Memory.Core;
using FFXIVAPP.Memory.Core.Enums;
using FFXIVAPP.Memory.Delegates;
using FFXIVAPP.Memory.Helpers;

namespace FFXIVAPP.Memory
{
    public class ActorReadResult
    {
        public ActorReadResult()
        {
            PreviousMonster = new Dictionary<uint, uint>();
            PreviousNPC = new Dictionary<uint, uint>();
            PreviousPC = new Dictionary<uint, uint>();

            NewMonster = new List<uint>();
            NewNPC = new List<uint>();
            NewPC = new List<uint>();
        }

        public ConcurrentDictionary<uint, ActorEntity> MonsterEntities => MonsterWorkerDelegate.EntitiesDictionary;
        public ConcurrentDictionary<uint, ActorEntity> NPCEntities => NPCWorkerDelegate.EntitiesDictionary;
        public ConcurrentDictionary<uint, ActorEntity> PCEntities => PCWorkerDelegate.EntitiesDictionary;
        public Dictionary<uint, uint> PreviousMonster { get; set; }
        public Dictionary<uint, uint> PreviousNPC { get; set; }
        public Dictionary<uint, uint> PreviousPC { get; set; }
        public List<UInt32> NewMonster { get; set; }
        public List<UInt32> NewNPC { get; set; }
        public List<UInt32> NewPC { get; set; }
    }

    public static partial class Reader
    {
        public static ActorReadResult GetActors()
        {
            var result = new ActorReadResult();

            if (Scanner.Instance.Locations.ContainsKey("CHARMAP"))
            {
                try
                {
                    #region Ensure Target & Map

                    var targetAddress = IntPtr.Zero;
                    uint mapIndex = 0;
                    if (Scanner.Instance.Locations.ContainsKey("TARGET"))
                    {
                        try
                        {
                            targetAddress = Scanner.Instance.Locations["TARGET"];
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    if (Scanner.Instance.Locations.ContainsKey("MAP"))
                    {
                        try
                        {
                            mapIndex = (uint) MemoryHandler.Instance.GetPlatformUInt(Scanner.Instance.Locations["MAP"]);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

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

                            UInt32 ID;
                            UInt32 NPCID2;
                            Actor.Type Type;

                            switch (MemoryHandler.Instance.GameLanguage)
                            {
                                case "Korean":
                                    // ActorEntityHelper.cs?
                                    ID = BitConverter.ToUInt32(source, 0x74);
                                    NPCID2 = BitConverter.ToUInt32(source, 0x80);
                                    Type = (Actor.Type) source[0x8A];
                                    break;
                                case "Chinese":
                                    ID = BitConverter.ToUInt32(source, 0x74);
                                    NPCID2 = BitConverter.ToUInt32(source, 0x80);
                                    Type = (Actor.Type) source[0x8A];
                                    break;
                                default:
                                    ID = BitConverter.ToUInt32(source, 0x74);
                                    NPCID2 = BitConverter.ToUInt32(source, 0x80);
                                    Type = (Actor.Type) source[0x8A];
                                    break;
                            }

                            ActorEntity existing = null;
                            var newEntry = false;

                            switch (Type)
                            {
                                case Actor.Type.Monster:
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
                                case Actor.Type.PC:
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
                                case Actor.Type.NPC:
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

                            entry.MapIndex = mapIndex;
                            if (isFirstEntry)
                            {
                                if (targetAddress.ToInt64() > 0)
                                {
                                    uint currentTargetID;
                                    var targetInfoSource = MemoryHandler.Instance.GetByteArray(targetAddress, 128);
                                    switch (MemoryHandler.Instance.GameLanguage)
                                    {
                                        case "Korean":
                                            // MonsterWorker.cs:L194
                                            currentTargetID = BitConverter.ToUInt32(targetInfoSource, 0x68);
                                            break;
                                        case "Chinese":
                                            currentTargetID = BitConverter.ToUInt32(targetInfoSource, 0x68);
                                            break;
                                        default:
                                            currentTargetID = BitConverter.ToUInt32(targetInfoSource, 0x74);
                                            break;
                                    }
                                    entry.TargetID = (int) currentTargetID;
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
                        }
                    }

                    MemoryHandler.Instance.ScanCount++;

                    #endregion
                }
                catch (Exception ex)
                {
                }
            }

            return result;
        }
    }
}
