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

    public sealed class ScholarResources : IJobResource {
        public int Aetherflow { get; set; }
        public int FaerieGauge { get; set; }
        public TimeSpan Timer { get; set; }
    }
}