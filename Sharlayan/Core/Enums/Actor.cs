// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Actor.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Actor.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.Enums {
    public class Actor {
        public enum ActionStatus : byte {
            Unknown = 0x0,

            Idle = 0x01,

            Dead = 0x02,

            Sitting = 0x03,

            Mounted = 0x04,

            Crafting = 0x05,

            Gathering = 0x06,

            Melding = 0x07,

            SMachine = 0x08,
        }

        public enum EventObjectType : ushort {
            Unknown,

            BronzeTrap = 5478,

            SilverTreasureCoffer = 5479,

            CairnOfPassage = 11292,

            CairnOfReturn = 11297,

            GoldTreasureCoffer = 11500,

            Banded = 12347,

            Hoard = 12353,
        }

        public enum Icon : byte {
            None = 0x0,

            Producer = 0x1,

            GM = 0x2,

            SGM = 0x3,

            LGM = 0x4,

            Disconnected = 0x5,

            WaitingforFriendListApproval = 0x6,

            WaitingforLinkshellApproval = 0x7,

            WaitingforFreeCompanyApproval = 0x8,

            NotFound = 0x9,

            Offline = 0xA,

            BattleMentor = 0xB,

            Busy = 0xC,

            PvP = 0xD,

            PlayingTripleTriad = 0xE,

            ViewingCutscene = 0xF,

            UsingaChocoboPorter = 0x10,

            AwayfromKeyboard = 0x11,

            CameraMode = 0x12,

            LookingforRepairs = 0x13,

            LookingtoRepair = 0x14,

            LookingtoMeldMateria = 0x15,

            RolePlaying = 0x16,

            LookingforParty = 0x17,

            SwordforHire = 0x18,

            WaitingforDutyFinder = 0x19,

            RecruitingPartyMembers = 0x1A,

            Mentor = 0x1B,

            PvEMentor = 0x1C,

            TradeMentor = 0x1D,

            PvPMentor = 0x1E,

            Returner = 0x1F,

            NewAdventurer = 0x20,

            AllianceLeader = 0x21,

            AlliancePartyLeader = 0x22,

            AlliancePartyMember = 0x23,

            PartyLeader = 0x24,

            PartyMember = 0x25,

            PartyLeaderCrossWorld = 0x26,

            PartyMemberCrossWorld = 0x27,

            AnotherWorld = 0x28,

            SharingDuty = 0x29,

            SimilarDuty = 0x2A,

            InDuty = 0x2B,

            TrialAdventurer = 0x2C,

            FreeCompany = 0x2D,

            GrandCompany = 0x2E,

            Online = 0x2F,
        }

        public enum Job : byte {
            Unknown = 0x0,

            GLD = 0x1,

            PGL = 0x2,

            MRD = 0x3,

            LNC = 0x4,

            ARC = 0x5,

            CNJ = 0x6,

            THM = 0x7,

            CPT = 0x8,

            BSM = 0x9,

            ARM = 0xA,

            GSM = 0xB,

            LTW = 0xC,

            WVR = 0xD,

            ALC = 0xE,

            CUL = 0xF,

            MIN = 0x10,

            BTN = 0x11,

            FSH = 0x12,

            PLD = 0x13,

            MNK = 0x14,

            WAR = 0x15,

            DRG = 0x16,

            BRD = 0x17,

            WHM = 0x18,

            BLM = 0x19,

            ACN = 0x1A,

            SMN = 0x1B,

            SCH = 0x1C,

            ROG = 0x1D,

            NIN = 0x1E,

            MCH = 0x1F,

            DRK = 0x20,

            AST = 0x21,

            SAM = 0x22,

            RDM = 0x23,

            BLU = 0x24,

            GNB = 0x25,

            DNC = 0x26,

            RPR = 0x27,

            SGE = 0x28,

            VPR = 0x29,

            PCT = 0x2A,
        }

        public enum Sex : byte {
            Male = 0x0,

            Female = 0x1,
        }

        public enum Status : byte {
            Unknown = 0x0,

            Claimed = 0x01,

            Idle = 0x02,

            Crafting = 0x05,

            UnknownUnSheathed = 0x06,

            UnknownSheathed = 0x07,
        }

        public enum TargetType : byte {
            Unknown = 0x0,

            Own = 0x1,

            True = 0x2,

            False = 0x4,
        }

        public enum Type : byte {
            Unknown = 0x0,

            PC = 0x01,

            Monster = 0x02,

            NPC = 0x03,

            TreasureCoffer = 0x04,

            Aetheryte = 0x05,

            Gathering = 0x06,

            EventObject = 0x07,

            Mount = 0x08,

            Minion = 0x09,
        }
    }
}