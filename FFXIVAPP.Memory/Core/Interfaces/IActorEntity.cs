// FFXIVAPP.Memory ~ IActorEntity.cs
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

using System.Collections.Generic;
using FFXIVAPP.Memory.Core.Enums;

namespace FFXIVAPP.Memory.Core.Interfaces
{
    public interface IActorEntity
    {
        ActorEntity CurrentUser { get; set; }
        uint MapIndex { get; set; }
        string Name { get; set; }
        uint ID { get; set; }
        uint NPCID1 { get; set; }
        uint NPCID2 { get; set; }
        uint OwnerID { get; set; }
        Actor.Type Type { get; set; }
        Actor.TargetType TargetType { get; set; }
        byte Distance { get; set; }
        byte GatheringStatus { get; set; }
        double X { get; set; }
        double Z { get; set; }
        double Y { get; set; }
        float Heading { get; set; }
        float HitBoxRadius { get; set; }
        byte GatheringInvisible { get; set; }
        uint Fate { get; set; }
        uint ModelID { get; set; }
        Actor.ActionStatus ActionStatus { get; set; }
        bool IsGM { get; set; }
        Actor.Icon Icon { get; set; }
        Actor.Status Status { get; set; }
        uint ClaimedByID { get; set; }
        int TargetID { get; set; }
        Actor.Job Job { get; set; }
        byte Level { get; set; }
        byte GrandCompany { get; set; }
        byte GrandCompanyRank { get; set; }
        byte Title { get; set; }
        int HPCurrent { get; set; }
        int HPMax { get; set; }
        int MPCurrent { get; set; }
        int MPMax { get; set; }
        int TPCurrent { get; set; }
        short GPCurrent { get; set; }
        short GPMax { get; set; }
        short CPCurrent { get; set; }
        short CPMax { get; set; }
        byte Race { get; set; }
        Actor.Sex Sex { get; set; }
        List<StatusEntry> StatusEntries { get; set; }
        bool IsCasting { get; set; }
        short CastingID { get; set; }
        uint CastingTargetID { get; set; }
        float CastingProgress { get; set; }
        float CastingTime { get; set; }
    }
}
