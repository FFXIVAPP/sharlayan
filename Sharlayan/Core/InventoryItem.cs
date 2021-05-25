// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InventoryItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   InventoryItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core {
    using Sharlayan.Core.Enums;
    using Sharlayan.Core.Interfaces;

    public class InventoryItem : IInventoryItem {
        public uint Amount { get; set; }
        public uint Condition { get; set; }
        public double ConditionPercent => (double) decimal.Divide(this.Condition, 30000);
        public uint DyeID { get; set; }
        public uint GlamourID { get; set; }
        public uint ID { get; set; }
        public bool IsHQ { get; set; }
        public byte[] MateriaRanks { get; set; }
        public Inventory.MateriaType[] MateriaTypes { get; set; }
        public uint SB { get; set; }
        public double SBPercent => (double) decimal.Divide(this.SB, 10000);
        public int Slot { get; set; }
    }
}