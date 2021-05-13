// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResources.Bard.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResources.Bard.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public partial class JobResources {
        public sealed class BardResources {
            public int ActiveSong { get; set; }
            public int Repertoire { get; set; }
            public int SoulVoice { get; set; }
            public int Timer { get; set; }
        }
    }
}