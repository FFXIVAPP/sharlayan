// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionLookup.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
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
        private static ConcurrentDictionary<uint, ActionItem> _actions = new ConcurrentDictionary<uint, ActionItem>();

        private static bool _loading;

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

        public static List<ActionItem> DamageOverTimeActions() {
            return _actions.Where(kvp => kvp.Value.IsDamageOverTime).Select(kvp => kvp.Value).ToList();
        }

        public static ActionItem GetActionInfo(string name) {
            return _actions.FirstOrDefault(kvp => kvp.Value.Name.Matches(name)).Value ?? DefaultActionInfo;
        }

        public static ActionItem GetActionInfo(uint id) {
            return _actions.ContainsKey(id)
                       ? _actions[id]
                       : DefaultActionInfo;
        }

        public static List<ActionItem> HealingOverTimeActions() {
            return _actions.Where(kvp => kvp.Value.IsHealingOverTime).Select(kvp => kvp.Value).ToList();
        }

        internal static void Resolve(SharlayanConfiguration configuration) {
            if (_loading) {
                return;
            }

            _loading = true;
            Task.Run(
                () => {
                    APIHelper.GetActions(_actions, configuration);
                    _loading = false;
                });
        }
    }
}