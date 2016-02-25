// FFXIVAPP.Memory ~ PlayerEntityHelper.cs
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
                    case "Korean":
                        entry.JobID = source[0x64];
                        entry.Job = (Actor.Job) entry.JobID;

                        #region Job Levels

                        entry.GLD = source[0x66];
                        entry.PGL = source[0x68];
                        entry.MRD = source[0x6A];
                        entry.LNC = source[0x6C];
                        entry.ARC = source[0x6E];
                        entry.CNJ = source[0x70];
                        entry.THM = source[0x72];

                        entry.CPT = source[0x74];
                        entry.BSM = source[0x76];
                        entry.ARM = source[0x78];
                        entry.GSM = source[0x7A];
                        entry.LTW = source[0x7C];
                        entry.WVR = source[0x7E];
                        entry.ALC = source[0x80];
                        entry.CUL = source[0x82];

                        entry.MIN = source[0x84];
                        entry.BTN = source[0x86];
                        entry.FSH = source[0x88];

                        entry.ACN = source[0x8A];
                        entry.ROG = source[0x8C];

                        #endregion

                        #region Current Experience

                        entry.GLD_CurrentEXP = BitConverter.ToInt32(source, 0x94);
                        entry.PGL_CurrentEXP = BitConverter.ToInt32(source, 0x98);
                        entry.MRD_CurrentEXP = BitConverter.ToInt32(source, 0x9C);
                        entry.LNC_CurrentEXP = BitConverter.ToInt32(source, 0xA0);
                        entry.ARC_CurrentEXP = BitConverter.ToInt32(source, 0xA4);
                        entry.CNJ_CurrentEXP = BitConverter.ToInt32(source, 0xA8);
                        entry.THM_CurrentEXP = BitConverter.ToInt32(source, 0xAC);

                        entry.CPT_CurrentEXP = BitConverter.ToInt32(source, 0xB0);
                        entry.BSM_CurrentEXP = BitConverter.ToInt32(source, 0xB4);
                        entry.ARM_CurrentEXP = BitConverter.ToInt32(source, 0xB8);
                        entry.GSM_CurrentEXP = BitConverter.ToInt32(source, 0xBC);
                        entry.LTW_CurrentEXP = BitConverter.ToInt32(source, 0xC0);
                        entry.WVR_CurrentEXP = BitConverter.ToInt32(source, 0xC4);
                        entry.ALC_CurrentEXP = BitConverter.ToInt32(source, 0xC8);
                        entry.CUL_CurrentEXP = BitConverter.ToInt32(source, 0xCC);

                        entry.MIN_CurrentEXP = BitConverter.ToInt32(source, 0xD0);
                        entry.BTN_CurrentEXP = BitConverter.ToInt32(source, 0xD4);
                        entry.FSH_CurrentEXP = BitConverter.ToInt32(source, 0xD8);

                        entry.ACN_CurrentEXP = BitConverter.ToInt32(source, 0xDC);
                        entry.ROG_CurrentEXP = BitConverter.ToInt32(source, 0xE0);

                        #endregion

                        #region Base Stats

                        entry.BaseStrength = BitConverter.ToInt16(source, 0xFC);
                        entry.BaseDexterity = BitConverter.ToInt16(source, 0x100);
                        entry.BaseVitality = BitConverter.ToInt16(source, 0x104);
                        entry.BaseIntelligence = BitConverter.ToInt16(source, 0x108);
                        entry.BaseMind = BitConverter.ToInt16(source, 0x10C);
                        entry.BasePiety = BitConverter.ToInt16(source, 0x110);

                        #endregion

                        #region Base Stats (base+gear+bonus)

                        entry.Strength = BitConverter.ToInt16(source, 0x118);
                        entry.Dexterity = BitConverter.ToInt16(source, 0x11C);
                        entry.Vitality = BitConverter.ToInt16(source, 0x120);
                        entry.Intelligence = BitConverter.ToInt16(source, 0x124);
                        entry.Mind = BitConverter.ToInt16(source, 0x128);
                        entry.Piety = BitConverter.ToInt16(source, 0x12C);

                        #endregion

                        #region Basic Info

                        entry.HPMax = BitConverter.ToInt16(source, 0x130);
                        entry.MPMax = BitConverter.ToInt16(source, 0x134);
                        entry.TPMax = BitConverter.ToInt16(source, 0x138);
                        entry.GPMax = BitConverter.ToInt16(source, 0x13C);
                        entry.CPMax = BitConverter.ToInt16(source, 0x140);

                        #endregion

                        #region Offensive Properties

                        entry.Accuracy = BitConverter.ToInt16(source, 0x16C);
                        entry.CriticalHitRate = BitConverter.ToInt16(source, 0x180);
                        entry.Determination = BitConverter.ToInt16(source, 0x1C4);

                        #endregion

                        #region Defensive Properties

                        entry.Parry = BitConverter.ToInt16(source, 0x160);
                        entry.Defense = BitConverter.ToInt16(source, 0x168);
                        entry.MagicDefense = BitConverter.ToInt16(source, 0x174);

                        #endregion

                        #region Phyiscal Properties

                        entry.AttackPower = BitConverter.ToInt16(source, 0x164);
                        entry.SkillSpeed = BitConverter.ToInt16(source, 0x1C8);

                        #endregion

                        #region Mental Properties

                        entry.SpellSpeed = BitConverter.ToInt16(source, 0x170);
                        entry.AttackMagicPotency = BitConverter.ToInt16(source, 0x198);
                        entry.HealingMagicPotency = BitConverter.ToInt16(source, 0x19C);

                        #endregion

                        #region Status Resistances

                        //entry.SlowResistance = BitConverter.ToInt16(source, 0x1C8);
                        //entry.SilenceResistance = BitConverter.ToInt16(source, 0x1CC);
                        //entry.BindResistance = BitConverter.ToInt16(source, 0x1D0);
                        //entry.PoisionResistance = BitConverter.ToInt16(source, 0x1D4);
                        //entry.StunResistance = BitConverter.ToInt16(source, 0x1D8);
                        //entry.SleepResistance = BitConverter.ToInt16(source, 0x1DC);
                        //entry.BindResistance = BitConverter.ToInt16(source, 0x1E0);
                        //entry.HeavyResistance = BitConverter.ToInt16(source, 0x1E4);

                        #endregion

                        #region Elemental Resistances

                        entry.FireResistance = BitConverter.ToInt16(source, 0x1A8);
                        entry.IceResistance = BitConverter.ToInt16(source, 0x1AC);
                        entry.WindResistance = BitConverter.ToInt16(source, 0x1B0);
                        entry.EarthResistance = BitConverter.ToInt16(source, 0x1B4);
                        entry.LightningResistance = BitConverter.ToInt16(source, 0x1B8);
                        entry.WaterResistance = BitConverter.ToInt16(source, 0x1BC);

                        #endregion

                        #region Physical Resistances

                        entry.SlashingResistance = BitConverter.ToInt16(source, 0x188);
                        entry.PiercingResistance = BitConverter.ToInt16(source, 0x18C);
                        entry.BluntResistance = BitConverter.ToInt16(source, 0x190);

                        #endregion

                        #region Crafting

                        entry.Craftmanship = BitConverter.ToInt16(source, 0x22C);
                        entry.Control = BitConverter.ToInt16(source, 0x230);

                        #endregion

                        #region Gathering

                        entry.Gathering = BitConverter.ToInt16(source, 0x234);
                        entry.Perception = BitConverter.ToInt16(source, 0x238);

                        #endregion

                        break;
                    case "Chinese":
                        entry.JobID = source[0x64];
                        entry.Job = (Actor.Job) entry.JobID;

                        #region Job Levels

                        entry.GLD = source[0x66];
                        entry.PGL = source[0x68];
                        entry.MRD = source[0x6A];
                        entry.LNC = source[0x6C];
                        entry.ARC = source[0x6E];
                        entry.CNJ = source[0x70];
                        entry.THM = source[0x72];

                        entry.CPT = source[0x74];
                        entry.BSM = source[0x76];
                        entry.ARM = source[0x78];
                        entry.GSM = source[0x7A];
                        entry.LTW = source[0x7C];
                        entry.WVR = source[0x7E];
                        entry.ALC = source[0x80];
                        entry.CUL = source[0x82];

                        entry.MIN = source[0x84];
                        entry.BTN = source[0x86];
                        entry.FSH = source[0x88];

                        entry.ACN = source[0x8A];
                        entry.ROG = source[0x8C];

                        #endregion

                        #region Current Experience

                        entry.GLD_CurrentEXP = BitConverter.ToInt32(source, 0x94);
                        entry.PGL_CurrentEXP = BitConverter.ToInt32(source, 0x98);
                        entry.MRD_CurrentEXP = BitConverter.ToInt32(source, 0x9C);
                        entry.LNC_CurrentEXP = BitConverter.ToInt32(source, 0xA0);
                        entry.ARC_CurrentEXP = BitConverter.ToInt32(source, 0xA4);
                        entry.CNJ_CurrentEXP = BitConverter.ToInt32(source, 0xA8);
                        entry.THM_CurrentEXP = BitConverter.ToInt32(source, 0xAC);

                        entry.CPT_CurrentEXP = BitConverter.ToInt32(source, 0xB0);
                        entry.BSM_CurrentEXP = BitConverter.ToInt32(source, 0xB4);
                        entry.ARM_CurrentEXP = BitConverter.ToInt32(source, 0xB8);
                        entry.GSM_CurrentEXP = BitConverter.ToInt32(source, 0xBC);
                        entry.LTW_CurrentEXP = BitConverter.ToInt32(source, 0xC0);
                        entry.WVR_CurrentEXP = BitConverter.ToInt32(source, 0xC4);
                        entry.ALC_CurrentEXP = BitConverter.ToInt32(source, 0xC8);
                        entry.CUL_CurrentEXP = BitConverter.ToInt32(source, 0xCC);

                        entry.MIN_CurrentEXP = BitConverter.ToInt32(source, 0xD0);
                        entry.BTN_CurrentEXP = BitConverter.ToInt32(source, 0xD4);
                        entry.FSH_CurrentEXP = BitConverter.ToInt32(source, 0xD8);

                        entry.ACN_CurrentEXP = BitConverter.ToInt32(source, 0xDC);
                        entry.ROG_CurrentEXP = BitConverter.ToInt32(source, 0xE0);

                        #endregion

                        #region Base Stats

                        entry.BaseStrength = BitConverter.ToInt16(source, 0xFC);
                        entry.BaseDexterity = BitConverter.ToInt16(source, 0x100);
                        entry.BaseVitality = BitConverter.ToInt16(source, 0x104);
                        entry.BaseIntelligence = BitConverter.ToInt16(source, 0x108);
                        entry.BaseMind = BitConverter.ToInt16(source, 0x10C);
                        entry.BasePiety = BitConverter.ToInt16(source, 0x110);

                        #endregion

                        #region Base Stats (base+gear+bonus)

                        entry.Strength = BitConverter.ToInt16(source, 0x118);
                        entry.Dexterity = BitConverter.ToInt16(source, 0x11C);
                        entry.Vitality = BitConverter.ToInt16(source, 0x120);
                        entry.Intelligence = BitConverter.ToInt16(source, 0x124);
                        entry.Mind = BitConverter.ToInt16(source, 0x128);
                        entry.Piety = BitConverter.ToInt16(source, 0x12C);

                        #endregion

                        #region Basic Info

                        entry.HPMax = BitConverter.ToInt16(source, 0x130);
                        entry.MPMax = BitConverter.ToInt16(source, 0x134);
                        entry.TPMax = BitConverter.ToInt16(source, 0x138);
                        entry.GPMax = BitConverter.ToInt16(source, 0x13C);
                        entry.CPMax = BitConverter.ToInt16(source, 0x140);

                        #endregion

                        #region Offensive Properties

                        entry.Accuracy = BitConverter.ToInt16(source, 0x16C);
                        entry.CriticalHitRate = BitConverter.ToInt16(source, 0x180);
                        entry.Determination = BitConverter.ToInt16(source, 0x1C4);

                        #endregion

                        #region Defensive Properties

                        entry.Parry = BitConverter.ToInt16(source, 0x160);
                        entry.Defense = BitConverter.ToInt16(source, 0x168);
                        entry.MagicDefense = BitConverter.ToInt16(source, 0x174);

                        #endregion

                        #region Phyiscal Properties

                        entry.AttackPower = BitConverter.ToInt16(source, 0x164);
                        entry.SkillSpeed = BitConverter.ToInt16(source, 0x1C8);

                        #endregion

                        #region Mental Properties

                        entry.SpellSpeed = BitConverter.ToInt16(source, 0x170);
                        entry.AttackMagicPotency = BitConverter.ToInt16(source, 0x198);
                        entry.HealingMagicPotency = BitConverter.ToInt16(source, 0x19C);

                        #endregion

                        #region Status Resistances

                        //entry.SlowResistance = BitConverter.ToInt16(source, 0x1C8);
                        //entry.SilenceResistance = BitConverter.ToInt16(source, 0x1CC);
                        //entry.BindResistance = BitConverter.ToInt16(source, 0x1D0);
                        //entry.PoisionResistance = BitConverter.ToInt16(source, 0x1D4);
                        //entry.StunResistance = BitConverter.ToInt16(source, 0x1D8);
                        //entry.SleepResistance = BitConverter.ToInt16(source, 0x1DC);
                        //entry.BindResistance = BitConverter.ToInt16(source, 0x1E0);
                        //entry.HeavyResistance = BitConverter.ToInt16(source, 0x1E4);

                        #endregion

                        #region Elemental Resistances

                        entry.FireResistance = BitConverter.ToInt16(source, 0x1A8);
                        entry.IceResistance = BitConverter.ToInt16(source, 0x1AC);
                        entry.WindResistance = BitConverter.ToInt16(source, 0x1B0);
                        entry.EarthResistance = BitConverter.ToInt16(source, 0x1B4);
                        entry.LightningResistance = BitConverter.ToInt16(source, 0x1B8);
                        entry.WaterResistance = BitConverter.ToInt16(source, 0x1BC);

                        #endregion

                        #region Physical Resistances

                        entry.SlashingResistance = BitConverter.ToInt16(source, 0x188);
                        entry.PiercingResistance = BitConverter.ToInt16(source, 0x18C);
                        entry.BluntResistance = BitConverter.ToInt16(source, 0x190);

                        #endregion

                        #region Crafting

                        entry.Craftmanship = BitConverter.ToInt16(source, 0x22C);
                        entry.Control = BitConverter.ToInt16(source, 0x230);

                        #endregion

                        #region Gathering

                        entry.Gathering = BitConverter.ToInt16(source, 0x234);
                        entry.Perception = BitConverter.ToInt16(source, 0x238);

                        #endregion

                        break;
                    default:
                        entry.JobID = source[0x66];
                        entry.Job = (Actor.Job) entry.JobID;

                        #region Job Levels

                        var step = 2;
                        var i = 0x68 - step;

                        entry.PGL = source[i += step];
                        entry.GLD = source[i += step];
                        entry.MRD = source[i += step];
                        entry.ARC = source[i += step];
                        entry.LNC = source[i += step];
                        entry.THM = source[i += step];
                        entry.CNJ = source[i += step];

                        entry.CPT = source[i += step];
                        entry.BSM = source[i += step];
                        entry.ARM = source[i += step];
                        entry.GSM = source[i += step];
                        entry.LTW = source[i += step];
                        entry.WVR = source[i += step];
                        entry.ALC = source[i += step];
                        entry.CUL = source[i += step];

                        entry.MIN = source[i += step];
                        entry.BTN = source[i += step];
                        entry.FSH = source[i += step];

                        entry.ACN = source[i += step];
                        entry.ROG = source[i += step];

                        entry.MCH = source[i += step];
                        entry.DRK = source[i += step];
                        entry.AST = source[i += step];

                        #endregion

                        #region Current Experience

                        step = 4;
                        i = 0x98 - step;

                        entry.PGL_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.GLD_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.MRD_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.ARC_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.LNC_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.THM_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.CNJ_CurrentEXP = BitConverter.ToInt32(source, i += step);

                        entry.CPT_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.BSM_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.ARM_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.GSM_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.LTW_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.WVR_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.ALC_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.CUL_CurrentEXP = BitConverter.ToInt32(source, i += step);

                        entry.MIN_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.BTN_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.FSH_CurrentEXP = BitConverter.ToInt32(source, i += step);

                        entry.ACN_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.ROG_CurrentEXP = BitConverter.ToInt32(source, i += step);

                        entry.MCH_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.DRK_CurrentEXP = BitConverter.ToInt32(source, i += step);
                        entry.AST_CurrentEXP = BitConverter.ToInt32(source, i += step);

                        #endregion

                        #region Base Stats

                        step = 4;
                        i = 0x100 - step;

                        entry.BaseStrength = BitConverter.ToInt16(source, i += step);
                        entry.BaseDexterity = BitConverter.ToInt16(source, i += step);
                        entry.BaseVitality = BitConverter.ToInt16(source, i += step);
                        entry.BaseIntelligence = BitConverter.ToInt16(source, i += step);
                        entry.BaseMind = BitConverter.ToInt16(source, i += step);
                        entry.BasePiety = BitConverter.ToInt16(source, i += step);

                        #endregion

                        #region Base Stats (base+gear+bonus)

                        step = 4;
                        i = 0x11C - step;

                        entry.Strength = BitConverter.ToInt16(source, i += step);
                        entry.Dexterity = BitConverter.ToInt16(source, i += step);
                        entry.Vitality = BitConverter.ToInt16(source, i += step);
                        entry.Intelligence = BitConverter.ToInt16(source, i += step);
                        entry.Mind = BitConverter.ToInt16(source, i += step);
                        entry.Piety = BitConverter.ToInt16(source, i += step);

                        #endregion

                        #region Basic Info

                        step = 4;
                        i = 0x138 - step;

                        entry.HPMax = BitConverter.ToInt16(source, i += step);
                        entry.MPMax = BitConverter.ToInt16(source, i += step);
                        entry.TPMax = BitConverter.ToInt16(source, i += step);
                        entry.GPMax = BitConverter.ToInt16(source, i += step);
                        entry.CPMax = BitConverter.ToInt16(source, i += step);

                        #endregion

                        #region Offensive Properties

                        entry.Accuracy = BitConverter.ToInt16(source, 0x170);
                        entry.CriticalHitRate = BitConverter.ToInt16(source, 0x184);
                        entry.Determination = BitConverter.ToInt16(source, 0x1C8);

                        #endregion

                        #region Defensive Properties

                        entry.Parry = BitConverter.ToInt16(source, 0x164);
                        entry.Defense = BitConverter.ToInt16(source, 0x16C);
                        entry.MagicDefense = BitConverter.ToInt16(source, 0x178);

                        #endregion

                        #region Phyiscal Properties

                        entry.AttackPower = BitConverter.ToInt16(source, 0x168);
                        entry.SkillSpeed = BitConverter.ToInt16(source, 0x1CC);

                        #endregion

                        #region Mental Properties

                        entry.SpellSpeed = BitConverter.ToInt16(source, 0x174);
                        entry.AttackMagicPotency = BitConverter.ToInt16(source, 0x19C);
                        entry.HealingMagicPotency = BitConverter.ToInt16(source, 0x1A0);

                        #endregion

                        #region Status Resistances

                        //entry.SlowResistance = BitConverter.ToInt16(source, 0x1C8);
                        //entry.SilenceResistance = BitConverter.ToInt16(source, 0x1CC);
                        //entry.BindResistance = BitConverter.ToInt16(source, 0x1D0);
                        //entry.PoisionResistance = BitConverter.ToInt16(source, 0x1D4);
                        //entry.StunResistance = BitConverter.ToInt16(source, 0x1D8);
                        //entry.SleepResistance = BitConverter.ToInt16(source, 0x1DC);
                        //entry.BindResistance = BitConverter.ToInt16(source, 0x1E0);
                        //entry.HeavyResistance = BitConverter.ToInt16(source, 0x1E4);

                        #endregion

                        #region Elemental Resistances

                        step = 4;
                        i = 0x1AC - step;

                        entry.FireResistance = BitConverter.ToInt16(source, i += step);
                        entry.IceResistance = BitConverter.ToInt16(source, i += step);
                        entry.WindResistance = BitConverter.ToInt16(source, i += step);
                        entry.EarthResistance = BitConverter.ToInt16(source, i += step);
                        entry.LightningResistance = BitConverter.ToInt16(source, i += step);
                        entry.WaterResistance = BitConverter.ToInt16(source, i += step);

                        #endregion

                        #region Physical Resistances

                        step = 4;
                        i = 0x18C - step;

                        entry.SlashingResistance = BitConverter.ToInt16(source, i += step);
                        entry.PiercingResistance = BitConverter.ToInt16(source, i += step);
                        entry.BluntResistance = BitConverter.ToInt16(source, i += step);

                        #endregion

                        #region Crafting

                        entry.Craftmanship = BitConverter.ToInt16(source, 0x230);
                        entry.Control = BitConverter.ToInt16(source, 0x234);

                        #endregion

                        #region Gathering

                        entry.Gathering = BitConverter.ToInt16(source, 0x238);
                        entry.Perception = BitConverter.ToInt16(source, 0x23C);

                        #endregion

                        break;
                }
            }
            catch (Exception ex)
            {
            }
            return entry;
        }
    }
}
