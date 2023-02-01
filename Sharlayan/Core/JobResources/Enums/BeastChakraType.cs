// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AetherFlags.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   AetherFlags.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources.Enums {
    using System;

    [Flags]
    public enum BeastChakraType : byte
    {
        None = 0,
        Coeurl = 1,
        OpoOpo = 2,
        Raptor = 3
    }
}