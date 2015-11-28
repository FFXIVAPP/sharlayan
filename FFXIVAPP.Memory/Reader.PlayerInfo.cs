// FFXIVAPP.Memory
// Reader.PlayerInfo.cs
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
using FFXIVAPP.Memory.Helpers;

namespace FFXIVAPP.Memory
{
    public class PlayerInfoReadResult
    {
        public PlayerInfoReadResult()
        {
            PlayerEntity = new PlayerEntity();
        }

        public PlayerEntity PlayerEntity { get; set; }
    }

    public static partial class Reader
    {
        private static IntPtr PlayerInfoMap { get; set; }
        private static PlayerEntity LastPlayerEntity { get; set; }

        public static PlayerInfoReadResult GetPlayerInfo()
        {
            var result = new PlayerInfoReadResult();

            if (Scanner.Instance.Locations.ContainsKey("CHARMAP"))
            {
                if (Scanner.Instance.Locations.ContainsKey("PLAYERINFO"))
                {
                    PlayerInfoMap = Scanner.Instance.Locations["PLAYERINFO"];
                    if (PlayerInfoMap.ToInt64() <= 6496)
                    {
                        return result;
                    }
                    try
                    {
                        short enmityCount;
                        var enmityStructure = IntPtr.Zero;
                        switch (MemoryHandler.Instance.GameLanguage)
                        {
                            case "Chinese":
                                enmityCount = MemoryHandler.Instance.GetInt16((IntPtr) Scanner.Instance.Locations["CHARMAP"] + 5688);
                                enmityStructure = (IntPtr) Scanner.Instance.Locations["CHARMAP"] + 3384;
                                break;
                            default:
                                enmityCount = MemoryHandler.Instance.GetInt16(Scanner.Instance.Locations["AGRO_COUNT"]);
                                enmityStructure = Scanner.Instance.Locations["AGRO"];
                                break;
                        }
                        var enmityEntries = new List<EnmityEntry>();
                        if (enmityCount > 0 && enmityCount < 32 && enmityStructure.ToInt64() > 0)
                        {
                            for (uint i = 0; i < enmityCount; i++)
                            {
                                var address = new IntPtr(enmityStructure.ToInt64() + (i * 72));
                                var enmityEntry = new EnmityEntry
                                {
                                    Name = MemoryHandler.Instance.GetString(address),
                                    ID = (uint) MemoryHandler.Instance.GetPlatformInt(address + 64),
                                    Enmity = (uint) MemoryHandler.Instance.GetInt16(address + 68)
                                };
                                if (enmityEntry.ID > 0)
                                {
                                    enmityEntries.Add(enmityEntry);
                                }
                            }
                        }
                        var source = MemoryHandler.Instance.GetByteArray(PlayerInfoMap, 0x256);
                        try
                        {
                            result.PlayerEntity = PlayerEntityHelper.ResolvePlayerFromBytes(source);
                            result.PlayerEntity.EnmityEntries = enmityEntries;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            return result;
        }
    }
}
