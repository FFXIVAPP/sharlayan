// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResources.Bard.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResources.Bard.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public partial class JobResources {
        public sealed class BardResources {
            // F88: this is a byte OFFSET into the gauge struct like every sibling —
            // int, so a future FCS layout push past 255 can't silently truncate.
            public int ActiveSong { get; set; }
            public int Repertoire { get; set; }
            public int SoulVoice { get; set; }
            public int RadiantFinaleCoda { get; set; }
            public int Timer { get; set; }
        }
    }
}