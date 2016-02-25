// FFXIVAPP.Memory ~ IPlayerEntity.cs
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
    public interface IPlayerEntity
    {
        string Name { get; set; }

        #region Property Bindings

        List<EnmityEntry> EnmityEntries { get; set; }

        byte JobID { get; set; }
        Actor.Job Job { get; set; }

        #region Job Levels

        byte PGL { get; set; }

        byte GLD { get; set; }

        byte MRD { get; set; }

        byte ARC { get; set; }

        byte LNC { get; set; }

        byte THM { get; set; }

        byte CNJ { get; set; }

        byte ACN { get; set; }

        byte ROG { get; set; }

        byte AST { get; set; }

        byte DRK { get; set; }

        byte MCH { get; set; }

        byte CPT { get; set; }

        byte BSM { get; set; }

        byte ARM { get; set; }

        byte GSM { get; set; }

        byte LTW { get; set; }

        byte WVR { get; set; }

        byte ALC { get; set; }

        byte CUL { get; set; }

        byte MIN { get; set; }

        byte BTN { get; set; }

        byte FSH { get; set; }

        #endregion

        #region Job Exp In Level

        int PGL_CurrentEXP { get; set; }

        int GLD_CurrentEXP { get; set; }

        int MRD_CurrentEXP { get; set; }

        int ARC_CurrentEXP { get; set; }

        int LNC_CurrentEXP { get; set; }

        int THM_CurrentEXP { get; set; }

        int CNJ_CurrentEXP { get; set; }

        int ACN_CurrentEXP { get; set; }

        int ROG_CurrentEXP { get; set; }

        int AST_CurrentEXP { get; set; }

        int DRK_CurrentEXP { get; set; }

        int MCH_CurrentEXP { get; set; }

        int CPT_CurrentEXP { get; set; }

        int BSM_CurrentEXP { get; set; }

        int ARM_CurrentEXP { get; set; }

        int GSM_CurrentEXP { get; set; }

        int LTW_CurrentEXP { get; set; }

        int WVR_CurrentEXP { get; set; }

        int ALC_CurrentEXP { get; set; }

        int CUL_CurrentEXP { get; set; }

        int MIN_CurrentEXP { get; set; }

        int BTN_CurrentEXP { get; set; }

        int FSH_CurrentEXP { get; set; }

        #endregion

        #region Base Stats

        short BaseStrength { get; set; }

        short BaseDexterity { get; set; }

        short BaseVitality { get; set; }

        short BaseIntelligence { get; set; }

        short BaseMind { get; set; }

        short BasePiety { get; set; }

        #endregion

        #region Stats (base+gear+bonus)

        short Strength { get; set; }

        short Dexterity { get; set; }

        short Vitality { get; set; }

        short Intelligence { get; set; }

        short Mind { get; set; }

        short Piety { get; set; }

        #endregion

        #region Basic Infos

        int HPMax { get; set; }

        int MPMax { get; set; }

        int TPMax { get; set; }

        int GPMax { get; set; }

        int CPMax { get; set; }

        #endregion

        #region Defensive stats

        short Parry { get; set; }

        short Defense { get; set; }

        short Evasion { get; set; }

        short MagicDefense { get; set; }

        short SlashingResistance { get; set; }

        short PiercingResistance { get; set; }

        short BluntResistance { get; set; }

        short FireResistance { get; set; }

        short IceResistance { get; set; }

        short WindResistance { get; set; }

        short EarthResistance { get; set; }

        short LightningResistance { get; set; }

        short WaterResistance { get; set; }

        #endregion

        #region Offensive stats

        short AttackPower { get; set; }

        short Accuracy { get; set; }

        short CriticalHitRate { get; set; }

        short AttackMagicPotency { get; set; }

        short HealingMagicPotency { get; set; }

        short Determination { get; set; }

        short SkillSpeed { get; set; }

        short SpellSpeed { get; set; }

        #endregion

        #region DoH/DoL stats

        short Craftmanship { get; set; }

        short Control { get; set; }

        short Gathering { get; set; }

        short Perception { get; set; }

        #endregion

        #endregion
    }
}
