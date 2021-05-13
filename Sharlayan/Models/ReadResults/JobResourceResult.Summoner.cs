// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.Summoner.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.Summoner.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System;

    public partial class JobResourceResult {
        public sealed class SummonerResources {
            internal SummonerResources(JobResourceResult result) {
                this.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Summoner.Timer));
                this.Aether = (AetherFlags) result.Data[result.Offsets.Summoner.Aether];
            }

            [Flags]
            public enum AetherFlags : byte {
                Empty = 0,

                AetherFlow1 = 1,

                AetherFlow2 = 2,

                Dreadwyrm1 = 4,

                Dreadwyrm2 = 8,
            }

            public AetherFlags Aether { get; }

            public int AetherFlow =>
                this.Aether.HasFlag(AetherFlags.AetherFlow1)
                    ? 1
                    : this.Aether.HasFlag(AetherFlags.AetherFlow2)
                        ? 2
                        : 0;

            public int DreadwyrmAether =>
                this.Aether.HasFlag(AetherFlags.Dreadwyrm1)
                    ? 1
                    : this.Aether.HasFlag(AetherFlags.Dreadwyrm2)
                        ? 2
                        : 0;

            public TimeSpan Timer { get; }
        }
    }
}