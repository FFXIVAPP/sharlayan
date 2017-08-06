// Sharlayan ~ Structures.cs
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

namespace Sharlayan.Models
{
    public partial class Structures
    {
        public ActorEntityStructure ActorEntity { get; set; }
        public ActorInfoStructure ActorInfo { get; set; }
        public ChatLogPointersStructure ChatLogPointers { get; set; }
        public EnmityEntryStructure EnmityEntry { get; set; }
        public HotBarEntityStructure HotBarEntity { get; set; }
        public InventoryEntityStructure InventoryEntity { get; set; }
        public ItemInfoStructure ItemInfo { get; set; }
        public PartyEntityStructure PartyEntity { get; set; }
        public PartyInfoStructure PartyInfo { get; set; }
        public PlayerEntityStructure PlayerEntity { get; set; }
        public RecastEntityStructure RecastEntity { get; set; }
        public StatusEntryStructure StatusEntry { get; set; }
        public TargetInfoStructure TargetInfo { get; set; }
    }
}
