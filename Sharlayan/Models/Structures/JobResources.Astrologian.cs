// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResources.Astrologian.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResources.Astrologian.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public partial class JobResources {
        public sealed class AstrologianResources {
            public int Arcana { get; set; }
            public int Seal1 { get; set; }
            public int Seal2 { get; set; }
            public int Seal3 { get; set; }
            public int Timer { get; set; }
        }
    }
}