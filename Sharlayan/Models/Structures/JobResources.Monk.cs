// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResources.Monk.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResources.Monk.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public partial class JobResources {
        public sealed class MonkResources {
            public int Chakra { get; set; }
            public int BeastChakra1 { get; set; }
            public int BeastChakra2 { get; set; }
            public int BeastChakra3 { get; set; }
            public int Nadi { get; set; }
            public int Timer { get; set; }
        }
    }
}