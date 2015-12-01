// FFXIVAPP.Memory
// Reader.PartyInfo.cs
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
using FFXIVAPP.Memory.Core;
using FFXIVAPP.Memory.Delegates;
using FFXIVAPP.Memory.Helpers;

namespace FFXIVAPP.Memory
{
    public class PartyInfoReadResult
    {
        public PartyInfoReadResult()
        {
            PreviousParty = new Dictionary<uint, uint>();

            NewParty = new List<uint>();
        }

        public ConcurrentDictionary<uint, PartyEntity> PartyEntities => PartyInfoWorkerDelegate.EntitiesDictionary;
        public Dictionary<uint, uint> PreviousParty { get; set; }
        public List<UInt32> NewParty { get; set; }
    }

    public static partial class Reader
    {
        public static IntPtr PartyInfoMap { get; set; }
        public static IntPtr PartyCountMap { get; set; }

        public static PartyInfoReadResult GetPartyMembers()
        {
            var result = new PartyInfoReadResult();

            if (Scanner.Instance.Locations.ContainsKey("CHARMAP"))
            {
                if (Scanner.Instance.Locations.ContainsKey("PARTYMAP"))
                {
                    if (Scanner.Instance.Locations.ContainsKey("PARTYCOUNT"))
                    {
                        PartyInfoMap = Scanner.Instance.Locations["PARTYMAP"];
                        PartyCountMap = Scanner.Instance.Locations["PARTYCOUNT"];
                        try
                        {
                            var partyCount = MemoryHandler.Instance.GetByte(PartyCountMap);

                            if (partyCount > 1 && partyCount < 9)
                            {
                                for (uint i = 0; i < partyCount; i++)
                                {
                                    UInt32 ID;
                                    uint size;
                                    switch (MemoryHandler.Instance.GameLanguage)
                                    {
                                        case "Korean":
                                            size = 594;
                                            break;
                                        case "Chinese":
                                            size = 594;
                                            break;
                                        default:
                                            size = 544;
                                            break;
                                    }
                                    var address = PartyInfoMap.ToInt64() + (i * size);
                                    var source = MemoryHandler.Instance.GetByteArray(new IntPtr(address), (int) size);
                                    switch (MemoryHandler.Instance.GameLanguage)
                                    {
                                        case "Korean":
                                            ID = BitConverter.ToUInt32(source, 0x10);
                                            break;
                                        case "Chinese":
                                            ID = BitConverter.ToUInt32(source, 0x10);
                                            break;
                                        default:
                                            ID = BitConverter.ToUInt32(source, 0x10);
                                            break;
                                    }
                                    ActorEntity existing = null;
                                    if (result.PreviousParty.ContainsKey(ID))
                                    {
                                        result.PreviousParty.Remove(ID);
                                        if (MonsterWorkerDelegate.EntitiesDictionary.ContainsKey(ID))
                                        {
                                            existing = MonsterWorkerDelegate.GetEntity(ID);
                                        }
                                        if (PCWorkerDelegate.EntitiesDictionary.ContainsKey(ID))
                                        {
                                            existing = PCWorkerDelegate.GetEntity(ID);
                                        }
                                    }
                                    else
                                    {
                                        result.NewParty.Add(ID);
                                    }
                                    var entry = PartyEntityHelper.ResolvePartyMemberFromBytes(source, existing);
                                    if (!entry.IsValid)
                                    {
                                        continue;
                                    }
                                    if (existing != null)
                                    {
                                        continue;
                                    }
                                    PartyInfoWorkerDelegate.EnsureEntity(entry.ID, entry);
                                }
                            }
                            else if (partyCount == 0 || partyCount == 1)
                            {
                                var entry = PartyEntityHelper.ResolvePartyMemberFromBytes(new byte[0], PCWorkerDelegate.CurrentUser);
                                if (entry.IsValid)
                                {
                                    var exists = false;
                                    if (result.PreviousParty.ContainsKey(entry.ID))
                                    {
                                        result.PreviousParty.Remove(entry.ID);
                                        exists = true;
                                    }
                                    else
                                    {
                                        result.NewParty.Add(entry.ID);
                                    }
                                    if (!exists)
                                    {
                                        PartyInfoWorkerDelegate.EnsureEntity(entry.ID, entry);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }

            return result;
        }
    }
}
