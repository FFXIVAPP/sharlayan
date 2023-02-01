// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResources.Scholar.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResources.Scholar.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public partial class JobResources {
        public sealed class ReaperResources {
            public int Soul { get; set; }
            public int Shroud { get; set; }
            public int EnshroudedTimeRemaining { get; set; }
            public int LemureShroud { get; set; }
            public int VoidShroud { get; set; }
        }
    }
}