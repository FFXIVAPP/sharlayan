// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.Samurai.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.Samurai.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System;

    public partial class JobResourceResult {
        public sealed class SamuraiResources {
            internal SamuraiResources(JobResourceResult result) {
                this.Kenki = result.Data[result.Offsets.Samurai.Kenki];
                this.Meditation = result.Data[result.Offsets.Samurai.Meditation];
                this.Sen = (Iaijutsu) result.Data[result.Offsets.Samurai.Sen];
            }

            [Flags]
            public enum Iaijutsu : byte {
                Setsu = 1,

                Getsu = 2,

                Ka = 4,
            }

            public int Kenki { get; }
            public int Meditation { get; }
            public Iaijutsu Sen { get; }
        }
    }
}