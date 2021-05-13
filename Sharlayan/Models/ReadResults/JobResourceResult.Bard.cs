// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.Bard.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.Bard.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System;

    public partial class JobResourceResult {
        public sealed class BardResources {
            private readonly byte _song;

            internal BardResources(JobResourceResult result) {
                this.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Bard.Timer));
                this.Repertoire = result.Data[result.Offsets.Bard.Repertoire];
                this.SoulVoice = result.Data[result.Offsets.Bard.SoulVoice];
                this._song = result.Data[result.Offsets.Bard.ActiveSong];
            }

            public enum BardSong : byte {
                None,

                MagesBallad = 5,

                ArmysPaeon = 10,

                WanderersMinuet = 15,
            }

            public BardSong ActiveSong {
                get {
                    BardSong song = (BardSong) this._song;
                    if (song != BardSong.MagesBallad && song != BardSong.ArmysPaeon && song != BardSong.WanderersMinuet) {
                        return BardSong.None;
                    }

                    return song;
                }
            }

            public int Repertoire { get; }
            public int SoulVoice { get; }
            public TimeSpan Timer { get; }
        }
    }
}