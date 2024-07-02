// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActorItemResolver.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ActorItemResolver.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NLog;

    using Sharlayan.Core;
    using Sharlayan.Core.Enums;
    using Sharlayan.Delegates;
    using Sharlayan.Enums;

    internal class ActorItemResolver {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<StatusItem> _foundStatuses;

        private MemoryHandler _memoryHandler;

        private MonsterWorkerDelegate _monsterWorkerDelegate;

        private NPCWorkerDelegate _npcWorkerDelegate;

        private PCWorkerDelegate _pcWorkerDelegate;

        public ActorItemResolver(MemoryHandler memoryHandler, PCWorkerDelegate pcWorkerDelegate, NPCWorkerDelegate npcWorkerDelegate, MonsterWorkerDelegate monsterWorkerDelegate) {
            this._memoryHandler = memoryHandler;
            this._pcWorkerDelegate = pcWorkerDelegate;
            this._npcWorkerDelegate = npcWorkerDelegate;
            this._monsterWorkerDelegate = monsterWorkerDelegate;
            this._foundStatuses = new List<StatusItem>();
        }

        public ActorItem ResolveActorFromBytes(byte[] source, bool isCurrentUser = false, ActorItem existingActorItem = null) {
            this._foundStatuses.Clear();

            ActorItem entry = existingActorItem ?? new ActorItem();

            int defaultBaseOffset = this._memoryHandler.Structures.ActorItem.DefaultBaseOffset;
            int defaultStatOffset = this._memoryHandler.Structures.ActorItem.DefaultStatOffset;
            int defaultStatusEffectOffset = this._memoryHandler.Structures.ActorItem.DefaultStatusEffectOffset;

            try {
                entry.MapTerritory = 0;
                entry.MapIndex = 0;
                entry.MapID = 0;
                entry.TargetID = 0;
                entry.Name = this._memoryHandler.GetStringFromBytes(source, this._memoryHandler.Structures.ActorItem.Name);
                entry.ID = SharlayanBitConverter.TryToUInt32(source, this._memoryHandler.Structures.ActorItem.ID);
                entry.UUID = string.IsNullOrEmpty(entry.UUID)
                                 ? Guid.NewGuid().ToString()
                                 : entry.UUID;
                entry.NPCID1 = SharlayanBitConverter.TryToUInt32(source, this._memoryHandler.Structures.ActorItem.NPCID1);
                entry.NPCID2 = SharlayanBitConverter.TryToUInt32(source, this._memoryHandler.Structures.ActorItem.NPCID2);
                entry.OwnerID = SharlayanBitConverter.TryToUInt32(source, this._memoryHandler.Structures.ActorItem.OwnerID);

                if (this._memoryHandler.Structures.ActorItem.Type >= 0 && this._memoryHandler.Structures.ActorItem.Type < source.Length)
                {
                    entry.TypeID = source[this._memoryHandler.Structures.ActorItem.Type];
                    entry.Type = (Actor.Type) entry.TypeID;
                }

                if (this._memoryHandler.Structures.ActorItem.TargetType >= 0 && this._memoryHandler.Structures.ActorItem.TargetType < source.Length)
                {
                    entry.TargetTypeID = source[this._memoryHandler.Structures.ActorItem.TargetType];
                    entry.TargetType = (Actor.TargetType) entry.TargetTypeID;
                }

                if (this._memoryHandler.Structures.ActorItem.GatheringStatus >= 0 && this._memoryHandler.Structures.ActorItem.GatheringStatus < source.Length)
                {
                    entry.GatheringStatus = source[this._memoryHandler.Structures.ActorItem.GatheringStatus];
                    entry.Distance = source[this._memoryHandler.Structures.ActorItem.Distance];
                }

                entry.X = SharlayanBitConverter.TryToSingle(source, this._memoryHandler.Structures.ActorItem.X + defaultBaseOffset);
                entry.Z = SharlayanBitConverter.TryToSingle(source, this._memoryHandler.Structures.ActorItem.Z + defaultBaseOffset);
                entry.Y = SharlayanBitConverter.TryToSingle(source, this._memoryHandler.Structures.ActorItem.Y + defaultBaseOffset);
                entry.Heading = SharlayanBitConverter.TryToSingle(source, this._memoryHandler.Structures.ActorItem.Heading + defaultBaseOffset);
                entry.HitBoxRadius = SharlayanBitConverter.TryToSingle(source, this._memoryHandler.Structures.ActorItem.HitBoxRadius + defaultBaseOffset);
                entry.Fate = SharlayanBitConverter.TryToUInt32(source, this._memoryHandler.Structures.ActorItem.Fate + defaultBaseOffset); // ??
                if (this._memoryHandler.Structures.ActorItem.TargetFlags >= 0 && this._memoryHandler.Structures.ActorItem.TargetFlags < source.Length) entry.TargetFlags = source[this._memoryHandler.Structures.ActorItem.TargetFlags]; // ??
                if (this._memoryHandler.Structures.ActorItem.GatheringInvisible >= 0 && this._memoryHandler.Structures.ActorItem.GatheringInvisible < source.Length) entry.GatheringInvisible = source[this._memoryHandler.Structures.ActorItem.GatheringInvisible]; // ??
                entry.ModelID = SharlayanBitConverter.TryToUInt32(source, this._memoryHandler.Structures.ActorItem.ModelID);
                entry.ActionStatusID = source[this._memoryHandler.Structures.ActorItem.ActionStatus];
                entry.ActionStatus = (Actor.ActionStatus) entry.ActionStatusID;

                // 0x17D - 0 = Green name, 4 = non-agro (yellow name)
                entry.IsGM = SharlayanBitConverter.TryToBoolean(source, this._memoryHandler.Structures.ActorItem.IsGM); // ?
                if (this._memoryHandler.Structures.ActorItem.Icon >= 0 && this._memoryHandler.Structures.ActorItem.Icon < source.Length) entry.IconID = source[this._memoryHandler.Structures.ActorItem.Icon];
                entry.Icon = (Actor.Icon) entry.IconID;

                entry.InCutscene = SharlayanBitConverter.TryToBoolean(source, this._memoryHandler.Structures.ActorItem.InCutscene);

                if (this._memoryHandler.Structures.ActorItem.Status >= 0 && this._memoryHandler.Structures.ActorItem.Status < source.Length) entry.StatusID = source[this._memoryHandler.Structures.ActorItem.Status];
                entry.Status = (Actor.Status) entry.StatusID;

                entry.ClaimedByID = SharlayanBitConverter.TryToUInt32(source, this._memoryHandler.Structures.ActorItem.ClaimedByID);
                uint targetID = SharlayanBitConverter.TryToUInt32(source, this._memoryHandler.Structures.ActorItem.TargetID);
                uint pcTargetID = targetID;

                if (this._memoryHandler.Structures.ActorItem.Job >= 0 && this._memoryHandler.Structures.ActorItem.Job < source.Length) entry.JobID = source[this._memoryHandler.Structures.ActorItem.Job + defaultStatOffset];
                entry.Job = (Actor.Job) entry.JobID;

                if (this._memoryHandler.Structures.ActorItem.Level >= 0 && this._memoryHandler.Structures.ActorItem.Level < source.Length) entry.Level = source[this._memoryHandler.Structures.ActorItem.Level + defaultStatOffset];
                if (this._memoryHandler.Structures.ActorItem.GrandCompany >= 0 && this._memoryHandler.Structures.ActorItem.GrandCompany < source.Length) entry.GrandCompany = source[this._memoryHandler.Structures.ActorItem.GrandCompany + defaultStatOffset];
                if (this._memoryHandler.Structures.ActorItem.GrandCompanyRank >= 0 && this._memoryHandler.Structures.ActorItem.GrandCompanyRank < source.Length) entry.GrandCompanyRank = source[this._memoryHandler.Structures.ActorItem.GrandCompanyRank + defaultStatOffset];
                if (this._memoryHandler.Structures.ActorItem.Title >= 0 && this._memoryHandler.Structures.ActorItem.Title < source.Length) entry.Title = source[this._memoryHandler.Structures.ActorItem.Title + defaultStatOffset];
                entry.HPCurrent = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.ActorItem.HPCurrent + defaultStatOffset);
                entry.HPMax = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.ActorItem.HPMax + defaultStatOffset);
                entry.MPCurrent = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.ActorItem.MPCurrent + defaultStatOffset);
                entry.MPMax = 10000;
                entry.GPCurrent = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.ActorItem.GPCurrent + defaultStatOffset);
                entry.GPMax = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.ActorItem.GPMax + defaultStatOffset);
                entry.CPCurrent = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.ActorItem.CPCurrent + defaultStatOffset);
                entry.CPMax = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.ActorItem.CPMax + defaultStatOffset);

                // entry.Race = source[0x2578]; // ??
                // entry.Sex = (Actor.Sex) source[0x2579]; //?
                entry.IsCasting1 = SharlayanBitConverter.TryToBoolean(source, this._memoryHandler.Structures.ActorItem.IsCasting1);
                if (this._memoryHandler.Structures.ActorItem.AgroFlags >= 0 && this._memoryHandler.Structures.ActorItem.AgroFlags < source.Length) entry.AgroFlags = source[this._memoryHandler.Structures.ActorItem.AgroFlags];
                if (this._memoryHandler.Structures.ActorItem.CombatFlags >= 0 && this._memoryHandler.Structures.ActorItem.CombatFlags < source.Length) entry.CombatFlags = source[this._memoryHandler.Structures.ActorItem.CombatFlags];
                if (this._memoryHandler.Structures.ActorItem.DifficultyRank >= 0 && this._memoryHandler.Structures.ActorItem.DifficultyRank < source.Length) entry.DifficultyRank = source[this._memoryHandler.Structures.ActorItem.DifficultyRank];
                entry.CastingID = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.ActorItem.CastingID); // 0x2C94);
                entry.CastingTargetID = SharlayanBitConverter.TryToUInt32(source, this._memoryHandler.Structures.ActorItem.CastingTargetID); // 0x2CA0);
                entry.CastingProgress = SharlayanBitConverter.TryToSingle(source, this._memoryHandler.Structures.ActorItem.CastingProgress); // 0x2CC4);
                entry.CastingTime = SharlayanBitConverter.TryToSingle(source, this._memoryHandler.Structures.ActorItem.CastingTime); // 0x2DA8);
                entry.Coordinate = new Coordinate(entry.X, entry.Z, entry.Y);
                if (targetID > 0) {
                    entry.TargetID = (int) targetID;
                }
                else {
                    if (pcTargetID > 0) {
                        entry.TargetID = (int) pcTargetID;
                    }
                }

                if (entry.CastingTargetID == 3758096384) {
                    entry.CastingTargetID = 0;
                }

                entry.MapIndex = 0;
                int limit = 60;
                switch (entry.Type) {
                    case Actor.Type.PC:
                        limit = 30;
                        break;
                }

                int statusSize = this._memoryHandler.Structures.StatusItem.SourceSize;

                byte[] statusesMap = this._memoryHandler.BufferPool.Rent(statusSize * limit);
                byte[] statusMap = this._memoryHandler.BufferPool.Rent(statusSize);

                try {
                    Buffer.BlockCopy(source, defaultStatusEffectOffset, statusesMap, 0, limit * statusSize);

                    for (int i = 0; i < limit; i++) {
                        bool isNewStatus = false;

                        Buffer.BlockCopy(statusesMap, i * statusSize, statusMap, 0, statusSize);

                        short statusID = SharlayanBitConverter.TryToInt16(statusMap, this._memoryHandler.Structures.StatusItem.StatusID);
                        uint casterID = SharlayanBitConverter.TryToUInt32(statusMap, this._memoryHandler.Structures.StatusItem.CasterID);

                        StatusItem statusEntry = entry.StatusItems.FirstOrDefault(x => x.CasterID == casterID && x.StatusID == statusID);

                        if (statusEntry == null) {
                            statusEntry = new StatusItem();
                            isNewStatus = true;
                        }

                        statusEntry.TargetEntity = entry;
                        statusEntry.TargetName = entry.Name;
                        statusEntry.StatusID = statusID;
                        statusEntry.Stacks = statusMap[this._memoryHandler.Structures.StatusItem.Stacks];
                        statusEntry.Duration = SharlayanBitConverter.TryToSingle(statusMap, this._memoryHandler.Structures.StatusItem.Duration);
                        statusEntry.CasterID = casterID;

                        try {
                            ActorItem pc = this._pcWorkerDelegate.GetActorItem(statusEntry.CasterID);
                            ActorItem npc = this._npcWorkerDelegate.GetActorItem(statusEntry.CasterID);
                            ActorItem monster = this._monsterWorkerDelegate.GetActorItem(statusEntry.CasterID);
                            statusEntry.SourceEntity = (pc ?? npc) ?? monster;
                        }
                        catch (Exception ex) {
                            this._memoryHandler.RaiseException(Logger, ex);
                        }

                        try {
                            Models.XIVDatabase.StatusItem statusInfo = StatusEffectLookup.GetStatusInfo((uint) statusEntry.StatusID);
                            if (statusInfo != null) {
                                statusEntry.IsCompanyAction = statusInfo.CompanyAction;
                                string statusKey = statusInfo.Name.English;
                                switch (this._memoryHandler.Configuration.GameLanguage) {
                                    case GameLanguage.French:
                                        statusKey = statusInfo.Name.French;
                                        break;
                                    case GameLanguage.Japanese:
                                        statusKey = statusInfo.Name.Japanese;
                                        break;
                                    case GameLanguage.German:
                                        statusKey = statusInfo.Name.German;
                                        break;
                                    case GameLanguage.Chinese:
                                        statusKey = statusInfo.Name.Chinese;
                                        break;
                                    case GameLanguage.Korean:
                                        statusKey = statusInfo.Name.Korean;
                                        break;
                                }

                                statusEntry.StatusName = statusKey;
                            }
                        }
                        catch (Exception) {
                            statusEntry.StatusName = Constants.UNKNOWN_LOCALIZED_NAME;
                        }

                        if (statusEntry.IsValid()) {
                            if (isNewStatus) {
                                entry.StatusItems.Add(statusEntry);
                            }

                            this._foundStatuses.Add(statusEntry);
                        }
                    }
                }
                catch (Exception ex) {
                    this._memoryHandler.RaiseException(Logger, ex);
                }
                finally {
                    this._memoryHandler.BufferPool.Return(statusesMap);
                    this._memoryHandler.BufferPool.Return(statusMap);
                }

                entry.StatusItems.RemoveAll(x => !this._foundStatuses.Contains(x));

                // handle empty names
                if (string.IsNullOrEmpty(entry.Name)) {
                    if (entry.Type == Actor.Type.EventObject) {
                        entry.Name = $"{nameof(entry.EventObjectTypeID)}: {entry.EventObjectTypeID}";
                    }
                    else {
                        entry.Name = $"{nameof(entry.TypeID)}: {entry.TypeID}";
                    }
                }
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }

            this.CleanXPValue(ref entry);

            if (isCurrentUser) {
                this._pcWorkerDelegate.CurrentUser = entry;
            }

            return entry;
        }

        private void CleanXPValue(ref ActorItem entity) {
            if (entity.HPCurrent < 0 || entity.HPMax < 0) {
                entity.HPCurrent = 1;
                entity.HPMax = 1;
            }

            if (entity.HPCurrent > entity.HPMax) {
                if (entity.HPMax == 0) {
                    entity.HPCurrent = 1;
                    entity.HPMax = 1;
                }
                else {
                    entity.HPCurrent = entity.HPMax;
                }
            }

            if (entity.MPCurrent < 0 || entity.MPMax < 0) {
                entity.MPCurrent = 1;
                entity.MPMax = 1;
            }

            if (entity.MPCurrent > entity.MPMax) {
                if (entity.MPMax == 0) {
                    entity.MPCurrent = 1;
                    entity.MPMax = 1;
                }
                else {
                    entity.MPCurrent = entity.MPMax;
                }
            }

            if (entity.GPCurrent < 0 || entity.GPMax < 0) {
                entity.GPCurrent = 1;
                entity.GPMax = 1;
            }

            if (entity.GPCurrent > entity.GPMax) {
                if (entity.GPMax == 0) {
                    entity.GPCurrent = 1;
                    entity.GPMax = 1;
                }
                else {
                    entity.GPCurrent = entity.GPMax;
                }
            }

            if (entity.CPCurrent < 0 || entity.CPMax < 0) {
                entity.CPCurrent = 1;
                entity.CPMax = 1;
            }

            if (entity.CPCurrent > entity.CPMax) {
                if (entity.CPMax == 0) {
                    entity.CPCurrent = 1;
                    entity.CPMax = 1;
                }
                else {
                    entity.CPCurrent = entity.CPMax;
                }
            }
        }
    }
}