// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ActionItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core {
    using System.Collections.Generic;

    using Sharlayan.Core.Interfaces;

    public class ActionItem : IActionItem {
        public bool IsKeyBindAssigned => !string.IsNullOrWhiteSpace(this.KeyBinds);
        public string ActionKey { get; set; }

        public int Amount { get; set; }

        public int Category { get; set; }

        public int CoolDownPercent { get; set; }

        public int Icon { get; set; }

        public int ID { get; set; }

        public bool InRange { get; set; }

        public bool IsAvailable { get; set; }

        public bool ChargeReady { get; set; }

        public int ChargesRemaining { get; set; }

        public bool IsProcOrCombo { get; set; }

        public string KeyBinds { get; set; }

        public List<string> Modifiers { get; } = new List<string>();

        public string Name { get; set; }

        public int RemainingCost { get; set; }

        public int Slot { get; set; }

        public int Type { get; set; }
    }
}