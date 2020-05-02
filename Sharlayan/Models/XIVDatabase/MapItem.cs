﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   MapItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.XIVDatabase {
    public class MapItem {
        public uint Index { get; set; }

        public bool IsDungeonInstance { get; set; }

        public Localization Name { get; set; }
    }
}