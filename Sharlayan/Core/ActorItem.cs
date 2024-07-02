// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActorItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ActorItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core {
    using Sharlayan.Core.Enums;
    using Sharlayan.Core.Interfaces;

    public class ActorItem : ActorItemBase, IActorItem {
        public double CastingPercentage =>
            this.IsCasting1 && this.CastingTime > 0
                ? this.CastingProgress / this.CastingTime
                : 0;

        public bool IsAgroed => (this.AgroFlags & 1) > 0;

        public bool IsClaimed => this.Status == Actor.Status.Claimed;

        public bool IsFate => this.Fate == 0x801AFFFF && this.Type == Actor.Type.Monster;

        // 0xBF if targetable, 0xBD if not. Assuming a 0x2 bitmask for now.
        public bool IsTargetable => (this.TargetFlags & 2) > 0;

        public bool IsValid {
            get {
                switch (this.Type) {
                    case Actor.Type.NPC:
                        return this.ID != 0 && (this.NPCID1 != 0 || this.NPCID2 != 0);
                    default:
                        return this.ID != 0;
                }
            }
        }

        public Actor.ActionStatus ActionStatus { get; set; }

        public byte ActionStatusID { get; set; }

        public byte AgroFlags { get; set; }

        public short CastingID { get; set; }

        public float CastingProgress { get; set; }

        public uint CastingTargetID { get; set; }

        public float CastingTime { get; set; }

        public uint ClaimedByID { get; set; }

        public byte CombatFlags { get; set; }

        public byte DifficultyRank { get; set; }

        public byte Distance { get; set; }

        public Actor.EventObjectType EventObjectType { get; set; }

        public ushort EventObjectTypeID { get; set; }

        public uint Fate { get; set; }

        public byte GatheringInvisible { get; set; }

        public byte GatheringStatus { get; set; }

        public byte GrandCompany { get; set; }

        public byte GrandCompanyRank { get; set; }

        public float Heading { get; set; }

        public Actor.Icon Icon { get; set; }

        public byte IconID { get; set; }

        public bool IsCasting1 { get; set; }

        public bool InCombat => (this.CombatFlags & (1 << 5)) != 0; //(this.CombatFlags & (1 << 1)) != 0;

        public bool IsAggressive => (this.CombatFlags & (1 << 4)) != 0; //(this.CombatFlags & (1 << 0)) != 0;

        public bool IsCasting => (this.CombatFlags & (1 << 6)) != 0; //(this.CombatFlags & (1 << 7)) != 0;

        public bool IsGM { get; set; }

        public uint MapID { get; set; }

        public uint MapIndex { get; set; }

        public uint MapTerritory { get; set; }

        public uint ModelID { get; set; }

        public uint NPCID1 { get; set; }

        public uint NPCID2 { get; set; }

        public uint OwnerID { get; set; }

        public byte Race { get; set; }

        public Actor.Sex Sex { get; set; }

        public byte SexID { get; set; }

        public Actor.Status Status { get; set; }

        public byte StatusID { get; set; }

        public byte TargetFlags { get; set; }

        public int TargetID { get; set; }

        public Actor.TargetType TargetType { get; set; }

        public byte TargetTypeID { get; set; }

        public byte Title { get; set; }

        public Actor.Type Type { get; set; }

        public byte TypeID { get; set; }

        public bool InCutscene { get; set; }

        public bool WeaponUnsheathed => (this.CombatFlags & (1 << 0)) != 0;

        public ActorItem Clone() {
            ActorItem cloned = (ActorItem) this.MemberwiseClone();

            cloned.Coordinate = new Coordinate(this.Coordinate.X, this.Coordinate.Z, this.Coordinate.Y);
            cloned.EnmityItems = new System.Collections.Generic.List<EnmityItem>();
            cloned.StatusItems = new System.Collections.Generic.List<StatusItem>();

            foreach (EnmityItem enmityItem in this.EnmityItems) {
                cloned.EnmityItems.Add(
                    new EnmityItem {
                        Enmity = enmityItem.Enmity,
                        ID = enmityItem.ID,
                        Name = enmityItem.Name,
                    });
            }

            foreach (StatusItem statusItem in this.StatusItems) {
                cloned.StatusItems.Add(
                    new StatusItem {
                        CasterID = statusItem.CasterID,
                        Duration = statusItem.Duration,
                        IsCompanyAction = statusItem.IsCompanyAction,
                        Stacks = statusItem.Stacks,
                        StatusID = statusItem.StatusID,
                        StatusName = statusItem.StatusName,
                        TargetName = statusItem.TargetName,
                    });
            }

            return cloned;
        }
    }
}