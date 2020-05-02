// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionItem.cs" company="SyndicatedLife">
//   Copyright� 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
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
        public string ActionKey { get; set; }

        public int Amount { get; set; }

        public int Category { get; set; }

        public int CoolDownPercent { get; set; }

        public int Icon { get; set; }

        public int ID { get; set; }

        public bool InRange { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsKeyBindAssigned => !string.IsNullOrWhiteSpace(this.KeyBinds);

        public bool IsProcOrCombo { get; set; }

        public string KeyBinds { get; set; }

        public List<string> Modifiers { get; } = new List<string>();

        public string Name { get; set; }

        public int RemainingCost { get; set; }

        public int Slot { get; set; }

        public int Type { get; set; }
    }
}