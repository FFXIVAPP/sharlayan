// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInventoryItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
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

        uint DyeID { get; set; }

        uint GlamourID { get; set; }

        uint ID { get; set; }

        bool IsHQ { get; set; }

        byte[] MateriaRanks { get; set; }

        Inventory.MateriaType[] MateriaTypes { get; set; }

        uint SB { get; set; }

        double SBPercent { get; }

        int Slot { get; set; }
    }
}