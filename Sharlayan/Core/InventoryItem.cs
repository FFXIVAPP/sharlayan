﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InventoryItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   InventoryItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core {
    using Sharlayan.Core.Interfaces;

    public class InventoryItem : IInventoryItem {
        public uint Amount { get; set; }

        public uint Durability { get; set; }

        public double DurabilityPercent => (double) decimal.Divide(this.Durability, 30000);

        public uint GlamourID { get; set; }

        public uint ID { get; set; }

        public bool IsHQ { get; set; }

        public uint SB { get; set; }

        public double SBPercent => (double) decimal.Divide(this.SB, 10000);

        public int Slot { get; set; }
    }
}