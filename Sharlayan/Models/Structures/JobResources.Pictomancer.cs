// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResources.WhiteMage.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResources.WhiteMage.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    using Sharlayan.Core.JobResources.Enums;

    public partial class JobResources {
        public sealed class PictomancerResources {
            public int PalleteGauge { get; set; }

            public int WhitePaint { get; set; }

            public int CanvasFlags { get; set; }

            public int CreatureFlags { get; set; }
        }
    }
}