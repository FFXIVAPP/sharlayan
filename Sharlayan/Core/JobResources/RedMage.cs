// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedMage.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   RedMage.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;

    using Sharlayan.Core.Interfaces;

    public sealed class RedMageResources : IJobResource {
        public int BlackMana { get; set; }
        public int WhiteMana { get; set; }
        public TimeSpan Timer { get; set; }
    }
}