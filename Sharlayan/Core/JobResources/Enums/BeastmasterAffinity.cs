// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BeastmasterAffinity.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2026 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Mirrors FFXIVClientStructs' BeastmasterAffinity (JobGaugeEnums.cs). Values are the
//   game's own, so they are safe to compare against the raw gauge byte.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources.Enums {
    public enum BeastmasterAffinity : byte {
        None = 0,

        Volant = 1,

        Rampant = 2,

        Durant = 3,

        Eldritch = 4,

        Sunstrider = 5,

        Moonstalker = 6,
    }
}
