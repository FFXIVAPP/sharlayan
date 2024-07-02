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
    public enum AetherFlags : byte {
        None = 0,
        Aetherflow1 = 1 << 0,
        Aetherflow2 = 1 << 1,
        Aetherflow = Aetherflow1 | Aetherflow2,
        IfritAttuned = 1 << 2,
        TitanAttuned = 1 << 3,
        PhoenixReady = 1 << 4,
        IfritReady = 1 << 5,
        TitanReady = 1 << 6,
        GarudaReady = 1 << 7,
        GarudaAttuned = IfritAttuned | TitanAttuned
    }
}