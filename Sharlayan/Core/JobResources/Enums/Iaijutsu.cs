// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Iaijutsu.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Iaijutsu.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources.Enums {
    using System;

    [Flags]
    public enum Iaijutsu : byte {
        Setsu = 1,

        Getsu = 2,

        Ka = 4,
    }
}