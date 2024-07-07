// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DreadCombo.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   DreadCombo.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources.Enums {
    using System;

    [Flags]
    public enum DreadCombo : byte {
        Dreadwinder = 1,
        HuntersCoil = 2,
        SwiftskinsCoil = 3,
        PitOfDread = 4,
        HuntersDen = 5,
        SwiftskinsDen = 6,
    }
}