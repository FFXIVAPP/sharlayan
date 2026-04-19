# Sharlayan 9 — Project Context

Out-of-process FFXIV memory reader. Active branch: `sharlayan-9-rebuild`.

## What's happening on this branch

Migrating away from the unmaintained [sharlayan-resources](https://github.com/FFXIVAPP/sharlayan-resources) JSON (stale signatures & struct offsets every patch) to [FFXIVClientStructs](https://github.com/aers/FFXIVClientStructs) as a git submodule, reflected at runtime.

- **Default provider** is now `FFXIVClientStructsDirect` (was `LegacySharlayanResources`).
- **All 16 scanner keys** resolve via FCS (`CHARMAP`, `PLAYERINFO`, `PARTYMAP`, `PARTYCOUNT`, `TARGET`, `INVENTORY`, `JOBRESOURCES`, `RECAST`, `MAPINFO`, `ZONEINFO`, `ENMITYMAP`, `ENMITY_COUNT`, `AGROMAP`, `AGRO_COUNT`, `CHATLOG`, `HOTBAR`). `CHATLOG`/`HOTBAR` use a multi-hop chain Framework→UIModule→submodule.
- **Reader verified end-to-end** via `Sharlayan.Harness` [7b]: `CurrentPlayer`, `Party`, `Actors`, `ChatLog`, hotbar keybinds, BRD JobResources, target info all produce correct values.
- **sharlayan-resources HTTP fetch removed** (P3-B15). `APIHelper` is now local-JSON-only; `LegacyJsonProvider` kept for harness [3] A/B diffs using snapshotted JSON.
- **`xivdatabase`** (actions/statuses/zones) comes from Lumina reading the user's sqpack — not network.

## Key files

- `Sharlayan/Resources/Providers/FFXIVClientStructsDirectProvider.cs` — per-key `(fcsType, innerOffset)` wiring
- `Sharlayan/Resources/Providers/FFXIVClientStructsSignatureExtractor.cs` — reflects `[StaticAddress]` → `Signature` with `isPointer` + multi-hop support
- `Sharlayan/Resources/Mappers/*.cs` — each `Structures.*` field derives from FCS via `Marshal.OffsetOf`
- `Sharlayan/FFXIVClientStructs/` — git submodule (pinned commit), ILRepacked into `Sharlayan.dll`
- `Sharlayan.Harness/Program.cs` — validation tool with [3] offset diff, [4]/[5] legacy reader, [7]/[7b]/[7c] direct provider diff + eye-checks

## Harness workflow

`pwsh .\harness.ps1` from an **elevated terminal** (FFXIV ACG blocks `PROCESS_VM_READ` from non-admin). Writes timestamped report to `reports/harness-YYYYMMDD-HHmmss.txt` plus a `harness-report.txt` copy at repo root. Both are gitignored.

## Known unmapped ActorItem fields

No clean FCS equivalent: `ActionStatus`, `DifficultyRank`, `GatheringInvisible`, `GatheringStatus`, `GrandCompany`, `GrandCompanyRank`, `InCutscene`, `ModelID`. PlayerInfo derived attributes (Strength/Crit/Det/etc.) need dynamic Lumina BaseParam indices — not compile-time struct offsets.

## Notes

- `IsAgroed` was changed to test bit 1 (`InCombat`) of `AgroFlags` — old semantic was bit 0 which in FCS is `IsHostile` (training dummies aren't hostile).
- Struct offset diffs in harness [3] that remain are **legacy being stale**, not direct being wrong. Eye-check `[7b]`/`[7c]` output against in-game UI to verify.
