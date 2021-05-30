// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Dragoon.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Dragoon.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;

    using Sharlayan.Core.Interfaces;
    using Sharlayan.Core.JobResources.Enums;

    public sealed class DragoonResources : IJobResource {
        public int DragonGaze { get; set; }
        public DragoonMode Mode { get; set; }
        public TimeSpan Timer { get; set; }
    }
}