﻿// Sharlayan ~ PartyEntityHelper.cs
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
using NLog;
using Sharlayan.Core;
using Sharlayan.Core.Enums;
using Sharlayan.Delegates;

namespace Sharlayan.Helpers
{
    internal static class PartyEntityHelper
    {
        #region Logger

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        public static PartyEntity ResolvePartyMemberFromBytes(byte[] source, ActorEntity actorEntity = null)
        {
            if (actorEntity != null)
            {
                var entry = new PartyEntity
                {
                    X = actorEntity.X,
                    Y = actorEntity.Y,
                    Z = actorEntity.Z,
                    Coordinate = actorEntity.Coordinate,
                    ID = actorEntity.ID,
                    UUID = actorEntity.UUID,
                    Name = actorEntity.Name,
                    Job = actorEntity.Job,
                    Level = actorEntity.Level,
                    HPCurrent = actorEntity.HPCurrent,
                    HPMax = actorEntity.HPMax,
                    MPCurrent = actorEntity.MPCurrent,
                    MPMax = actorEntity.MPMax,
                    StatusEntries = actorEntity.StatusEntries
                };
                CleanXPValue(ref entry);
                return entry;
            }
            else
            {
                var defaultStatusEffectOffset = MemoryHandler.Instance.Structures.PartyEntity.DefaultStatusEffectOffset;
                var entry = new PartyEntity();
                try
                {
                    entry.X = BitConverter.TryToSingle(source, MemoryHandler.Instance.Structures.PartyEntity.X);
                    entry.Z = BitConverter.TryToSingle(source, MemoryHandler.Instance.Structures.PartyEntity.Z);
                    entry.Y = BitConverter.TryToSingle(source, MemoryHandler.Instance.Structures.PartyEntity.Y);
                    entry.Coordinate = new Coordinate(entry.X, entry.Z, entry.Z);
                    entry.ID = BitConverter.TryToUInt32(source, MemoryHandler.Instance.Structures.PartyEntity.ID);
                    entry.UUID = Guid.NewGuid()
                                     .ToString();
                    entry.Name = MemoryHandler.Instance.GetStringFromBytes(source, MemoryHandler.Instance.Structures.PartyEntity.Name);
                    entry.JobID = source[MemoryHandler.Instance.Structures.PartyEntity.Job];
                    entry.Job = (Actor.Job) entry.JobID;
                    entry.Level = source[MemoryHandler.Instance.Structures.PartyEntity.Level];
                    entry.HPCurrent = BitConverter.TryToInt32(source, MemoryHandler.Instance.Structures.PartyEntity.HPCurrent);
                    entry.HPMax = BitConverter.TryToInt32(source, MemoryHandler.Instance.Structures.PartyEntity.HPMax);
                    entry.MPCurrent = BitConverter.TryToInt16(source, MemoryHandler.Instance.Structures.PartyEntity.MPCurrent);
                    entry.MPMax = BitConverter.TryToInt16(source, MemoryHandler.Instance.Structures.PartyEntity.MPMax);
                    const int limit = 15;
                    entry.StatusEntries = new List<StatusEntry>();
                    const int statusSize = 12;
                    var statusesSource = new byte[limit * statusSize];
                    var statusSource = new byte[statusSize];

                    Buffer.BlockCopy(source, defaultStatusEffectOffset, statusesSource, 0, limit * 12);

                    for (var i = 0; i < limit; i++)
                    {
                        Buffer.BlockCopy(statusesSource, i * statusSize, statusSource, 0, statusSize);
                        var statusEntry = new StatusEntry
                        {
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
                            if (statusEntry.StatusID > 0)
                            {
                                var statusInfo = StatusEffectHelper.StatusInfo((uint) statusEntry.StatusID);
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
                return entry;
            }
        }

        private static void CleanXPValue(ref PartyEntity entity)
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
        }
    }
}
