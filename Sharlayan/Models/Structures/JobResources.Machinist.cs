// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResources.Machinist.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResources.Machinist.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public partial class JobResources {
        public sealed class MachinistResources {
            public int Battery { get; set; }
            public int Heat { get; set; }
            public int OverheatTimer { get; set; }
            public int SummonTimer { get; set; }
        }
    }
}