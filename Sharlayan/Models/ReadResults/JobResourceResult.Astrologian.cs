// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.Astrologian.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.Astrologian.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System;

    public partial class JobResourceResult {
        public sealed class AstrologianResources {
            internal AstrologianResources(JobResourceResult result) {
                this.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Astrologian.Timer));
                this.Arcana = (AstrologianCard) result.Data[result.Offsets.Astrologian.Arcana];
                this.Seals = new[] {
                    (AstrologianSeal) result.Data[result.Offsets.Astrologian.Seal1],
                    (AstrologianSeal) result.Data[result.Offsets.Astrologian.Seal2],
                    (AstrologianSeal) result.Data[result.Offsets.Astrologian.Seal3],
                };
            }

            public enum AstrologianCard : byte {
                None,

                Balance,

                Bole,

                Arrow,

                Spear,

                Ewer,

                Spire,

                LordofCrowns = 112,

                LadyofCrowns = 128,
            }

            public enum AstrologianSeal : byte {
                None,

                SolarSeal,

                LunarSeal,

                CelestialSeal,
            }

            public AstrologianCard Arcana { get; }
            public AstrologianSeal[] Seals { get; }
            public TimeSpan Timer { get; }
        }
    }
}