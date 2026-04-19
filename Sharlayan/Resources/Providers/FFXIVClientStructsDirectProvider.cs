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
    using System.Threading.Tasks;

    using Sharlayan.Models;
    using Sharlayan.Models.Structures;
    using Sharlayan.Models.XIVDatabase;
    using Sharlayan.Resources.Mappers;

    using StatusItem = Sharlayan.Models.XIVDatabase.StatusItem;

    internal sealed class FFXIVClientStructsDirectProvider : IResourceProvider {
        private readonly LuminaXivDatabaseProvider _xivDatabase = new LuminaXivDatabaseProvider();

        public Task<StructuresContainer> GetStructuresAsync(SharlayanConfiguration configuration) {
            // P3-B7: per-struct mappers are being added one at a time. Each mapper below
            // corresponds to a class in Sharlayan/Models/Structures and populates its
            // int-offset fields from FFXIVClientStructs struct layouts via
            // Marshal.OffsetOf / FieldOffsetReader. Unmapped Sharlayan fields default to 0.
            return Task.FromResult(new StructuresContainer {
                ActorItem = ActorItemMapper.Build(),
                PartyMember = PartyMemberMapper.Build(),
                StatusItem = StatusItemMapper.Build(),
                InventoryItem = InventoryItemMapper.Build(),
                InventoryContainer = InventoryContainerMapper.Build(),
                EnmityItem = EnmityItemMapper.Build(),
                TargetInfo = TargetInfoMapper.Build(),
                HotBarItem = HotBarItemMapper.Build(),
                RecastItem = RecastItemMapper.Build(),
                ChatLogPointers = ChatLogPointersMapper.Build(),
                PlayerInfo = PlayerInfoMapper.Build(),
                JobResources = JobResourcesMapper.Build(),
            });
        }

        public Task<Signature[]> GetSignaturesAsync(SharlayanConfiguration configuration) {
            // Signatures via FFXIVClientStructs are deferred: its InteropGenerator.Runtime.Resolver
            // expects in-process memory and a full .text section copy. Sharlayan is out-of-process.
            // A faithful port needs to:
            //   1. ReadProcessMemory the game's .text section into a pinned local buffer.
            //   2. Walk each relevant FFXIVClientStructs type's Instance() / [StaticAddress]
            //      attribute for its scan signature + RIP-relative offsets.
            //   3. Match the pattern in the local buffer, compute the corresponding in-game
            //      address (gameBase + offset), and post-apply any Sharlayan-side field offset
            //      (e.g. CHARMAP = CharacterManager.Instance() + offset_of(_battleCharas)).
            //
            // Until that lands, consumers who opt into FFXIVClientStructsDirect get an empty
            // Signature[] — Scanner.Locations stays empty and Reader methods that depend on
            // signatures return empty results rather than crashing. Consumers that need
            // signatures should stay on LegacySharlayanResources (still the default).
            return Task.FromResult(Array.Empty<Signature>());
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
