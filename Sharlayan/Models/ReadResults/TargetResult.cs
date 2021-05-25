// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetResult.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   TargetResult.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using Sharlayan.Core;

    public class TargetResult {
        public TargetInfo TargetInfo { get; internal set; } = new TargetInfo();

        public bool TargetsFound { get; internal set; }
    }
}