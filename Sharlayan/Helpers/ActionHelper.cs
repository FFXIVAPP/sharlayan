// Sharlayan ~ ActionHelper.cs
// 
// Copyright © 2007 - 2017 Ryan Wilson - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Sharlayan.Models;

namespace Sharlayan.Helpers
{
    public static class ActionHelper
    {
        private static bool Loading;

        private static ActionItem DefaultActionItem = new ActionItem
        {
            Name = new Localization
            {
                Chinese = "???",
                English = "???",
                French = "???",
                German = "???",
                Japanese = "???",
                Korean = "???"
            }
        };

        private static ConcurrentDictionary<uint, ActionItem> _actions;

        private static ConcurrentDictionary<uint, ActionItem> Actions
        {
            get { return _actions ?? (_actions = new ConcurrentDictionary<uint, ActionItem>()); }
            set
            {
                if (_actions == null)
                {
                    _actions = new ConcurrentDictionary<uint, ActionItem>();
                }
                _actions = value;
            }
        }

        public static ActionItem ActionInfo(string name)
        {
            if (Loading)
            {
                return DefaultActionItem;
            }
            lock (Actions)
            {
                if (Actions.Any())
                {
                    return Actions.FirstOrDefault(kvp => kvp.Value.Name.Matches(name))
                                  .Value ?? DefaultActionItem;
                }
                Resolve();
                return DefaultActionItem;
            }
        }

        public static List<ActionItem> HealingOverTimeActions()
        {
            var results = new List<ActionItem>();
            if (Loading)
            {
                return results;
            }
            lock (Actions)
            {
                if (Actions.Any())
                {
                    results.AddRange(Actions.Where(kvp => kvp.Value.IsHealingOverTime)
                                            .Select(kvp => kvp.Value));
                    return results;
                }
                Resolve();
                return results;
            }
        }

        public static List<ActionItem> DamageOverTimeActions()
        {
            var results = new List<ActionItem>();
            if (Loading)
            {
                return results;
            }
            lock (Actions)
            {
                if (Actions.Any())
                {
                    results.AddRange(Actions.Where(kvp => kvp.Value.IsDamageOverTime)
                                            .Select(kvp => kvp.Value));
                    return results;
                }
                Resolve();
                return results;
            }
        }

        public static ActionItem ActionInfo(uint id)
        {
            if (Loading)
            {
                return DefaultActionItem;
            }
            lock (Actions)
            {
                if (Actions.Any())
                {
                    return Actions.ContainsKey(id) ? Actions[id] : DefaultActionItem;
                }
                Resolve();
                return DefaultActionItem;
            }
        }

        internal static void Resolve()
        {
            if (Loading)
            {
                return;
            }
            Loading = true;
            APIHelper.GetActions(Actions);
            Loading = false;
        }
    }
}
