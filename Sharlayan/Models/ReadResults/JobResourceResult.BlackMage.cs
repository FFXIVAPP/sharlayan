// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.BlackMage.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.BlackMage.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System;

    public partial class JobResourceResult {
        public sealed class BlackMageResources {
            private readonly sbyte _stacks;

            internal BlackMageResources(JobResourceResult result) {
                this.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.BlackMage.Timer));
                this._stacks = (sbyte) result.Data[result.Offsets.BlackMage.Stacks];
                this.UmbralHearts = result.Data[result.Offsets.BlackMage.UmbralHearts];
                this.PolyglotCount = result.Data[result.Offsets.BlackMage.PolyglotCount];
                this.Enochian = result.Data[result.Offsets.BlackMage.Enochian] != 0;
            }

            public int AstralStacks =>
                this._stacks <= 0
                    ? 0
                    : this._stacks;

            public bool Enochian { get; }
            public int PolyglotCount { get; }
            public TimeSpan Timer { get; }
            public int UmbralHearts { get; }

            public int UmbralStacks =>
                this._stacks >= 0
                    ? 0
                    : this._stacks * -1;
        }
    }
}