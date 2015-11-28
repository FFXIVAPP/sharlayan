// FFXIVAPP.Memory
// IActorEntity.cs
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
