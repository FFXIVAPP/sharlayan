// FFXIVAPP.Memory
// FFXIVAPP & Related Plugins/Modules
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

namespace FFXIVAPP.Memory.Models
{
    public class ActionItem
    {
        public Localization Name { get; set; }
        public int Icon { get; set; }
        public int Level { get; set; }
        public int ClassJobCategory { get; set; }
        public int ClassJob { get; set; }
        public int SpellGroup { get; set; }
        public int CanTargetSelf { get; set; }
        public int CanTargetParty { get; set; }
        public int CanTargetFriendly { get; set; }
        public int CanTargetHostile { get; set; }
        public int CanTargetDead { get; set; }
        public int StatusRequired { get; set; }
        public int StatusGainSelf { get; set; }
        public int Cost { get; set; }
        public object CostHP { get; set; }
        public object CostMP { get; set; }
        public object CostTP { get; set; }
        public int CostCP { get; set; }
        public int CastRange { get; set; }
        public decimal CastTime { get; set; }
        public decimal RecastTime { get; set; }
        public int IsInGame { get; set; }
        public int IsTrait { get; set; }
        public int IsPvp { get; set; }
        public int IsTargetArea { get; set; }
        public int ActionCategory { get; set; }
        public int ActionCombo { get; set; }
        public int ActionProcStatus { get; set; }
        public int ActionTimelineHit { get; set; }
        public int ActionTimelineUse { get; set; }
        public int ActionData { get; set; }
        public int EffectRange { get; set; }
        public int Type { get; set; }
    }
}
