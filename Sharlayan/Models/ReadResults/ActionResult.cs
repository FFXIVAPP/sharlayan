// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionResult.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ActionResult.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System.Collections.Concurrent;

    using Sharlayan.Core;

    public class ActionResult {
        public ConcurrentBag<ActionContainer> ActionContainers { get; internal set; } = new ConcurrentBag<ActionContainer>();
    }
}