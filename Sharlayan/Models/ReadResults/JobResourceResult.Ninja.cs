// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.Ninja.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.Ninja.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System;

    public partial class JobResourceResult {
        public sealed class NinjaResources {
            private readonly ushort _time;

            private readonly byte _timerFlag;

            internal NinjaResources(JobResourceResult result) {
                this._time = BitConverter.ToUInt16(result.Data, result.Offsets.Ninja.Timer);
                this._timerFlag = result.Data[result.Offsets.Ninja.TimerFlag];
                this.NinkiGauge = result.Data[result.Offsets.Ninja.NinkiGauge];
            }

            public int NinkiGauge { get; }

            public TimeSpan Timer =>
                TimeSpan.FromMilliseconds(
                    this._timerFlag == 1
                        ? ushort.MaxValue + this._time
                        : this._time);
        }
    }
}