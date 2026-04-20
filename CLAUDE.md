# Sharlayan 9 — Project Context

Out-of-process FFXIV memory reader. Active branch: `sharlayan-9-rebuild`.

## What's happening on this branch

Migrating away from the unmaintained [sharlayan-resources](https://github.com/FFXIVAPP/sharlayan-resources) JSON (stale signatures & struct offsets every patch) to [FFXIVClientStructs](https://github.com/aers/FFXIVClientStructs) as a git submodule, reflected at runtime. Lumina reads the user's sqpack for `xivdatabase` content (actions/statuses/zones).

- **Default provider:** `FFXIVClientStructsDirect` (only provider — `LegacySharlayanResources` removed).
- **21 scanner keys** all resolve via FCS `[StaticAddress]` — no hand-rolled byte patterns. `CHATLOG` / `HOTBAR` use a multi-hop `Framework → UIModule → submodule` chain; `RECAST` walks `AtkStage → AtkArrayDataHolder.NumberArrays[7] → IntArray + 60` to land on `ActionBarNumberArray._bars[0]` (the UI-side per-slot state — where `IsAvailable` / `InRange` / `CoolDownPercent` / `Glows` live, *not* on `ActionManager.RecastDetail`).
- **Public Reader API:** `GetCurrentPlayer` / `GetActors` / `GetPartyMembers` / `GetTargetInfo` / `GetChatLog` / `GetActions` / `GetJobResources` / `GetInventory` / **`GetGameState`** (the last consolidates Chromatics' old CutsceneAnimation/DutyFinderBell/GameState/Music/Weather extensions).
- **sharlayan-resources removed.** `APIHelper`, `LegacyJsonProvider`, `GameRegion`, `ProcessExtensions` deleted. `SharlayanConfiguration` no longer has `JSONCacheDirectory`/`PatchVersion`/`UseLocalCache`/`GameRegion`.

## Key files

- `Sharlayan/Resources/Providers/FFXIVClientStructsDirectProvider.cs` — per-key `(fcsType, innerOffsets)` wiring.
- `Sharlayan/Resources/Providers/FFXIVClientStructsSignatureExtractor.cs` — reflects `[StaticAddress]` → `Signature` with `isPointer` + multi-hop chain support.
- `Sharlayan/Resources/Mappers/*.cs` — each `Structures.*` field derives from FCS via `Marshal.OffsetOf`.
- `Sharlayan/Reader.GameState.cs` — reads GameMain/Conditions/ContentsFinder/WeatherManager/BGMSystem; lazy-caches Lumina `GameData` for weather/BGM name lookups.
- `Sharlayan/FFXIVClientStructs/` — git submodule (pinned commit), ILRepacked into `Sharlayan.dll`.
- `Sharlayan.Harness/Program.cs` — validation tool; [3]/[3b]/[3c]/[3c.2]/[3d] direct provider scanner + reader + eye-check + GameState + chat log; [4] Lumina smoke test.

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
- Eye-check `[3b]`/`[3c]` harness output against in-game UI to verify live values.
- Genuinely unmapped (no clean FCS equivalent): `ActorItem.ActionStatus` / `DifficultyRank` / `GatheringInvisible` / `GatheringStatus` / `GrandCompany*` / `ModelID`. `PlayerState` has `GrandCompany`/`_GCRanks` for the local player, but `Character` has no GC field for arbitrary actors. `ModelID` appearance data lives in `DrawDataContainer` (not yet exposed). These all default to 0 and are not consumed by known callers.
- `PlayerInfo` derived attributes (`Strength`, `CriticalHitRate`, `HPMax`, resistances, etc.) are now fully mapped via `PlayerState._attributes[PlayerAttribute.*]` — see `PlayerInfoMapper.cs`.

## Versioning policy

Version is `MAJOR.MINOR.PATCH`, stored in [`Sharlayan/version.props`](Sharlayan/version.props). The csproj imports it; `<Version>`, `<AssemblyVersion>`, and `<FileVersion>` are derived automatically — never edit those in the csproj directly.

| Component | Who changes it | When |
|-----------|---------------|------|
| **Major** | Human only | Breaking API changes, major rewrites |
| **Minor** | Human only | New public API surface, significant features |
| **Patch** | **Claude** | Every session that makes a substantive code change to Sharlayan |

**Claude's rule**: at the end of any session where you modified Sharlayan source files (not just docs, scripts, or tests), increment `<VersionPatch>` by 1 in `Sharlayan/version.props`. Do this as the last change in the session so one patch bump covers the whole batch of changes. Do NOT touch `<VersionMajor>` or `<VersionMinor>` — those are human decisions.

## What's left

- **ActorItem fields with no FCS source** — `ActionStatus`, `DifficultyRank`, `GatheringInvisible`, `GatheringStatus`, `GrandCompany*` (not on `Character` for arbitrary actors), `ModelID` (appearance lives in `DrawDataContainer`, not yet exposed). All default to 0; no known callers need them.
- **BGM.Name** — the BGM Excel sheet has no Name column; `CurrentBgmFile` (the `.scd` path) is the appropriate field. No further action.
- **Branch housekeeping** — ~20 `P3-B*` commits, easy to squash per logical unit before PR to `master`. Do this interactively via `git rebase -i`.
