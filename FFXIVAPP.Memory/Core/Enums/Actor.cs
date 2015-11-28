// FFXIVAPP.Memory
// Actor.cs
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
