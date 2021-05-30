// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlackMage.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   BlackMage.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;

    using Sharlayan.Core.Interfaces;

    public sealed class BlackMageResources : IJobResource {
        private int _astralStacks;

        private int _umbralStacks;

        public int AstralStacks { get; set; }

        public bool Enochian { get; set; }
        public int PolyglotCount { get; set; }
        public int UmbralHearts { get; set; }

        public int UmbralStacks { get; set; }
        public TimeSpan Timer { get; set; }
    }
}