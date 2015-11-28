// FFXIVAPP.Memory
// Reader.Actor.cs
// 
// Copyright © 2007 - 2015 Ryan Wilson - All Rights Reserved
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions are met: 
// 
//  * Redistributions of source code must retain the above copyright notice, 
//    this list of conditions and the following disclaimer. 
//  * Redistributions in binary form must reproduce the above copyright 
//    notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution. 
//  * Neither the name of SyndicatedLife nor the names of its contributors may 
//    be used to endorse or promote products derived from this software 
//    without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE. 

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

            if (MemoryHandler.Instance.SigScanner.Locations.ContainsKey("CHARMAP"))
            {
                try
                {
                    #region Ensure Target & Map

                    var targetAddress = IntPtr.Zero;
                    uint mapIndex = 0;
                    if (MemoryHandler.Instance.SigScanner.Locations.ContainsKey("TARGET"))
                    {
                        try
                        {
                            targetAddress = MemoryHandler.Instance.SigScanner.Locations["TARGET"];
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    if (MemoryHandler.Instance.SigScanner.Locations.ContainsKey("MAP"))
                    {
                        try
                        {
                            mapIndex = (uint) MemoryHandler.Instance.GetPlatformUInt(MemoryHandler.Instance.SigScanner.Locations["MAP"]);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    #endregion

                    var endianSize = MemoryHandler.Instance.ProcessModel.IsWin64 ? 8 : 4;

                    const int limit = 1372;

                    var characterAddressMap = MemoryHandler.Instance.GetByteArray(MemoryHandler.Instance.SigScanner.Locations["CHARMAP"], endianSize * limit);
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

                    MemoryHandler._scanCount++;

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
