// FFXIVAPP.Memory ~ Reader.PlayerInfo.cs
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
                    switch (MemoryHandler.Instance.GameLanguage)
                    {
                        case "Korean":
                            PlayerInfoMap = (IntPtr) Scanner.Instance.Locations["CHARMAP"] - 115996;
                            break;
                        default:
                            PlayerInfoMap = Scanner.Instance.Locations["PLAYERINFO"];
                            break;
                    }
                    
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
                            case "Korean":
                                enmityCount = MemoryHandler.Instance.GetInt16((IntPtr) Scanner.Instance.Locations["CHARMAP"] - 0x1C590);
                                enmityStructure = (IntPtr) Scanner.Instance.Locations["CHARMAP"] + 0x1CE94;
                                break;
                            case "Chinese":
                                enmityCount = MemoryHandler.Instance.GetInt16((IntPtr) Scanner.Instance.Locations["CHARMAP"] + 5688);
                                enmityStructure = (IntPtr) Scanner.Instance.Locations["CHARMAP"] + 3384;
                                break;
                            default:
                                //enmityCount = MemoryHandler.Instance.GetInt16(Scanner.Instance.Locations["AGRO_COUNT"]);
                                //enmityStructure = Scanner.Instance.Locations["AGRO"];

                                enmityCount =  MemoryHandler.Instance.GetInt16(PlayerInfoMap - 0x24);
                                enmityStructure = PlayerInfoMap - 0x924;
                                break;
                        }
                        var enmityEntries = new List<EnmityEntry>();
                        if (enmityCount > 0 && enmityCount < 32 && enmityStructure.ToInt64() > 0)
                        {
                            for (uint i = 0; i < enmityCount; i++)
                            {
                                var address = new IntPtr(enmityStructure.ToInt64() + (i * 72));

                                EnmityEntry enmityEntry = null;
                                switch (MemoryHandler.Instance.GameLanguage)
                                {
                                    case "Korean":
                                        enmityEntry = new EnmityEntry
                                        {
                                            ID = (uint) MemoryHandler.Instance.GetPlatformInt(address),
                                            Name = MemoryHandler.Instance.GetString(address + 4),
                                            Enmity = (uint) MemoryHandler.Instance.GetInt16(address + 72)
                                        };
                                        break;
                                    default:
                                        enmityEntry = new EnmityEntry
                                        {
                                            Name = MemoryHandler.Instance.GetString(address),
                                            ID = (uint) MemoryHandler.Instance.GetPlatformInt(address + 64),
                                            Enmity = (uint) MemoryHandler.Instance.GetInt16(address + 68)
                                        };
                                        break;
                                }

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
