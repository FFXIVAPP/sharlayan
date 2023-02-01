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

    public sealed class ReaperResources : IJobResource {
        public int Soul { get; set; }
        public int Shroud { get; set; }
        public int LemureShroud { get; set; }
        public int VoidShroud { get; set; }
        public TimeSpan EnshroudedTimeRemaining { get; set; }
        public TimeSpan Timer { get; set; }
    }
}