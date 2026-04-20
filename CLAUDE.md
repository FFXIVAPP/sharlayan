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

## Zero-touch patch-day architecture

Every inner offset (provider `TryAdd` args, multi-hop chains, `Reader.GameState` reads) is derived at runtime from `[FieldOffset]` via `FieldOffsetReader`. FCS field-offset bumps propagate automatically — no edits. Patch-day manual work is only needed for renames (public field → compile error; private field / singleton type name → integrity test flags) and silent `[StaticAddress]` pattern changes (harness [7] scanner diff flags).

**Keep `DEPENDENCY.md` and `Sharlayan.Tests/Resources/Providers/FCSDependencyIntegrityTests.cs` in sync.** Add an entry to both whenever you introduce a new scanner key, mapper field, or `FieldOffsetReader` string-name lookup. DEPENDENCY.md is the human-readable inventory; the integrity test is the enforced contract.

## Notes

- `IsAgroed` tests **bit 1** (`InCombat`) of `AgroFlags`, not bit 0 (`IsHostile`) — training dummies aren't hostile but they *are* engaged.
- `ActionItem.KeyBinds` comes from `HotbarSlot._popUpKeybindHint` (+0x88, human-readable " [Ctrl+Alt+0]"), not `_keybindHint` (+0xA8, packed binary). Reader.Actions trims each parsed segment so `Modifiers.Contains("Ctrl")` matches cleanly.
- `ActorItem.InCutscene` maps to `GameObject.RenderFlags + 1` (bit 11 `Nameplate` in byte 1 — flips during cutscenes).
- `RecastItem.Category` / `.Type` both map to `ActionBarSlotNumberArray.ActionType` — FCS names it `ActionType` but the game writes the `ActionCategory` row id there (BRD weaponskills = 47, role/LB = 56).
- `RecastItem.ActionProc` → `Glows` (steady proc indicator), not `Pulses` (short animation trigger).
- Struct offset diffs in harness [3] that remain are **legacy being stale**, not direct being wrong. Eye-check `[7b]`/`[7c]` output against in-game UI to verify.
- Genuinely unmapped (no clean FCS equivalent): `ActorItem.ActionStatus` / `DifficultyRank` / `Gathering*` / `GrandCompany*` / `ModelID`; `PlayerInfo` derived attributes (need dynamic Lumina BaseParam indices).

## What's left

- **Chromatics integration validation** — Keybinds layer is wired (P3-B17/B18). `Category == 56` added alongside 49/51 for role/LB special-action coloring (P3-B23). Remaining: pass against BGM / cutscene surface.
- **Unmapped fields with no clean FCS source** — `ActorItem.{ActionStatus, DifficultyRank, Gathering*, GrandCompany*, ModelID}`, `PlayerInfo` derived attributes (Str/Dex/Crit/etc.) via `PlayerState._attributes + BaseParam`, `BGM.Name` (only `.File` is on the sheet). Needs Lumina-backed helpers, not raw memory offsets.
- **Legacy provider removal** — `LegacyJsonProvider` still exists so harness [3] can A/B diff against snapshotted JSON. Delete it + `APIHelper` once the struct/offset churn stabilises.
- **Harness [3] polish** — currently 200+ legacy-stale diff rows. Filter to only "unmapped in direct" rows so the output stays actionable.
- **FFXIVClientStructsETL** — retargeted to net10.0 so the sln builds, but the HTTP-fetch workflow it existed for is gone. Likely a delete candidate.
- **Branch housekeeping** — ~20 `P3-B*` commits, easy to squash per logical unit before PR to `master`.
