// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerInfoMapper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   PlayerInfo mapper. Derives offsets from FFXIVClientStructs' PlayerState struct:
//
//     - Base stats (Str/Dex/Vit/Int/Mnd/Pie): direct [FieldOffset] fields.
//     - JobID: PlayerState.CurrentClassJobId.
//     - Per-job levels: PlayerState._classJobLevels @ 0x8A, FixedSizeArray35<short>.
//       Index is the ExpArrayIndex from each job's ClassJob sheet row.
//     - Per-job EXP: PlayerState._classJobExperience @ 0xD0, FixedSizeArray35<int>.
//
//   Fields still unmapped (no clean PlayerState equivalent and/or live in Character,
//   not PlayerState — require different reader path): HPMax, MPMax, CPMax, GPMax,
//   derived attributes (Strength, Dexterity, ..., CriticalHitRate, Determination,
//   DirectHit, SkillSpeed, SpellSpeed, Tenacity, Piety, AttackPower, AttackMagicPotency,
//   HealingMagicPotency, Defense, MagicDefense, Control, Craftmanship, Gathering,
//   Perception, element/physical resistances, BaseSubstat). These need
//   PlayerState._attributes indexed by BaseParam row, which is dynamic Lumina data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game.UI;

    using Sharlayan.Models.Structures;

    internal static class PlayerInfoMapper {
        // ExpArrayIndex values from the ClassJob Excel sheet. Stable since Shadowbringers
        // (new jobs get appended — VPR=30, PCT=31 in Dawntrail). Ordered here by index
        // value to make insertions at the tail obvious. Base classes only — combat jobs
        // (PLD/WAR/MNK/...) share their base class's index, so we don't list them twice.
        private const int ExpIdx_PGL = 0;
        private const int ExpIdx_GLD = 1;
        private const int ExpIdx_MRD = 2;
        private const int ExpIdx_ARC = 3;
        private const int ExpIdx_LNC = 4;
        private const int ExpIdx_THM = 5;
        private const int ExpIdx_CNJ = 6;
        private const int ExpIdx_CPT = 7;
        private const int ExpIdx_BSM = 8;
        private const int ExpIdx_ARM = 9;
        private const int ExpIdx_GSM = 10;
        private const int ExpIdx_LTW = 11;
        private const int ExpIdx_WVR = 12;
        private const int ExpIdx_ALC = 13;
        private const int ExpIdx_CUL = 14;
        private const int ExpIdx_MIN = 15;
        private const int ExpIdx_BTN = 16;
        private const int ExpIdx_FSH = 17;
        private const int ExpIdx_ACN = 18;
        private const int ExpIdx_ROG = 19;
        private const int ExpIdx_MCH = 20;
        private const int ExpIdx_DRK = 21;
        private const int ExpIdx_AST = 22;
        private const int ExpIdx_SAM = 23;
        private const int ExpIdx_RDM = 24;
        private const int ExpIdx_BLU = 25;
        private const int ExpIdx_GNB = 26;
        private const int ExpIdx_DNC = 27;
        private const int ExpIdx_RPR = 28;
        private const int ExpIdx_SGE = 29;
        private const int ExpIdx_VPR = 30;
        private const int ExpIdx_PCT = 31;

        public static PlayerInfo Build() {
            // Levels are shorts packed contiguously; EXP are ints.
            int levelsBase = (int)Marshal.OffsetOf<PlayerState>("_classJobLevels");
            int expBase    = (int)Marshal.OffsetOf<PlayerState>("_classJobExperience");
            int Lvl(int i) => levelsBase + i * sizeof(short);
            int Exp(int i) => expBase + i * sizeof(int);

            return new PlayerInfo {
                JobID = (int)Marshal.OffsetOf<PlayerState>(nameof(PlayerState.CurrentClassJobId)),
                BaseStrength = (int)Marshal.OffsetOf<PlayerState>(nameof(PlayerState.BaseStrength)),
                BaseDexterity = (int)Marshal.OffsetOf<PlayerState>(nameof(PlayerState.BaseDexterity)),
                BaseVitality = (int)Marshal.OffsetOf<PlayerState>(nameof(PlayerState.BaseVitality)),
                BaseIntelligence = (int)Marshal.OffsetOf<PlayerState>(nameof(PlayerState.BaseIntelligence)),
                BaseMind = (int)Marshal.OffsetOf<PlayerState>(nameof(PlayerState.BaseMind)),
                BasePiety = (int)Marshal.OffsetOf<PlayerState>(nameof(PlayerState.BasePiety)),

                // Per-job levels.
                PGL = Lvl(ExpIdx_PGL), GLD = Lvl(ExpIdx_GLD), MRD = Lvl(ExpIdx_MRD), ARC = Lvl(ExpIdx_ARC),
                LNC = Lvl(ExpIdx_LNC), THM = Lvl(ExpIdx_THM), CNJ = Lvl(ExpIdx_CNJ),
                CPT = Lvl(ExpIdx_CPT), BSM = Lvl(ExpIdx_BSM), ARM = Lvl(ExpIdx_ARM), GSM = Lvl(ExpIdx_GSM),
                LTW = Lvl(ExpIdx_LTW), WVR = Lvl(ExpIdx_WVR), ALC = Lvl(ExpIdx_ALC), CUL = Lvl(ExpIdx_CUL),
                MIN = Lvl(ExpIdx_MIN), BTN = Lvl(ExpIdx_BTN), FSH = Lvl(ExpIdx_FSH),
                ACN = Lvl(ExpIdx_ACN), ROG = Lvl(ExpIdx_ROG), MCH = Lvl(ExpIdx_MCH), DRK = Lvl(ExpIdx_DRK),
                AST = Lvl(ExpIdx_AST), SAM = Lvl(ExpIdx_SAM), RDM = Lvl(ExpIdx_RDM), BLU = Lvl(ExpIdx_BLU),
                GNB = Lvl(ExpIdx_GNB), DNC = Lvl(ExpIdx_DNC), VPR = Lvl(ExpIdx_VPR), PCT = Lvl(ExpIdx_PCT),

                // Per-job EXP.
                PGL_CurrentEXP = Exp(ExpIdx_PGL), GLD_CurrentEXP = Exp(ExpIdx_GLD), MRD_CurrentEXP = Exp(ExpIdx_MRD),
                ARC_CurrentEXP = Exp(ExpIdx_ARC), LNC_CurrentEXP = Exp(ExpIdx_LNC), THM_CurrentEXP = Exp(ExpIdx_THM),
                CNJ_CurrentEXP = Exp(ExpIdx_CNJ), CPT_CurrentEXP = Exp(ExpIdx_CPT), BSM_CurrentEXP = Exp(ExpIdx_BSM),
                ARM_CurrentEXP = Exp(ExpIdx_ARM), GSM_CurrentEXP = Exp(ExpIdx_GSM), LTW_CurrentEXP = Exp(ExpIdx_LTW),
                WVR_CurrentEXP = Exp(ExpIdx_WVR), ALC_CurrentEXP = Exp(ExpIdx_ALC), CUL_CurrentEXP = Exp(ExpIdx_CUL),
                MIN_CurrentEXP = Exp(ExpIdx_MIN), BTN_CurrentEXP = Exp(ExpIdx_BTN), FSH_CurrentEXP = Exp(ExpIdx_FSH),
                ACN_CurrentEXP = Exp(ExpIdx_ACN), ROG_CurrentEXP = Exp(ExpIdx_ROG), MCH_CurrentEXP = Exp(ExpIdx_MCH),
                DRK_CurrentEXP = Exp(ExpIdx_DRK), AST_CurrentEXP = Exp(ExpIdx_AST), SAM_CurrentEXP = Exp(ExpIdx_SAM),
                RDM_CurrentEXP = Exp(ExpIdx_RDM), BLU_CurrentEXP = Exp(ExpIdx_BLU), GNB_CurrentEXP = Exp(ExpIdx_GNB),
                DNC_CurrentEXP = Exp(ExpIdx_DNC), VPR_CurrentEXP = Exp(ExpIdx_VPR), PCT_CurrentEXP = Exp(ExpIdx_PCT),

                SourceSize = Marshal.SizeOf<PlayerState>(),
            };
        }
    }
}
