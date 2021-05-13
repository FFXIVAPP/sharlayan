// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.Machinist.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.Machinist.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System;

    public partial class JobResourceResult {
        public sealed class MachinistResources {
            internal MachinistResources(JobResourceResult result) {
                this.OverheatTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Machinist.OverheatTimer));
                this.SummonTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Machinist.SummonTimer));
                this.Heat = result.Data[result.Offsets.Machinist.Heat];
                this.Battery = result.Data[result.Offsets.Machinist.Battery];
            }

            public int Battery { get; }
            public int Heat { get; }
            public TimeSpan OverheatTimer { get; }
            public TimeSpan SummonTimer { get; }
        }
    }
}