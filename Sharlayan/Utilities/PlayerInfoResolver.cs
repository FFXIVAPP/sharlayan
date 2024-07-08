// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerInfoResolver.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   PlayerInfoResolver.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System;

    using NLog;

    using Sharlayan.Core;
    using Sharlayan.Core.Enums;

    internal class PlayerInfoResolver {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private MemoryHandler _memoryHandler;

        public PlayerInfoResolver(MemoryHandler memoryHandler) {
            this._memoryHandler = memoryHandler;
        }

        public PlayerInfo ResolvePlayerFromBytes(byte[] source) {
            PlayerInfo entry = new PlayerInfo();
            try {
                switch (this._memoryHandler.Configuration.GameLanguage) {
                    default:
                        entry.JobID = source[this._memoryHandler.Structures.PlayerInfo.JobID];
                        entry.Job = (Actor.Job) entry.JobID;

                        #region Job Levels

                        entry.PGL = source[this._memoryHandler.Structures.PlayerInfo.PGL];
                        entry.GLD = source[this._memoryHandler.Structures.PlayerInfo.GLD];
                        entry.MRD = source[this._memoryHandler.Structures.PlayerInfo.MRD];
                        entry.ARC = source[this._memoryHandler.Structures.PlayerInfo.ARC];
                        entry.LNC = source[this._memoryHandler.Structures.PlayerInfo.LNC];
                        entry.THM = source[this._memoryHandler.Structures.PlayerInfo.THM];
                        entry.CNJ = source[this._memoryHandler.Structures.PlayerInfo.CNJ];

                        entry.CPT = source[this._memoryHandler.Structures.PlayerInfo.CPT];
                        entry.BSM = source[this._memoryHandler.Structures.PlayerInfo.BSM];
                        entry.ARM = source[this._memoryHandler.Structures.PlayerInfo.ARM];
                        entry.GSM = source[this._memoryHandler.Structures.PlayerInfo.GSM];
                        entry.LTW = source[this._memoryHandler.Structures.PlayerInfo.LTW];
                        entry.WVR = source[this._memoryHandler.Structures.PlayerInfo.WVR];
                        entry.ALC = source[this._memoryHandler.Structures.PlayerInfo.ALC];
                        entry.CUL = source[this._memoryHandler.Structures.PlayerInfo.CUL];

                        entry.MIN = source[this._memoryHandler.Structures.PlayerInfo.MIN];
                        entry.BTN = source[this._memoryHandler.Structures.PlayerInfo.BTN];
                        entry.FSH = source[this._memoryHandler.Structures.PlayerInfo.FSH];

                        entry.ACN = source[this._memoryHandler.Structures.PlayerInfo.ACN];
                        entry.ROG = source[this._memoryHandler.Structures.PlayerInfo.ROG];

                        entry.MCH = source[this._memoryHandler.Structures.PlayerInfo.MCH];
                        entry.DRK = source[this._memoryHandler.Structures.PlayerInfo.DRK];
                        entry.AST = source[this._memoryHandler.Structures.PlayerInfo.AST];

                        entry.SAM = source[this._memoryHandler.Structures.PlayerInfo.SAM];
                        entry.RDM = source[this._memoryHandler.Structures.PlayerInfo.RDM];

                        entry.BLU = source[this._memoryHandler.Structures.PlayerInfo.BLU];

                        entry.DNC = source[this._memoryHandler.Structures.PlayerInfo.DNC];
                        entry.GNB = source[this._memoryHandler.Structures.PlayerInfo.GNB];

                        entry.VPR = source[this._memoryHandler.Structures.PlayerInfo.VPR];
                        entry.PCT = source[this._memoryHandler.Structures.PlayerInfo.PCT];

                        #endregion

                        #region Current Experience

                        entry.PGL_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.PGL_CurrentEXP);
                        entry.GLD_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.GLD_CurrentEXP);
                        entry.MRD_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.MRD_CurrentEXP);
                        entry.ARC_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.ARC_CurrentEXP);
                        entry.LNC_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.LNC_CurrentEXP);
                        entry.THM_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.THM_CurrentEXP);
                        entry.CNJ_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.CNJ_CurrentEXP);

                        entry.CPT_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.CPT_CurrentEXP);
                        entry.BSM_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.BSM_CurrentEXP);
                        entry.ARM_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.ARM_CurrentEXP);
                        entry.GSM_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.GSM_CurrentEXP);
                        entry.LTW_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.LTW_CurrentEXP);
                        entry.WVR_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.WVR_CurrentEXP);
                        entry.ALC_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.ALC_CurrentEXP);
                        entry.CUL_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.CUL_CurrentEXP);

                        entry.MIN_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.MIN_CurrentEXP);
                        entry.BTN_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.BTN_CurrentEXP);
                        entry.FSH_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.FSH_CurrentEXP);

                        entry.ACN_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.ACN_CurrentEXP);
                        entry.ROG_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.ROG_CurrentEXP);

                        entry.MCH_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.MCH_CurrentEXP);
                        entry.DRK_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.DRK_CurrentEXP);
                        entry.AST_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.AST_CurrentEXP);

                        entry.SAM_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.SAM_CurrentEXP);
                        entry.RDM_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.RDM_CurrentEXP);

                        entry.BLU_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.BLU_CurrentEXP);

                        entry.DNC_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.DNC_CurrentEXP);
                        entry.GNB_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.GNB_CurrentEXP);

                        entry.VPR_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.VPR_CurrentEXP);
                        entry.PCT_CurrentEXP = SharlayanBitConverter.TryToInt32(source, this._memoryHandler.Structures.PlayerInfo.PCT_CurrentEXP);

                        #endregion

                        #region Base Stats

                        entry.BaseStrength = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.BaseStrength);
                        entry.BaseDexterity = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.BaseDexterity);
                        entry.BaseVitality = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.BaseVitality);
                        entry.BaseIntelligence = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.BaseIntelligence);
                        entry.BaseMind = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.BaseMind);
                        entry.BasePiety = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.BasePiety);
                        entry.BaseSubstat = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.BaseSubstat);

                        #endregion

                        #region Base Stats (base+gear+bonus)

                        entry.Strength = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.Strength);
                        entry.Dexterity = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.Dexterity);
                        entry.Vitality = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.Vitality);
                        entry.Intelligence = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.Intelligence);
                        entry.Mind = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.Mind);
                        entry.Piety = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.Piety);

                        #endregion

                        #region Basic Info

                        entry.HPMax = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.HPMax);
                        entry.GPMax = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.GPMax);
                        entry.CPMax = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.CPMax);

                        #endregion

                        #region Offensive Properties

                        entry.DirectHit = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.DirectHit);
                        entry.CriticalHitRate = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.CriticalHitRate);
                        entry.Determination = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.Determination);

                        #endregion

                        #region Defensive Properties

                        entry.Tenacity = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.Tenacity);
                        entry.Defense = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.Defense);
                        entry.MagicDefense = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.MagicDefense);

                        #endregion

                        #region Phyiscal Properties

                        entry.AttackPower = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.AttackPower);
                        entry.SkillSpeed = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.SkillSpeed);

                        #endregion

                        #region Mental Properties

                        entry.SpellSpeed = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.SpellSpeed);
                        entry.AttackMagicPotency = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.AttackMagicPotency);
                        entry.HealingMagicPotency = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.HealingMagicPotency);

                        #endregion

                        #region Elemental Resistances

                        entry.FireResistance = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.FireResistance);
                        entry.IceResistance = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.IceResistance);
                        entry.WindResistance = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.WindResistance);
                        entry.EarthResistance = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.EarthResistance);
                        entry.LightningResistance = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.LightningResistance);
                        entry.WaterResistance = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.WaterResistance);

                        #endregion

                        #region Physical Resistances

                        entry.SlashingResistance = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.SlashingResistance);
                        entry.PiercingResistance = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.PiercingResistance);
                        entry.BluntResistance = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.BluntResistance);

                        #endregion

                        #region Crafting

                        entry.Craftmanship = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.Craftmanship);
                        entry.Control = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.Control);

                        #endregion

                        #region Gathering

                        entry.Gathering = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.Gathering);
                        entry.Perception = SharlayanBitConverter.TryToInt16(source, this._memoryHandler.Structures.PlayerInfo.Perception);

                        #endregion

                        break;
                }
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }

            return entry;
        }
    }
}