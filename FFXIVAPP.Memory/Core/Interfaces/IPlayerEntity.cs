// FFXIVAPP.Memory
// IPlayerEntity.cs
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
