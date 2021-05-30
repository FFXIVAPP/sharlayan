// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Summoner.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Summoner.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;

    using Sharlayan.Core.Interfaces;
    using Sharlayan.Core.JobResources.Enums;

    public sealed class SummonerResources : IJobResource {
        public AetherFlags Aether { get; set; }

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

        public TimeSpan Timer { get; set; }
    }
}