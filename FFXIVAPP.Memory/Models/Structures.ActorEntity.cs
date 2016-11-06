// FFXIVAPP.Memory
// FFXIVAPP & Related Plugins/Modules
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

namespace FFXIVAPP.Memory.Models
{
    public partial class Structures
    {
        public class ActoryEntityStructure
        {
            public int Name { get; set; }
            public int ID { get; set; }
            public int NPCID1 { get; set; }
            public int NPCID2 { get; set; }
            public int OwnerID { get; set; }
            public int Type { get; set; }
            public int TargetType { get; set; }
            public int GatheringStatus { get; set; }
            public int Distance { get; set; }
            public int X { get; set; }
            public int Z { get; set; }
            public int Y { get; set; }
            public int Heading { get; set; }
            public int HitBoxRadius { get; set; }
            public int Fate { get; set; }
            public int GatheringInvisible { get; set; }
            public int ModelID { get; set; }
            public int ActionStatus { get; set; }
            public int IsGM { get; set; }
            public int Icon { get; set; }
            public int Status { get; set; }
            public int ClaimedByID { get; set; }
            public int TargetID { get; set; }
            public int Job { get; set; }
            public int Level { get; set; }
            public int GrandCompany { get; set; }
            public int GrandCompanyRank { get; set; }
            public int Title { get; set; }
            public int HPCurrent { get; set; }
            public int HPMax { get; set; }
            public int MPCurrent { get; set; }
            public int MPMax { get; set; }
            public int TPCurrent { get; set; }
            public int GPCurrent { get; set; }
            public int GPMax { get; set; }
            public int CPCurrent { get; set; }
            public int CPMax { get; set; }
            public int IsCasting1 { get; set; }
            public int IsCasting2 { get; set; }
            public int CastingID { get; set; }
            public int CastingTargetID { get; set; }
            public int CastingProgress { get; set; }
            public int CastingTime { get; set; }
            public int DefaultBaseOffset { get; set; }
            public int DefaultStatOffset { get; set; }
            public int DefaultStatusEffectOffset { get; set; }
        }
    }
}
