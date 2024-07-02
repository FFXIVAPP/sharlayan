// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Summoner.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
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
        public int Attunement { get; set; }
        public TimeSpan SummonTimer { get; set; }
        public TimeSpan AttunementTimer { get; set; }
        public TimeSpan Timer { get; set; }

    }
}