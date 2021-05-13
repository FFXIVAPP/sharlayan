// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.Paladin.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.Paladin.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    public partial class JobResourceResult {
        public sealed class PaladinResources {
            internal PaladinResources(JobResourceResult result) {
                this.OathGauge = result.Data[result.Offsets.Paladin.OathGauge];
            }

            public int OathGauge { get; }
        }
    }
}