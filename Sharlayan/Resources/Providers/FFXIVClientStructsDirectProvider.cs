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
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Sharlayan.Models;
    using Sharlayan.Models.Structures;
    using Sharlayan.Models.XIVDatabase;
    using Sharlayan.Resources.Mappers;

    using Signatures = Sharlayan.Signatures;
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
            // Sharlayan's Scanner consumes Signature[] and produces one MemoryLocation per Key.
            // We build those Signatures from FFXIVClientStructs' [StaticAddress] metadata via the
            // extractor, then attach a final PointerPath hop to reach the exact field inside the
            // resolved struct that Sharlayan's Reader expects at that scanner key.
            //
            // The (FFXIVClientStructs type → inner offset) table below is hand-curated per scanner
            // key. Innerset is where the field Sharlayan reads lives inside the struct pointed to by
            // FFXIVClientStructs' static-address resolver. Keys not yet mapped fall back to the
            // legacy JSON signatures if UseLocalCache is on and a JSON file sits next to the app,
            // otherwise they're simply missing (Scanner.Locations won't contain them, and the
            // corresponding Reader.CanGet* calls return false).
            List<Signature> signatures = new List<Signature>();
            // Core singleton structs — directly accessible via [StaticAddress] LEA patterns.
            TryAdd(signatures, Signatures.CHARMAP_KEY,      "GameObjectManager", innerOffset: 0x20);     // ObjectArrays._indexSorted (819 × GameObject*)
            TryAdd(signatures, Signatures.PLAYERINFO_KEY,   "PlayerState",       innerOffset: 0);        // PlayerState struct base
            TryAdd(signatures, Signatures.PARTYMAP_KEY,     "GroupManager",      innerOffset: 0x20);     // MainGroup (Group @ +0x20) → _partyMembers[0] @ Group+0x00
            TryAdd(signatures, Signatures.PARTYCOUNT_KEY,   "GroupManager",      innerOffset: 0x7FFC);   // MainGroup.MemberCount @ Group+0x7FDC → GroupManager+0x7FFC
            TryAdd(signatures, Signatures.TARGET_KEY,       "TargetSystem",      innerOffset: 0);        // TargetSystem struct base
            TryAdd(signatures, Signatures.INVENTORY_KEY,    "InventoryManager",  innerOffset: 0x1E08);   // Inventories pointer field — Reader does GetInt64(KEY) to deref
            TryAdd(signatures, Signatures.JOBRESOURCES_KEY, "JobGaugeManager",   innerOffset: 0);        // JobGaugeManager struct base; gauge data union @ +0x08
            TryAdd(signatures, Signatures.RECAST_KEY,       "ActionManager",     innerOffset: 0x184);    // _cooldowns[0] — 80 × RecastDetail × 0x14 bytes
            TryAdd(signatures, Signatures.MAPINFO_KEY,      "GameMain",          innerOffset: 0x4118);   // TransitionTerritoryTypeId (= CurrentTerritory in stable state); +8 gives CurrentMapId @ 0x4120
            TryAdd(signatures, Signatures.ZONEINFO_KEY,     "TerritoryInfo",     innerOffset: 0x14);     // MapIdOverride; Reader uses +0 as currentActiveMapID override
            TryAdd(signatures, Signatures.ENMITYMAP_KEY,    "UIState",           innerOffset: 0x08);     // UIState.Hate._hateInfo[0] (32 × HateInfo × 0x08 bytes)
            TryAdd(signatures, Signatures.ENMITY_COUNT_KEY, "UIState",           innerOffset: 0x108);    // UIState.Hate.HateArrayLength
            TryAdd(signatures, Signatures.AGROMAP_KEY,      "UIState",           innerOffset: 0x110);    // UIState.Hater._haters[0] (32 × HaterInfo × 0x48 bytes)
            TryAdd(signatures, Signatures.AGRO_COUNT_KEY,   "UIState",           innerOffset: 0xA10);    // UIState.Hater.HaterCount
            // Multi-hop: Framework (isPointer=true) → UIModule* @ +0x2B68 → sub-module inside UIModule.
            // ResolvePointerPath: hop0 = ASM follow of Framework static ptr, hop1 = deref to Framework,
            // hop2 = +0x2B68 then deref to UIModule, hop3 = +inner (trailing add, no deref).
            TryAddChain(signatures, Signatures.CHATLOG_KEY, "Framework", 0x2B68, 0x19E0);   // UIModule → RaptureLogModule
            TryAddChain(signatures, Signatures.HOTBAR_KEY,  "Framework", 0x2B68, 0x57B80);  // UIModule → RaptureHotbarModule
            return Task.FromResult(signatures.ToArray());
        }

        private static void TryAdd(List<Signature> list, string sharlayanKey, string fcsTypeName, long innerOffset) {
            if (!FFXIVClientStructsSignatureExtractor.TryGet(fcsTypeName, out var info)) {
                return;
            }
            try {
                list.Add(FFXIVClientStructsSignatureExtractor.BuildSignature(sharlayanKey, info, innerOffset));
            }
            catch {
                // Conversion failures (bad pattern length etc.) should not prevent other keys from
                // resolving — individual missing keys are already handled by Reader.CanGet* guards.
            }
        }

        private static void TryAddChain(List<Signature> list, string sharlayanKey, string fcsTypeName, params long[] offsetChain) {
            if (!FFXIVClientStructsSignatureExtractor.TryGet(fcsTypeName, out var info)) {
                return;
            }
            try {
                list.Add(FFXIVClientStructsSignatureExtractor.BuildSignature(sharlayanKey, info, offsetChain));
            }
            catch {
                // Same graceful degradation as TryAdd.
            }
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
