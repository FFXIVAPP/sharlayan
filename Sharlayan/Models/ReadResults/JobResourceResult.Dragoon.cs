// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.Dragoon.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.Dragoon.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System;

    public partial class JobResourceResult {
        public sealed class DragoonResources {
            internal DragoonResources(JobResourceResult result) {
                this.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Dragoon.Timer));
                this.Mode = (DragoonMode) result.Data[result.Offsets.Dragoon.Mode];
                this.DragonGaze = result.Data[result.Offsets.Dragoon.DragonGaze];
            }

            public enum DragoonMode : byte {
                None,

                Blood,

                Life,
            }

            public int DragonGaze { get; }
            public DragoonMode Mode { get; }
            public TimeSpan Timer { get; }
        }
    }
}