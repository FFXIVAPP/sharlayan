﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotBarItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   HotBarItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public class HotBarItem {
        public int ContainerSize { get; set; }

        public int ID { get; set; }

        public int ItemSize { get; set; }

        public int KeyBinds { get; set; }

        public int Name { get; set; }
    }
}