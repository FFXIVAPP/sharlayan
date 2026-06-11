// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Dancer.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Dancer.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;

    using Sharlayan.Core.Interfaces;
    using Sharlayan.Core.JobResources.Enums;

    public sealed class DancerResources : IJobResource {
        public DanceStep CurrentStep =>
            this.Steps.Length <= this.StepIndex
                ? 0
                : this.Steps[this.StepIndex];

        public int Esprit { get; set; }
        public int FourFoldFeathers { get; set; }
        public int StepIndex { get; set; }

        public DanceStep[] Steps { get; set; } = Array.Empty<DanceStep>();
        public TimeSpan Timer { get; set; }
    }
}