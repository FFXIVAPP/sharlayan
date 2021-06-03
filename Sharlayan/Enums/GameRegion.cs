// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameRegion.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   GameRegion.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Enums {
    public enum GameRegion {
        Global, // NA, JA, EU are all handled with the same global client

        China, // specialized for Chinese market

        Korea, // built from Chinese client; specialized for Korean market
    }
}