// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResources.Summoner.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResources.Summoner.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    using System;

    public partial class JobResources {
        public sealed class SummonerResources {
            public int Aether { get; set; }
            public int Attunement { get; set; }
            public int AttunementTimer { get; set; }
            public int SummonTimer { get; set; }

        }
    }
}