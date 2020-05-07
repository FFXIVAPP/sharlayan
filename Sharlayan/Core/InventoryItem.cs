// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InventoryItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   InventoryItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core {
    using Interfaces;
    using Enums;

    public class InventoryItem : IInventoryItem {
        public int Slot { get; set; }
        public uint ID { get; set; }
        public uint Amount { get; set; }
        public uint Condition { get; set; }
        public double ConditionPercent => (double)decimal.Divide(Condition, 30000);
        public uint Spiritbond { get; set; }
        public double SpiritbondPercent => (double)decimal.Divide(Spiritbond, 10000);
        public bool IsHQ { get; set; }
        public Inventory.MateriaType[] MateriaTypes { get; set; }
        public byte[] MateriaRanks { get; set; }
        public uint DyeID { get; set; }
        public uint GlamourID { get; set; }
    }
}