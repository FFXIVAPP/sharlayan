// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FFXIVClientStructsDirectProvider.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Populates Sharlayan's StructuresContainer and Signature[] using FFXIVClientStructs
//   types directly (referenced as a git submodule + ProjectReference, ILRepacked into
//   Sharlayan.dll). Every inner offset in this file is derived via reflection from the
//   current FCS submodule — no hard-coded field offsets. When FCS bumps, this file
//   inherits the new offsets without edits. xivdatabase methods are delegated to
//   LuminaXivDatabaseProvider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Providers {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FFXIVClientStructs.FFXIV.Client.Game;
    using FFXIVClientStructs.FFXIV.Client.Game.Group;
    using FFXIVClientStructs.FFXIV.Client.Game.Object;
    using FFXIVClientStructs.FFXIV.Client.Game.UI;
    using FFXIVClientStructs.FFXIV.Client.UI;
    using FFXIVClientStructs.FFXIV.Client.UI.Arrays;
    using FFXIVClientStructs.FFXIV.Client.UI.Misc;
    using FFXIVClientStructs.FFXIV.Component.GUI;

    using Sharlayan.Models;
    using Sharlayan.Models.Structures;
    using Sharlayan.Models.XIVDatabase;
    using Sharlayan.Resources.Mappers;

    // FCS has its own `Task` class in Client.System.Framework; alias Framework instead of
    // importing the namespace to avoid colliding with System.Threading.Tasks.Task.
    using Framework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;
    using Signatures = Sharlayan.Signatures;
    using StatusItem = Sharlayan.Models.XIVDatabase.StatusItem;

    internal sealed class FFXIVClientStructsDirectProvider : IResourceProvider {
        private readonly LuminaXivDatabaseProvider _xivDatabase = new LuminaXivDatabaseProvider();

        public Task<StructuresContainer> GetStructuresAsync(SharlayanConfiguration configuration) {
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
            // Every inner offset below is resolved via FieldOffsetReader, which reads
            // [FieldOffset] attributes at runtime. When FCS bumps a field's offset, this
            // file inherits the change for free — no edits, no patch-day diff.
            //
            // For keys whose Reader target is a nested field (e.g. GroupManager.MainGroup
            // → Group.MemberCount), we sum the hop offsets. The integrity test covers
            // renames and layout shifts; a silent move WITHIN a struct still resolves.
            List<Signature> signatures = new List<Signature>();

            // --- Flat struct targets ---------------------------------------------------
            // CHARMAP → GameObjectManager.Objects (ObjectArrays); _indexSorted is at +0 inside.
            TryAdd(signatures, Signatures.CHARMAP_KEY, "GameObjectManager",
                FieldOffsetReader.OffsetOf<GameObjectManager>(nameof(GameObjectManager.Objects)));

            // PLAYERINFO → PlayerState struct base.
            TryAdd(signatures, Signatures.PLAYERINFO_KEY, "PlayerState", 0);

            // PARTYMAP → GroupManager.MainGroup (_partyMembers at Group+0, so offset equals MainGroup's).
            TryAdd(signatures, Signatures.PARTYMAP_KEY, "GroupManager",
                FieldOffsetReader.OffsetOf<GroupManager>(nameof(GroupManager.MainGroup)));

            // PARTYCOUNT → GroupManager.MainGroup + Group.MemberCount.
            TryAdd(signatures, Signatures.PARTYCOUNT_KEY, "GroupManager",
                FieldOffsetReader.OffsetOf<GroupManager>(nameof(GroupManager.MainGroup)) +
                FieldOffsetReader.OffsetOf<GroupManager.Group>(nameof(GroupManager.Group.MemberCount)));

            // TARGET → TargetSystem struct base.
            TryAdd(signatures, Signatures.TARGET_KEY, "TargetSystem", 0);

            // INVENTORY → InventoryManager.Inventories (pointer field Reader deref's via GetInt64).
            TryAdd(signatures, Signatures.INVENTORY_KEY, "InventoryManager",
                FieldOffsetReader.OffsetOf<InventoryManager>(nameof(InventoryManager.Inventories)));

            // JOBRESOURCES → JobGaugeManager struct base; gauge data union is at +0x08 inside.
            TryAdd(signatures, Signatures.JOBRESOURCES_KEY, "JobGaugeManager", 0);

            // MAPINFO → GameMain.TransitionTerritoryTypeId (= CurrentTerritoryTypeId in stable
            // state; CurrentMapId sits at +8 from there, which is what Reader reads at KEY+8).
            TryAdd(signatures, Signatures.MAPINFO_KEY, "GameMain",
                FieldOffsetReader.OffsetOf<GameMain>(nameof(GameMain.TransitionTerritoryTypeId)));

            // ZONEINFO → TerritoryInfo.MapIdOverride.
            TryAdd(signatures, Signatures.ZONEINFO_KEY, "TerritoryInfo",
                FieldOffsetReader.OffsetOf<TerritoryInfo>(nameof(TerritoryInfo.MapIdOverride)));

            // ENMITY/AGRO family: UIState → UIState.Hate._hateInfo / HateArrayLength /
            // Hater._haters / HaterCount. Using reflection avoids both inner-struct and
            // outer-field layout drift.
            int hateOff   = FieldOffsetReader.OffsetOf<UIState>(nameof(UIState.Hate));
            int haterOff  = FieldOffsetReader.OffsetOf<UIState>(nameof(UIState.Hater));
            TryAdd(signatures, Signatures.ENMITYMAP_KEY,    "UIState", hateOff  + FieldOffsetReader.OffsetOf<Hate>("_hateInfo"));
            TryAdd(signatures, Signatures.ENMITY_COUNT_KEY, "UIState", hateOff  + FieldOffsetReader.OffsetOf<Hate>(nameof(Hate.HateArrayLength)));
            TryAdd(signatures, Signatures.AGROMAP_KEY,      "UIState", haterOff + FieldOffsetReader.OffsetOf<Hater>("_haters"));
            TryAdd(signatures, Signatures.AGRO_COUNT_KEY,   "UIState", haterOff + FieldOffsetReader.OffsetOf<Hater>(nameof(Hater.HaterCount)));

            // --- GameState family ------------------------------------------------------
            TryAdd(signatures, Signatures.GAMEMAIN_KEY,       "GameMain",       0);
            TryAdd(signatures, Signatures.CONDITIONS_KEY,     "Conditions",     0);
            TryAdd(signatures, Signatures.CONTENTSFINDER_KEY, "ContentsFinder", 0);
            TryAdd(signatures, Signatures.WEATHER_KEY,        "WeatherManager", 0);
            TryAdd(signatures, Signatures.BGMSYSTEM_KEY,      "BGMSystem",      0);

            // --- Multi-hop chains ------------------------------------------------------
            // CHATLOG: Framework (isPointer=true) → UIModule* @ Framework.UIModule → trailing
            // add to RaptureLogModule within UIModule.
            TryAddChain(signatures, Signatures.CHATLOG_KEY, "Framework",
                FieldOffsetReader.OffsetOf<Framework>(nameof(Framework.UIModule)),        // deref to UIModule*
                FieldOffsetReader.OffsetOf<UIModule>("RaptureLogModule"));                // trailing add

            // HOTBAR: Framework → UIModule* → trailing add to _hotbars[0] inside
            // UIModule.RaptureHotbarModule. The trailing hop is additive (no deref),
            // so the two nested field offsets are summed.
            TryAddChain(signatures, Signatures.HOTBAR_KEY, "Framework",
                FieldOffsetReader.OffsetOf<Framework>(nameof(Framework.UIModule)),        // deref to UIModule*
                FieldOffsetReader.OffsetOf<UIModule>("RaptureHotbarModule") +
                FieldOffsetReader.OffsetOf<RaptureHotbarModule>("_hotbars"));             // trailing add

            // RECAST: AtkStage (isPointer=true) → AtkArrayDataHolder* → NumberArrays** →
            // NumberArrays[ActionBar = 7] → NumberArrayData* → IntArray (int*) → trailing
            // add to ActionBarNumberArray._bars[0]. Each pointer-array index is 8 bytes
            // (size of a pointer), so the ActionBar hop is `(int)ActionBar * 8`.
            TryAddChain(signatures, Signatures.RECAST_KEY, "AtkStage",
                FieldOffsetReader.OffsetOf<AtkStage>(nameof(AtkStage.AtkArrayDataHolder)),         // deref
                FieldOffsetReader.OffsetOf<AtkArrayDataHolder>(nameof(AtkArrayDataHolder.NumberArrays)), // deref
                (int)NumberArrayType.ActionBar * sizeof(long),                                     // deref NumberArrays[7]
                FieldOffsetReader.OffsetOf<NumberArrayData>(nameof(NumberArrayData.IntArray)),     // deref IntArray*
                FieldOffsetReader.OffsetOf<ActionBarNumberArray>("_bars"));                        // trailing add

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
