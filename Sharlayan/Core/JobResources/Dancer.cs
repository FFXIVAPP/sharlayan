// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Dancer.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Dancer.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;
    using System.Collections.Generic;

    using Sharlayan.Core.Interfaces;
    using Sharlayan.Core.JobResources.Enums;

    public sealed class DancerResources : IJobResource {
        public DanceStep CurrentStep =>
            this.Steps.Count <= this.StepIndex
                ? 0
                : this.Steps[this.StepIndex];

        public int Esprit { get; set; }
        public int FourFoldFeathers { get; set; }
        public int StepIndex { get; set; }

        public List<DanceStep> Steps { get; set; }
        public TimeSpan Timer { get; set; }
    }
}