// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Machinist.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Machinist.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;

    using Sharlayan.Core.Interfaces;

    public sealed class MachinistResources : IJobResource {
        public int Battery { get; set; }
        public int Heat { get; set; }
        public TimeSpan OverheatTimer { get; set; }
        public TimeSpan SummonTimer { get; set; }
        public TimeSpan Timer { get; set; }
    }
}