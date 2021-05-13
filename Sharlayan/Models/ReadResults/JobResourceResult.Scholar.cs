// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.Scholar.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.Scholar.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System;

    public partial class JobResourceResult {
        public sealed class ScholarResources {
            internal ScholarResources(JobResourceResult result) {
                this.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Scholar.Timer));
                this.Aetherflow = result.Data[result.Offsets.Scholar.Aetherflow];
                this.FaerieGauge = result.Data[result.Offsets.Scholar.FaerieGauge];
            }

            public int Aetherflow { get; }
            public int FaerieGauge { get; }
            public TimeSpan Timer { get; }
        }
    }
}