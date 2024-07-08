// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActionItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   IActionItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.Interfaces {
    using System.Collections.Generic;

    public interface IActionItem {
        string ActionKey { get; set; }

        int Amount { get; set; }

        int Category { get; set; }

        int CoolDownPercent { get; set; }

        int Icon { get; set; }

        int ID { get; set; }

        bool InRange { get; set; }

        bool IsAvailable { get; set; }

        bool ChargeReady { get; set; }

        int ChargesRemaining { get; set; }

        bool IsProcOrCombo { get; set; }

        string KeyBinds { get; set; }

        List<string> Modifiers { get; }

        string Name { get; set; }

        int RemainingCost { get; set; }

        int Slot { get; set; }

        int Type { get; set; }
    }
}