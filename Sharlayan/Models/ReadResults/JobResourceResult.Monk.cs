// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.Monk.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.Monk.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    public partial class JobResourceResult {
        public sealed class MonkResources {
            internal MonkResources(JobResourceResult result) {
                this.Chakra = result.Data[result.Offsets.Monk.Chakra];
            }

            public int Chakra { get; }
        }
    }
}