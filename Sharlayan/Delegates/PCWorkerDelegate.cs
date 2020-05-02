﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PCWorkerDelegate.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   PCWorkerDelegate.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Delegates {
    using System.Collections.Concurrent;

    using Sharlayan.Core;

    internal static class PCWorkerDelegate {
        public static ConcurrentDictionary<uint, ActorItem> ActorItems = new ConcurrentDictionary<uint, ActorItem>();

        public static ActorItem CurrentUser { get; set; }

        public static void EnsureActorItem(uint key, ActorItem entity) {
            ActorItems.AddOrUpdate(key, entity, (k, v) => entity);
        }

        public static ActorItem GetActorItem(uint key) {
            ActorItems.TryGetValue(key, out ActorItem entity);
            return entity;
        }

        public static bool RemoveActorItem(uint key) {
            return ActorItems.TryRemove(key, out ActorItem entity);
        }
    }
}