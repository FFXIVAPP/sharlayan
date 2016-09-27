// FFXIVAPP.Memory ~ Reader.Target.cs
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
using FFXIVAPP.Memory.Delegates;
using FFXIVAPP.Memory.Helpers;

namespace FFXIVAPP.Memory
{
    public static partial class Reader
    {
        public static TargetReadResult GetTargetInfo()
        {
            var result = new TargetReadResult();

            if (Scanner.Instance.Locations.ContainsKey("CHARMAP"))
            {
                try
                {
                    var targetHateStructure = IntPtr.Zero;
                    switch (MemoryHandler.Instance.GameLanguage)
                    {
                        case "Korean":
                            targetHateStructure = (IntPtr) Scanner.Instance.Locations["CHARMAP"] - 120664;
                            break;
                        case "Chinese":
                            targetHateStructure = (IntPtr) Scanner.Instance.Locations["CHARMAP"] + 1136;
                            break;
                        default:
                            if (Scanner.Instance.Locations.ContainsKey("ENMITYMAP"))
                            {
                                //targetHateStructure = Scanner.Instance.Locations["ENMITYMAP"];
                                targetHateStructure = (Scanner.Instance.Locations["PLAYERINFO"].GetAddress()) - 0x122C;
                            }
                            break;
                    }
                    var enmityEntries = new List<EnmityEntry>();

                    if (Scanner.Instance.Locations.ContainsKey("TARGET"))
                    {
                        var targetAddress = (IntPtr) Scanner.Instance.Locations["TARGET"];
                        var somethingFound = false;
                        if (targetAddress.ToInt64() > 0)
                        {
                            //var targetInfo = MemoryHandler.Instance.GetStructure<Structures.Target>(targetAddress);
                            uint currentTarget = 0;
                            uint mouseOverTarget = 0;
                            uint focusTarget = 0;
                            uint previousTarget = 0;
                            uint currentTargetID = 0;
                            var targetInfoSource = MemoryHandler.Instance.GetByteArray(targetAddress, 192);
                            switch (MemoryHandler.Instance.GameLanguage)
                            {
                                case "Korean":
                                    currentTarget = BitConverter.ToUInt32(targetInfoSource, 0x0);
                                    mouseOverTarget = BitConverter.ToUInt32(targetInfoSource, 0xC);
                                    focusTarget = BitConverter.ToUInt32(targetInfoSource, 0x3C);
                                    previousTarget = BitConverter.ToUInt32(targetInfoSource, 0x48);
                                    currentTargetID = BitConverter.ToUInt32(targetInfoSource, 0x5C);
                                    break;
                                case "Chinese":
                                    currentTarget = BitConverter.ToUInt32(targetInfoSource, 0x0);
                                    mouseOverTarget = BitConverter.ToUInt32(targetInfoSource, 0xC);
                                    focusTarget = BitConverter.ToUInt32(targetInfoSource, 0x3C);
                                    previousTarget = BitConverter.ToUInt32(targetInfoSource, 0x48);
                                    currentTargetID = BitConverter.ToUInt32(targetInfoSource, 0x5C);
                                    break;
                                default:
                                    currentTarget = BitConverter.ToUInt32(targetInfoSource, 0x0);
                                    if (MemoryHandler.Instance.ProcessModel.IsWin64)
                                    {
                                        mouseOverTarget = BitConverter.ToUInt32(targetInfoSource, 0x10);
                                        focusTarget = BitConverter.ToUInt32(targetInfoSource, 0x50);
                                        previousTarget = BitConverter.ToUInt32(targetInfoSource, 0x68);
                                        currentTargetID = BitConverter.ToUInt32(targetInfoSource, 0x80);
                                    }
                                    else
                                    {
                                        mouseOverTarget = BitConverter.ToUInt32(targetInfoSource, 0x10);
                                        focusTarget = BitConverter.ToUInt32(targetInfoSource, 0x30);
                                        previousTarget = BitConverter.ToUInt32(targetInfoSource, 0x3C);
                                        currentTargetID = BitConverter.ToUInt32(targetInfoSource, 0x64);
                                    }
                                    break;
                            }

                            int targetSize;
                            switch (MemoryHandler.Instance.GameLanguage)
                            {
                                case "Korean":
                                    targetSize = 0x3F40;
                                    break;
                                default:
                                    targetSize = 0x23F0;
                                    break;
                            }
                            if (currentTarget > 0)
                            {
                                try
                                {
                                    var source = MemoryHandler.Instance.GetByteArray(new IntPtr(currentTarget), targetSize); // old size: 0x3F40
                                    var entry = ActorEntityHelper.ResolveActorFromBytes(source);
                                    currentTargetID = entry.ID;
                                    if (Scanner.Instance.Locations.ContainsKey("MAP"))
                                    {
                                        try
                                        {
                                            entry.MapIndex = (uint) MemoryHandler.Instance.GetPlatformUInt(Scanner.Instance.Locations["MAP"]);
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                    if (entry.IsValid)
                                    {
                                        result.TargetsFound = true;
                                        result.TargetEntity.CurrentTarget = entry;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                            if (mouseOverTarget > 0)
                            {
                                try
                                {
                                    var source = MemoryHandler.Instance.GetByteArray(new IntPtr(mouseOverTarget), targetSize); // old size: 0x3F40
                                    var entry = ActorEntityHelper.ResolveActorFromBytes(source);
                                    if (Scanner.Instance.Locations.ContainsKey("MAP"))
                                    {
                                        try
                                        {
                                            entry.MapIndex = (uint) MemoryHandler.Instance.GetPlatformUInt(Scanner.Instance.Locations["MAP"]);
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                    if (entry.IsValid)
                                    {
                                        result.TargetsFound = true;
                                        result.TargetEntity.MouseOverTarget = entry;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                            if (focusTarget > 0)
                            {
                                var source = MemoryHandler.Instance.GetByteArray(new IntPtr(focusTarget), targetSize); // old size: 0x3F40
                                var entry = ActorEntityHelper.ResolveActorFromBytes(source);
                                if (Scanner.Instance.Locations.ContainsKey("MAP"))
                                {
                                    try
                                    {
                                        entry.MapIndex = (uint) MemoryHandler.Instance.GetPlatformUInt(Scanner.Instance.Locations["MAP"]);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                                if (entry.IsValid)
                                {
                                    result.TargetsFound = true;
                                    result.TargetEntity.FocusTarget = entry;
                                }
                            }
                            if (previousTarget > 0)
                            {
                                try
                                {
                                    var source = MemoryHandler.Instance.GetByteArray(new IntPtr(previousTarget), targetSize); // old size: 0x3F40
                                    var entry = ActorEntityHelper.ResolveActorFromBytes(source);
                                    if (Scanner.Instance.Locations.ContainsKey("MAP"))
                                    {
                                        try
                                        {
                                            entry.MapIndex = (uint) MemoryHandler.Instance.GetPlatformUInt(Scanner.Instance.Locations["MAP"]);
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                    if (entry.IsValid)
                                    {
                                        result.TargetsFound = true;
                                        result.TargetEntity.PreviousTarget = entry;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                            if (currentTargetID > 0)
                            {
                                result.TargetsFound = true;
                                result.TargetEntity.CurrentTargetID = currentTargetID;
                            }
                        }
                        if (result.TargetEntity.CurrentTargetID > 0 && targetHateStructure.ToInt64() > 0)
                        {
                            for (uint i = 0; i < 16; i++)
                            {
                                try
                                {
                                    var address = targetHateStructure.ToInt64() + (i * 72);

                                    var enmityEntry = new EnmityEntry();
                                    switch (MemoryHandler.Instance.GameLanguage)
                                    {
                                        case "Korean":
                                            enmityEntry.Name   = null; // Search from the list later (old impl)
                                            enmityEntry.ID     = (uint) MemoryHandler.Instance.GetPlatformInt(new IntPtr(address));
                                            enmityEntry.Enmity = (uint) MemoryHandler.Instance.GetPlatformInt(new IntPtr(address), 4);
                                            break;
                                        default:
                                            enmityEntry.Name = MemoryHandler.Instance.GetString(new IntPtr(address));
                                            enmityEntry.ID = (uint) MemoryHandler.Instance.GetPlatformInt(new IntPtr(address), 64);
                                            enmityEntry.Enmity = (uint) MemoryHandler.Instance.GetPlatformInt(new IntPtr(address), 68);
                                            break;
                                    }

                                    if (enmityEntry.ID <= 0)
                                    {
                                        continue;
                                    }
                                    if (String.IsNullOrWhiteSpace(enmityEntry.Name))
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
                                        }
                                    }
                                    enmityEntries.Add(enmityEntry);
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                        result.TargetEntity.EnmityEntries = enmityEntries;
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return result;
        }

        public class TargetReadResult
        {
            public TargetReadResult()
            {
                TargetEntity = new TargetEntity();
            }

            public TargetEntity TargetEntity { get; set; }
            public bool TargetsFound { get; set; }
        }
    }
}
