// FFXIVAPP.Memory ~ ActorEntity.cs
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
using FFXIVAPP.Memory.Core.Enums;
using FFXIVAPP.Memory.Core.Interfaces;
using FFXIVAPP.Memory.Helpers;

namespace FFXIVAPP.Memory.Core
{
    public class ActorEntity : IActorEntity
    {
        private string _name;
        private List<StatusEntry> _statusEntries;

        public double HPPercent
        {
            get { return (double) (HPMax == 0 ? 0 : Decimal.Divide(HPCurrent, HPMax)); }
        }

        public string HPString
        {
            get { return String.Format("{0}/{1} [{2:P2}]", HPCurrent, HPMax, HPPercent); }
        }

        public double MPPercent
        {
            get { return (double) (MPMax == 0 ? 0 : Decimal.Divide(MPCurrent, MPMax)); }
        }

        public string MPString
        {
            get { return String.Format("{0}/{1} [{2:P2}]", MPCurrent, MPMax, MPPercent); }
        }

        public int TPMax { get; set; }

        public double TPPercent
        {
            get { return (double) (TPMax == 0 ? 0 : Decimal.Divide(TPCurrent, TPMax)); }
        }

        public string TPString
        {
            get { return String.Format("{0}/{1} [{2:P2}]", TPCurrent, TPMax, TPPercent); }
        }

        public double GPPercent
        {
            get { return (double) (GPMax == 0 ? 0 : Decimal.Divide(GPCurrent, GPMax)); }
        }

        public string GPString
        {
            get { return String.Format("{0}/{1} [{2:P2}]", GPCurrent, GPMax, GPPercent); }
        }

        public double CPPercent
        {
            get { return (double) (CPMax == 0 ? 0 : Decimal.Divide(CPCurrent, CPMax)); }
        }

        public string CPString
        {
            get { return String.Format("{0}/{1} [{2:P2}]", CPCurrent, CPMax, CPPercent); }
        }

        public bool IsFate
        {
            get { return Fate == 0x801AFFFF && Type == Actor.Type.Monster; }
        }

        public bool IsClaimed
        {
            get { return Status == Actor.Status.Claimed; }
        }

        public bool IsValid
        {
            get
            {
                switch (Type)
                {
                    case Actor.Type.NPC:
                        return !String.IsNullOrEmpty(Name) && ID != 0 && (NPCID1 != 0 || NPCID2 != 0);
                    default:
                        return !String.IsNullOrEmpty(Name) && ID != 0;
                }
            }
        }

        public Coordinate Coordinate { get; set; }

        public double CastingPercentage
        {
            get { return IsCasting && CastingTime > 0 ? CastingProgress / CastingTime : 0; }
        }

        public byte GatheringStatus { get; set; }
        public ActorEntity CurrentUser { get; set; }
        public uint MapIndex { get; set; }

        public string Name
        {
            get { return _name ?? ""; }
            set { _name = StringHelper.TitleCase(value); }
        }

        public uint ID { get; set; }
        public uint NPCID1 { get; set; }
        public uint NPCID2 { get; set; }
        public uint OwnerID { get; set; }
        public Actor.Type Type { get; set; }
        public Actor.TargetType TargetType { get; set; }
        public byte Distance { get; set; }
        public double X { get; set; }
        public double Z { get; set; }
        public double Y { get; set; }
        public float Heading { get; set; }
        public float HitBoxRadius { get; set; }
        public byte GatheringInvisible { get; set; }
        public uint Fate { get; set; }
        public uint ModelID { get; set; }
        public Actor.ActionStatus ActionStatus { get; set; }
        public bool IsGM { get; set; }
        public Actor.Icon Icon { get; set; }
        public Actor.Status Status { get; set; }
        public uint ClaimedByID { get; set; }
        public int TargetID { get; set; }
        public Actor.Job Job { get; set; }
        public byte Level { get; set; }
        public byte GrandCompany { get; set; }
        public byte GrandCompanyRank { get; set; }
        public byte Title { get; set; }
        public int HPCurrent { get; set; }
        public int HPMax { get; set; }
        public int MPCurrent { get; set; }
        public int MPMax { get; set; }
        public int TPCurrent { get; set; }
        public short GPCurrent { get; set; }
        public short GPMax { get; set; }
        public short CPCurrent { get; set; }
        public short CPMax { get; set; }
        public byte Race { get; set; }
        public Actor.Sex Sex { get; set; }

        public List<StatusEntry> StatusEntries
        {
            get { return _statusEntries ?? (_statusEntries = new List<StatusEntry>()); }
            set
            {
                if (_statusEntries == null)
                {
                    _statusEntries = new List<StatusEntry>();
                }
                _statusEntries = value;
            }
        }

        public bool IsCasting { get; set; }
        public short CastingID { get; set; }
        public uint CastingTargetID { get; set; }
        public float CastingProgress { get; set; }
        public float CastingTime { get; set; }

        public float GetDistanceTo(ActorEntity compare)
        {
            var distanceX = (float) Math.Abs(X - compare.X);
            var distanceY = (float) Math.Abs(Y - compare.Y);
            var distanceZ = (float) Math.Abs(Z - compare.Z);
            return (float) Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY) + (distanceZ * distanceZ));
        }

        public float GetHorizontalDistanceTo(ActorEntity compare)
        {
            var distanceX = (float) Math.Abs(X - compare.X);
            var distanceY = (float) Math.Abs(Y - compare.Y);
            return (float) Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY));
        }

        public float GetCastingDistanceTo(ActorEntity compare)
        {
            var distance = GetHorizontalDistanceTo(compare) - compare.HitBoxRadius;
            return distance > 0 ? distance : 0;
        }
    }
}
