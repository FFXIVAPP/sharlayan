// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionLookup.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
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
    using System.Threading.Tasks;

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
                Korean = "???"
            }
        };

        private static bool Loading;

        public static async Task<List<ActionItem>> DamageOverTimeActions(string patchVersion = "latest") {
            List<ActionItem> results = new List<ActionItem>();
            if (Loading) {
                return results;
            }

            if (Actions.Any()) {
                results.AddRange(Actions.Where(kvp => kvp.Value.IsDamageOverTime).Select(kvp => kvp.Value));
                return results;
            }

            await Resolve(patchVersion);
            return results;
        }

        public static async Task<ActionItem> GetActionInfo(string name, string patchVersion = "latest") {
            if (Loading) {
                return DefaultActionInfo;
            }

            if (Actions.Any()) {
                return Actions.FirstOrDefault(kvp => kvp.Value.Name.Matches(name)).Value ?? DefaultActionInfo;
            }

            await Resolve(patchVersion);
            return DefaultActionInfo;
        }

        public static async Task<ActionItem> GetActionInfo(uint id, string patchVersion = "latest") {
            if (Loading) {
                return DefaultActionInfo;
            }

            if (Actions.Any()) {
                return Actions.ContainsKey(id)
                           ? Actions[id]
                           : DefaultActionInfo;
            }

            await Resolve(patchVersion);
            return DefaultActionInfo;
        }

        public static async Task<List<ActionItem>> HealingOverTimeActions(string patchVersion = "latest") {
            List<ActionItem> results = new List<ActionItem>();
            if (Loading) {
                return results;
            }

            if (Actions.Any()) {
                results.AddRange(Actions.Where(kvp => kvp.Value.IsHealingOverTime).Select(kvp => kvp.Value));
                return results;
            }

            await Resolve(patchVersion);
            return results;
        }

        internal static async Task Resolve(string patchVersion = "latest") {
            if (Loading == false) {
                Loading = true;
                await APIHelper.GetActions(Actions, patchVersion);
                Loading = false;
            }
        }
    }
}