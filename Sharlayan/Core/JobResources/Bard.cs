// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bard.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Bard.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;

    using Sharlayan.Core.Interfaces;
    using Sharlayan.Core.JobResources.Enums;

    public sealed class BardResources : IJobResource {
        public SongFlags ActiveSong { get; set; }
        public int Repertoire { get; set; }
        public int SoulVoice { get; set; }
        public TimeSpan Timer { get; set; }
    }
}