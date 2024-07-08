// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotBarItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
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