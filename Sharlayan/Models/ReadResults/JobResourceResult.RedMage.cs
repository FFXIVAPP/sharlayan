// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.RedMage.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.RedMage.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    public partial class JobResourceResult {
        public sealed class RedMageResources {
            internal RedMageResources(JobResourceResult result) {
                this.WhiteMana = result.Data[result.Offsets.RedMage.WhiteMana];
                this.BlackMana = result.Data[result.Offsets.RedMage.BlackMana];
            }

            public int BlackMana { get; }
            public int WhiteMana { get; }
        }
    }
}