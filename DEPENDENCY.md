# FFXIVClientStructs dependency surface

Sharlayan 9 vendors [FFXIVClientStructs](https://github.com/aers/FFXIVClientStructs) as a git submodule at `Sharlayan/FFXIVClientStructs/`. This file catalogues exactly which FCS files Sharlayan reaches into, what we read from each, and the failure mode when an upstream change breaks the contract.

**Keep this file in sync** with `Sharlayan/Resources/Mappers/*`, `Sharlayan/Resources/Providers/FFXIVClientStructsDirectProvider.cs`, and `Sharlayan/Reader.GameState.cs`. A companion test at `Sharlayan.Tests/Resources/Providers/FCSDependencyIntegrityTests.cs` asserts at compile/run time that every binding in this file still resolves against the current submodule — run it after every FCS submodule bump.

All paths below are relative to `Sharlayan/FFXIVClientStructs/FFXIVClientStructs/`.

---

## Core singletons (`[StaticAddress]` scanned at runtime)

Each of these has a `public static partial Instance()` method decorated with `[StaticAddress(pattern, relativeFollowOffset, isPointer)]`. `FFXIVClientStructsSignatureExtractor` reflects over the attribute and converts the triple into a Sharlayan `Signature` at provider startup. If the attribute is removed or the type is renamed, the corresponding scanner key silently stops resolving.

| File | Type | Sharlayan scanner key(s) | Read at |
|---|---|---|---|
| `FFXIV/Client/Game/Object/GameObjectManager.cs` | `GameObjectManager` | `CHARMAP` | `_indexSorted` @ +0x20 (819-slot `Pointer<GameObject>` array) |
| `FFXIV/Client/Game/UI/PlayerState.cs` | `PlayerState` | `PLAYERINFO` | struct base (+0); per-job levels & EXP, Base{Str,Dex,Vit,Int,Mnd,Pie} |
| `FFXIV/Client/Game/Group/GroupManager.cs` | `GroupManager` | `PARTYMAP` (+0x20), `PARTYCOUNT` (+0x7FFC) | `MainGroup._partyMembers[0]` / `MainGroup.MemberCount` |
| `FFXIV/Client/Game/Control/TargetSystem.cs` | `TargetSystem` | `TARGET` | `Target` / `MouseOverTarget` / `FocusTarget` / `PreviousTarget` / `TargetObjectId` |
| `FFXIV/Client/Game/InventoryManager.cs` | `InventoryManager` | `INVENTORY` | `Inventories` pointer @ +0x1E08 |
| `FFXIV/Client/Game/JobGaugeManager.cs` | `JobGaugeManager` | `JOBRESOURCES` | struct base (+0); 21 gauges overlay at +0x08 |
| `FFXIV/Client/Game/GameMain.cs` | `GameMain` | `GAMEMAIN` (+0), `MAPINFO` (+0x4118) | `TerritoryLoadState` @ +0x4100; `TransitionTerritoryTypeId` / `CurrentMapId` |
| `FFXIV/Client/Game/UI/TerritoryInfo.cs` | `TerritoryInfo` | `ZONEINFO` | `MapIdOverride` @ +0x14 |
| `FFXIV/Client/Game/UI/UIState.cs` | `UIState` | `ENMITYMAP` (+0x08), `ENMITY_COUNT` (+0x108), `AGROMAP` (+0x110), `AGRO_COUNT` (+0xA10) | `Hate._hateInfo` / `Hate.HateArrayLength` / `Hater._haters` / `Hater.HaterCount` |
| `FFXIV/Client/Game/Conditions.cs` | `Conditions` | `CONDITIONS` | bool array; bits 35 / 58 / 78 for cutscene, 34 for BoundByDuty |
| `FFXIV/Client/Game/UI/ContentsFinder.cs` | `ContentsFinder` | `CONTENTSFINDER` | `QueueInfo.QueueState` @ +0x75 |
| `FFXIV/Client/Game/WeatherManager.cs` | `WeatherManager` | `WEATHER` | `WeatherId` @ +0x64 |
| `FFXIV/Client/Game/BGMSystem.cs` | `BGMSystem` | `BGMSYSTEM` | `Scenes` StdVector @ +0xC0 walked for highest-priority PlayingBgmId; `isPointer=true` |
| `FFXIV/Client/System/Framework/Framework.cs` | `Framework` | `CHATLOG`, `HOTBAR` | Multi-hop base for UIModule chain (see below); `isPointer=true` |
| `FFXIV/Component/GUI/AtkStage.cs` | `AtkStage` | `RECAST` | Multi-hop base for NumberArray chain (see below); `isPointer=true` |

---

## Multi-hop chain dependencies (hard-coded offsets)

`CHATLOG`, `HOTBAR`, and `RECAST` walk a chain of FCS structs. Each intermediate hop has a `[FieldOffset]` that Sharlayan encodes as a **hard-coded long in the provider** — if upstream changes the offset, the final address lands in the wrong place without any compile error. The integrity test validates each hard-coded hop equals the current `Marshal.OffsetOf<T>(nameof(Field))`.

### CHATLOG: `Framework` → `UIModule` → `RaptureLogModule`

| File | Field | Used offset | Why |
|---|---|---|---|
| `FFXIV/Client/System/Framework/Framework.cs` | `Framework.UIModule` | `0x2B68` | Deref hop to `UIModule*` |
| `FFXIV/Client/UI/UIModule.cs` | `UIModule.RaptureLogModule` | `0x19E0` | Trailing offset to land on RaptureLogModule |
| `FFXIV/Client/UI/Misc/RaptureLogModule.cs` | (exposed via `FFXIV/Component/Log/LogModule.cs`) | n/a | RaptureLogModule contains a `LogModule` instance Sharlayan parses via `ChatLogPointersMapper` |
| `FFXIV/Component/Log/LogModule.cs` | `LogModule.LogMessageIndex` / `LogMessageData` | `Marshal.OffsetOf` | Two `StdVector<T>` offsets (First/Last/End) for the ring buffer |

### HOTBAR: `Framework` → `UIModule` → `RaptureHotbarModule._hotbars[0]`

| File | Field | Used offset |
|---|---|---|
| `FFXIV/Client/System/Framework/Framework.cs` | `Framework.UIModule` | `0x2B68` |
| `FFXIV/Client/UI/UIModule.cs` | `UIModule.RaptureHotbarModule` | `0x57B80` |
| `FFXIV/Client/UI/Misc/RaptureHotbarModule.cs` | `_hotbars` private array | `0xA0` (additive) |
| `FFXIV/Client/UI/Misc/RaptureHotbarModule.HotbarSlot.cs` | `HotbarSlot.PopUpHelp` / `.CommandId` / `_popUpKeybindHint` | `Marshal.OffsetOf` / `FieldOffsetReader` |

### RECAST: `AtkStage` → `AtkArrayDataHolder` → `NumberArrays[ActionBar]` → `IntArray` + 60

| File | Field / hop | Used offset | Why |
|---|---|---|---|
| `FFXIV/Component/GUI/AtkStage.cs` | `AtkStage.AtkArrayDataHolder` | `0x38` | Deref |
| `FFXIV/Component/GUI/AtkArrayDataHolder.cs` | `NumberArrays**` | `0x18` | Deref |
| `FFXIV/Component/GUI/AtkArrayData.cs` | `NumberArrayType.ActionBar` enum value | `7` (× 8 = `0x38` byte offset into the NumberArrays table) | Index selection |
| `FFXIV/Component/GUI/NumberArrayData.cs` | `NumberArrayData.IntArray` | `0x28` | Deref to `int*` |
| `FFXIV/Client/UI/Arrays/ActionBarNumberArray.cs` | `_bars` private array | `60` (=15 × 4, skips `HotBarLocked` header) | Trailing add |
| `FFXIV/Client/UI/Arrays/Common/ActionBarSlotNumberArray.cs` | per-slot fields | `Marshal.OffsetOf` | Reader.Actions indexes into this |

---

## Struct-field mappings (compile-time checked via `Marshal.OffsetOf<T>(nameof(T.Field))`)

Each entry below is `Sharlayan.Resources.Mappers.*Mapper.Build()` computing a byte offset from an FCS field. Because they use `nameof(FCSType.Field)`, an upstream rename is caught at **compile time** (Sharlayan.csproj won't build). Listed here so DEPENDENCY.md is a complete inventory — the integrity test covers them only via "Build does not throw".

| Mapper | FCS file(s) | Fields consumed |
|---|---|---|
| `ActorItemMapper` | `FFXIV/Client/Game/Object/GameObject.cs` + `FFXIV/Client/Game/Character/Character.cs` + `FFXIV/Client/Game/Character/CharacterData.cs` + `FFXIV/Client/Game/Character/BattleChara.cs` + `FFXIV/Client/Game/Character/CastInfo.cs` + `FFXIV/Common/Math/Vector3.cs` | GameObject `_name`, Character `EntityId` / `OwnerId` / `ObjectKind` / `Rotation` / `HitboxRadius` / `FateId` / `BaseId` / `YalmDistanceFromPlayerX` / `Position` / `Health` / `MaxHealth` / `Mana` / `GatheringPoints` / `MaxGatheringPoints` / `CraftingPoints` / `MaxCraftingPoints` / `TitleId` / `ClassJob` / `Level` / `Icon` / `GMRank` / `TargetId` / `NameId` / `CombatTaggerId` / `TargetableStatus` / `EventId` / `RenderFlags` / `Flags`; BattleChara `CastInfo` / `StatusManager`; CastInfo `IsCasting` / `Interruptible` / `ActionId` / `TargetId` / `CurrentCastTime` / `TotalCastTime` |
| `PartyMemberMapper` | `FFXIV/Client/Game/Group/PartyMember.cs` + `FFXIV/Common/Math/Vector3.cs` | `CurrentHP` / `MaxHP` / `CurrentMP` / `EntityId` / `_name` / `ClassJob` / `Level` / `Position` / `StatusManager` |
| `PlayerInfoMapper` | `FFXIV/Client/Game/UI/PlayerState.cs` | `CurrentClassJobId`, `BaseStrength` / `BaseDexterity` / `BaseVitality` / `BaseIntelligence` / `BaseMind` / `BasePiety`, `_classJobLevels` + `_classJobExperience` (string-name reflection) |
| `StatusItemMapper` | `FFXIV/Client/Game/StatusManager.cs` (nested `Status` struct) | `StatusId` / `Param` / `RemainingTime` / `SourceObject` |
| `InventoryItemMapper` | `FFXIV/Client/Game/InventoryItem.cs` | `Slot` / `ItemId` / `Quantity` / `SpiritbondOrCollectability` / `Condition` / `Flags` / `_materia` / `_materiaGrades` / `_stains` / `GlamourId` |
| `InventoryContainerMapper` | `FFXIV/Client/Game/InventoryContainer.cs` | `Type` / `Size` |
| `HotBarItemMapper` | `FFXIV/Client/UI/Misc/RaptureHotbarModule.HotbarSlot.cs` + `FFXIV/Client/System/String/Utf8String.cs` | `HotbarSlot.CommandId` / `PopUpHelp` / `_popUpKeybindHint`; `Utf8String._inlineBuffer` |
| `RecastItemMapper` | `FFXIV/Client/UI/Arrays/Common/ActionBarSlotNumberArray.cs` | `ActionType` / `ActionId` / `IconId` / `Executable` / `GlobalCoolDownPercentage` / `CurrentCharges` / `Glows` / `ManaCost` / `InRange` |
| `TargetInfoMapper` | `FFXIV/Client/Game/Control/TargetSystem.cs` + `FFXIV/Client/Game/Character/Character.cs` | `Target` / `TargetObjectId` / `MouseOverTarget` / `FocusTarget` / `PreviousTarget`; `sizeof(Character)` |
| `EnmityItemMapper` | `FFXIV/Client/Game/UI/Hater.cs` (nested `HaterInfo`) | `_name` / `EntityId` / `Enmity` |
| `JobResourcesMapper` | `FFXIV/Client/Game/Gauge/JobGauges.cs` (all 21 gauges in one file) | Per-gauge fields — see mapper source for the full table (PLD/WAR/DRK/GNB/MNK/DRG/NIN/SAM/RPR/VPR/BRD/MCH/DNC/BLM/SMN/RDM/PCT/WHM/SCH/AST/SGE) |
| `ChatLogPointersMapper` | `FFXIV/Component/Log/LogModule.cs` | `LogMessageIndex` / `LogMessageData` StdVector pairs |

---

## Reader-level consumers (not offset-mapped)

- **`Reader.GameState.cs`** also reads these via Lumina (NuGet, not FCS):
  - `Lumina.Excel.Sheets.Weather` — `.Name` for `CurrentWeatherName`
  - `Lumina.Excel.Sheets.BGM` — `.File` for `CurrentBgmFile`
- **`LuminaXivDatabaseProvider.cs`** reads `Action` / `Status` / `TerritoryType` / `PlaceName` from Lumina for `xivdatabase` content.

---

## When it breaks

Upstream FCS changes that will surface as failures (in order of severity):

1. **Renamed struct field referenced via `nameof(T.Field)`** — Sharlayan.csproj fails to build. Trivial to spot and fix.
2. **Renamed struct field referenced by string in `FieldOffsetReader`** — passes compile, fails at mapper build time (test: `FCSDependencyIntegrityTests.PrivateFieldOffsets_Resolve`).
3. **Renamed singleton type (e.g. `BGMSystem` → `BGMPlayer`)** — passes compile, scanner key silently missing at runtime (test: `FCSDependencyIntegrityTests.SingletonTypeNames_Resolve`).
4. **Changed `[FieldOffset]` on a hard-coded chain hop (e.g. `Framework.UIModule`)** — no error, scanner key resolves to wrong memory (test: `FCSDependencyIntegrityTests.HardCodedChainOffsets_Match`).
5. **`[StaticAddress]` attribute removed / pattern changed** — test catches absence; a pattern change is silent until an actual scan runs against the current game binary and misses (detected by `harness.ps1` [7] diff).
