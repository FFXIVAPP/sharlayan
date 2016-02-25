// FFXIVAPP.Memory ~ PlayerEntity.cs
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
using FFXIVAPP.Memory.Core.Interfaces;
using FFXIVAPP.Memory.Helpers;

namespace FFXIVAPP.Memory.Core
{
    public class PlayerEntity : IPlayerEntity
    {
        private List<EnmityEntry> _enmityEntries;
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = StringHelper.TitleCase(value); }
        }

        public List<EnmityEntry> EnmityEntries
        {
            get { return _enmityEntries ?? (_enmityEntries = new List<EnmityEntry>()); }
            set
            {
                if (_enmityEntries == null)
                {
                    _enmityEntries = new List<EnmityEntry>();
                }
                _enmityEntries = value;
            }
        }

        public byte JobID { get; set; }
        public Actor.Job Job { get; set; }
        public byte PGL { get; set; }
        public byte GLD { get; set; }
        public byte MRD { get; set; }
        public byte ARC { get; set; }
        public byte LNC { get; set; }
        public byte THM { get; set; }
        public byte CNJ { get; set; }
        public byte ACN { get; set; }
        public byte ROG { get; set; }
        public byte AST { get; set; }
        public byte DRK { get; set; }
        public byte MCH { get; set; }
        public byte CPT { get; set; }
        public byte BSM { get; set; }
        public byte ARM { get; set; }
        public byte GSM { get; set; }
        public byte LTW { get; set; }
        public byte WVR { get; set; }
        public byte ALC { get; set; }
        public byte CUL { get; set; }
        public byte MIN { get; set; }
        public byte BTN { get; set; }
        public byte FSH { get; set; }
        public int PGL_CurrentEXP { get; set; }
        public int GLD_CurrentEXP { get; set; }
        public int MRD_CurrentEXP { get; set; }
        public int ARC_CurrentEXP { get; set; }
        public int LNC_CurrentEXP { get; set; }
        public int THM_CurrentEXP { get; set; }
        public int CNJ_CurrentEXP { get; set; }
        public int ACN_CurrentEXP { get; set; }
        public int ROG_CurrentEXP { get; set; }
        public int AST_CurrentEXP { get; set; }
        public int DRK_CurrentEXP { get; set; }
        public int MCH_CurrentEXP { get; set; }
        public int CPT_CurrentEXP { get; set; }
        public int BSM_CurrentEXP { get; set; }
        public int ARM_CurrentEXP { get; set; }
        public int GSM_CurrentEXP { get; set; }
        public int LTW_CurrentEXP { get; set; }
        public int WVR_CurrentEXP { get; set; }
        public int ALC_CurrentEXP { get; set; }
        public int CUL_CurrentEXP { get; set; }
        public int MIN_CurrentEXP { get; set; }
        public int BTN_CurrentEXP { get; set; }
        public int FSH_CurrentEXP { get; set; }
        public short BaseStrength { get; set; }
        public short BaseDexterity { get; set; }
        public short BaseVitality { get; set; }
        public short BaseIntelligence { get; set; }
        public short BaseMind { get; set; }
        public short BasePiety { get; set; }
        public short Strength { get; set; }
        public short Dexterity { get; set; }
        public short Vitality { get; set; }
        public short Intelligence { get; set; }
        public short Mind { get; set; }
        public short Piety { get; set; }
        public int HPMax { get; set; }
        public int MPMax { get; set; }
        public int TPMax { get; set; }
        public int GPMax { get; set; }
        public int CPMax { get; set; }
        public short Parry { get; set; }
        public short Defense { get; set; }
        public short Evasion { get; set; }
        public short MagicDefense { get; set; }
        public short SlashingResistance { get; set; }
        public short PiercingResistance { get; set; }
        public short BluntResistance { get; set; }
        public short FireResistance { get; set; }
        public short IceResistance { get; set; }
        public short WindResistance { get; set; }
        public short EarthResistance { get; set; }
        public short LightningResistance { get; set; }
        public short WaterResistance { get; set; }
        public short AttackPower { get; set; }
        public short Accuracy { get; set; }
        public short CriticalHitRate { get; set; }
        public short AttackMagicPotency { get; set; }
        public short HealingMagicPotency { get; set; }
        public short Determination { get; set; }
        public short SkillSpeed { get; set; }
        public short SpellSpeed { get; set; }
        public short Craftmanship { get; set; }
        public short Control { get; set; }
        public short Gathering { get; set; }
        public short Perception { get; set; }
    }
}
