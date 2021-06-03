// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonsterWorkerDelegate.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   MonsterWorkerDelegate.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Delegates {
    using System.Collections.Concurrent;

    using Sharlayan.Core;

    internal class MonsterWorkerDelegate {
        public ConcurrentDictionary<uint, ActorItem> ActorItems = new ConcurrentDictionary<uint, ActorItem>();

        public void EnsureActorItem(uint key, ActorItem entity) {
            this.ActorItems.AddOrUpdate(key, entity, (k, v) => entity);
        }

        public ActorItem GetActorItem(uint key) {
            this.ActorItems.TryGetValue(key, out ActorItem entity);
            return entity;
        }

        public bool RemoveActorItem(uint key) {
            return this.ActorItems.TryRemove(key, out ActorItem entity);
        }
    }
}