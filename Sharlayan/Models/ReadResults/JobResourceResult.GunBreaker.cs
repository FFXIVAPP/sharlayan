// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.GunBreaker.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.GunBreaker.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    public partial class JobResourceResult {
        public sealed class GunBreakerResources {
            internal GunBreakerResources(JobResourceResult result) {
                this.Cartridge = result.Data[result.Offsets.GunBreaker.Cartridge];
                this.ComboStep = result.Data[result.Offsets.GunBreaker.ComboStep];
            }

            public int Cartridge { get; }
            public int ComboStep { get; }
        }
    }
}