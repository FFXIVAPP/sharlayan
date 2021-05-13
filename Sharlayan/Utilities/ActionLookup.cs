// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionLookup.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ActionLookup.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Sharlayan.Models;
    using Sharlayan.Models.XIVDatabase;

    public static class ActionLookup {
        private static ConcurrentDictionary<uint, ActionItem> Actions = new ConcurrentDictionary<uint, ActionItem>();

        private static ActionItem DefaultActionInfo = new ActionItem {
            Name = new Localization {
                Chinese = "???",
                English = "???",
                French = "???",
                German = "???",
                Japanese = "???",
                Korean = "???",
            },
        };

        private static bool Loading;

        public static List<ActionItem> DamageOverTimeActions(string patchVersion = "latest", bool useLocalCache = true) {
            List<ActionItem> results = new List<ActionItem>();
            if (Loading) {
                return results;
            }

            lock (Actions) {
                if (Actions.Any()) {
                    results.AddRange(Actions.Where(kvp => kvp.Value.IsDamageOverTime).Select(kvp => kvp.Value));
                    return results;
                }

                Resolve(patchVersion, useLocalCache);
                return results;
            }
        }

        public static ActionItem GetActionInfo(string name, string patchVersion = "latest", bool useLocalCache = true) {
            if (Loading) {
                return DefaultActionInfo;
            }

            lock (Actions) {
                if (Actions.Any()) {
                    return Actions.FirstOrDefault(kvp => kvp.Value.Name.Matches(name)).Value ?? DefaultActionInfo;
                }

                Resolve(patchVersion, useLocalCache);
                return DefaultActionInfo;
            }
        }

        public static ActionItem GetActionInfo(uint id, string patchVersion = "latest", bool useLocalCache = true) {
            if (Loading) {
                return DefaultActionInfo;
            }

            lock (Actions) {
                if (Actions.Any()) {
                    return Actions.ContainsKey(id)
                               ? Actions[id]
                               : DefaultActionInfo;
                }

                Resolve(patchVersion, useLocalCache);
                return DefaultActionInfo;
            }
        }

        public static List<ActionItem> HealingOverTimeActions(string patchVersion = "latest", bool useLocalCache = true) {
            List<ActionItem> results = new List<ActionItem>();
            if (Loading) {
                return results;
            }

            lock (Actions) {
                if (Actions.Any()) {
                    results.AddRange(Actions.Where(kvp => kvp.Value.IsHealingOverTime).Select(kvp => kvp.Value));
                    return results;
                }

                Resolve(patchVersion, useLocalCache);
                return results;
            }
        }

        internal static void Resolve(string patchVersion = "latest", bool useLocalCache = true) {
            if (Loading) {
                return;
            }

            Loading = true;
            APIHelper.GetActions(Actions, patchVersion, useLocalCache);
            Loading = false;
        }
    }
}