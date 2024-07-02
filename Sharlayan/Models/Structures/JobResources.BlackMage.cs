// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResources.BlackMage.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResources.BlackMage.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public partial class JobResources {
        public sealed class BlackMageResources {
            public int Enochian { get; set; }
            public int PolyglotCount { get; set; }
            public int Stacks { get; set; }
            public int Timer { get; set; }
            public int AstralTimer { get; set; }
            public int UmbralHearts { get; set; }
        }
    }
}