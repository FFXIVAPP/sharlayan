// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Samurai.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Samurai.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;

    using Sharlayan.Core.Interfaces;
    using Sharlayan.Core.JobResources.Enums;

    public sealed class SamuraiResources : IJobResource {
        public int Kenki { get; set; }
        public int Meditation { get; set; }
        public Iaijutsu Sen { get; set; }
        public TimeSpan Timer { get; set; }
    }
}