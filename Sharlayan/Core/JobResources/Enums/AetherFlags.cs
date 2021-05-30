// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AetherFlags.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   AetherFlags.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources.Enums {
    using System;

    [Flags]
    public enum AetherFlags : byte {
        Empty = 0,

        AetherFlow1 = 1,

        AetherFlow2 = 2,

        Dreadwyrm1 = 4,

        Dreadwyrm2 = 8,
    }
}