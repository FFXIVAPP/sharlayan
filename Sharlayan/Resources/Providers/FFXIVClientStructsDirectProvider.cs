// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FFXIVClientStructsDirectProvider.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Populates Sharlayan's StructuresContainer and Signature[] using FFXIVClientStructs
//   types directly (referenced as a git submodule + ProjectReference, ILRepacked into
//   Sharlayan.dll). Field offsets come from [FieldOffset] attributes on generated types;
//   signatures resolve via InteropGenerator.Runtime.Resolver against the running game.
//   xivdatabase methods are delegated to LuminaXivDatabaseProvider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Providers {
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    using FFXIVClientStructs.FFXIV.Client.Game.Character;

    using Sharlayan.Models;
    using Sharlayan.Models.Structures;
    using Sharlayan.Models.XIVDatabase;

    using StatusItem = Sharlayan.Models.XIVDatabase.StatusItem;

    internal sealed class FFXIVClientStructsDirectProvider : IResourceProvider {
        private readonly LuminaXivDatabaseProvider _xivDatabase = new LuminaXivDatabaseProvider();

        public Task<StructuresContainer> GetStructuresAsync(SharlayanConfiguration configuration) {
            // SMOKE TEST — proves FFXIVClientStructs types are reachable and Marshal.OffsetOf works
            // against their [StructLayout(LayoutKind.Explicit)] + [FieldOffset] shape. Real per-struct
            // mappers land in P3-B7.
            StructuresContainer container = new StructuresContainer();
            container.ActorItem ??= new ActorItem();
            container.ActorItem.HPCurrent = (int)Marshal.OffsetOf<CharacterData>(nameof(CharacterData.Health));
            container.ActorItem.HPMax = (int)Marshal.OffsetOf<CharacterData>(nameof(CharacterData.MaxHealth));
            container.ActorItem.MPCurrent = (int)Marshal.OffsetOf<CharacterData>(nameof(CharacterData.Mana));
            return Task.FromResult(container);
        }

        public Task<Signature[]> GetSignaturesAsync(SharlayanConfiguration configuration) {
            throw new NotImplementedException("FFXIVClientStructsDirectProvider.GetSignaturesAsync is pending implementation (P3-B9).");
        }

        public Task GetActionsAsync(ConcurrentDictionary<uint, ActionItem> actions, SharlayanConfiguration configuration) {
            return this._xivDatabase.GetActionsAsync(actions, configuration);
        }

        public Task GetStatusEffectsAsync(ConcurrentDictionary<uint, StatusItem> statusEffects, SharlayanConfiguration configuration) {
            return this._xivDatabase.GetStatusEffectsAsync(statusEffects, configuration);
        }

        public Task GetZonesAsync(ConcurrentDictionary<uint, MapItem> zones, SharlayanConfiguration configuration) {
            return this._xivDatabase.GetZonesAsync(zones, configuration);
        }
    }
}
