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
            TryAdd(signatures, Signatures.CHARMAP_KEY,    "GameObjectManager", innerOffset: 0x20);     // ObjectArrays._indexSorted — 819 x GameObject*
            TryAdd(signatures, Signatures.PLAYERINFO_KEY, "PlayerState",       innerOffset: 0);        // PlayerState struct base
            TryAdd(signatures, Signatures.PARTYMAP_KEY,   "GroupManager",      innerOffset: 0x20);     // MainGroup._partyMembers[0]
            TryAdd(signatures, Signatures.PARTYCOUNT_KEY, "GroupManager",      innerOffset: 0x7FFC);   // MainGroup.MemberCount (MainGroup @ 0x20 + MemberCount @ 0x7FDC)
            TryAdd(signatures, Signatures.TARGET_KEY,     "TargetSystem",      innerOffset: 0);        // TargetSystem struct base
            // TODO(P3-B9 follow-up): CHATLOG / HOTBAR go through Framework.Instance()->GetUIModule()
            // which is a method call chain (not expressible in Sharlayan's PointerPath model).
            // Needs a different resolver path. Tracked in refactor notes.
            // TODO(P3-B9 follow-up): INVENTORY / RECAST / MAPINFO / ZONEINFO / ENMITYMAP / ENMITY_COUNT /
            // AGROMAP / AGRO_COUNT / COOLDOWNS / JOBRESOURCES — need confirmed inner-offset math against
            // live comparisons with legacy scanner addresses.
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
