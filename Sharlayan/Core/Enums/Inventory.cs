// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Inventory.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Inventory.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.Enums {
    public class Inventory {
        public enum Container : uint {
            Inventory1 = 0,

            Inventory2 = 1,

            Inventory3 = 2,

            Inventory4 = 3,

            EquippedItems = 1000,

            Currency = 2000,

            Crystals = 2001,

            //Unknown2002 = 2002,
            //Unknown2003 = 2003,

            KeyItems = 2004,

            //Unknown2005 = 2005,
            //Unknown2006 = 2006,
            //Unknown2007 = 2007,
            //Unknown2008 = 2008,
            Examine = 2009,
            //Unknown2010 = 2010,
            //Unknown2011 = 2011,
            //Unknown2012 = 2012,
            //Unknown2013 = 2013,

            ArmoryOffHand = 3200,

            ArmoryHelmet = 3201,

            ArmoryChest = 3202,

            ArmoryGlove = 3203,

            ArmoryBelt = 3204,

            ArmoryPants = 3205,

            ArmoryBoots = 3206,

            ArmoryEarrings = 3207,

            ArmoryNecklace = 3208,

            ArmoryWrist = 3209,

            ArmoryRings = 3300,

            ArmorySouls = 3400,

            ArmoryMainHand = 3500,

            SaddleBag1 = 4000,

            SaddleBag2 = 4001,

            PremiumSaddleBag1 = 4100,

            PremiumSaddleBag2 = 4101,

            RetainerPage1 = 10000,

            RetainerPage2 = 10001,

            RetainerPage3 = 10002,

            RetainerPage4 = 10003,

            RetainerPage5 = 10004,

            RetainerPage6 = 10005,

            RetainerPage7 = 10006,

            RetainerEquippedItems = 11000,

            RetainerGil = 12000,

            RetainerCrystals = 12001,

            RetainerMarket = 12002,

            FreeCompanyPage1 = 20000,

            FreeCompanyPage2 = 20001,

            FreeCompanyPage3 = 20002,

            //Unknown20003 = 20003,
            //Unknown20004 = 20004,

            FreeCompanyGil = 22000,

            FreeCompanyCrystals = 22001,

            //Unknown25000 = 25000,
            //Unknown25001 = 25001,
            //Unknown25002 = 25002,
            //Unknown25003 = 25003,
            //Unknown25004 = 25004,
            //Unknown25005 = 25005,
            //Unknown25006 = 25006,
            //Unknown25007 = 25007,
            //Unknown25008 = 25008,
            //Unknown25009 = 25009,
            //Unknown25010 = 25010,

            //Unknown27000 = 27000,
            //Unknown27001 = 27001,
            //Unknown27002 = 27002,
            //Unknown27003 = 27003,
            //Unknown27004 = 27004,
            //Unknown27005 = 27005,
            //Unknown27006 = 27006,
            //Unknown27007 = 27007,
            //Unknown27008 = 27008,
        }

        public enum MateriaType : byte {
            None = 0,

            Cracked,

            Strength,

            Vitality,

            Dexterity,

            Intelligence,

            Mind,

            Piety,

            FireResistance,

            IceResistance,

            WindResistance,

            EarthResistance,

            LightningResistance,

            WaterResistance,

            DirectHitRate,

            CriticalHit,

            Determination,

            Tenacity,

            Gathering,

            Perception,

            GatheringPoints,

            Craftsmanship,

            CraftingPoints,

            Control,

            SkillSpeed,

            SpellSpeed,
        }
    }
}