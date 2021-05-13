// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.Warrior.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.Warrior.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    public partial class JobResourceResult {
        public sealed class WarriorResources {
            internal WarriorResources(JobResourceResult result) {
                this.BeastGauge = result.Data[result.Offsets.Warrior.BeastGauge];
            }

            public int BeastGauge { get; }
        }
    }
}