// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActorResult.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ActorResult.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System.Collections.Concurrent;

    using Sharlayan.Core;

    public class ActorResult {
        public ConcurrentDictionary<uint, ActorItem> CurrentMonsters { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();

        public ConcurrentDictionary<uint, ActorItem> CurrentNPCs { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();

        public ConcurrentDictionary<uint, ActorItem> CurrentPCs { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();

        public ConcurrentDictionary<uint, ActorItem> NewMonsters { get; } = new ConcurrentDictionary<uint, ActorItem>();

        public ConcurrentDictionary<uint, ActorItem> NewNPCs { get; } = new ConcurrentDictionary<uint, ActorItem>();

        public ConcurrentDictionary<uint, ActorItem> NewPCs { get; } = new ConcurrentDictionary<uint, ActorItem>();

        public ConcurrentDictionary<uint, ActorItem> RemovedMonsters { get; } = new ConcurrentDictionary<uint, ActorItem>();

        public ConcurrentDictionary<uint, ActorItem> RemovedNPCs { get; } = new ConcurrentDictionary<uint, ActorItem>();

        public ConcurrentDictionary<uint, ActorItem> RemovedPCs { get; } = new ConcurrentDictionary<uint, ActorItem>();
    }
}