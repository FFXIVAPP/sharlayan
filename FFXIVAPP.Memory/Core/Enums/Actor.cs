// FFXIVAPP.Memory ~ Actor.cs
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

namespace FFXIVAPP.Memory.Core.Enums
{
    public class Actor
    {
        public enum ActionStatus : byte
        {
            Unknown = 0x0,
            Idle = 0x01,
            Dead = 0x02,
            Sitting = 0x03,
            Mounted = 0x04,
            Crafting = 0x05,
            Gathering = 0x06,
            Melding = 0x07,
            SMachine = 0x08
        }

        public enum Icon : byte
        {
            None = 0x0,
            Yoshida = 0x1,
            GM = 0x2,
            SGM = 0x3,
            Clover = 0x4,
            DC = 0x5,
            Smiley = 0x6,
            RedCross = 0x9,
            GreyDC = 0xA,
            Processing = 0xB,
            Busy = 0xC,
            Duty = 0xD,
            ProcessingYellow = 0xE,
            ProcessingGrey = 0xF,
            Cutscene = 0x10,
            Away = 0x12,
            Sitting = 0x13,
            WrenchYellow = 0x14,
            Wrench = 0x15,
            Dice = 0x16,
            ProcessingGreen = 0x17,
            Sword = 0x18,
            AllianceLeader = 0x1A,
            AllianceBlueLeader = 0x1B,
            AllianceBlue = 0x1C,
            PartyLeader = 0x1D,
            PartyMember = 0x1E,
            DutyFinder = 0x18,
            Recruiting = 0x19,
            Sprout = 0x1F,
            Gil = 0x20
        }

        public enum Job : byte
        {
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
            AST = 0x21
        }

        public enum Sex : byte
        {
            Male = 0x0,
            Female = 0x1
        }

        public enum Status : byte
        {
            Unknown = 0x0,
            Claimed = 0x01,
            Idle = 0x02,
            Crafting = 0x05,
            UnknownUnSheathed = 0x06,
            UnknownSheathed = 0x07
        }

        public enum TargetType : byte
        {
            Unknown = 0x0,
            Own = 0x1,
            True = 0x2,
            False = 0x4
        }

        public enum Type : byte
        {
            Unknown = 0x0,
            PC = 0x01,
            Monster = 0x02,
            NPC = 0x03,
            Aetheryte = 0x05,
            Gathering = 0x06,
            Minion = 0x09
        }
    }
}
