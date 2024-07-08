// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResources.Dragoon.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResources.Dragoon.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public partial class JobResources {
        public sealed class DragoonResources {
            public int DragonGaze { get; set; }
            public int Mode { get; set; }
            public int Timer { get; set; }
        }
    }
}