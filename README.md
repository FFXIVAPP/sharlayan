# Sharlayan

Out-of-process memory reader for **Final Fantasy XIV** (Windows, DirectX 11). Originally a component of FFXIVAPP, Sharlayan has since been split into its own library. It exposes a stable C# API for reading game state (the player, party, actors, hotbars, chat log, inventory, job gauges, etc.) without injecting into the game process.

## What's new in 9.0

The long-dead [sharlayan-resources](https://github.com/FFXIVAPP/sharlayan-resources) JSON feed is gone. Signatures, struct offsets, and game-data lookups now come from two actively maintained upstreams:

- **[FFXIVClientStructs](https://github.com/aers/FFXIVClientStructs)** (vendored as a git submodule and ILRepacked into `Sharlayan.dll`) provides the game struct layouts and `[StaticAddress]` byte patterns. Every FFXIV patch day, bumping the submodule is usually enough.
- **[Lumina](https://github.com/NotAdam/Lumina)** reads `sqpack` directly for `xivdatabase` content — action names, status effects, territory/map rows — so the library no longer ships a separate JSON mirror of the game's Excel data.

The resulting provider (`FFXIVClientStructsDirect`) is the default; `LegacySharlayanResources` remains available as an opt-in fallback but is marked `[Obsolete]`.

## Installation

Add the NuGet-packed `Sharlayan.dll` (or a `ProjectReference` to `Sharlayan.csproj`) to your project. The submodule build is driven from `build.ps1`; see the harness project for a working example.

## Usage

```csharp
using System.Diagnostics;
using Sharlayan;
using Sharlayan.Models;

Process game = Process.GetProcessesByName("ffxiv_dx11").FirstOrDefault()
              ?? throw new InvalidOperationException("FFXIV not running");

SharlayanConfiguration configuration = new() {
    ProcessModel = new ProcessModel { Process = game },
    // ResourceProvider defaults to FFXIVClientStructsDirect since 9.0.
    // GameInstallPath is optional; if set, Lumina will load xivdatabase
    // (actions, statuses, zones) from your local sqpack.
    GameInstallPath = Path.GetDirectoryName(game.MainModule!.FileName),
};

MemoryHandler handler = SharlayanMemoryManager.Instance.AddHandler(configuration);

// FFXIV's anti-tamper (ACG) blocks PROCESS_VM_READ from non-elevated processes —
// your consumer must run as Administrator or AddHandler will fail to attach.
```

Reading:

```csharp
var player = handler.Reader.GetCurrentPlayer().Entity;
Console.WriteLine($"{player.Name} — {player.Job} Lv.{player.Level} HP {player.HPCurrent}/{player.HPMax}");

var actors  = handler.Reader.GetActors();        // PCs, NPCs, monsters
var party   = handler.Reader.GetPartyMembers();
var target  = handler.Reader.GetTargetInfo();
var chat    = handler.Reader.GetChatLog();       // first call primes the cursor; call again for new lines
var actions = handler.Reader.GetActions();       // hotbars + recast timers
```

When switching processes:

```csharp
SharlayanMemoryManager.Instance.RemoveHandler(configuration.ProcessModel);
```

Custom signature overrides (unusual — the built-in set covers every `Reader.*` surface) go through `configuration.ResourceProvider` by implementing `IResourceProvider`, not by hand-editing JSON in the working directory.

## Harness

`pwsh .\harness.ps1` (from an elevated terminal) builds Sharlayan + the `Sharlayan.Harness` tool, attaches to the running client, and produces a diagnostic report at `reports/harness-<timestamp>.txt`. Useful both for verifying a new FCS submodule bump and for eye-checking live-read fields against what you see in-game.

## Credits

Sharlayan's 9.0 rebuild is built on the work of several upstream projects. Each retains its own license notice inside `Sharlayan/FFXIVClientStructs/LICENSE` and the respective NuGet packages; the MIT terms below reproduce as required:

- **[FFXIVClientStructs](https://github.com/aers/FFXIVClientStructs)** — MIT, © 2021–2023 aers. Provides every game struct layout and the `[StaticAddress]` signatures Sharlayan resolves at runtime.
- **[Lumina](https://github.com/NotAdam/Lumina)** and **Lumina.Excel** — MIT, © NotAdam and contributors. `sqpack` / Excel data loader used for actions, statuses, and territory data.
- **[Newtonsoft.Json](https://www.newtonsoft.com/json)** — MIT, © James Newton-King. JSON serialisation for snapshotted legacy data.
- **[NLog](https://nlog-project.org/)** — BSD-3-Clause, © NLog authors.
- **[ILRepack](https://github.com/gluck/il-repack)** — Apache 2.0. Merges `FFXIVClientStructs.dll` + `InteropGenerator.Runtime.dll` into `Sharlayan.dll` at build time.

## License

Sharlayan is MIT-licensed — see [LICENSE.md](LICENSE.md). Not affiliated with or endorsed by Square Enix.
