// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DarkKnight.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   DarkKnight.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;

    using Sharlayan.Core.Interfaces;

    public sealed class DarkKnightResources : IJobResource {
        public int BlackBlood { get; set; }
        public bool DarkArts { get; set; }
        public TimeSpan Timer { get; set; }
    }
}