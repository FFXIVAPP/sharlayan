// Sharlayan ~ Reader.PlayerInfo.cs
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
using Sharlayan.Core;
using Sharlayan.Helpers;
using Sharlayan.Models;

namespace Sharlayan
{
    public static partial class Reader
    {
        public static bool CanGetPlayerInfo()
        {
            var canRead = Scanner.Instance.Locations.ContainsKey(Signatures.CharacterMapKey) && Scanner.Instance.Locations.ContainsKey(Signatures.PlayerInformationKey);
            if (canRead)
            {
                // OTHER STUFF?
            }
            return canRead;
        }

        public static PlayerInfoReadResult GetPlayerInfo()
        {
            var result = new PlayerInfoReadResult();

            if (!CanGetPlayerInfo())
            {
                return result;
            }

            var PlayerInfoMap = (IntPtr) Scanner.Instance.Locations[Signatures.PlayerInformationKey];

            if (PlayerInfoMap.ToInt64() <= 6496)
            {
                return result;
            }

            try
            {
                var agroEntries = new List<EnmityEntry>();

                if (CanGetAgroEntities())
                {
                    var agroCount = MemoryHandler.Instance.GetInt16(Scanner.Instance.Locations[Signatures.AgroCountKey]);
                    var agroStructure = (IntPtr) Scanner.Instance.Locations[Signatures.AgroMapKey];

                    if (agroCount > 0 && agroCount < 32 && agroStructure.ToInt64() > 0)
                    {
                        for (uint i = 0; i < agroCount; i++)
                        {
                            var address = new IntPtr(agroStructure.ToInt64() + i * 72);
                            var agroEntry = new EnmityEntry
                            {
                                ID = (uint) MemoryHandler.Instance.GetPlatformInt(address, MemoryHandler.Instance.Structures.EnmityEntry.ID),
                                Name = MemoryHandler.Instance.GetString(address + MemoryHandler.Instance.Structures.EnmityEntry.Name),
                                Enmity = MemoryHandler.Instance.GetUInt32(address + MemoryHandler.Instance.Structures.EnmityEntry.Enmity)
                            };
                            if (agroEntry.ID > 0)
                            {
                                agroEntries.Add(agroEntry);
                            }
                        }
                    }
                }

                var source = MemoryHandler.Instance.GetByteArray(PlayerInfoMap, 0x256);

                try
                {
                    result.PlayerEntity = PlayerEntityHelper.ResolvePlayerFromBytes(source);
                    result.PlayerEntity.EnmityEntries = agroEntries;
                }
                catch (Exception ex)
                {
                    MemoryHandler.Instance.RaiseException(Logger, ex, true);
                }
            }
            catch (Exception ex)
            {
                MemoryHandler.Instance.RaiseException(Logger, ex, true);
            }

            return result;
        }
    }
}
