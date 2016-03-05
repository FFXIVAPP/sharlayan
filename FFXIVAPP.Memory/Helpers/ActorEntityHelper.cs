// FFXIVAPP.Memory ~ ActorEntityHelper.cs
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
using FFXIVAPP.Memory.Core.Enums;
using FFXIVAPP.Memory.Delegates;

namespace FFXIVAPP.Memory.Helpers
{
    internal static class ActorEntityHelper
    {
        public static ActorEntity ResolveActorFromBytes(byte[] source, bool isCurrentUser = false, ActorEntity entry = null)
        {
            entry = entry ?? new ActorEntity();
            var defaultBaseOffset = 0;
            var defaultStatOffset = 0;
            var defaultStatusEffectOffset = 0;
            try
            {
                uint targetID;
                uint pcTargetID;
                entry.MapIndex = 0;
                entry.TargetID = 0;
                switch (MemoryHandler.Instance.GameLanguage)
                {
                    case "Korean":
                        entry.Name = MemoryHandler.Instance.GetStringFromBytes(source, 48);
                        entry.ID = BitConverter.ToUInt32(source, 0x74);
                        entry.NPCID1 = BitConverter.ToUInt32(source, 0x78);
                        entry.NPCID2 = BitConverter.ToUInt32(source, 0x80);
                        entry.OwnerID = BitConverter.ToUInt32(source, 0x84);
                        entry.Type = (Actor.Type) source[0x8A];
                        entry.TargetType = (Actor.TargetType) source[0x8C];
                        entry.GatheringStatus = source[0x8F];
                        entry.Distance = source[0x90];
                        entry.X = BitConverter.ToSingle(source, 0xA0);
                        entry.Z = BitConverter.ToSingle(source, 0xA4);
                        entry.Y = BitConverter.ToSingle(source, 0xA8);
                        entry.Heading = BitConverter.ToSingle(source, 0xB0);
                        entry.Fate = BitConverter.ToUInt32(source, 0xE4); // ??
                        entry.GatheringInvisible = source[0x11C]; // ??
                        entry.ModelID = BitConverter.ToUInt32(source, 0x174);
                        entry.ActionStatus = (Actor.ActionStatus) source[0x17C];
                        entry.IsGM = BitConverter.ToBoolean(source, 0x183); // ?
                        entry.Icon = (Actor.Icon) source[0x18C];
                        entry.Status = (Actor.Status) source[0x17E]; //0x18E];
                        entry.ClaimedByID = BitConverter.ToUInt32(source, 0x180); // 0x1A0);
                        targetID = BitConverter.ToUInt32(source, 0x188); // 0x1A8);
                        pcTargetID = BitConverter.ToUInt32(source, 0x938); // 0xAA8);
                        entry.Job = (Actor.Job) source[0x1540]; // 0x17C0];
                        entry.Level = source[0x1541]; // 0x17C1];
                        entry.GrandCompany = source[0x1543]; // 0x17C3];
                        entry.GrandCompanyRank = source[0x1544]; //0x17C4];
                        entry.Title = source[0x1546]; //0x17C6];
                        entry.HPCurrent = BitConverter.ToInt32(source, 0x1548); // 0x17C8);
                        entry.HPMax = BitConverter.ToInt32(source, 0x154C); // 0x17CC);
                        entry.MPCurrent = BitConverter.ToInt32(source, 0x1550); // 0x17D0);
                        entry.MPMax = BitConverter.ToInt32(source, 0x1554); // 0x17D4);
                        entry.TPCurrent = BitConverter.ToInt16(source, 0x1558); // 0x17D8);
                        entry.TPMax = 1000;
                        entry.GPCurrent = BitConverter.ToInt16(source, 0x155A); // 0x17DA);
                        entry.GPMax = BitConverter.ToInt16(source, 0x155C); // 0x17DC);
                        entry.CPCurrent = BitConverter.ToInt16(source, 0x155E); // 0x17DE);
                        entry.CPMax = BitConverter.ToInt16(source, 0x1560); // 0x17E0);
                        entry.Race = source[0x2808]; // ??
                        entry.Sex = (Actor.Sex) source[0x2809]; //?
                        entry.IsCasting = BitConverter.ToBoolean(source, 0x2A30); // 0x2C90);
                        entry.CastingID = BitConverter.ToInt16(source, 0x2A34); // 0x2C94);
                        entry.CastingTargetID = BitConverter.ToUInt32(source, 0x2A40); // 0x2CA0);
                        entry.CastingProgress = BitConverter.ToSingle(source, 0x2A64); // 0x2CC4);
                        entry.CastingTime = BitConverter.ToSingle(source, 0x2A68); // 0x2DA8);
                        entry.Coordinate = new Coordinate(entry.X, entry.Z, entry.Y);
                        break;
                    case "Chinese":
                        entry.Name = MemoryHandler.Instance.GetStringFromBytes(source, 48);
                        entry.ID = BitConverter.ToUInt32(source, 0x74);
                        entry.NPCID1 = BitConverter.ToUInt32(source, 0x7C);
                        entry.NPCID2 = BitConverter.ToUInt32(source, 0x80);
                        entry.OwnerID = BitConverter.ToUInt32(source, 0x84);
                        entry.Type = (Actor.Type) source[0x8A];
                        entry.TargetType = (Actor.TargetType) source[0x8C];
                        entry.GatheringStatus = source[0x8F];
                        entry.Distance = source[0x91];
                        defaultBaseOffset = MemoryHandler.Instance.ProcessModel.IsWin64 ? 0x10 : 0;
                        entry.X = BitConverter.ToSingle(source, 0xA0 + defaultBaseOffset);
                        entry.Z = BitConverter.ToSingle(source, 0xA4 + defaultBaseOffset);
                        entry.Y = BitConverter.ToSingle(source, 0xA8 + defaultBaseOffset);
                        entry.Heading = BitConverter.ToSingle(source, 0xB0 + defaultBaseOffset);
                        entry.HitBoxRadius = BitConverter.ToSingle(source, 0xC0 + defaultBaseOffset);
                        entry.Fate = BitConverter.ToUInt32(source, 0xE4 + defaultBaseOffset); // ??
                        entry.GatheringInvisible = source[0x11C]; // ??
                        entry.ModelID = BitConverter.ToUInt32(source, 0x174);
                        entry.ActionStatus = (Actor.ActionStatus) source[0x17C];
                        // 0x17D - 0 = Green name, 4 = non-agro (yellow name)
                        entry.IsGM = BitConverter.ToBoolean(source, 0x183); // ?
                        entry.Icon = (Actor.Icon) source[0x18C];
                        entry.Status = (Actor.Status) source[0x17E]; //0x18E];
                        entry.ClaimedByID = BitConverter.ToUInt32(source, 0x180); // 0x1A0);
                        targetID = BitConverter.ToUInt32(source, 0x188); // 0x1A8);
                        pcTargetID = BitConverter.ToUInt32(source, 0x938); // 0xAA8);
                        defaultStatOffset = MemoryHandler.Instance.ProcessModel.IsWin64 ? 0x230 : 0;
                        entry.Job = (Actor.Job) source[0x14C0 + defaultStatOffset]; // 0x17C0];
                        entry.Level = source[0x14C1 + defaultStatOffset]; // 0x17C1];
                        entry.GrandCompany = source[0x14C3 + defaultStatOffset]; // 0x17C3];
                        entry.GrandCompanyRank = source[0x14C4 + defaultStatOffset]; //0x17C4];
                        entry.Title = source[0x1546 + defaultStatOffset]; //0x17C6];
                        entry.HPCurrent = BitConverter.ToInt32(source, 0x14C8 + defaultStatOffset); // 0x17C8);
                        entry.HPMax = BitConverter.ToInt32(source, 0x14CC + defaultStatOffset); // 0x17CC);
                        entry.MPCurrent = BitConverter.ToInt32(source, 0x14D0 + defaultStatOffset); // 0x17D0);
                        entry.MPMax = BitConverter.ToInt32(source, 0x14D4 + defaultStatOffset); // 0x17D4);
                        entry.TPCurrent = BitConverter.ToInt16(source, 0x14D8 + defaultStatOffset); // 0x17D8);
                        entry.TPMax = 1000;
                        entry.GPCurrent = BitConverter.ToInt16(source, 0x14DA + defaultStatOffset); // 0x17DA);
                        entry.GPMax = BitConverter.ToInt16(source, 0x14DC + defaultStatOffset); // 0x17DC);
                        entry.CPCurrent = BitConverter.ToInt16(source, 0x14DE + defaultStatOffset); // 0x17DE);
                        entry.CPMax = BitConverter.ToInt16(source, 0x14E0 + defaultStatOffset); // 0x17E0);
                        entry.Race = source[0x2808]; // ??
                        entry.Sex = (Actor.Sex) source[0x2809]; //?
                        entry.IsCasting = BitConverter.ToBoolean(source, 0x2A30); // 0x2C90);
                        entry.CastingID = BitConverter.ToInt16(source, 0x2A34); // 0x2C94);
                        entry.CastingTargetID = BitConverter.ToUInt32(source, 0x2A40); // 0x2CA0);
                        entry.CastingProgress = BitConverter.ToSingle(source, 0x2A64); // 0x2CC4);
                        entry.CastingTime = BitConverter.ToSingle(source, 0x2A68); // 0x2DA8);
                        entry.Coordinate = new Coordinate(entry.X, entry.Z, entry.Y);
                        break;
                    default:
                        entry.Name = MemoryHandler.Instance.GetStringFromBytes(source, 48);
                        entry.ID = BitConverter.ToUInt32(source, 0x74);
                        entry.NPCID1 = BitConverter.ToUInt32(source, 0x7C);
                        entry.NPCID2 = BitConverter.ToUInt32(source, 0x80);
                        entry.OwnerID = BitConverter.ToUInt32(source, 0x84);
                        entry.Type = (Actor.Type) source[0x8A];
                        entry.TargetType = (Actor.TargetType) source[0x8C];
                        entry.GatheringStatus = source[0x8F];
                        entry.Distance = source[0x91];
                        defaultBaseOffset = MemoryHandler.Instance.ProcessModel.IsWin64 ? 0x10 : 0;
                        entry.X = BitConverter.ToSingle(source, 0xA0 + defaultBaseOffset);
                        entry.Z = BitConverter.ToSingle(source, 0xA4 + defaultBaseOffset);
                        entry.Y = BitConverter.ToSingle(source, 0xA8 + defaultBaseOffset);
                        entry.Heading = BitConverter.ToSingle(source, 0xB0 + defaultBaseOffset);
                        entry.HitBoxRadius = BitConverter.ToSingle(source, 0xC0 + defaultBaseOffset);
                        entry.Fate = BitConverter.ToUInt32(source, 0xE4 + defaultBaseOffset); // ??
                        entry.GatheringInvisible = source[0x11C]; // ??
                        entry.ModelID = BitConverter.ToUInt32(source, 0x174);
                        entry.ActionStatus = (Actor.ActionStatus) source[0x16C];
                        // 0x17D - 0 = Green name, 4 = non-agro (yellow name)
                        entry.IsGM = BitConverter.ToBoolean(source, 0x183); // ?
                        entry.Icon = (Actor.Icon) source[0x18C];
                        entry.Status = (Actor.Status) source[0x17E];
                        entry.ClaimedByID = BitConverter.ToUInt32(source, 0x180);
                        targetID = BitConverter.ToUInt32(source, 0x818);
                        pcTargetID = targetID; //BitConverter.ToUInt32(source, 0x938); // no longer exists?
                        defaultStatOffset = MemoryHandler.Instance.ProcessModel.IsWin64 ? 0x230 : 0;
                        entry.Job = (Actor.Job) source[0x1230 + defaultStatOffset];
                        entry.Level = source[0x1231 + defaultStatOffset];
                        entry.GrandCompany = source[0x1233 + defaultStatOffset];
                        entry.GrandCompanyRank = source[0x1234 + defaultStatOffset];
                        entry.Title = source[0x12B6 + defaultStatOffset];
                        entry.HPCurrent = BitConverter.ToInt32(source, 0x1238 + defaultStatOffset);
                        entry.HPMax = BitConverter.ToInt32(source, 0x123C + defaultStatOffset);
                        entry.MPCurrent = BitConverter.ToInt32(source, 0x1240 + defaultStatOffset);
                        entry.MPMax = BitConverter.ToInt32(source, 0x1244 + defaultStatOffset);
                        entry.TPCurrent = BitConverter.ToInt16(source, 0x1248 + defaultStatOffset);
                        entry.TPMax = 1000;
                        entry.GPCurrent = BitConverter.ToInt16(source, 0x124A + defaultStatOffset);
                        entry.GPMax = BitConverter.ToInt16(source, 0x124C + defaultStatOffset);
                        entry.CPCurrent = BitConverter.ToInt16(source, 0x124E + defaultStatOffset);
                        entry.CPMax = BitConverter.ToInt16(source, 0x1250 + defaultStatOffset);
                        //entry.Race = source[0x2578]; // ??
                        //entry.Sex = (Actor.Sex) source[0x2579]; //?
                        entry.IsCasting = BitConverter.ToBoolean(source, 0x1690) && BitConverter.ToBoolean(source, 0x1691); // 0x2C90);
                        entry.CastingID = BitConverter.ToInt16(source, 0x1694); // 0x2C94);
                        entry.CastingTargetID = BitConverter.ToUInt32(source, 0x16A0); // 0x2CA0);
                        entry.CastingProgress = BitConverter.ToSingle(source, 0x16C4); // 0x2CC4);
                        entry.CastingTime = BitConverter.ToSingle(source, 0x16C8); // 0x2DA8);
                        entry.Coordinate = new Coordinate(entry.X, entry.Z, entry.Y);
                        break;
                }
                if (targetID > 0)
                {
                    entry.TargetID = (int) targetID;
                }
                else
                {
                    if (pcTargetID > 0)
                    {
                        entry.TargetID = (int) pcTargetID;
                    }
                }
                if (entry.CastingTargetID == 3758096384)
                {
                    entry.CastingTargetID = 0;
                }
                entry.MapIndex = 0;
                var limit = 60;
                switch (entry.Type)
                {
                    case Actor.Type.PC:
                        limit = 30;
                        break;
                }
                entry.StatusEntries = new List<StatusEntry>();
                const int statusSize = 12;
                var statusesSource = new byte[limit * statusSize];
                switch (MemoryHandler.Instance.GameLanguage)
                {
                    case "Korean":
                        Buffer.BlockCopy(source, 0x2B18, statusesSource, 0, limit * statusSize);
                        break;
                    case "Chinese":
                        defaultStatusEffectOffset = MemoryHandler.Instance.ProcessModel.IsWin64 ? 0x3740 : 0x2878;
                        Buffer.BlockCopy(source, defaultStatusEffectOffset, statusesSource, 0, limit * 12);
                        break;
                    default:
                        defaultStatusEffectOffset = MemoryHandler.Instance.ProcessModel.IsWin64 ? 0x3740 : 0x1518;
                        Buffer.BlockCopy(source, defaultStatusEffectOffset, statusesSource, 0, limit * statusSize);
                        break;
                }
                for (var i = 0; i < limit; i++)
                {
                    var statusSource = new byte[statusSize];
                    Buffer.BlockCopy(statusesSource, i * statusSize, statusSource, 0, statusSize);
                    var statusEntry = new StatusEntry
                    {
                        TargetEntity = entry,
                        TargetName = entry.Name,
                        StatusID = BitConverter.ToInt16(statusSource, 0x0),
                        Stacks = statusSource[0x2],
                        Duration = BitConverter.ToSingle(statusSource, 0x4),
                        CasterID = BitConverter.ToUInt32(statusSource, 0x8)
                    };
                    try
                    {
                        var pc = PCWorkerDelegate.GetEntity(statusEntry.CasterID);
                        var npc = NPCWorkerDelegate.GetEntity(statusEntry.CasterID);
                        var monster = MonsterWorkerDelegate.GetEntity(statusEntry.CasterID);
                        statusEntry.SourceEntity = (pc ?? npc) ?? monster;
                    }
                    catch (Exception ex)
                    {
                    }
                    try
                    {
                        var statusInfo = StatusEffectHelper.StatusInfo(statusEntry.StatusID);
                        if (statusInfo != null)
                        {
                            statusEntry.IsCompanyAction = statusInfo.CompanyAction;
                            var statusKey = statusInfo.Name.English;
                            switch (MemoryHandler.Instance.GameLanguage)
                            {
                                case "French":
                                    statusKey = statusInfo.Name.French;
                                    break;
                                case "Japanese":
                                    statusKey = statusInfo.Name.Japanese;
                                    break;
                                case "German":
                                    statusKey = statusInfo.Name.German;
                                    break;
                                case "Chinese":
                                    statusKey = statusInfo.Name.Chinese;
                                    break;
                                case "Korean":
                                    statusKey = statusInfo.Name.Korean;
                                    break;
                            }
                            statusEntry.StatusName = statusKey;
                        }
                    }
                    catch (Exception ex)
                    {
                        statusEntry.StatusName = "UNKNOWN";
                    }
                    if (statusEntry.IsValid())
                    {
                        entry.StatusEntries.Add(statusEntry);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            CleanXPValue(ref entry);

            if (isCurrentUser)
            {
                PCWorkerDelegate.CurrentUser = entry;
            }
            entry.CurrentUser = PCWorkerDelegate.CurrentUser;
            return entry;
        }

        private static void CleanXPValue(ref ActorEntity entity)
        {
            if (entity.HPCurrent < 0 || entity.HPMax < 0)
            {
                entity.HPCurrent = 1;
                entity.HPMax = 1;
            }
            if (entity.HPCurrent > entity.HPMax)
            {
                if (entity.HPMax == 0)
                {
                    entity.HPCurrent = 1;
                    entity.HPMax = 1;
                }
                else
                {
                    entity.HPCurrent = entity.HPMax;
                }
            }
            if (entity.MPCurrent < 0 || entity.MPMax < 0)
            {
                entity.MPCurrent = 1;
                entity.MPMax = 1;
            }
            if (entity.MPCurrent > entity.MPMax)
            {
                if (entity.MPMax == 0)
                {
                    entity.MPCurrent = 1;
                    entity.MPMax = 1;
                }
                else
                {
                    entity.MPCurrent = entity.MPMax;
                }
            }
            if (entity.GPCurrent < 0 || entity.GPMax < 0)
            {
                entity.GPCurrent = 1;
                entity.GPMax = 1;
            }
            if (entity.GPCurrent > entity.GPMax)
            {
                if (entity.GPMax == 0)
                {
                    entity.GPCurrent = 1;
                    entity.GPMax = 1;
                }
                else
                {
                    entity.GPCurrent = entity.GPMax;
                }
            }
            if (entity.CPCurrent < 0 || entity.CPMax < 0)
            {
                entity.CPCurrent = 1;
                entity.CPMax = 1;
            }
            if (entity.CPCurrent > entity.CPMax)
            {
                if (entity.CPMax == 0)
                {
                    entity.CPCurrent = 1;
                    entity.CPMax = 1;
                }
                else
                {
                    entity.CPCurrent = entity.CPMax;
                }
            }
        }
    }
}
