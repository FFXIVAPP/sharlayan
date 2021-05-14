// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhiteMage.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   WhiteMage.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;

    using Sharlayan.Core.Interfaces;

    public sealed class WhiteMageResources : IJobResource {
        public int BloodLily { get; set; }
        public int Lily { get; set; }
        public TimeSpan Timer { get; set; }
    }
}