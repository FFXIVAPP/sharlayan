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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Sharlayan.Core;
    using Sharlayan.Models.ReadResults;
    using Sharlayan.Utilities;

    using Action = Sharlayan.Core.Enums.Action;

    public partial class Reader {
        private readonly Regex KeyBindsRegex = new Regex(@"[\[\]]", RegexOptions.Compiled);

        private byte[] _hotbarItemsMap;

        private byte[] _hotbarMap;

        private byte[] _recastItemsMap;

        private byte[] _recastMap;

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
                this._memoryHandler.RaiseException(ex);
            }

            return result;
        }

        private ActionContainer GetHotBarRecast(Action.Container type) {
            MemoryLocation HotBarMap = this._memoryHandler.Scanner.Locations[Signatures.HotBarKey];
            MemoryLocation RecastMap = this._memoryHandler.Scanner.Locations[Signatures.RecastKey];

            int hotbarContainerSize = this._memoryHandler.Structures.HotBarItem.ContainerSize;
            IntPtr hotbarContainerAddress = IntPtr.Add(HotBarMap, (int) type * hotbarContainerSize);
            if (this._hotbarItemsMap == null) {
                this._hotbarItemsMap = new byte[hotbarContainerSize];
            }

            int recastContainerSize = this._memoryHandler.Structures.RecastItem.ContainerSize;
            IntPtr recastContainerAddress = IntPtr.Add(RecastMap, (int) type * recastContainerSize);
            if (this._recastItemsMap == null) {
                this._recastItemsMap = new byte[recastContainerSize];
            }

            ActionContainer container = new ActionContainer {
                ContainerType = type,
            };

            bool canUseKeyBinds = false;

            int hotbarItemSize = this._memoryHandler.Structures.HotBarItem.ItemSize;
            if (this._hotbarMap == null) {
                this._hotbarMap = new byte[hotbarItemSize];
            }

            int recastItemSize = this._memoryHandler.Structures.RecastItem.ItemSize;
            if (this._recastMap == null) {
                this._recastMap = new byte[recastItemSize];
            }

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

            this._memoryHandler.GetByteArray(hotbarContainerAddress, this._hotbarItemsMap);
            this._memoryHandler.GetByteArray(recastContainerAddress, this._recastItemsMap);

            for (int i = 0; i < limit; i++) {
                Buffer.BlockCopy(this._hotbarItemsMap, i * hotbarItemSize, this._hotbarMap, 0, hotbarItemSize);
                Buffer.BlockCopy(this._recastItemsMap, i * recastItemSize, this._recastMap, 0, recastItemSize);

                string name = this._memoryHandler.GetStringFromBytes(this._hotbarMap, this._memoryHandler.Structures.HotBarItem.Name);
                int slot = i;

                if (string.IsNullOrWhiteSpace(name)) {
                    continue;
                }

                ActionItem item = new ActionItem {
                    Name = name,
                    ID = SharlayanBitConverter.TryToInt16(this._hotbarMap, this._memoryHandler.Structures.HotBarItem.ID),
                    KeyBinds = this._memoryHandler.GetStringFromBytes(this._hotbarMap, this._memoryHandler.Structures.HotBarItem.KeyBinds),
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

                item.Category = SharlayanBitConverter.TryToInt32(this._recastMap, this._memoryHandler.Structures.RecastItem.Category);
                item.Type = SharlayanBitConverter.TryToInt32(this._recastMap, this._memoryHandler.Structures.RecastItem.Type);
                item.Icon = SharlayanBitConverter.TryToInt32(this._recastMap, this._memoryHandler.Structures.RecastItem.Icon);
                item.CoolDownPercent = this._recastMap[this._memoryHandler.Structures.RecastItem.CoolDownPercent];
                item.IsAvailable = SharlayanBitConverter.TryToBoolean(this._recastMap, this._memoryHandler.Structures.RecastItem.IsAvailable);

                int remainingCost = SharlayanBitConverter.TryToInt32(this._recastMap, this._memoryHandler.Structures.RecastItem.RemainingCost);

                item.RemainingCost = remainingCost != -1
                                         ? remainingCost
                                         : 0;
                item.Amount = SharlayanBitConverter.TryToInt32(this._recastMap, this._memoryHandler.Structures.RecastItem.Amount);
                item.InRange = SharlayanBitConverter.TryToBoolean(this._recastMap, this._memoryHandler.Structures.RecastItem.InRange);
                item.IsProcOrCombo = this._recastMap[this._memoryHandler.Structures.RecastItem.ActionProc] > 0;

                container.ActionItems.Add(item);
            }

            return container;
        }
    }
}