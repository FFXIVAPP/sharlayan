// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Actions.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.Actions.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Sharlayan.Core;
    using Sharlayan.Models.ReadResults;

    using Action = Sharlayan.Core.Enums.Action;
    using BitConverter = Sharlayan.Utilities.BitConverter;

    public partial class Reader {
        private readonly Regex KeyBindsRegex = new Regex(@"[\[\]]", RegexOptions.Compiled);

        public bool CanGetActions() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.HotBarKey) && this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.RecastKey);
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
                this._memoryHandler.RaiseException(Logger, ex, true);
            }

            return result;
        }

        private ActionContainer GetHotBarRecast(Action.Container type) {
            MemoryLocation HotBarMap = this._memoryHandler.Scanner.Locations[Signatures.HotBarKey];
            MemoryLocation RecastMap = this._memoryHandler.Scanner.Locations[Signatures.RecastKey];

            int hotbarContainerSize = this._memoryHandler.Structures.HotBarItem.ContainerSize;
            IntPtr hotbarContainerAddress = IntPtr.Add(HotBarMap, (int) type * hotbarContainerSize);

            int recastContainerSize = this._memoryHandler.Structures.RecastItem.ContainerSize;
            IntPtr recastContainerAddress = IntPtr.Add(RecastMap, (int) type * recastContainerSize);

            ActionContainer container = new ActionContainer {
                ContainerType = type,
            };

            bool canUseKeyBinds = false;

            int hotbarItemSize = this._memoryHandler.Structures.HotBarItem.ItemSize;
            int recastItemSize = this._memoryHandler.Structures.RecastItem.ItemSize;

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

            byte[] hotbarItemsSource = this._memoryHandler.GetByteArray(hotbarContainerAddress, hotbarContainerSize);
            byte[] recastItemsSource = this._memoryHandler.GetByteArray(recastContainerAddress, recastContainerSize);

            for (int i = 0; i < limit; i++) {
                byte[] hotbarSource = new byte[hotbarItemSize];
                byte[] recastSource = new byte[recastItemSize];

                Buffer.BlockCopy(hotbarItemsSource, i * hotbarItemSize, hotbarSource, 0, hotbarItemSize);
                Buffer.BlockCopy(recastItemsSource, i * recastItemSize, recastSource, 0, recastItemSize);

                string name = this._memoryHandler.GetStringFromBytes(hotbarSource, this._memoryHandler.Structures.HotBarItem.Name);
                int slot = i;

                if (string.IsNullOrWhiteSpace(name)) {
                    continue;
                }

                ActionItem item = new ActionItem {
                    Name = name,
                    ID = BitConverter.TryToInt16(hotbarSource, this._memoryHandler.Structures.HotBarItem.ID),
                    KeyBinds = this._memoryHandler.GetStringFromBytes(hotbarSource, this._memoryHandler.Structures.HotBarItem.KeyBinds),
                    Slot = slot,
                };

                if (canUseKeyBinds) {
                    if (!string.IsNullOrWhiteSpace(item.KeyBinds)) {
                        item.Name = item.Name.Replace($" {item.KeyBinds}", string.Empty);
                        item.KeyBinds = this.KeyBindsRegex.Replace(item.KeyBinds, string.Empty);
                        List<string> buttons = item.KeyBinds.Split(
                            new[] {
                                '+',
                            }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (buttons.Count > 0) {
                            item.ActionKey = buttons.Last();
                        }

                        if (buttons.Count > 1) {
                            for (int x = 0; x < buttons.Count - 1; x++) {
                                item.Modifiers.Add(buttons[x]);
                            }
                        }
                    }
                }

                item.Category = BitConverter.TryToInt32(recastSource, this._memoryHandler.Structures.RecastItem.Category);
                item.Type = BitConverter.TryToInt32(recastSource, this._memoryHandler.Structures.RecastItem.Type);
                item.Icon = BitConverter.TryToInt32(recastSource, this._memoryHandler.Structures.RecastItem.Icon);
                item.CoolDownPercent = recastSource[this._memoryHandler.Structures.RecastItem.CoolDownPercent];
                item.IsAvailable = BitConverter.TryToBoolean(recastSource, this._memoryHandler.Structures.RecastItem.IsAvailable);

                int remainingCost = BitConverter.TryToInt32(recastSource, this._memoryHandler.Structures.RecastItem.RemainingCost);

                item.RemainingCost = remainingCost != -1
                                         ? remainingCost
                                         : 0;
                item.Amount = BitConverter.TryToInt32(recastSource, this._memoryHandler.Structures.RecastItem.Amount);
                item.InRange = BitConverter.TryToBoolean(recastSource, this._memoryHandler.Structures.RecastItem.InRange);
                item.IsProcOrCombo = recastSource[this._memoryHandler.Structures.RecastItem.ActionProc] > 0;

                container.ActionItems.Add(item);
            }

            return container;
        }
    }
}