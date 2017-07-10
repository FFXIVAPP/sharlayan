// Sharlayan ~ ActorEntityHelper.cs
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
using Sharlayan.Core.Enums;
using Sharlayan.Delegates;
using NLog;

namespace Sharlayan.Helpers
{
    internal static class ActorEntityHelper
    {
        #region Logger

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        public static ActorEntity ResolveActorFromBytes(byte[] source, bool isCurrentUser = false, ActorEntity entry = null)
        {
            entry = entry ?? new ActorEntity();
            var defaultBaseOffset = MemoryHandler.Instance.Structures.ActorEntity.DefaultBaseOffset;
            var defaultStatOffset = MemoryHandler.Instance.Structures.ActorEntity.DefaultStatOffset;
            var defaultStatusEffectOffset = MemoryHandler.Instance.Structures.ActorEntity.DefaultStatusEffectOffset;
            try
            {
                entry.MapTerritory = 0;
                entry.MapIndex = 0;
                entry.MapID = 0;
                entry.TargetID = 0;
                entry.Name = MemoryHandler.Instance.GetStringFromBytes(source, MemoryHandler.Instance.Structures.ActorEntity.Name);
                entry.ID = BitConverter.TryToUInt32(source, MemoryHandler.Instance.Structures.ActorEntity.ID);
                entry.UUID = string.IsNullOrEmpty(entry.UUID) ? Guid.NewGuid()
                                                                    .ToString() : entry.UUID;
                entry.NPCID1 = BitConverter.TryToUInt32(source, MemoryHandler.Instance.Structures.ActorEntity.NPCID1);
                entry.NPCID2 = BitConverter.TryToUInt32(source, MemoryHandler.Instance.Structures.ActorEntity.NPCID2);
                entry.OwnerID = BitConverter.TryToUInt32(source, MemoryHandler.Instance.Structures.ActorEntity.OwnerID);
                entry.TypeID = source[MemoryHandler.Instance.Structures.ActorEntity.Type];
                entry.Type = (Actor.Type) entry.TypeID;
                entry.TargetTypeID = source[MemoryHandler.Instance.Structures.ActorEntity.TargetType];
                entry.TargetType = (Actor.TargetType) entry.TargetTypeID;

                entry.GatheringStatus = source[MemoryHandler.Instance.Structures.ActorEntity.GatheringStatus];
                entry.Distance = source[MemoryHandler.Instance.Structures.ActorEntity.Distance];

                entry.X = BitConverter.TryToSingle(source, MemoryHandler.Instance.Structures.ActorEntity.X + defaultBaseOffset);
                entry.Z = BitConverter.TryToSingle(source, MemoryHandler.Instance.Structures.ActorEntity.Z + defaultBaseOffset);
                entry.Y = BitConverter.TryToSingle(source, MemoryHandler.Instance.Structures.ActorEntity.Y + defaultBaseOffset);
                entry.Heading = BitConverter.TryToSingle(source, MemoryHandler.Instance.Structures.ActorEntity.Heading + defaultBaseOffset);
                entry.HitBoxRadius = BitConverter.TryToSingle(source, MemoryHandler.Instance.Structures.ActorEntity.HitBoxRadius + defaultBaseOffset);
                entry.Fate = BitConverter.TryToUInt32(source, MemoryHandler.Instance.Structures.ActorEntity.Fate + defaultBaseOffset); // ??
                entry.GatheringInvisible = source[MemoryHandler.Instance.Structures.ActorEntity.GatheringInvisible]; // ??
                entry.ModelID = BitConverter.TryToUInt32(source, MemoryHandler.Instance.Structures.ActorEntity.ModelID);
                entry.ActionStatusID = source[MemoryHandler.Instance.Structures.ActorEntity.ActionStatus];
                entry.ActionStatus = (Actor.ActionStatus) entry.ActionStatusID;

                // 0x17D - 0 = Green name, 4 = non-agro (yellow name)
                entry.IsGM = BitConverter.TryToBoolean(source, MemoryHandler.Instance.Structures.ActorEntity.IsGM); // ?
                entry.IconID = source[MemoryHandler.Instance.Structures.ActorEntity.Icon];
                entry.Icon = (Actor.Icon) entry.IconID;

                entry.StatusID = source[MemoryHandler.Instance.Structures.ActorEntity.Status];
                entry.Status = (Actor.Status) entry.StatusID;

                entry.ClaimedByID = BitConverter.TryToUInt32(source, MemoryHandler.Instance.Structures.ActorEntity.ClaimedByID);
                var targetID = BitConverter.TryToUInt32(source, MemoryHandler.Instance.Structures.ActorEntity.TargetID);
                var pcTargetID = targetID;

                entry.JobID = source[MemoryHandler.Instance.Structures.ActorEntity.Job + defaultStatOffset];
                entry.Job = (Actor.Job) entry.JobID;
                entry.Level = source[MemoryHandler.Instance.Structures.ActorEntity.Level + defaultStatOffset];
                entry.GrandCompany = source[MemoryHandler.Instance.Structures.ActorEntity.GrandCompany + defaultStatOffset];
                entry.GrandCompanyRank = source[MemoryHandler.Instance.Structures.ActorEntity.GrandCompanyRank + defaultStatOffset];
                entry.Title = source[MemoryHandler.Instance.Structures.ActorEntity.Title + defaultStatOffset];
                entry.HPCurrent = BitConverter.TryToInt32(source, MemoryHandler.Instance.Structures.ActorEntity.HPCurrent + defaultStatOffset);
                entry.HPMax = BitConverter.TryToInt32(source, MemoryHandler.Instance.Structures.ActorEntity.HPMax + defaultStatOffset);
                entry.MPCurrent = BitConverter.TryToInt32(source, MemoryHandler.Instance.Structures.ActorEntity.MPCurrent + defaultStatOffset);
                entry.MPMax = BitConverter.TryToInt32(source, MemoryHandler.Instance.Structures.ActorEntity.MPMax + defaultStatOffset);
                entry.TPCurrent = BitConverter.TryToInt16(source, MemoryHandler.Instance.Structures.ActorEntity.TPCurrent + defaultStatOffset);
                entry.TPMax = 1000;
                entry.GPCurrent = BitConverter.TryToInt16(source, MemoryHandler.Instance.Structures.ActorEntity.GPCurrent + defaultStatOffset);
                entry.GPMax = BitConverter.TryToInt16(source, MemoryHandler.Instance.Structures.ActorEntity.GPMax + defaultStatOffset);
                entry.CPCurrent = BitConverter.TryToInt16(source, MemoryHandler.Instance.Structures.ActorEntity.CPCurrent + defaultStatOffset);
                entry.CPMax = BitConverter.TryToInt16(source, MemoryHandler.Instance.Structures.ActorEntity.CPMax + defaultStatOffset);
                //entry.Race = source[0x2578]; // ??
                //entry.Sex = (Actor.Sex) source[0x2579]; //?
                entry.IsCasting = BitConverter.TryToBoolean(source, MemoryHandler.Instance.Structures.ActorEntity.IsCasting1) && BitConverter.TryToBoolean(source, MemoryHandler.Instance.Structures.ActorEntity.IsCasting2); // 0x2C90);
                entry.CastingID = BitConverter.TryToInt16(source, MemoryHandler.Instance.Structures.ActorEntity.CastingID); // 0x2C94);
                entry.CastingTargetID = BitConverter.TryToUInt32(source, MemoryHandler.Instance.Structures.ActorEntity.CastingTargetID); // 0x2CA0);
                entry.CastingProgress = BitConverter.TryToSingle(source, MemoryHandler.Instance.Structures.ActorEntity.CastingProgress); // 0x2CC4);
                entry.CastingTime = BitConverter.TryToSingle(source, MemoryHandler.Instance.Structures.ActorEntity.CastingTime); // 0x2DA8);
                entry.Coordinate = new Coordinate(entry.X, entry.Z, entry.Y);
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

                Buffer.BlockCopy(source, defaultStatusEffectOffset, statusesSource, 0, limit * statusSize);
                for (var i = 0; i < limit; i++)
                {
                    var statusSource = new byte[statusSize];
                    Buffer.BlockCopy(statusesSource, i * statusSize, statusSource, 0, statusSize);
                    var statusEntry = new StatusEntry
                    {
                        TargetEntity = entry,
                        TargetName = entry.Name,
                        StatusID = BitConverter.TryToInt16(statusSource, MemoryHandler.Instance.Structures.StatusEntry.StatusID),
                        Stacks = statusSource[MemoryHandler.Instance.Structures.StatusEntry.Stacks],
                        Duration = BitConverter.TryToSingle(statusSource, MemoryHandler.Instance.Structures.StatusEntry.Duration),
                        CasterID = BitConverter.TryToUInt32(statusSource, MemoryHandler.Instance.Structures.StatusEntry.CasterID)
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
                        MemoryHandler.Instance.RaiseException(Logger, ex, true);
                    }
                    try
                    {
                        var statusInfo = StatusEffectHelper.StatusInfo((uint) statusEntry.StatusID);
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
                    catch (Exception)
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
                MemoryHandler.Instance.RaiseException(Logger, ex, true);
            }
            CleanXPValue(ref entry);

            if (isCurrentUser)
            {
                PCWorkerDelegate.CurrentUser = entry;
            }
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
