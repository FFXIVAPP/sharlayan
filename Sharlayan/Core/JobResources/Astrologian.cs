// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Astrologian.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Astrologian.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;
    using System.Collections.Generic;

    using Sharlayan.Core.Interfaces;
    using Sharlayan.Core.JobResources.Enums;

    public sealed class AstrologianResources : IJobResource {
        public List<AstrologianCard> DrawnCards { get; set; } = new List<AstrologianCard>();
        public AstrologianCard CurrentArcana { get; set; }
        public AstrologianDraw DrawType { get; set; }
        public TimeSpan Timer { get; set; }
    }
}