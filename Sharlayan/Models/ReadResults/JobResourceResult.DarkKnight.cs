// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.DarkKnight.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.DarkKnight.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System;

    public partial class JobResourceResult {
        public sealed class DarkKnightResources {
            internal DarkKnightResources(JobResourceResult result) {
                this.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.DarkKnight.Timer));
                this.BlackBlood = result.Data[result.Offsets.DarkKnight.BlackBlood];
                this.DarkArts = result.Data[result.Offsets.DarkKnight.DarkArts] != 0;
            }

            public int BlackBlood { get; }
            public bool DarkArts { get; }
            public TimeSpan Timer { get; }
        }
    }
}