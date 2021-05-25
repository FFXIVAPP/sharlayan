// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecastItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   RecastItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public class RecastItem {
        public int ActionProc { get; set; }

        public int Amount { get; set; }

        public int Category { get; set; }

        public int ContainerSize { get; set; }

        public int CoolDownPercent { get; set; }

        public int Icon { get; set; }

        public int ID { get; set; }

        public int InRange { get; set; }

        public int IsAvailable { get; set; }

        public int ItemSize { get; set; }

        public int RemainingCost { get; set; }

        public int Type { get; set; }
    }
}