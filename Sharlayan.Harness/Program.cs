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

            // Live-values dump for hand-verification. Legacy offsets are stale so the [3]
            // byte-diff table can't tell us whether a given direct offset is correct — but
            // we can eye-check by reading the fields and comparing against what the game
            // shows in-game. Pick the first few actors plus the local player and log the
            // fields that are mapped from FCS (ID, Type, HP, Position, Job, TargetID, etc.).
            try {
                var dp = directHandler.Reader.GetCurrentPlayer()?.Entity;
                if (dp != null) {
                    Log(string.Empty);
                    Log("  [7c] EYE-CHECK VALUES (direct provider)");
                    Log($"    LocalPlayer.TargetID   = 0x{dp.TargetID:X8}   (0 when not targeting)");
                    Log($"    LocalPlayer.ClaimedByID= 0x{dp.ClaimedByID:X8}   (combat-tag owner; 0 when not in combat)");
                    Log($"    LocalPlayer.NPCID1     = {dp.NPCID1}");
                    Log($"    LocalPlayer.NPCID2     = {dp.NPCID2}   (NameId — 0 for players, row id for NPCs)");
                    Log($"    LocalPlayer.Type       = {dp.Type}   (1=Pc, 2=BattleNpc, 3=EventNpc, 4=Treasure, ...)");
                    Log($"    LocalPlayer.TargetFlags= {dp.TargetFlags}   (targetability; see ObjectTargetableFlags)");
                    Log($"    LocalPlayer.Title      = {dp.Title}   (TitleId — compare against /title list)");
                    Log($"    LocalPlayer.Icon       = {dp.Icon}   (nameplate icon id)");
                    Log($"    LocalPlayer.HitBoxRadius={dp.HitBoxRadius}");
                    Log($"    LocalPlayer.IsAgroed   = {dp.IsAgroed}   (CharacterData.Flags bit 0 / IsHostile)");
                    Log($"    LocalPlayer.AgroFlags  = 0x{dp.AgroFlags:X2} CombatFlags=0x{dp.CombatFlags:X2}   (bit 1 = InCombat)");
                    Log($"    LocalPlayer.InCutscene = {dp.InCutscene}   (ActorItem.InCutscene — currently unmapped in direct, see raw RenderFlags below)");

                    // Raw per-actor cutscene-candidate bytes from the CHARMAP pointer array. During
                    // a cutscene the local player's GameObject.RenderFlags should lose the Model bit
                    // (1 << 1 = 0x02), and TargetableStatus often also changes. We pull the first
                    // non-null pointer from CHARMAP (the local player is typically slot 0) and read
                    // the raw bytes so we can see which byte actually reflects cutscene state.
                    try {
                        if (directHandler.Scanner.Locations.TryGetValue(Sharlayan.Signatures.CHARMAP_KEY, out var charmap)) {
                            // CHARMAP is a pointer array (819 × 8 bytes). First pointer → first GameObject*.
                            long firstActorAddr = directHandler.GetInt64(charmap);
                            if (firstActorAddr != 0) {
                                IntPtr actor = new IntPtr(firstActorAddr);
                                byte tgtStatus      = directHandler.GetByte(actor, 0x95);   // TargetStatus
                                byte targetable     = directHandler.GetByte(actor, 0x9A);   // TargetableStatus
                                byte renderFlagsLo  = directHandler.GetByte(actor, 0x118);  // VisibilityFlags low byte
                                byte renderFlagsHi  = directHandler.GetByte(actor, 0x119);  // high byte (Nameplate = bit 11)
                                Log($"    LocalPlayer raw @ CHARMAP[0]: TargetStatus@0x95=0x{tgtStatus:X2} TargetableStatus@0x9A=0x{targetable:X2} RenderFlags@0x118=0x{renderFlagsLo:X2} RenderFlags@0x119=0x{renderFlagsHi:X2}");
                                Log($"      (Model bit = 0x02 on 0x118; during a cutscene this typically drops. Watch which byte changes.)");
                            }
                        }
                    }
                    catch (Exception ex) {
                        Log($"    ✗ Raw actor byte dump: {ex.GetType().Name}: {ex.Message}");
                    }
                }
                var dActors2 = directHandler.Reader.GetActors();
                // Sample up to 3 non-self actors so the user can eye-check names/types.
                var sample = dActors2.CurrentNPCs.Values.Take(3).ToList();
                foreach (var mob in dActors2.CurrentMonsters.Values.Take(2)) sample.Add(mob);
                if (sample.Count > 0) {
                    Log($"    Sample actors ({sample.Count}):");
                    foreach (var a in sample) {
                        Log($"      - {a.Name,-24} Type={a.Type} ID=0x{a.ID:X8} HP={a.HPCurrent}/{a.HPMax} IsAgroed={a.IsAgroed} AgroFlags=0x{a.AgroFlags:X2}");
                    }
                }

                // Current target — pulls the actor the player is hard-targeted on. When
                // engaged with a mob this should show IsAgroed=True for the mob; when
                // targeting a friendly NPC it should show IsAgroed=False. Soft-target
                // (MouseOver) and FocusTarget are shown too when set.
                try {
                    var tr = directHandler.Reader.GetTargetInfo();
                    var ti = tr?.TargetInfo;
                    if (ti == null) {
                        Log("    CurrentTarget: (TargetInfo null)");
                    }
                    else {
                        void DumpTarget(string label, Sharlayan.Core.ActorItem? t) {
                            if (t == null) { Log($"    {label}: (none)"); return; }
                            Log($"    {label}: \"{t.Name}\" Type={t.Type} ID=0x{t.ID:X8} HP={t.HPCurrent}/{t.HPMax}");
                            Log($"      IsAgroed={t.IsAgroed}  AgroFlags=0x{t.AgroFlags:X2}  CombatFlags=0x{t.CombatFlags:X2}  ClaimedByID=0x{t.ClaimedByID:X8}");
                            Log($"      IsCasting={t.IsCasting1}  CastingID={t.CastingID}  CastingProgress={t.CastingProgress:F2}/{t.CastingTime:F2}s  CastTargetID=0x{t.CastingTargetID:X8}");
                            Log($"      Pos=({t.X:F1},{t.Y:F1},{t.Z:F1})  TargetID=0x{t.TargetID:X8}  NPCID2={t.NPCID2}");
                        }
                        DumpTarget("CurrentTarget", ti.CurrentTarget);
                        if (ti.FocusTarget != null) DumpTarget("FocusTarget",   ti.FocusTarget);
                        if (ti.MouseOverTarget != null) DumpTarget("MouseOverTarget", ti.MouseOverTarget);
                        if (ti.CurrentTarget == null && ti.FocusTarget == null && ti.MouseOverTarget == null) {
                            Log($"    CurrentTargetID = 0x{ti.CurrentTargetID:X8}  (no resolved target — target offscreen or between actor frames)");
                        }
                    }
                }
                catch (Exception ex) {
                    Log($"    ✗ Target: {ex.GetType().Name}: {ex.Message}");
                }

                // Explicitly target HOTBAR_1 and HOTBAR_2 — scanning for "first populated"
                // was finding CROSS_PETBAR because earlier HOTBARs looked empty (Name offset
                // bug — reading Utf8String pointer bytes instead of the inline char buffer).
                if (directHandler.Reader.CanGetActions()) {
                    var actions = directHandler.Reader.GetActions();
                    foreach (var target in new[] { Sharlayan.Core.Enums.Action.Container.HOTBAR_1, Sharlayan.Core.Enums.Action.Container.HOTBAR_2 }) {
                        var bar = actions.ActionContainers.FirstOrDefault(c => c.ContainerType == target);
                        if (bar == null || bar.ActionItems.Count == 0) {
                            Log($"    Hotbar {target}: (no populated slots)");
                            continue;
                        }
                        Log($"    Hotbar {target} ({bar.ActionItems.Count} populated slots):");
                        foreach (var item in bar.ActionItems) {
                            string keybind = string.IsNullOrEmpty(item.KeyBinds) ? "<unbound>" : item.KeyBinds;
                            // Dump the recast-driven UI state too so Chromatics' Keybinds layer can
                            // verify IsAvailable / InRange / ChargeReady / CoolDown% are tracking the
                            // live ActionBarSlotNumberArray. Slots Chromatics checks for "special
                            // action" colour are category 49/51 — ActionType for reference.
                            Log($"      slot {item.Slot}: \"{item.Name}\" ID={item.ID} keybind=\"{keybind}\"");
                            string mods = item.Modifiers.Count == 0 ? "(none)" : string.Join("+", item.Modifiers);
                            Log($"        ActionKey=\"{item.ActionKey ?? string.Empty}\" Modifiers=[{mods}]");
                            Log($"        Avail={item.IsAvailable} InRange={item.InRange} ChargeReady={item.ChargeReady}/{item.ChargesRemaining} CD={item.CoolDownPercent}% Proc={item.IsProcOrCombo} Cat={item.Category} Icon={item.Icon} Cost={item.RemainingCost}");
                        }
                    }
                }
                else {
                    Log("    Hotbar: CanGetActions=false (HOTBAR or RECAST signature missing)");
                }

                // Job resources — pulls the active job's gauge from JobGaugeManager. For our
                // test (BRD), the BardResources fields (ActiveSong / SongTimer / Repertoire /
                // SoulVoice) should reflect what's shown on the gauge UI in-game.
                try {
                    if (directHandler.Reader.CanGetJobResources()) {
                        var jr = directHandler.Reader.GetJobResources();
                        var c = jr?.JobResourcesContainer;
                        if (c != null) {
                            Log($"    JobResources populated=true  (current job: {dp?.Job})");
                            Log($"      Bard: ActiveSong={c.Bard.ActiveSong} Timer={c.Bard.Timer}ms Repertoire={c.Bard.Repertoire} SoulVoice={c.Bard.SoulVoice}");
                        }
                        else {
                            Log("    JobResources: Container is null (reader returned empty)");
                        }
                    }
                    else {
                        Log("    JobResources: CanGetJobResources=false (JOBRESOURCES signature missing)");
                    }
                }
                catch (Exception ex) {
                    Log($"    ✗ JobResources: {ex.GetType().Name}: {ex.Message}");
                }
            }
            catch (Exception ex) {
                Log($"    ✗ EyeCheck: {ex.GetType().Name}: {ex.Message}");
            }

            // GameState — validates GAMEMAIN / CONDITIONS / CONTENTSFINDER / WEATHER /
            // BGMSYSTEM signatures + Lumina name lookups in one shot. Populated fields here
            // should match the in-game UI: weather matches /weather-inspect, BGM matches
            // what you can hear, DutyFinderBellPopped iff the queue is "Ready".
            try {
                if (directHandler.Reader.CanGetGameState()) {
                    var gs = directHandler.Reader.GetGameState();
                    Log(string.Empty);
                    Log("  [7c.2] GAMESTATE (direct provider)");
                    Log($"    IsLoggedIn={gs.IsLoggedIn}  IsReadyToRead={gs.IsReadyToRead}  TerritoryLoadState={gs.TerritoryLoadState}  (1=loading,2=loaded,3=unloading)");
                    // Dump raw Conditions bytes too so a "WatchingCutscene=False when I am"
                    // report can be pinned to whichever bit the game set for that cutscene.
                    byte occCut = 0, watch58 = 0, watch78 = 0, bound = 0, occEvt = 0;
                    if (directHandler.Scanner.Locations.TryGetValue(Sharlayan.Signatures.CONDITIONS_KEY, out var condLoc)) {
                        IntPtr cond = condLoc;
                        try { occCut  = directHandler.GetByte(cond, 35); } catch { }
                        try { watch58 = directHandler.GetByte(cond, 58); } catch { }
                        try { watch78 = directHandler.GetByte(cond, 78); } catch { }
                        try { bound   = directHandler.GetByte(cond, 34); } catch { }
                        try { occEvt  = directHandler.GetByte(cond, 31); } catch { }
                    }
                    Log($"    WatchingCutscene={gs.WatchingCutscene}  (raw: OccupiedInCutSceneEvent@35={occCut} WatchingCutscene@58={watch58} WatchingCutscene78@78={watch78} OccupiedInEvent@31={occEvt} BoundByDuty@34={bound})");
                    Log($"    ContentsFinderQueueState={gs.ContentsFinderQueueState}  (DutyFinderBellPopped={gs.DutyFinderBellPopped}  InInstance={gs.InInstance})");
                    Log($"    Weather id={gs.CurrentWeatherId}  name=\"{gs.CurrentWeatherName ?? "(no lumina)"}\"");
                    Log($"    BGM id={gs.CurrentBgmId}  scene={gs.CurrentBgmSceneId}  file=\"{gs.CurrentBgmFile ?? "(no lumina)"}\"");
                }
                else {
                    Log("    GameState: CanGetGameState=false (GAMEMAIN signature didn't resolve)");
                }
            }
            catch (Exception ex) {
                Log($"    ✗ GameState: {ex.GetType().Name}: {ex.Message}");
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
