// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResources.Astrologian.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResources.Astrologian.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public partial class JobResources {
        public sealed class AstrologianResources {
            // Cards (short at 0x08): nibbles 0-2 are the three held cards, nibble 3 is CurrentArcana.
            public int Cards { get; set; }
            // CurrentDraw (byte at 0x0A): Astral=0, Umbral=1.
            public int CurrentDraw { get; set; }
        }
    }
}