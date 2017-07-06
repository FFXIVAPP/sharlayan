// FFXIVAPP.Memory ~ Reader.Target.cs
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
using FFXIVAPP.Memory.Core;
using FFXIVAPP.Memory.Delegates;
using FFXIVAPP.Memory.Helpers;
using FFXIVAPP.Memory.Models;
using BitConverter = FFXIVAPP.Memory.Helpers.BitConverter;

namespace FFXIVAPP.Memory
{
    public static partial class Reader
    {
        public static TargetReadResult GetTargetInfo()
        {
            var result = new TargetReadResult();

            if (!Scanner.Instance.Locations.ContainsKey("CHARMAP"))
            {
                return result;
            }

            try
            {
                var enmityEntries = new List<EnmityEntry>();
                var agroPointersFound = Scanner.Instance.Locations.ContainsKey("AGROMAP") && Scanner.Instance.Locations.ContainsKey("AGRO_COUNT");

                if (!Scanner.Instance.Locations.ContainsKey("TARGET"))
                {
                    return result;
                }

                var targetAddress = (IntPtr) Scanner.Instance.Locations["TARGET"];

                if (targetAddress.ToInt64() > 0)
                {
                    var targetInfoSource = MemoryHandler.Instance.GetByteArray(targetAddress, MemoryHandler.Instance.Structures.TargetInfo.SourceSize);

                    var currentTarget = MemoryHandler.Instance.GetPlatformIntFromBytes(targetInfoSource, MemoryHandler.Instance.Structures.TargetInfo.Current);
                    var mouseOverTarget = MemoryHandler.Instance.GetPlatformIntFromBytes(targetInfoSource, MemoryHandler.Instance.Structures.TargetInfo.MouseOver);
                    var focusTarget = MemoryHandler.Instance.GetPlatformIntFromBytes(targetInfoSource, MemoryHandler.Instance.Structures.TargetInfo.Focus);
                    var previousTarget = MemoryHandler.Instance.GetPlatformIntFromBytes(targetInfoSource, MemoryHandler.Instance.Structures.TargetInfo.Previous);

                    var currentTargetID = BitConverter.TryToUInt32(targetInfoSource, MemoryHandler.Instance.Structures.TargetInfo.CurrentID);

                    if (currentTarget > 0)
                    {
                        try
                        {
                            var entry = GetTargetActorEntityFromSource(currentTarget);
                            currentTargetID = entry.ID;
                            if (entry.IsValid)
                            {
                                result.TargetsFound = true;
                                result.TargetEntity.CurrentTarget = entry;
                            }
                        }
                        catch (Exception ex)
                        {
                            MemoryHandler.Instance.RaiseException(Logger, ex, true);
                        }
                    }
                    if (mouseOverTarget > 0)
                    {
                        try
                        {
                            var entry = GetTargetActorEntityFromSource(mouseOverTarget);
                            if (entry.IsValid)
                            {
                                result.TargetsFound = true;
                                result.TargetEntity.MouseOverTarget = entry;
                            }
                        }
                        catch (Exception ex)
                        {
                            MemoryHandler.Instance.RaiseException(Logger, ex, true);
                        }
                    }
                    if (focusTarget > 0)
                    {
                        try
                        {
                            var entry = GetTargetActorEntityFromSource(focusTarget);
                            if (entry.IsValid)
                            {
                                result.TargetsFound = true;
                                result.TargetEntity.FocusTarget = entry;
                            }
                        }
                        catch (Exception ex)
                        {
                            MemoryHandler.Instance.RaiseException(Logger, ex, true);
                        }
                    }
                    if (previousTarget > 0)
                    {
                        try
                        {
                            var entry = GetTargetActorEntityFromSource(previousTarget);
                            if (entry.IsValid)
                            {
                                result.TargetsFound = true;
                                result.TargetEntity.PreviousTarget = entry;
                            }
                        }
                        catch (Exception ex)
                        {
                            MemoryHandler.Instance.RaiseException(Logger, ex, true);
                        }
                    }
                    if (currentTargetID > 0)
                    {
                        result.TargetsFound = true;
                        result.TargetEntity.CurrentTargetID = currentTargetID;
                    }
                }
                if (result.TargetEntity.CurrentTargetID > 0)
                {
                    if (agroPointersFound)
                    {
                        var agroCount = MemoryHandler.Instance.GetInt16(Scanner.Instance.Locations["ENMITY_COUNT"]);
                        if (agroCount > 0)
                        {
                            var agroStructure = (IntPtr) Scanner.Instance.Locations["ENMITYMAP"];
                            for (uint i = 0; i < 16; i++)
                            {
                                try
                                {
                                    var address = new IntPtr(agroStructure.ToInt64() + i * 72);
                                    var enmityEntry = new EnmityEntry
                                    {
                                        ID = (uint) MemoryHandler.Instance.GetPlatformInt(address, MemoryHandler.Instance.Structures.EnmityEntry.ID),
                                        Name = MemoryHandler.Instance.GetString(address + MemoryHandler.Instance.Structures.EnmityEntry.Name),
                                        Enmity = MemoryHandler.Instance.GetUInt32(address + MemoryHandler.Instance.Structures.EnmityEntry.Enmity)
                                    };
                                    if (enmityEntry.ID <= 0)
                                    {
                                        continue;
                                    }
                                    if (string.IsNullOrWhiteSpace(enmityEntry.Name))
                                    {
                                        var pc = PCWorkerDelegate.GetEntity(enmityEntry.ID);
                                        var npc = NPCWorkerDelegate.GetEntity(enmityEntry.ID);
                                        var monster = MonsterWorkerDelegate.GetEntity(enmityEntry.ID);
                                        try
                                        {
                                            enmityEntry.Name = (pc ?? npc).Name ?? monster.Name;
                                        }
                                        catch (Exception ex)
                                        {
                                            MemoryHandler.Instance.RaiseException(Logger, ex, true);
                                        }
                                    }
                                    enmityEntries.Add(enmityEntry);
                                }
                                catch (Exception ex)
                                {
                                    MemoryHandler.Instance.RaiseException(Logger, ex, true);
                                }
                            }
                        }
                    }
                }
                result.TargetEntity.EnmityEntries = enmityEntries;
            }
            catch (Exception ex)
            {
                MemoryHandler.Instance.RaiseException(Logger, ex, true);
            }

            return result;
        }

        private static ActorEntity GetTargetActorEntityFromSource(long address)
        {
            var source = MemoryHandler.Instance.GetByteArray(new IntPtr(address), MemoryHandler.Instance.Structures.TargetInfo.Size); // old size: 0x3F40
            var entry = ActorEntityHelper.ResolveActorFromBytes(source);

            EnsureMapAndZone(entry);

            return entry;
        }
    }
}
