﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InventoryContainer.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   InventoryContainer.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core {
    using System.Collections.Generic;

    using Sharlayan.Core.Enums;
    using Sharlayan.Core.Interfaces;

    public class InventoryContainer : IInventoryContainer {
        public uint Amount { get; set; }

        public Inventory.Container ContainerType { get; set; }

        public List<InventoryItem> InventoryItems { get; } = new List<InventoryItem>();

        public byte TypeID { get; set; }
    }
}