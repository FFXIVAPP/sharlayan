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
using System.Collections.Generic;
using FFXIVAPP.Memory.Core;
using FFXIVAPP.Memory.Delegates;
using FFXIVAPP.Memory.Helpers;

namespace FFXIVAPP.Memory
{
    public static partial class Reader
    {
        private static IntPtr PartyInfoMap { get; set; }
        private static IntPtr PartyCountMap { get; set; }

        public static List<PartyEntity> GetPartyMembers()
        {
            var entities = new List<PartyEntity>();

            if (MemoryHandler.Instance.SigScanner.Locations.ContainsKey("CHARMAP"))
            {
                if (MemoryHandler.Instance.SigScanner.Locations.ContainsKey("PARTYMAP"))
                {
                    if (MemoryHandler.Instance.SigScanner.Locations.ContainsKey("PARTYCOUNT"))
                    {
                        PartyInfoMap = MemoryHandler.Instance.SigScanner.Locations["PARTYMAP"];
                        PartyCountMap = MemoryHandler.Instance.SigScanner.Locations["PARTYCOUNT"];
                        try
                        {
                            var partyCount = MemoryHandler.Instance.GetByte(PartyCountMap);

                            if (partyCount > 1 && partyCount < 9)
                            {
                                for (uint i = 0; i < partyCount; i++)
                                {
                                    uint size;
                                    switch (MemoryHandler.Instance.GameLanguage)
                                    {
                                        case "Chinese":
                                            size = 594;
                                            break;
                                        default:
                                            size = 544;
                                            break;
                                    }
                                    var address = new IntPtr(PartyInfoMap.ToInt64() + (i * size));
                                    var source = MemoryHandler.Instance.GetByteArray(address, (int) size);
                                    var entry = PartyEntityHelper.ResolvePartyMemberFromBytes(source);
                                    if (entry.IsValid)
                                    {
                                        PartyInfoWorkerDelegate.EnsureEntity(entry.ID, entry);
                                    }
                                }
                            }
                            else if (partyCount == 0 || partyCount == 1)
                            {
                                var entry = PartyEntityHelper.ResolvePartyMemberFromBytes(new byte[0], PCWorkerDelegate.CurrentUser);
                                if (entry.IsValid)
                                {
                                    PartyInfoWorkerDelegate.EnsureEntity(entry.ID, entry);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }

            return entities;
        }
    }
}
