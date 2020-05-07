// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInventoryItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   IInventoryItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.Interfaces {
    using Sharlayan.Core.Enums;

    public interface IInventoryItem {
        uint Amount { get; set; }

        uint Condition { get; set; }

        double ConditionPercent { get; }

        uint GlamourID { get; set; }

        uint ID { get; set; }

        uint Spiritbond { get; set; }

        double SpiritbondPercent { get; }

        bool IsHQ { get; set; }

        Inventory.MateriaType[] MateriaTypes { get; set; }

        byte[] MateriaRanks { get; set; }

        int Slot { get; set; }

        uint DyeID { get; set; }
    }
}