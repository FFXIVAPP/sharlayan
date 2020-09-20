﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetResult.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   TargetResult.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using Sharlayan.Core;

    public class TargetResult {
        public TargetInfo TargetInfo { get; set; } = new TargetInfo();

        public bool TargetsFound { get; set; }
    }
}