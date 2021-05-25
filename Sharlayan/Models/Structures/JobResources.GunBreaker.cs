// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResources.GunBreaker.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResources.GunBreaker.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public partial class JobResources {
        public sealed class GunBreakerResources {
            public int Cartridge { get; set; }
            public int ComboStep { get; set; }
        }
    }
}