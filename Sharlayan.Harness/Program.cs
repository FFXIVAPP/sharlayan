// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Validation harness for Sharlayan v9. Run while FFXIV is running; produces a
//   structured report comparing the legacy (sharlayan-resources) and new
//   (FFXIVClientStructsDirect) providers, plus invariant checks against live game state.
//   Intended to be pasted back to the session that's developing the refactor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.Log;

using Sharlayan;
using Sharlayan.Models;
using Sharlayan.Models.Structures;
using Sharlayan.Resources;

namespace Sharlayan.Harness;

internal static class Program {
    private const string ProcessName = "ffxiv_dx11";

    private static async Task<int> Main(string[] args) {
        StringBuilder report = new();
        string? outFilePath = args.FirstOrDefault(a => a.StartsWith("--out="))?.Substring("--out=".Length);

        void Log(string line) {
            Console.WriteLine(line);
            report.AppendLine(line);
        }

        Log($"====== SHARLAYAN VALIDATION HARNESS ======");
        Log($"Timestamp      : {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        Log($"OS             : {System.Runtime.InteropServices.RuntimeInformation.OSDescription}");
        Log($".NET runtime   : {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
        Log($"Sharlayan assy : {typeof(Sharlayan.SharlayanConfiguration).Assembly.GetName().Version}");
        Log(string.Empty);

        // [1] Find process ----------------------------------------------------
        Log("[1] PROCESS DETECTION");
        Process[] candidates = Process.GetProcessesByName(ProcessName);
        if (candidates.Length == 0) {
            Log($"  ✗ No {ProcessName} process found. Start FFXIV and re-run.");
            if (outFilePath != null) await File.WriteAllTextAsync(outFilePath, report.ToString());
            return 1;
        }
        Process game = candidates[0];
        Log($"  ✓ {ProcessName} PID {game.Id}");
        try {
            Log($"  ✓ MainModule : {game.MainModule?.FileName}");
            Log($"  ✓ FileVersion: {game.MainModule?.FileVersionInfo.FileVersion}");
            Log($"  ✓ BaseAddress: 0x{game.MainModule?.BaseAddress.ToInt64():X}");
            Log($"  ✓ ModuleSize : {game.MainModule?.ModuleMemorySize} (0x{game.MainModule?.ModuleMemorySize:X})");
        }
        catch (Exception ex) {
            Log($"  ⚠ MainModule read failed: {ex.Message} (run as admin?)");
        }
        Log(string.Empty);

        // [2] FFXIVClientStructs struct sizes (runtime vs declared) -----------
        Log("[2] FFXIVCLIENTSTRUCTS STRUCT SIZES (runtime Marshal.SizeOf)");
        DumpSize<Character>(Log, 0x2370);
        DumpSize<CharacterData>(Log, 0x50);
        DumpSize<GameObject>(Log, 0x1A0);
        DumpSize<TargetSystem>(Log, 0x6EF0);
        DumpSize<GroupManager>(Log);
        DumpSize<FFXIVClientStructs.FFXIV.Client.Game.Group.PartyMember>(Log, 0x490);
        DumpSize<JobGaugeManager>(Log, 0x60);
        DumpSize<BardGauge>(Log, 0x10);
        DumpSize<BlackMageGauge>(Log, 0x30);
        DumpSize<WhiteMageGauge>(Log, 0x10);
        DumpSize<FFXIVClientStructs.FFXIV.Client.Game.InventoryItem>(Log, 0x48);
        DumpSize<FFXIVClientStructs.FFXIV.Client.Game.InventoryContainer>(Log, 0x20);
        DumpSize<Status>(Log, 0x10);
        DumpSize<HaterInfo>(Log, 0x48);
        DumpSize<RecastDetail>(Log, 0x14);
        DumpSize<RaptureHotbarModule.HotbarSlot>(Log, 0xE8);
        DumpSize<PlayerState>(Log, 0x908);
        DumpSize<LogModule>(Log, 0x80);
        DumpSize<Vector3>(Log, 0x0C);
        Log(string.Empty);

        // [3] Build StructuresContainer from BOTH providers -------------------
        Log("[3] PROVIDER OFFSET COMPARISON (Legacy vs FFXIVClientStructsDirect)");
        SharlayanConfiguration legacyConfig = new() {
            ProcessModel = new ProcessModel { Process = game },
#pragma warning disable CS0618 // LegacySharlayanResources is obsolete but the harness must exercise it for the A/B diff.
            ResourceProvider = ResourceProviderKind.LegacySharlayanResources,
#pragma warning restore CS0618
            UseLocalCache = true,
            JSONCacheDirectory = AppContext.BaseDirectory, // JSON files placed next to harness exe
        };
        SharlayanConfiguration directConfig = new() {
            ProcessModel = new ProcessModel { Process = game },
            ResourceProvider = ResourceProviderKind.FFXIVClientStructsDirect,
        };

        StructuresContainer? legacy = null;
        StructuresContainer? direct = null;
        try {
            IResourceProvider legacyProv = ResourceProviderFactoryShim.Create(legacyConfig);
            legacy = await legacyProv.GetStructuresAsync(legacyConfig);
            Log("  ✓ Legacy provider produced StructuresContainer");
        }
        catch (Exception ex) {
            Log($"  ✗ Legacy provider failed: {ex.GetType().Name}: {ex.Message}");
        }

        try {
            IResourceProvider directProv = ResourceProviderFactoryShim.Create(directConfig);
            direct = await directProv.GetStructuresAsync(directConfig);
            Log("  ✓ Direct provider produced StructuresContainer");
        }
        catch (Exception ex) {
            Log($"  ✗ Direct provider failed: {ex.GetType().Name}: {ex.Message}");
        }

        if (legacy != null && direct != null) {
            Log("  --- offset diff (only showing mismatches + unmapped-in-direct) ---");
            int diffs = 0;
            int matches = 0;
            DiffContainers(legacy, direct, Log, ref diffs, ref matches);
            Log($"  === {matches} match, {diffs} diff ===");
        }
        Log(string.Empty);

        // [4] Attach MemoryHandler with legacy provider (signatures work here) -
        //
        // Note on provenance: sections [4]/[5] exercise the LEGACY provider
        // (sharlayan-resources) because it's the only path with working signature
        // scans today. The FFXIVClientStructsDirect provider currently returns an
        // empty signature array (see FFXIVClientStructsDirectProvider), so its
        // Scanner would find 0 locations and the Reader would be empty. Section [2]
        // (struct sizes), section [3] (offset diff) and section [6] (Lumina) DO
        // exercise the FFXIVClientStructs submodule directly.
        Log("[4] LEGACY PROVIDER — MEMORY HANDLER + READER");
        MemoryHandler? handler = null;
        try {
            // Scanner.LoadOffsets() is called from a Task.Run inside the MemoryHandler
            // constructor — IsScanning starts false and only flips true once the async
            // signature fetch completes and the scan begins.  Polling IsScanning==false
            // exits immediately before the scan ever starts.  Use the event instead.
            var scanDone = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            handler = SharlayanMemoryManager.Instance.AddHandler(legacyConfig);
            handler.OnMemoryLocationsFound += (_, _, _) => scanDone.TrySetResult(true);
            // Guard: scan may have completed before we subscribed to the event.
            if (handler.Scanner.Locations.Count > 0) {
                scanDone.TrySetResult(true);
            }

            Log("  Waiting for scanner (up to 30 s)…");
            bool scanCompleted = await Task.WhenAny(scanDone.Task, Task.Delay(30_000)) == scanDone.Task;
            int foundCount = handler.Scanner.Locations.Count;
            Log($"  {(scanCompleted ? "✓" : "⚠ timed out")} Scanner locations found: {foundCount}");
            foreach (string key in handler.Scanner.Locations.Keys.OrderBy(k => k)) {
                IntPtr addr = handler.Scanner.Locations[key];
                Log($"    {key,-20} 0x{addr.ToInt64():X}");
            }
        }
        catch (Exception ex) {
            Log($"  ✗ Attach failed: {ex.GetType().Name}: {ex.Message}");
        }
        Log(string.Empty);

        // [5] Read live game state via Reader, check invariants ---------------
        if (handler != null) {
            Log("[5] READER OUTPUT (legacy provider)");

            TryRead(Log, "CurrentPlayer", () => {
                var r = handler.Reader.GetCurrentPlayer();
                var p = r?.Entity;
                if (p == null) {
                    return "Entity=null — CanGetPlayerInfo probably false";
                }
                Check("  CurrentPlayer.Name non-empty", !string.IsNullOrWhiteSpace(p.Name), Log);
                Check("  CurrentPlayer.HP > 0", p.HPCurrent > 0, Log);
                Check("  CurrentPlayer.MaxHP >= HP", p.HPMax >= p.HPCurrent, Log);
                Check("  CurrentPlayer.Level in [1,100]", p.Level >= 1 && p.Level <= 100, Log);
                Check($"  CurrentPlayer.Job ({p.Job}) in [0,42]", (int)p.Job >= 0 && (int)p.Job <= 42, Log);
                Check("  CurrentPlayer.X finite", double.IsFinite(p.X), Log);
                Check("  CurrentPlayer.Y finite", double.IsFinite(p.Y), Log);
                Check("  CurrentPlayer.Z finite", double.IsFinite(p.Z), Log);
                Check("  CurrentPlayer.ID non-zero", p.ID != 0u, Log);
                return $"Name=\"{p.Name}\" HP={p.HPCurrent}/{p.HPMax} MP={p.MPCurrent}/{p.MPMax} Job={p.Job} Level={p.Level} Pos=({p.X:F2},{p.Y:F2},{p.Z:F2}) ID=0x{p.ID:X8}";
            });

            TryRead(Log, "Actors", () => {
                var r = handler.Reader.GetActors();
                int pcs = r.CurrentPCs.Count;
                int npcs = r.CurrentNPCs.Count;
                int mobs = r.CurrentMonsters.Count;
                // Self lives in CurrentPlayer.Entity, not in CurrentPCs — CurrentPCs
                // is *other* players, so 0 is legitimate in a solo zone (e.g. Mist).
                Check("  Actors: total actor count > 0", (pcs + npcs + mobs) > 0, Log);
                return $"PCs={pcs} NPCs={npcs} Monsters={mobs}";
            });

            TryRead(Log, "PartyMembers", () => {
                var r = handler.Reader.GetPartyMembers();
                int count = r.PartyMembers.Count;
                Check("  PartyMembers >= 1 (self always counts)", count >= 1, Log);
                return $"Count={count}";
            });

            TryRead(Log, "Target", () => {
                var r = handler.Reader.GetTargetInfo();
                bool hasAny = r != null && (r.TargetInfo?.CurrentTarget != null || r.TargetInfo?.FocusTarget != null || r.TargetInfo?.MouseOverTarget != null);
                return $"AnyTarget={hasAny}";
            });

            TryRead(Log, "JobResources", () => {
                var r = handler.Reader.GetJobResources();
                return $"Container populated: {r?.JobResourcesContainer != null}";
            });

            // --- Live values table --------------------------------------------------
            Log(string.Empty);
            Log("  [5b] LIVE VALUES");
            try {
                var cp = handler.Reader.GetCurrentPlayer();
                var p = cp?.Entity;
                if (p == null) {
                    Log("    Entity null — scanner locations needed before reader works.");
                }
                else {
                    Log($"    Name   : {p.Name}");
                    Log($"    Job    : {p.Job}  Level: {p.Level}");
                    Log($"    HP     : {p.HPCurrent,8} / {p.HPMax,-8}  ({(p.HPMax > 0 ? (double)p.HPCurrent / p.HPMax * 100 : 0.0):F1}%)");
                    Log($"    MP     : {p.MPCurrent,8} / {p.MPMax,-8}  ({(p.MPMax > 0 ? (double)p.MPCurrent / p.MPMax * 100 : 0.0):F1}%)");
                    Log($"    CP     : {p.CPCurrent,8} / {p.CPMax,-8}");
                    Log($"    GP     : {p.GPCurrent,8} / {p.GPMax,-8}");
                    Log($"    Pos    : X={p.X,8:F2}  Y={p.Y,8:F2}  Z={p.Z,8:F2}");
                    Log($"    ID     : 0x{p.ID:X8}  MapTerritory: {p.MapTerritory}");
                }
            }
            catch (Exception ex) {
                Log($"    ✗ {ex.GetType().Name}: {ex.Message}");
            }

            try {
                var partyResult = handler.Reader.GetPartyMembers();
                var members = partyResult?.PartyMembers;
                if (members != null && members.Count > 0) {
                    Log(string.Empty);
                    Log($"  [5c] PARTY MEMBERS ({members.Count})");
                    foreach (var m in members.Values.Take(8)) {
                        Log($"    {m.Name,-20} {m.Job,-12} HP:{m.HPCurrent,6}/{m.HPMax,-6}  MP:{m.MPCurrent,6}");
                    }
                }
            }
            catch (Exception ex) {
                Log($"    ✗ Party read: {ex.GetType().Name}: {ex.Message}");
            }

            // --- Chat log parsing check -----------------------------------------
            // Sharlayan's chat reader owns its own byte-to-ChatLogItem parser
            // (ChatEntry.Process) — FFXIVClientStructs is only consulted for the
            // LogModule pointer offsets (OffsetArrayStart/LogStart/etc.).  This
            // block confirms that the Legacy path's ChatLogPointers offsets still
            // walk the current game's buffer correctly.
            Log(string.Empty);
            Log("  [5d] CHAT LOG");
            try {
                if (!handler.Reader.CanGetChatLog()) {
                    Log("    ✗ CanGetChatLog()=false — CHATLOG signature missing.");
                }
                else {
                    // First call primes the reader's internal cursor to the tail of
                    // the current buffer and returns 0 items by design.
                    var primed = handler.Reader.GetChatLog();
                    // Calling again with (0, 0) replays the entire in-memory ring
                    // buffer — this is what we want for validation: it proves the
                    // ChatLogPointers offsets + ChatEntry.Process parser both work.
                    var historical = handler.Reader.GetChatLog(0, 0);
                    await Task.Delay(1500);
                    // And one final incremental poll to prove the cursor advances
                    // correctly when nothing new arrives (should be 0 new items).
                    var delta = handler.Reader.GetChatLog(primed.PreviousArrayIndex, primed.PreviousOffset);

                    Log($"    ✓ CanGetChatLog()=true — historical buffer: {historical.ChatLogItems.Count} items, +{delta.ChatLogItems.Count} new on 1.5 s poll");
                    if (historical.ChatLogItems.Count > 0) {
                        Log("    Last 3 entries (most recent first):");
                        foreach (var item in historical.ChatLogItems.Reverse().Take(3)) {
                            string line = (item.Line ?? string.Empty).Replace("\n", " ").Trim();
                            if (line.Length > 80) line = line.Substring(0, 77) + "…";
                            Log($"      [{item.TimeStamp:HH:mm:ss}] code={item.Code} line=\"{line}\"");
                        }
                    }
                }
            }
            catch (Exception ex) {
                Log($"    ✗ Chat read: {ex.GetType().Name}: {ex.Message}");
            }

            Log(string.Empty);
        }

        // [7] Direct provider scanner — compare addresses vs legacy ----------
        // P3-B9: FFXIVClientStructsDirect now produces real Signature[] derived from
        // FFXIVClientStructs' [StaticAddress] attributes + hand-curated inner offsets.
        // Validation strategy: for every key both providers resolve, the pointer
        // address must match. A mismatch means either the pattern (FCS side) or the
        // inner offset (our side) is off for that key.
        Log("[7] FFXIVCLIENTSTRUCTSDIRECT SCANNER ADDRESSES (vs Legacy)");
        try {
            var directScanDone = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            MemoryHandler? directHandler = SharlayanMemoryManager.Instance.AddHandler(directConfig);
            directHandler.OnMemoryLocationsFound += (_, _, _) => directScanDone.TrySetResult(true);
            if (directHandler.Scanner.Locations.Count > 0) directScanDone.TrySetResult(true);
            await Task.WhenAny(directScanDone.Task, Task.Delay(30_000));

            int directCount = directHandler.Scanner.Locations.Count;
            Log($"  Direct scanner: {directCount} location(s)");
            if (handler == null) {
                Log("  (legacy handler missing — cannot diff)");
            }
            else {
                var allKeys = new SortedSet<string>(handler.Scanner.Locations.Keys);
                foreach (var k in directHandler.Scanner.Locations.Keys) allKeys.Add(k);
                int matching = 0;
                int diverging = 0;
                int onlyLegacy = 0;
                int onlyDirect = 0;
                foreach (string key in allKeys) {
                    handler.Scanner.Locations.TryGetValue(key, out var legacyLoc);
                    directHandler.Scanner.Locations.TryGetValue(key, out var directLoc);
                    IntPtr legacyAddr = legacyLoc == null ? IntPtr.Zero : (IntPtr)legacyLoc;
                    IntPtr directAddr = directLoc == null ? IntPtr.Zero : (IntPtr)directLoc;
                    if (legacyAddr != IntPtr.Zero && directAddr != IntPtr.Zero) {
                        bool same = legacyAddr == directAddr;
                        Log($"    {(same ? "✓" : "⚠")} {key,-14} legacy=0x{legacyAddr.ToInt64():X12} direct=0x{directAddr.ToInt64():X12}{(same ? string.Empty : "  MISMATCH")}");
                        if (same) matching++; else diverging++;
                    }
                    else if (legacyAddr != IntPtr.Zero) {
                        Log($"    · {key,-14} legacy only (0x{legacyAddr.ToInt64():X12})");
                        onlyLegacy++;
                    }
                    else {
                        Log($"    · {key,-14} direct only (0x{directAddr.ToInt64():X12})");
                        onlyDirect++;
                    }
                }
                Log($"  === {matching} match, {diverging} diff, {onlyLegacy} legacy-only, {onlyDirect} direct-only ===");
            }

            // [7b] End-to-end Reader check against direct provider. Addresses in [7]
            // matching legacy is necessary but not sufficient — the Reader also needs
            // the StructuresContainer offsets to align with the live game layout. This
            // block pulls the same fields the legacy path reads and compares values.
            Log(string.Empty);
            Log("  [7b] READER OUTPUT (direct provider)");
            try {
                var dcp = directHandler.Reader.GetCurrentPlayer();
                var dp = dcp?.Entity;
                if (dp == null) {
                    Log("    Entity=null — PLAYERINFO signature missing or struct offsets broken");
                }
                else {
                    string legacyName = "?"; long legacyHP = 0, legacyMP = 0, legacyLvl = 0;
                    if (handler != null) {
                        var lcp = handler.Reader.GetCurrentPlayer()?.Entity;
                        if (lcp != null) { legacyName = lcp.Name ?? "?"; legacyHP = Convert.ToInt64(lcp.HPCurrent); legacyMP = Convert.ToInt64(lcp.MPCurrent); legacyLvl = Convert.ToInt64(lcp.Level); }
                    }
                    string nameMark = string.Equals(dp.Name, legacyName, StringComparison.Ordinal) ? "✓" : "⚠";
                    string hpMark   = Convert.ToInt64(dp.HPCurrent) == legacyHP ? "✓" : "⚠";
                    string mpMark   = Convert.ToInt64(dp.MPCurrent) == legacyMP ? "✓" : "⚠";
                    string lvlMark  = Convert.ToInt64(dp.Level) == legacyLvl ? "✓" : "⚠";
                    Log($"    {nameMark} Name   legacy=\"{legacyName}\" direct=\"{dp.Name}\"");
                    Log($"    {hpMark} HP     legacy={legacyHP} direct={dp.HPCurrent}");
                    Log($"    {mpMark} MP     legacy={legacyMP} direct={dp.MPCurrent}");
                    Log($"    {lvlMark} Level  legacy={legacyLvl} direct={dp.Level}");
                    Log($"      Job={dp.Job} Pos=({dp.X:F2},{dp.Y:F2},{dp.Z:F2}) ID=0x{dp.ID:X8} MapTerritory={dp.MapTerritory}");
                }
            }
            catch (Exception ex) {
                Log($"    ✗ CurrentPlayer: {ex.GetType().Name}: {ex.Message}");
            }

            try {
                var dActors = directHandler.Reader.GetActors();
                int lpcs = 0, lnpcs = 0, lmobs = 0;
                if (handler != null) {
                    var la = handler.Reader.GetActors();
                    lpcs = la.CurrentPCs.Count; lnpcs = la.CurrentNPCs.Count; lmobs = la.CurrentMonsters.Count;
                }
                string mark = (dActors.CurrentPCs.Count == lpcs && dActors.CurrentNPCs.Count == lnpcs && dActors.CurrentMonsters.Count == lmobs) ? "✓" : "⚠";
                Log($"    {mark} Actors legacy PCs={lpcs} NPCs={lnpcs} Mobs={lmobs}  direct PCs={dActors.CurrentPCs.Count} NPCs={dActors.CurrentNPCs.Count} Mobs={dActors.CurrentMonsters.Count}");
            }
            catch (Exception ex) {
                Log($"    ✗ Actors: {ex.GetType().Name}: {ex.Message}");
            }

            try {
                var dparty = directHandler.Reader.GetPartyMembers();
                int lcount = handler != null ? handler.Reader.GetPartyMembers().PartyMembers.Count : 0;
                string mark = dparty.PartyMembers.Count == lcount ? "✓" : "⚠";
                Log($"    {mark} Party  legacy={lcount} direct={dparty.PartyMembers.Count}");
            }
            catch (Exception ex) {
                Log($"    ✗ Party: {ex.GetType().Name}: {ex.Message}");
            }

            // Chat log via direct provider — validates the Framework→UIModule→RaptureLogModule
            // multi-hop signature AND the FCS-derived ChatLogPointers offsets in tandem. First
            // call primes the cursor; (0,0) replays the historical ring buffer; last call polls
            // for a delta. Mirrors [5d] but using the direct handler.
            try {
                if (!directHandler.Reader.CanGetChatLog()) {
                    Log("    ✗ ChatLog direct: CanGetChatLog=false (CHATLOG signature didn't resolve)");
                }
                else {
                    var primed     = directHandler.Reader.GetChatLog();
                    var historical = directHandler.Reader.GetChatLog(0, 0);
                    await Task.Delay(1500);
                    var delta      = directHandler.Reader.GetChatLog(primed.PreviousArrayIndex, primed.PreviousOffset);

                    int lHistCount = handler != null && handler.Reader.CanGetChatLog() ? handler.Reader.GetChatLog(0, 0).ChatLogItems.Count : -1;
                    string cntMark = lHistCount < 0 ? "?" : (historical.ChatLogItems.Count == lHistCount ? "✓" : "⚠");
                    Log($"    {cntMark} ChatLog historical: direct={historical.ChatLogItems.Count} items (legacy={(lHistCount < 0 ? "n/a" : lHistCount.ToString())}), +{delta.ChatLogItems.Count} new on 1.5 s poll");
                    if (historical.ChatLogItems.Count > 0) {
                        foreach (var item in historical.ChatLogItems.Reverse().Take(2)) {
                            string line = (item.Line ?? string.Empty).Replace("\n", " ").Trim();
                            if (line.Length > 70) line = line.Substring(0, 67) + "…";
                            Log($"      [{item.TimeStamp:HH:mm:ss}] code={item.Code} line=\"{line}\"");
                        }
                    }
                }
            }
            catch (Exception ex) {
                Log($"    ✗ ChatLog direct: {ex.GetType().Name}: {ex.Message}");
            }
        }
        catch (Exception ex) {
            Log($"  ✗ Direct scanner failed: {ex.GetType().Name}: {ex.Message}");
        }
        Log(string.Empty);

        // [6] Lumina xivdatabase smoke test -----------------------------------
        Log("[6] LUMINA XIVDATABASE VALIDATION (FFXIVClientStructsDirect path)");
        try {
            string sqpack = Path.Combine(Path.GetDirectoryName(game.MainModule!.FileName)!, "sqpack");
            Log($"  sqpack path : {sqpack}");
            Log($"  exists      : {Directory.Exists(sqpack)}");

            var lumina = new global::Lumina.GameData(sqpack);
            var action = lumina.Excel.GetSheet<global::Lumina.Excel.Sheets.Action>();
            var status = lumina.Excel.GetSheet<global::Lumina.Excel.Sheets.Status>();
            var territory = lumina.Excel.GetSheet<global::Lumina.Excel.Sheets.TerritoryType>();
            var place = lumina.Excel.GetSheet<global::Lumina.Excel.Sheets.PlaceName>();

            Log($"  ✓ Action sheet     : {action.Count} rows");
            Log($"  ✓ Status sheet     : {status.Count} rows");
            Log($"  ✓ TerritoryType    : {territory.Count} rows");
            Log($"  ✓ PlaceName        : {place.Count} rows");

            // Invariant: action row 7 is always a common action (existed for many patches);
            // print its name as a sanity check.
            if (action.HasRow(7)) {
                Log($"    Action[7].Name     : \"{action.GetRow(7).Name.ExtractText()}\"");
            }
            // Current player's map PlaceName (via CurrentPlayer.Entity.MapTerritory if populated).
            if (handler != null) {
                try {
                    var cp = handler.Reader.GetCurrentPlayer();
                    uint tid = cp?.Entity?.MapTerritory ?? 0;
                    if (tid != 0 && territory.HasRow(tid)) {
                        uint pnId = territory.GetRow(tid).PlaceName.RowId;
                        if (place.HasRow(pnId)) {
                            Log($"    Current territory[{tid}].PlaceName = \"{place.GetRow(pnId).Name.ExtractText()}\"");
                        }
                    }
                }
                catch { /* best-effort */ }
            }
        }
        catch (Exception ex) {
            Log($"  ✗ Lumina failed: {ex.GetType().Name}: {ex.Message}");
        }
        Log(string.Empty);

        Log("====== END HARNESS ======");

        if (outFilePath != null) {
            await File.WriteAllTextAsync(outFilePath, report.ToString());
            Console.WriteLine($"\nReport written to {outFilePath}");
        }
        return 0;
    }

    private static void DumpSize<T>(Action<string> log, int? expected = null) where T : unmanaged {
        int actual = Marshal.SizeOf<T>();
        string marker = expected is null ? "-" : (actual == expected.Value ? "✓" : "⚠");
        string exp = expected is null ? string.Empty : $" (declared 0x{expected:X})";
        log($"  {marker} {typeof(T).Name,-35} sizeof=0x{actual:X}{exp}");
    }

    private static void DiffContainers(StructuresContainer legacy, StructuresContainer direct, Action<string> log, ref int diffs, ref int matches) {
        foreach (PropertyInfo topProp in typeof(StructuresContainer).GetProperties()) {
            object? lv = topProp.GetValue(legacy);
            object? dv = topProp.GetValue(direct);
            if (lv == null && dv == null) {
                continue;
            }
            if (lv == null || dv == null) {
                log($"    {topProp.Name,-25} one provider returned null");
                diffs++;
                continue;
            }
            if (topProp.PropertyType == typeof(int)) {
                CompareInts(topProp.Name, (int)lv, (int)dv, log, ref diffs, ref matches);
                continue;
            }
            // Nested structure class — reflect fields
            foreach (PropertyInfo fieldProp in topProp.PropertyType.GetProperties()) {
                if (fieldProp.PropertyType == typeof(int)) {
                    int lfv = (int)(fieldProp.GetValue(lv) ?? 0);
                    int dfv = (int)(fieldProp.GetValue(dv) ?? 0);
                    CompareInts($"{topProp.Name}.{fieldProp.Name}", lfv, dfv, log, ref diffs, ref matches);
                }
                else if (fieldProp.PropertyType == typeof(byte)) {
                    int lfv = (int)(byte)(fieldProp.GetValue(lv) ?? (byte)0);
                    int dfv = (int)(byte)(fieldProp.GetValue(dv) ?? (byte)0);
                    CompareInts($"{topProp.Name}.{fieldProp.Name}", lfv, dfv, log, ref diffs, ref matches);
                }
                else if (fieldProp.PropertyType.IsClass) {
                    object? ln = fieldProp.GetValue(lv);
                    object? dn = fieldProp.GetValue(dv);
                    if (ln == null || dn == null) continue;
                    foreach (PropertyInfo subProp in fieldProp.PropertyType.GetProperties()) {
                        if (subProp.PropertyType == typeof(int)) {
                            int lfv = (int)(subProp.GetValue(ln) ?? 0);
                            int dfv = (int)(subProp.GetValue(dn) ?? 0);
                            CompareInts($"{topProp.Name}.{fieldProp.Name}.{subProp.Name}", lfv, dfv, log, ref diffs, ref matches);
                        }
                        else if (subProp.PropertyType == typeof(byte)) {
                            int lfv = (int)(byte)(subProp.GetValue(ln) ?? (byte)0);
                            int dfv = (int)(byte)(subProp.GetValue(dn) ?? (byte)0);
                            CompareInts($"{topProp.Name}.{fieldProp.Name}.{subProp.Name}", lfv, dfv, log, ref diffs, ref matches);
                        }
                    }
                }
            }
        }
    }

    private static void CompareInts(string path, int legacy, int direct, Action<string> log, ref int diffs, ref int matches) {
        if (legacy == direct) {
            matches++;
            return;
        }
        diffs++;
        string note = direct == 0 ? "(unmapped in direct)" : (legacy == 0 ? "(unmapped in legacy)" : "⚠ DIFF");
        log($"    {path,-55} legacy={legacy,6} direct={direct,6} {note}");
    }

    private static void TryRead(Action<string> log, string label, Func<string> body) {
        try {
            string result = body();
            log($"  {label}: {result}");
        }
        catch (Exception ex) {
            log($"  {label}: ✗ {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static void Check(string assertion, bool ok, Action<string> log) {
        log($"    {(ok ? "✓" : "✗")} {assertion}");
    }
}

// ResourceProviderFactory is internal — this shim lets the harness reach it via
// InternalsVisibleTo("Sharlayan.Harness") + a thin proxy call.
internal static class ResourceProviderFactoryShim {
    public static IResourceProvider Create(SharlayanConfiguration configuration) {
        return ResourceProviderFactory.Create(configuration);
    }
}
