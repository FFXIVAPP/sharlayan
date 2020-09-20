﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInventoryContainer.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   IInventoryContainer.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.Interfaces {
    using System.Collections.Generic;

    using Sharlayan.Core.Enums;

    public interface IInventoryContainer {
        uint Amount { get; set; }

        Inventory.Container ContainerType { get; set; }

        List<InventoryItem> InventoryItems { get; }

        byte TypeID { get; set; }
    }
}