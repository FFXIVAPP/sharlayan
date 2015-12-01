// FFXIVAPP.Memory
// PartyEntityHelper.cs
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
using FFXIVAPP.Memory.Core.Enums;
using FFXIVAPP.Memory.Delegates;

namespace FFXIVAPP.Memory.Helpers
{
    internal static class PartyEntityHelper
    {
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
                var entry = new PartyEntity();
                try
                {
                    switch (MemoryHandler.Instance.GameLanguage)
                    {
                        case "Korean":
                        case "Chinese":
                        default:
                            entry.X = BitConverter.ToSingle(source, 0x0);
                            entry.Z = BitConverter.ToSingle(source, 0x4);
                            entry.Y = BitConverter.ToSingle(source, 0x8);
                            entry.Coordinate = new Coordinate(entry.X, entry.Z, entry.Z);
                            entry.ID = BitConverter.ToUInt32(source, 0x10);
                            entry.Name = MemoryHandler.Instance.GetStringFromBytes(source, 0x20);
                            entry.Job = (Actor.Job) source[0x61];
                            entry.Level = source[0x63];
                            entry.HPCurrent = BitConverter.ToInt32(source, 0x68);
                            entry.HPMax = BitConverter.ToInt32(source, 0x6C);
                            entry.MPCurrent = BitConverter.ToInt16(source, 0x70);
                            entry.MPMax = BitConverter.ToInt16(source, 0x72);
                            break;
                    }
                    const int limit = 15;
                    entry.StatusEntries = new List<StatusEntry>();
                    const int statusSize = 12;
                    var statusesSource = new byte[limit * statusSize];
                    switch (MemoryHandler.Instance.GameLanguage)
                    {
                        case "Korean":
                        case "Chinese":
                        default:
                            var defaultStatusEffectOffset = MemoryHandler.Instance.ProcessModel.IsWin64 ? 0x88 : 0x80;
                            Buffer.BlockCopy(source, defaultStatusEffectOffset, statusesSource, 0, limit * 12);
                            break;
                    }
                    for (var i = 0; i < limit; i++)
                    {
                        var statusSource = new byte[statusSize];
                        Buffer.BlockCopy(statusesSource, i * statusSize, statusSource, 0, statusSize);
                        var statusEntry = new StatusEntry
                        {
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
                            if (statusEntry.StatusID > 0)
                            {
                                var statusInfo = StatusEffectHelper.StatusInfo(statusEntry.StatusID);
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
