// FFXIVAPP.Memory ~ StatusEffectHelper.cs
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
using System.Linq;
using FFXIVAPP.Memory.Models;

namespace FFXIVAPP.Memory.Helpers
{
    public static class StatusEffectHelper
    {
        private static bool Loading;

        private static StatusItem DefaultStatusItem = new StatusItem
        {
            Name = new Localization
            {
                Chinese = "???",
                English = "???",
                French = "???",
                German = "???",
                Japanese = "???",
                Korean = "???"
            },
            CompanyAction = false
        };

        private static ConcurrentDictionary<uint, StatusItem> _statusEffects;

        private static ConcurrentDictionary<uint, StatusItem> StatusEffects
        {
            get { return _statusEffects ?? (_statusEffects = new ConcurrentDictionary<uint, StatusItem>()); }
            set
            {
                if (_statusEffects == null)
                {
                    _statusEffects = new ConcurrentDictionary<uint, StatusItem>();
                }
                _statusEffects = value;
            }
        }

        public static StatusItem StatusInfo(uint id)
        {
            if (Loading)
            {
                return DefaultStatusItem;
            }
            lock (StatusEffects)
            {
                if (StatusEffects.Any())
                {
                    return StatusEffects.ContainsKey(id) ? StatusEffects[id] : DefaultStatusItem;
                }
                Generate();
                return DefaultStatusItem;
            }
        }

        private static void Generate()
        {
            if (Loading)
            {
                return;
            }
            Loading = true;
            APIHelper.GetStatusEffects(StatusEffects);
            Loading = false;
        }
    }
}
