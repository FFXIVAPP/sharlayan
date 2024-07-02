// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InventoryItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   InventoryItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public class InventoryItem {
        public int Amount { get; set; }

        public int Durability { get; set; }

        public int DyeID { get; set; }

        public int GlamourID { get; set; }

        public int ID { get; set; }

        public int IsHQ { get; set; }

        public int MateriaRank { get; set; }

        public int MateriaType { get; set; }

        public int SB { get; set; }

        public int Slot { get; set; }
    }
}