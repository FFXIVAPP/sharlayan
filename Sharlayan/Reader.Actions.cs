// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Actions.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.Actions.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Sharlayan.Core;
    using Sharlayan.Models.ReadResults;
    using Sharlayan.Utilities;

    using Action = Sharlayan.Core.Enums.Action;

    public partial class Reader {
        private readonly ConcurrentDictionary<string, (string, string, string, List<string>)> _hotbarActionCache = new ConcurrentDictionary<string, (string, string, string, List<string>)>();

        private readonly Regex KeyBindsRegex = new Regex(@"[\[\]]", RegexOptions.Compiled);

        public bool CanGetActions() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.HOTBAR_KEY) && this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.RECAST_KEY);
            if (canRead) {
                // OTHER STUFF
            }

            return canRead;
        }

        public ActionResult GetActions() {
            ActionResult result = new ActionResult();

            if (!this.CanGetActions() || !this._memoryHandler.IsAttached) {
                return result;
            }

            try {
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.HOTBAR_1));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.HOTBAR_2));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.HOTBAR_3));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.HOTBAR_4));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.HOTBAR_5));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.HOTBAR_6));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.HOTBAR_7));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.HOTBAR_8));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.HOTBAR_9));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.HOTBAR_10));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.CROSS_HOTBAR_1));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.CROSS_HOTBAR_2));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.CROSS_HOTBAR_3));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.CROSS_HOTBAR_4));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.CROSS_HOTBAR_5));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.CROSS_HOTBAR_6));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.CROSS_HOTBAR_7));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.CROSS_HOTBAR_8));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.PETBAR));
                result.ActionContainers.Add(this.GetHotBarRecast(Action.Container.CROSS_PETBAR));
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }

            return result;
        }

        private ActionContainer GetHotBarRecast(Action.Container type) {
            bool canUseKeyBinds = false;

            ActionContainer container = new ActionContainer {
                ContainerType = type,
            };

            IntPtr hotbarAddress = this._memoryHandler.Scanner.Locations[Signatures.HOTBAR_KEY];
            IntPtr recastAddress = this._memoryHandler.Scanner.Locations[Signatures.RECAST_KEY];

            int hotbarContainerSize = this._memoryHandler.Structures.HotBarItem.ContainerSize;
            int recastContainerSize = this._memoryHandler.Structures.RecastItem.ContainerSize;

            IntPtr hotbarContainerAddress = IntPtr.Add(hotbarAddress, (int) type * hotbarContainerSize);
            IntPtr recastContainerAddress = IntPtr.Add(recastAddress, (int) type * recastContainerSize);

            int hotbarItemSize = this._memoryHandler.Structures.HotBarItem.ItemSize;
            int recastItemSize = this._memoryHandler.Structures.RecastItem.ItemSize;

            byte[] hotbarItemsMap = this._memoryHandler.BufferPool.Rent(hotbarContainerSize);
            byte[] recastItemsMap = this._memoryHandler.BufferPool.Rent(recastContainerSize);

            byte[] hotbarMap = this._memoryHandler.BufferPool.Rent(hotbarItemSize);
            byte[] recastMap = this._memoryHandler.BufferPool.Rent(recastItemSize);

            int limit;

            switch (type) {
                case Action.Container.CROSS_HOTBAR_1:
                case Action.Container.CROSS_HOTBAR_2:
                case Action.Container.CROSS_HOTBAR_3:
                case Action.Container.CROSS_HOTBAR_4:
                case Action.Container.CROSS_HOTBAR_5:
                case Action.Container.CROSS_HOTBAR_6:
                case Action.Container.CROSS_HOTBAR_7:
                case Action.Container.CROSS_HOTBAR_8:
                case Action.Container.CROSS_PETBAR:
                    limit = 16;
                    break;
                default:
                    limit = 16;
                    canUseKeyBinds = true;
                    break;
            }

            try {
                this._memoryHandler.GetByteArray(hotbarContainerAddress, hotbarItemsMap);
                this._memoryHandler.GetByteArray(recastContainerAddress, recastItemsMap);

                for (int i = 0; i < limit; i++) {
                    Buffer.BlockCopy(hotbarItemsMap, i * hotbarItemSize, hotbarMap, 0, hotbarItemSize);
                    Buffer.BlockCopy(recastItemsMap, i * recastItemSize, recastMap, 0, recastItemSize);

                    string name = this._memoryHandler.GetStringFromBytes(hotbarMap, this._memoryHandler.Structures.HotBarItem.Name);
                    int slot = i;

                    if (string.IsNullOrWhiteSpace(name)) {
                        continue;
                    }

                    ActionItem item = new ActionItem {
                        Name = name,
                        ID = SharlayanBitConverter.TryToInt16(hotbarMap, this._memoryHandler.Structures.HotBarItem.ID),
                        KeyBinds = this._memoryHandler.GetStringFromBytes(hotbarMap, this._memoryHandler.Structures.HotBarItem.KeyBinds),
                        Slot = slot,
                    };

                    if (canUseKeyBinds) {
                        if (!string.IsNullOrWhiteSpace(item.KeyBinds)) {
                            // keep a cache based on the keybinds key, users will typically not change this so there isn't a concern about going through and handling those changes
                            if (this._hotbarActionCache.TryGetValue(item.KeyBinds, out (string itemName, string itemKeyBinds, string itemActionKey, List<string> itemModifiers) cached)) {
                                item.Name = cached.itemName;
                                item.KeyBinds = cached.itemKeyBinds;
                                item.ActionKey = cached.itemActionKey;
                                item.Modifiers.AddRange(cached.itemModifiers);
                            }
                            else {
                                string key = item.KeyBinds;
                                string itemName = item.Name.Replace($" {item.KeyBinds}", string.Empty);
                                string itemKeyBinds = this.KeyBindsRegex.Replace(item.KeyBinds, string.Empty);
                                string itemActionKey = string.Empty;

                                List<string> itemModifiers = new List<string>();

                                item.Name = itemName;
                                item.KeyBinds = itemKeyBinds;

                                string[] buttons = item.KeyBinds.Split(
                                    new[] {
                                        '+',
                                    }, StringSplitOptions.RemoveEmptyEntries);
                                if (buttons.Any()) {
                                    item.ActionKey = itemActionKey = buttons[buttons.Length - 1];
                                }

                                if (buttons.Length > 1) {
                                    for (int x = 0; x < buttons.Length - 1; x++) {
                                        itemModifiers.Add(buttons[x]);
                                    }

                                    item.Modifiers.AddRange(itemModifiers);
                                }

                                this._hotbarActionCache.TryAdd(key, (itemName, itemKeyBinds, itemActionKey, itemModifiers));
                            }
                        }
                    }

                    item.Category = SharlayanBitConverter.TryToInt32(recastMap, this._memoryHandler.Structures.RecastItem.Category);
                    item.Type = SharlayanBitConverter.TryToInt32(recastMap, this._memoryHandler.Structures.RecastItem.Type);
                    item.Icon = SharlayanBitConverter.TryToInt32(recastMap, this._memoryHandler.Structures.RecastItem.Icon);
                    item.CoolDownPercent = recastMap[this._memoryHandler.Structures.RecastItem.CoolDownPercent];
                    item.IsAvailable = SharlayanBitConverter.TryToBoolean(recastMap, this._memoryHandler.Structures.RecastItem.IsAvailable);

                    int remainingCost = SharlayanBitConverter.TryToInt32(recastMap, this._memoryHandler.Structures.RecastItem.RemainingCost);

                    item.RemainingCost = remainingCost != -1
                                             ? remainingCost
                                             : 0;
                    item.Amount = SharlayanBitConverter.TryToInt32(recastMap, this._memoryHandler.Structures.RecastItem.Amount);
                    item.InRange = SharlayanBitConverter.TryToBoolean(recastMap, this._memoryHandler.Structures.RecastItem.InRange);
                    item.IsProcOrCombo = recastMap[this._memoryHandler.Structures.RecastItem.ActionProc] > 0;

                    container.ActionItems.Add(item);
                }
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }
            finally {
                this._memoryHandler.BufferPool.Return(hotbarItemsMap);
                this._memoryHandler.BufferPool.Return(hotbarMap);
                this._memoryHandler.BufferPool.Return(recastItemsMap);
                this._memoryHandler.BufferPool.Return(recastMap);
            }

            return container;
        }
    }
}