// FFXIVAPP.Memory
// Inventory.cs
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
    public class Inventory
    {
        public enum Container : byte
        {
            INVENTORY_1 = 0,
            INVENTORY_2 = 1,
            INVENTORY_3 = 2,
            INVENTORY_4 = 3,
            CURRENT_EQ = 4,
            EXTRA_EQ = 5,
            CRYSTALS = 6,
            QUESTS_KI = 9,
            HIRE_1 = 18,
            HIRE_2 = 19,
            HIRE_3 = 20,
            HIRE_4 = 21,
            HIRE_5 = 22,
            HIRE_6 = 23,
            HIRE_7 = 24,

            AC_MH = 29,
            AC_OH = 30,
            AC_HEAD = 31,
            AC_BODY = 32,
            AC_HANDS = 33,
            AC_BELT = 34,
            AC_LEGS = 35,
            AC_FEET = 36,
            AC_EARRINGS = 37,
            AC_NECK = 38,
            AC_WRISTS = 39,
            AC_RINGS = 40,
            AC_SOULS = 41,
            COMPANY_1 = 42,
            COMPANY_2 = 43,
            COMPANY_3 = 44,
            COMPANY_CRYSTALS = 45
        }
    }
}
