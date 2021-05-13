// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.WhiteMage.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.WhiteMage.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System;

    public partial class JobResourceResult {
        public sealed class WhiteMageResources {
            internal WhiteMageResources(JobResourceResult result) {
                this.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.WhiteMage.Timer));
                this.Lily = result.Data[result.Offsets.WhiteMage.Lily];
                this.BloodLily = result.Data[result.Offsets.WhiteMage.BloodLily];
            }

            public int BloodLily { get; }
            public int Lily { get; }
            public TimeSpan Timer { get; }
        }
    }
}