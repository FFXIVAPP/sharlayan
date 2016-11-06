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
using FFXIVAPP.Memory.Core;
using FFXIVAPP.Memory.Helpers;
using FFXIVAPP.Memory.Models;

namespace FFXIVAPP.Memory
{
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
                        var enmityCount = MemoryHandler.Instance.GetInt16(PlayerInfoMap - MemoryHandler.Instance.Structures.PlayerInfo.EnmityCount);
                        var enmityStructure = PlayerInfoMap - MemoryHandler.Instance.Structures.PlayerInfo.EnmityStructure;
                        var enmityEntries = new List<EnmityEntry>();
                        if (enmityCount > 0 && enmityCount < 32 && enmityStructure.ToInt64() > 0)
                        {
                            for (uint i = 0; i < enmityCount; i++)
                            {
                                var address = new IntPtr(enmityStructure.ToInt64() + (i * 72));
                                var enmityEntry = new EnmityEntry
                                {
                                    ID = (uint) MemoryHandler.Instance.GetPlatformInt(address, MemoryHandler.Instance.Structures.EnmityEntry.ID),
                                    Name = MemoryHandler.Instance.GetString(address + MemoryHandler.Instance.Structures.EnmityEntry.Name),
                                    Enmity = (uint) MemoryHandler.Instance.GetInt16(address + MemoryHandler.Instance.Structures.EnmityEntry.Enmity)
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
