// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scholar.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Scholar.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;

    using Sharlayan.Core.Interfaces;

    public sealed class SageResources : IJobResource {
        public int Addersgall { get; set; }
        public int Addersting { get; set; }
        public int Eukrasia { get; set; }
        public bool EukrasiaActive { get; set; }
        public TimeSpan AddersgallTimer { get; set; }
        public TimeSpan Timer { get; set; }
    }
}