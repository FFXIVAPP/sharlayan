﻿// Sharlayan ~ Reader.Actions.cs
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
using Sharlayan.Core;
using Sharlayan.Core.Enums;
using Sharlayan.Models;
using BitConverter = Sharlayan.Helpers.BitConverter;

namespace Sharlayan
{
    public static partial class Reader
    {
        public static bool CanGetActions()
        {
            var canRead = Scanner.Instance.Locations.ContainsKey(Signatures.HotBarKey) && Scanner.Instance.Locations.ContainsKey(Signatures.RecastKey);
            if (canRead)
            {
                // OTHER STUFF
            }
            return canRead;
        }

        public static ActionReadResult GetActions()
        {
            var result = new ActionReadResult();

            if (!CanGetActions() || !MemoryHandler.Instance.IsAttached)
            {
                return result;
            }

            try
            {
                result.ActionEntities = new List<ActionEntity>
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
            var HotBarMap = Scanner.Instance.Locations[Signatures.HotBarKey];
            var RecastMap = Scanner.Instance.Locations[Signatures.RecastKey];

            var hotbarContainerSize = 0xD00;
            var hotbarContainerAddress = IntPtr.Add(HotBarMap, (int) type * hotbarContainerSize);

            var recastContainerSize = 0x280;
            var recastContainerAddress = IntPtr.Add(RecastMap, (int) type * recastContainerSize);

            var container = new ActionEntity
            {
                Actions = new List<HotBarRecastItem>(),
                Type = type
            };

            var canUseKeyBinds = false;

            var hotbarItemSize = 0xD0;
            var recastItemSize = 0x28;

            int limit;

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
                    limit = 16;
                    break;
                default:
                    limit = 12;
                    canUseKeyBinds = true;
                    break;
            }

            var hotbarItemsSource = MemoryHandler.Instance.GetByteArray(hotbarContainerAddress, hotbarContainerSize);
            var recastItemsSource = MemoryHandler.Instance.GetByteArray(recastContainerAddress, recastContainerSize);
            var hotbarItemSource = new byte[hotbarItemSize];
            var recastItemSource = new byte[recastItemSize];

            for (var i = 0; i < limit; i++)
            {
                Buffer.BlockCopy(hotbarItemsSource, i * hotbarItemSize, hotbarItemSource, 0, hotbarItemSize);
                Buffer.BlockCopy(recastItemsSource, i * recastItemSize, recastItemSource, 0, recastItemSize);

                var name = MemoryHandler.Instance.GetStringFromBytes(hotbarItemSource, MemoryHandler.Instance.Structures.HotBarEntity.Name);
                var slot = i;

                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }
                var item = new HotBarRecastItem
                {
                    Name = name,
                    ID = BitConverter.TryToInt16(hotbarItemSource, MemoryHandler.Instance.Structures.HotBarEntity.ID),
                    KeyBinds = MemoryHandler.Instance.GetStringFromBytes(hotbarItemSource, MemoryHandler.Instance.Structures.HotBarEntity.KeyBinds),
                    Slot = slot
                };

                #region KeyBind Resolution

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
                        if (buttons.Count > 0)
                        {
                            item.ActionKey = buttons.Last();
                        }
                        if (buttons.Count > 1)
                        {
                            for (var x = 0; x < buttons.Count - 1; x++)
                            {
                                item.Modifiers.Add(buttons[x]);
                            }
                        }
                    }
                }

                #endregion

                #region Recast Information

                item.Category = BitConverter.TryToInt32(recastItemSource, MemoryHandler.Instance.Structures.RecastEntity.Category);
                item.Type = BitConverter.TryToInt32(recastItemSource, MemoryHandler.Instance.Structures.RecastEntity.Type);
                item.Icon = BitConverter.TryToInt32(recastItemSource, MemoryHandler.Instance.Structures.RecastEntity.Icon);
                item.CoolDownPercent = recastItemSource[MemoryHandler.Instance.Structures.RecastEntity.CoolDownPercent];
                item.IsAvailable = BitConverter.TryToBoolean(recastItemSource, MemoryHandler.Instance.Structures.RecastEntity.IsAvailable);

                var remainingCost = BitConverter.TryToInt32(recastItemSource, MemoryHandler.Instance.Structures.RecastEntity.RemainingCost);

                item.RemainingCost = remainingCost != -1 ? remainingCost : 0;
                item.Amount = BitConverter.TryToInt32(recastItemSource, MemoryHandler.Instance.Structures.RecastEntity.Amount);
                item.InRange = BitConverter.TryToBoolean(recastItemSource, MemoryHandler.Instance.Structures.RecastEntity.InRange);
                item.IsProcOrCombo = recastItemSource[MemoryHandler.Instance.Structures.RecastEntity.ActionProc] > 0;

                #endregion

                container.Actions.Add(item);
            }

            return container;
        }
    }
}
