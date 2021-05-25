// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActorResult.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
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

        public ConcurrentDictionary<uint, ActorItem> NewMonsters { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();

        public ConcurrentDictionary<uint, ActorItem> NewNPCs { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();

        public ConcurrentDictionary<uint, ActorItem> NewPCs { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();

        public ConcurrentDictionary<uint, ActorItem> RemovedMonsters { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();

        public ConcurrentDictionary<uint, ActorItem> RemovedNPCs { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();

        public ConcurrentDictionary<uint, ActorItem> RemovedPCs { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();
    }
}