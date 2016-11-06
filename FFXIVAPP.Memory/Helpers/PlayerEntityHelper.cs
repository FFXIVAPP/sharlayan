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

using System;
using FFXIVAPP.Memory.Core;
using FFXIVAPP.Memory.Core.Enums;

namespace FFXIVAPP.Memory.Helpers
{
    internal static class PlayerEntityHelper
    {
        public static PlayerEntity ResolvePlayerFromBytes(byte[] source)
        {
            var entry = new PlayerEntity();
            try
            {
                entry.Name = MemoryHandler.Instance.GetStringFromBytes(source, 1);

                switch (MemoryHandler.Instance.GameLanguage)
                {
                    default:
                        entry.JobID = source[MemoryHandler.Instance.Structures.PlayerEntity.JobID];
                        entry.Job = Entity.Job[entry.JobID];

                        #region Job Levels

                        entry.PGL = source[MemoryHandler.Instance.Structures.PlayerEntity.PGL];
                        entry.GLD = source[MemoryHandler.Instance.Structures.PlayerEntity.GLD];
                        entry.MRD = source[MemoryHandler.Instance.Structures.PlayerEntity.MRD];
                        entry.ARC = source[MemoryHandler.Instance.Structures.PlayerEntity.ARC];
                        entry.LNC = source[MemoryHandler.Instance.Structures.PlayerEntity.LNC];
                        entry.THM = source[MemoryHandler.Instance.Structures.PlayerEntity.THM];
                        entry.CNJ = source[MemoryHandler.Instance.Structures.PlayerEntity.CNJ];

                        entry.CPT = source[MemoryHandler.Instance.Structures.PlayerEntity.CPT];
                        entry.BSM = source[MemoryHandler.Instance.Structures.PlayerEntity.BSM];
                        entry.ARM = source[MemoryHandler.Instance.Structures.PlayerEntity.ARM];
                        entry.GSM = source[MemoryHandler.Instance.Structures.PlayerEntity.GSM];
                        entry.LTW = source[MemoryHandler.Instance.Structures.PlayerEntity.LTW];
                        entry.WVR = source[MemoryHandler.Instance.Structures.PlayerEntity.WVR];
                        entry.ALC = source[MemoryHandler.Instance.Structures.PlayerEntity.ALC];
                        entry.CUL = source[MemoryHandler.Instance.Structures.PlayerEntity.CUL];

                        entry.MIN = source[MemoryHandler.Instance.Structures.PlayerEntity.MIN];
                        entry.BTN = source[MemoryHandler.Instance.Structures.PlayerEntity.BTN];
                        entry.FSH = source[MemoryHandler.Instance.Structures.PlayerEntity.FSH];

                        entry.ACN = source[MemoryHandler.Instance.Structures.PlayerEntity.ACN];
                        entry.ROG = source[MemoryHandler.Instance.Structures.PlayerEntity.ROG];

                        entry.MCH = source[MemoryHandler.Instance.Structures.PlayerEntity.MCH];
                        entry.DRK = source[MemoryHandler.Instance.Structures.PlayerEntity.DRK];
                        entry.AST = source[MemoryHandler.Instance.Structures.PlayerEntity.AST];

                        #endregion

                        #region Current Experience

                        entry.PGL_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.PGL_CurrentEXP);
                        entry.GLD_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.GLD_CurrentEXP);
                        entry.MRD_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.MRD_CurrentEXP);
                        entry.ARC_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.ARC_CurrentEXP);
                        entry.LNC_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.LNC_CurrentEXP);
                        entry.THM_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.THM_CurrentEXP);
                        entry.CNJ_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.CNJ_CurrentEXP);

                        entry.CPT_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.CPT_CurrentEXP);
                        entry.BSM_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.BSM_CurrentEXP);
                        entry.ARM_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.ARM_CurrentEXP);
                        entry.GSM_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.GSM_CurrentEXP);
                        entry.LTW_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.LTW_CurrentEXP);
                        entry.WVR_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.WVR_CurrentEXP);
                        entry.ALC_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.ALC_CurrentEXP);
                        entry.CUL_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.CUL_CurrentEXP);

                        entry.MIN_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.MIN_CurrentEXP);
                        entry.BTN_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.BTN_CurrentEXP);
                        entry.FSH_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.FSH_CurrentEXP);

                        entry.ACN_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.ACN_CurrentEXP);
                        entry.ROG_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.ROG_CurrentEXP);

                        entry.MCH_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.MCH_CurrentEXP);
                        entry.DRK_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.DRK_CurrentEXP);
                        entry.AST_CurrentEXP = BitConverter.ToInt32(source, MemoryHandler.Instance.Structures.PlayerEntity.AST_CurrentEXP);

                        #endregion

                        #region Base Stats

                        entry.BaseStrength = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.BaseStrength);
                        entry.BaseDexterity = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.BaseDexterity);
                        entry.BaseVitality = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.BaseVitality);
                        entry.BaseIntelligence = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.BaseIntelligence);
                        entry.BaseMind = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.BaseMind);
                        entry.BasePiety = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.BasePiety);

                        #endregion

                        #region Base Stats (base+gear+bonus)

                        entry.Strength = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.Strength);
                        entry.Dexterity = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.Dexterity);
                        entry.Vitality = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.Vitality);
                        entry.Intelligence = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.Intelligence);
                        entry.Mind = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.Mind);
                        entry.Piety = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.Piety);

                        #endregion

                        #region Basic Info

                        entry.HPMax = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.HPMax);
                        entry.MPMax = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.MPMax);
                        entry.TPMax = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.TPMax);
                        entry.GPMax = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.GPMax);
                        entry.CPMax = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.CPMax);

                        #endregion

                        #region Offensive Properties

                        entry.Accuracy = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.Accuracy);
                        entry.CriticalHitRate = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.CriticalHitRate);
                        entry.Determination = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.Determination);

                        #endregion

                        #region Defensive Properties

                        entry.Parry = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.Parry);
                        entry.Defense = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.Defense);
                        entry.MagicDefense = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.MagicDefense);

                        #endregion

                        #region Phyiscal Properties

                        entry.AttackPower = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.AttackPower);
                        entry.SkillSpeed = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.SkillSpeed);

                        #endregion

                        #region Mental Properties

                        entry.SpellSpeed = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.SpellSpeed);
                        entry.AttackMagicPotency = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.AttackMagicPotency);
                        entry.HealingMagicPotency = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.HealingMagicPotency);

                        #endregion

                        #region Elemental Resistances

                        entry.FireResistance = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.FireResistance);
                        entry.IceResistance = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.IceResistance);
                        entry.WindResistance = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.WindResistance);
                        entry.EarthResistance = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.EarthResistance);
                        entry.LightningResistance = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.LightningResistance);
                        entry.WaterResistance = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.WaterResistance);

                        #endregion

                        #region Physical Resistances

                        entry.SlashingResistance = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.SlashingResistance);
                        entry.PiercingResistance = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.PiercingResistance);
                        entry.BluntResistance = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.BluntResistance);

                        #endregion

                        #region Crafting

                        entry.Craftmanship = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.Craftmanship);
                        entry.Control = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.Control);

                        #endregion

                        #region Gathering

                        entry.Gathering = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.Gathering);
                        entry.Perception = BitConverter.ToInt16(source, MemoryHandler.Instance.Structures.PlayerEntity.Perception);

                        #endregion

                        break;
                }
            }
            catch (Exception)
            {
            }
            return entry;
        }
    }
}
