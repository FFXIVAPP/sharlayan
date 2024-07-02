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
    public enum CanvasFlags : byte {
        Pom = 1,
        Wing = 2,
        Claw = 4,
        Maw = 8,
        Weapon = 16,
        Landscape = 32,
    }

    [Flags]
    public enum CreatureFlags : byte {
        Pom = 1,
        Wings = 2,
        Claw = 4,
        MooglePortait = 16,
        MadeenPortrait = 32,
    }
}