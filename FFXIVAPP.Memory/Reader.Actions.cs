// FFXIVAPP.Memory ~ Reader.HotBar.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FFXIVAPP.Memory.Core;
using FFXIVAPP.Memory.Core.Enums;
using FFXIVAPP.Memory.Models;
using BitConverter = FFXIVAPP.Memory.Helpers.BitConverter;

namespace FFXIVAPP.Memory
{
    public static partial class Reader
    {
        public static IntPtr HotBarMap { get; set; }
        public static IntPtr RecastMap { get; set; }

        public static ActionReadResult GetActions()
        {
            var result = new ActionReadResult();

            if (!Scanner.Instance.Locations.ContainsKey("HOTBAR") && !Scanner.Instance.Locations.ContainsKey("RECAST"))
            {
                return result;
            }

            try
            {
                HotBarMap = Scanner.Instance.Locations["HOTBAR"];
                RecastMap = Scanner.Instance.Locations["RECAST"];

                result.HotBarEntities = new List<ActionEntity>
                {
                    GetHotBarRecast(HotBarRecast.Container.HOTBAR_1),
                    GetHotBarRecast(HotBarRecast.Container.HOTBAR_2),
                    GetHotBarRecast(HotBarRecast.Container.HOTBAR_3),
                    GetHotBarRecast(HotBarRecast.Container.HOTBAR_4),
                    GetHotBarRecast(HotBarRecast.Container.HOTBAR_5),
                    GetHotBarRecast(HotBarRecast.Container.HOTBAR_6),
                    GetHotBarRecast(HotBarRecast.Container.HOTBAR_7),
                    GetHotBarRecast(HotBarRecast.Container.HOTBAR_8),
                    GetHotBarRecast(HotBarRecast.Container.HOTBAR_9),
                    GetHotBarRecast(HotBarRecast.Container.HOTBAR_10),
                    GetHotBarRecast(HotBarRecast.Container.CROSS_HOTBAR_1),
                    GetHotBarRecast(HotBarRecast.Container.CROSS_HOTBAR_2),
                    GetHotBarRecast(HotBarRecast.Container.CROSS_HOTBAR_3),
                    GetHotBarRecast(HotBarRecast.Container.CROSS_HOTBAR_4),
                    GetHotBarRecast(HotBarRecast.Container.CROSS_HOTBAR_5),
                    GetHotBarRecast(HotBarRecast.Container.CROSS_HOTBAR_6),
                    GetHotBarRecast(HotBarRecast.Container.CROSS_HOTBAR_7),
                    GetHotBarRecast(HotBarRecast.Container.CROSS_HOTBAR_8),
                    GetHotBarRecast(HotBarRecast.Container.PETBAR),
                    GetHotBarRecast(HotBarRecast.Container.CROSS_PETBAR)
                };
            }
            catch (Exception ex)
            {
                MemoryHandler.Instance.RaiseException(Logger, ex, true);
            }

            return result;
        }

        private static ActionEntity GetHotBarRecast(HotBarRecast.Container type)
        {
            var hotbarContainerSize = 0xD00;
            var hotbarContainerAddress = IntPtr.Add(HotBarMap, (int) type * hotbarContainerSize);

            var recastContainerSize = 0x280;
            var recastContainerAddress = IntPtr.Add(HotBarMap, (int)type * recastContainerSize);

            var container = new ActionEntity
            {
                Actions = new List<HotBarRecastItem>(),
                Type = type
            };

            var canUseKeyBinds = false;

            var hotbarItemSize = 0xD0;
            var recastItemSize = 0x28;

            int hotbarLimit;
            int recastLimit;

            switch (type)
            {
                case HotBarRecast.Container.CROSS_HOTBAR_1:
                case HotBarRecast.Container.CROSS_HOTBAR_2:
                case HotBarRecast.Container.CROSS_HOTBAR_3:
                case HotBarRecast.Container.CROSS_HOTBAR_4:
                case HotBarRecast.Container.CROSS_HOTBAR_5:
                case HotBarRecast.Container.CROSS_HOTBAR_6:
                case HotBarRecast.Container.CROSS_HOTBAR_7:
                case HotBarRecast.Container.CROSS_HOTBAR_8:
                case HotBarRecast.Container.CROSS_PETBAR:
                    hotbarLimit = 16 * hotbarItemSize;
                    recastLimit = 16 * recastItemSize;
                    break;
                default:
                    hotbarLimit = 12 * hotbarItemSize;
                    recastLimit = 12 * recastItemSize;
                    canUseKeyBinds = true;
                    break;
            }

            var hotbarSource = MemoryHandler.Instance.GetByteArray(hotbarContainerAddress, hotbarContainerSize);
            var recastSoruce = MemoryHandler.Instance.GetByteArray(recastContainerAddress, recastContainerSize);

            for (var i = 0; i < limit; i += itemSize)
            {
                var itemOffset = IntPtr.Add(containerAddress, i);
                var source = MemoryHandler.Instance.GetByteArray(itemOffset, itemSize);

                var name = MemoryHandler.Instance.GetStringFromBytes(source, MemoryHandler.Instance.Structures.HotBarEntity.Name);
                var slot = i / itemSize;
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }
                var item = new HotBarRecastItem
                {
                    Name = name,
                    ID = BitConverter.TryToInt16(source, MemoryHandler.Instance.Structures.HotBarEntity.ID),
                    KeyBinds = MemoryHandler.Instance.GetStringFromBytes(source, MemoryHandler.Instance.Structures.HotBarEntity.KeyBinds),
                    Slot = slot
                };
                if (canUseKeyBinds)
                {
                    if (!string.IsNullOrWhiteSpace(item.KeyBinds))
                    {
                        item.Name = item.Name.Replace($" {item.KeyBinds}", "");
                        item.KeyBinds = Regex.Replace(item.KeyBinds, @"[\[\]]", "");
                        var buttons = item.KeyBinds.Split(new[]
                                          {
                                              '+'
                                          }, StringSplitOptions.RemoveEmptyEntries)
                                          .ToList();
                        if (buttons.Count <= 0)
                        {
                            continue;
                        }
                        item.ActionKey = buttons.Last();
                        if (buttons.Count <= 1)
                        {
                            continue;
                        }
                        for (var x = 0; x < buttons.Count - 1; x++)
                        {
                            item.Modifiers.Add(buttons[x]);
                        }
                    }
                }
                container.HotBarItems.Add(item);
            }

            return container;
        }
    }
}
