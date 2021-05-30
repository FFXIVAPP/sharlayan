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
                Chinese = Constants.UNKNOWN_LOCALIZED_NAME,
                English = Constants.UNKNOWN_LOCALIZED_NAME,
                French = Constants.UNKNOWN_LOCALIZED_NAME,
                German = Constants.UNKNOWN_LOCALIZED_NAME,
                Japanese = Constants.UNKNOWN_LOCALIZED_NAME,
                Korean = Constants.UNKNOWN_LOCALIZED_NAME,
            },
        };

        private static List<ActionItem> _damageOverTimeActions;

        private static List<ActionItem> _healingOverTimeActions;

        public static List<ActionItem> DamageOverTimeActions() {
            if (_damageOverTimeActions.Any()) {
                return _damageOverTimeActions;
            }

            return _damageOverTimeActions ??= _actions.Where(kvp => kvp.Value.IsDamageOverTime).Select(kvp => kvp.Value).ToList();
        }

        public static List<ActionItem> HealingOverTimeActions() {
            if (_healingOverTimeActions.Any()) {
                return _healingOverTimeActions;
            }

            return _healingOverTimeActions ??= _actions.Where(kvp => kvp.Value.IsHealingOverTime).Select(kvp => kvp.Value).ToList();
        }

        public static ActionItem GetActionInfo(string name) {
            return _actions.FirstOrDefault(kvp => kvp.Value.Name.Matches(name)).Value ?? DefaultActionInfo;
        }

        public static ActionItem GetActionInfo(uint id) {
            return _actions.ContainsKey(id)
                       ? _actions[id]
                       : DefaultActionInfo;
        }

        internal static async Task Resolve(SharlayanConfiguration configuration) {
            if (_loading) {
                return;
            }

            _loading = true;
            await APIHelper.GetActions(_actions, configuration);
            _loading = false;
        }
    }
}