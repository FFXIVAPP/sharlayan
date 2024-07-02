// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Monk.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Monk.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;

    using Sharlayan.Core.Interfaces;
    using Sharlayan.Core.JobResources.Enums;

    public sealed class MonkResources : IJobResource {
        public int Chakra { get; set; }
        public BeastChakraType BeastChakra1 { get; set; }
        public BeastChakraType BeastChakra2 { get; set; }
        public BeastChakraType BeastChakra3 { get; set; }
        public BeastChakraType[] BeastChakra { get; set; }
        public NadiFlags Nadi { get; set; }
        public TimeSpan Timer { get; set; }
    }
}