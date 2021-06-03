// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnmityItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   IEnmityItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.Interfaces {
    public interface IEnmityItem {
        uint Enmity { get; set; }

        uint ID { get; set; }

        string Name { get; set; }
    }
}