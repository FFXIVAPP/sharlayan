# Sharlayan 9 — Project Context

Out-of-process FFXIV memory reader. Active branch: `sharlayan-9-rebuild`.

## What's happening on this branch

Migrating away from the unmaintained [sharlayan-resources](https://github.com/FFXIVAPP/sharlayan-resources) JSON (stale signatures & struct offsets every patch) to [FFXIVClientStructs](https://github.com/aers/FFXIVClientStructs) as a git submodule, reflected at runtime. Lumina reads the user's sqpack for `xivdatabase` content (actions/statuses/zones).

- **Default provider:** `FFXIVClientStructsDirect` (was `LegacySharlayanResources`; latter is `[Obsolete]` but still opt-in for harness [3] A/B diffs).
- **21 scanner keys** all resolve via FCS `[StaticAddress]` — no hand-rolled byte patterns. `CHATLOG` / `HOTBAR` use a multi-hop `Framework → UIModule → submodule` chain; `RECAST` walks `AtkStage → AtkArrayDataHolder.NumberArrays[7] → IntArray + 60` to land on `ActionBarNumberArray._bars[0]` (the UI-side per-slot state — where `IsAvailable` / `InRange` / `CoolDownPercent` / `Glows` live, *not* on `ActionManager.RecastDetail`).
- **Public Reader API:** `GetCurrentPlayer` / `GetActors` / `GetPartyMembers` / `GetTargetInfo` / `GetChatLog` / `GetActions` / `GetJobResources` / `GetInventory` / **`GetGameState`** (the last consolidates Chromatics' old CutsceneAnimation/DutyFinderBell/GameState/Music/Weather extensions).
- **sharlayan-resources HTTP fetch removed.** `APIHelper` is local-JSON-only — `LegacyJsonProvider` still forwards to it against snapshotted files.

## Key files

- `Sharlayan/Resources/Providers/FFXIVClientStructsDirectProvider.cs` — per-key `(fcsType, innerOffsets)` wiring.
- `Sharlayan/Resources/Providers/FFXIVClientStructsSignatureExtractor.cs` — reflects `[StaticAddress]` → `Signature` with `isPointer` + multi-hop chain support.
- `Sharlayan/Resources/Mappers/*.cs` — each `Structures.*` field derives from FCS via `Marshal.OffsetOf`.
- `Sharlayan/Reader.GameState.cs` — reads GameMain/Conditions/ContentsFinder/WeatherManager/BGMSystem; lazy-caches Lumina `GameData` for weather/BGM name lookups.
- `Sharlayan/FFXIVClientStructs/` — git submodule (pinned commit), ILRepacked into `Sharlayan.dll`.
- `Sharlayan.Harness/Program.cs` — validation tool; [3] offset diff, [4]/[5] legacy reader, [7]/[7b]/[7c] direct provider diff + live-value eye-checks including GameState and full HOTBAR_1 / HOTBAR_2 dumps.

## Harness workflow

`pwsh .\harness.ps1` from an **elevated terminal** (FFXIV ACG blocks `PROCESS_VM_READ` from non-admin). Writes timestamped report to `reports/harness-YYYYMMDD-HHmmss.txt` plus `harness-report.txt` at repo root (both gitignored).

## Keep DEPENDENCY.md and the integrity test in sync

Two artifacts catch FCS upstream drift before it turns into silent runtime bugs:

- **`DEPENDENCY.md`** — human-readable inventory of every FCS file, type, and field Sharlayan touches, grouped by subsystem. Update it whenever you add/remove/retarget a signature key, a mapper field, a hard-coded chain offset, or a `FieldOffsetReader` string-name lookup. It's the reference anyone (including future-you) will read to understand "what does a patch-day FCS bump mean for Sharlayan?".
- **`Sharlayan.Tests/Resources/Providers/FCSDependencyIntegrityTests.cs`** — xunit guards that assert (a) every singleton type name passed to `TryAdd` resolves via the extractor, (b) every private-field string in `FieldOffsetReader` exists, (c) every hard-coded chain offset still matches the current `[FieldOffset]` attribute value, and (d) every mapper `Build()` runs without throwing. Add a new assertion whenever you introduce a new reflection-string or hard-coded offset.

If you bump the submodule and these tests fail, **fix the binding in Sharlayan AND update DEPENDENCY.md** — don't just edit the assertion.

## Notes

- `IsAgroed` tests **bit 1** (`InCombat`) of `AgroFlags`, not bit 0 (`IsHostile`) — training dummies aren't hostile but they *are* engaged.
- `ActionItem.KeyBinds` comes from `HotbarSlot._popUpKeybindHint` (+0x88, human-readable " [Ctrl+Alt+0]"), not `_keybindHint` (+0xA8, packed binary). Reader.Actions trims each parsed segment so `Modifiers.Contains("Ctrl")` matches cleanly.
- `ActorItem.InCutscene` maps to `GameObject.RenderFlags + 1` (bit 11 `Nameplate` in byte 1 — flips during cutscenes).
- `RecastItem.Category` / `.Type` both map to `ActionBarSlotNumberArray.ActionType` — FCS names it `ActionType` but the game writes the `ActionCategory` row id there (BRD weaponskills = 47, role/LB = 56).
- `RecastItem.ActionProc` → `Glows` (steady proc indicator), not `Pulses` (short animation trigger).
- Struct offset diffs in harness [3] that remain are **legacy being stale**, not direct being wrong. Eye-check `[7b]`/`[7c]` output against in-game UI to verify.
- Genuinely unmapped (no clean FCS equivalent): `ActorItem.ActionStatus` / `DifficultyRank` / `Gathering*` / `GrandCompany*` / `ModelID`; `PlayerInfo` derived attributes (need dynamic Lumina BaseParam indices).

## What's left

- **Chromatics integration validation** — Keybinds layer is wired (P3-B17/B18). Remaining downstream usages need a pass against the `GetGameState` / Weather / BGM / cutscene surface; in particular Chromatics' `Keybinds` special-action path checks `Category == 49 || 51` but the game now writes `56` for role/LB actions — either bump the Chromatics check or route via Lumina's `ActionCategory` sheet here.
- **Unmapped fields with no clean FCS source** — `ActorItem.{ActionStatus, DifficultyRank, Gathering*, GrandCompany*, ModelID}`, `PlayerInfo` derived attributes (Str/Dex/Crit/etc.) via `PlayerState._attributes + BaseParam`, `BGM.Name` (only `.File` is on the sheet). Needs Lumina-backed helpers, not raw memory offsets.
- **Legacy provider removal** — `LegacyJsonProvider` still exists so harness [3] can A/B diff against snapshotted JSON. Delete it + `APIHelper` once the struct/offset churn stabilises.
- **Harness [3] polish** — currently 200+ legacy-stale diff rows. Filter to only "unmapped in direct" rows so the output stays actionable.
- **FFXIVClientStructsETL** — retargeted to net10.0 so the sln builds, but the HTTP-fetch workflow it existed for is gone. Likely a delete candidate.
- **Branch housekeeping** — ~20 `P3-B*` commits, easy to squash per logical unit before PR to `master`.
