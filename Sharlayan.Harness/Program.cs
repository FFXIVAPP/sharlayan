// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Validation harness for Sharlayan v9. Run while FFXIV is running; produces a
//   structured report validating the FFXIVClientStructsDirect provider against
//   live game state and Lumina xivdatabase content.
//
//   On first pass the full report ([1]..[4]) is written to the console and to
//   the --out= file. The harness then enters a live-refresh loop that redraws
//   only the [3c]/[3c.2] eye-check + GameState blocks in place, so the user
//   can watch values change in-game without the console spamming. Press Ctrl+C
//   to exit.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
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
using Sharlayan.Resources;
using Sharlayan.Resources.Mappers;

using FCSGame = FFXIVClientStructs.FFXIV.Client.Game;

namespace Sharlayan.Harness;

internal static class Program {
    private const string ProcessName = "ffxiv_dx11";
    private const int DefaultLiveIntervalMs = 500;

    private static async Task<int> Main(string[] args) {
        StringBuilder report = new();
        string? outFilePath = args.FirstOrDefault(a => a.StartsWith("--out="))?.Substring("--out=".Length);
        bool runOnce = args.Any(a => a == "--once");
        int liveIntervalMs = ParseIntArg(args, "--interval=", DefaultLiveIntervalMs);

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

        // [2] FFXIVClientStructs struct sizes — commented out; re-enable for FCS-bump diagnostics.
        
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
        DumpSize<Vector3>(Log, 0x10);
        Log(string.Empty);
        

        // [3] FFXIVClientStructsDirect scanner + reader ----------------------
        SharlayanConfiguration directConfig = new() {
            ProcessModel = new ProcessModel { Process = game },
            ResourceProvider = ResourceProviderKind.FFXIVClientStructsDirect,
        };

        Log("[3] FFXIVCLIENTSTRUCTSDIRECT SCANNER ADDRESSES");
        MemoryHandler? directHandler = null;
        try {
            var directScanDone = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            directHandler = SharlayanMemoryManager.Instance.AddHandler(directConfig);
            directHandler.OnMemoryLocationsFound += (_, _, _) => directScanDone.TrySetResult(true);
            if (directHandler.Scanner.Locations.Count > 0) directScanDone.TrySetResult(true);

            Log("  Waiting for scanner (up to 30 s)…");
            await Task.WhenAny(directScanDone.Task, Task.Delay(30_000));

            int directCount = directHandler.Scanner.Locations.Count;
            Log($"  {directCount} location(s) resolved");
            // Address listing — commented out; re-enable to verify all scanner keys resolved.
            
            foreach (string key in directHandler.Scanner.Locations.Keys.OrderBy(k => k)) {
                IntPtr addr = directHandler.Scanner.Locations[key];
                Log($"    ✓ {key,-20} 0x{addr.ToInt64():X12}");
            }
            

            // [3b] End-to-end Reader check — commented out; re-enable to validate
            // CurrentPlayer / Actors / Party reads against the in-game UI.
            
            Log(string.Empty);
            Log("  [3b] READER OUTPUT");
            try {
                var dcp = directHandler.Reader.GetCurrentPlayer();
                var dp = dcp?.Entity;
                if (dp == null) {
                    Log("    Entity=null — PLAYERINFO signature missing or struct offsets broken");
                }
                else {
                    Log($"    ✓ Name   = \"{dp.Name}\"");
                    Log($"    ✓ HP     = {dp.HPCurrent}/{dp.HPMax}");
                    Log($"    ✓ MP     = {dp.MPCurrent}/{dp.MPMax}");
                    Log($"    ✓ Level  = {dp.Level}");
                    Log($"      Job={dp.Job} Pos=({dp.X:F2},{dp.Y:F2},{dp.Z:F2}) ID=0x{dp.ID:X8} MapTerritory={dp.MapTerritory}");
                }
            }
            catch (Exception ex) {
                Log($"    ✗ CurrentPlayer: {ex.GetType().Name}: {ex.Message}");
            }

            try {
                var dActors = directHandler.Reader.GetActors();
                int total = dActors.CurrentPCs.Count + dActors.CurrentNPCs.Count + dActors.CurrentMonsters.Count;
                string mark = total > 0 ? "✓" : "⚠";
                Log($"    {mark} Actors PCs={dActors.CurrentPCs.Count} NPCs={dActors.CurrentNPCs.Count} Mobs={dActors.CurrentMonsters.Count}");
            }
            catch (Exception ex) {
                Log($"    ✗ Actors: {ex.GetType().Name}: {ex.Message}");
            }

            try {
                var dparty = directHandler.Reader.GetPartyMembers();
                string mark = dparty.PartyMembers.Count >= 1 ? "✓" : "⚠";
                Log($"    {mark} Party count={dparty.PartyMembers.Count}");
            }
            catch (Exception ex) {
                Log($"    ✗ Party: {ex.GetType().Name}: {ex.Message}");
            }
            

            // [3c] + [3c.2] — extracted so the live-refresh loop below can call the
            // same renderer and keep its output pointing at fresh in-game values.
            RenderEyeCheckAndGameState(directHandler, Log);

            // [3d] Chat log — commented out; re-enable to validate Framework→UIModule→RaptureLogModule chain.
            
            Log(string.Empty);
            Log("  [3d] CHAT LOG");
            try {
                if (!directHandler.Reader.CanGetChatLog()) {
                    Log("    ✗ CanGetChatLog=false (CHATLOG signature didn't resolve)");
                }
                else {
                    var primed     = directHandler.Reader.GetChatLog();
                    var historical = directHandler.Reader.GetChatLog(0, 0);
                    await Task.Delay(1500);
                    var delta      = directHandler.Reader.GetChatLog(primed.PreviousArrayIndex, primed.PreviousOffset);

                    Log($"    ✓ historical: {historical.ChatLogItems.Count} items, +{delta.ChatLogItems.Count} new on 1.5 s poll");
                    if (historical.ChatLogItems.Count > 0) {
                        Log("    Last 2 entries (most recent first):");
                        foreach (var item in historical.ChatLogItems.Reverse().Take(2)) {
                            string line = (item.Line ?? string.Empty).Replace("\n", " ").Trim();
                            if (line.Length > 70) line = line.Substring(0, 67) + "…";
                            Log($"      [{item.TimeStamp:HH:mm:ss}] code={item.Code} line=\"{line}\"");
                        }
                    }
                }
            }
            catch (Exception ex) {
                Log($"    ✗ Chat read: {ex.GetType().Name}: {ex.Message}");
            }
            
        }
        catch (Exception ex) {
            Log($"  ✗ Direct scanner failed: {ex.GetType().Name}: {ex.Message}");
        }
        Log(string.Empty);

        // [4] Lumina xivdatabase smoke test — probes every sheet the rest of the codebase
        // depends on (xivdatabase + Reader.GameState) and reports each one independently.
        // A MismatchedColumnHashException on a single sheet is normal during the gap
        // between an FFXIV patch and a matching Lumina.Excel package release; this block
        // makes that gap explicit so we know exactly which sheet to wait on.

        Log("[4] LUMINA XIVDATABASE VALIDATION");
        global::Lumina.GameData? lumina = null;
        try {
            string sqpack = Path.Combine(Path.GetDirectoryName(game.MainModule!.FileName)!, "sqpack");
            Log($"  sqpack path : {sqpack}");
            Log($"  exists      : {Directory.Exists(sqpack)}");
            lumina = new global::Lumina.GameData(sqpack);
        }
        catch (Exception ex) {
            Log($"  ✗ GameData open failed: {ex.GetType().Name}: {ex.Message}");
        }

        if (lumina != null) {
            // Probe each typed sheet on its own. A MismatchedColumnHashException tells us
            // which sheet's column layout has shifted in the current game patch but not
            // yet in the Lumina.Excel package — that's the one blocking name lookups.
            void ProbeSheet<T>(string label) where T : struct, global::Lumina.Excel.IExcelRow<T> {
                try {
                    var sheet = lumina.Excel.GetSheet<T>();
                    Log($"  ✓ {label,-15} : {sheet.Count} rows");
                }
                catch (Exception ex) {
                    string msg = ex.Message.Replace("\n", " ").Trim();
                    if (msg.Length > 110) msg = msg.Substring(0, 107) + "…";
                    Log($"  ✗ {label,-15} : {ex.GetType().Name}: {msg}");
                }
            }

            // Sheets used by LuminaXivDatabaseProvider (xivdatabase actions/statuses/zones).
            ProbeSheet<global::Lumina.Excel.Sheets.Action>("Action");
            ProbeSheet<global::Lumina.Excel.Sheets.Status>("Status");
            ProbeSheet<global::Lumina.Excel.Sheets.TerritoryType>("TerritoryType");
            ProbeSheet<global::Lumina.Excel.Sheets.PlaceName>("PlaceName");
            // Sheets used by Reader.GameState (live weather + BGM + fade presets).
            ProbeSheet<global::Lumina.Excel.Sheets.Weather>("Weather");
            ProbeSheet<global::Lumina.Excel.Sheets.BGM>("BGM");
            ProbeSheet<global::Lumina.Excel.Sheets.BGMFade>("BGMFade");
            ProbeSheet<global::Lumina.Excel.Sheets.BGMFadeType>("BGMFadeType");

            // Sample lookups — best-effort, only run when the relevant sheets resolved.
            try {
                var action = lumina.Excel.GetSheet<global::Lumina.Excel.Sheets.Action>();
                if (action.HasRow(7)) {
                    Log($"    Action[7].Name     : \"{action.GetRow(7).Name.ExtractText()}\"");
                }
            }
            catch { }
            if (directHandler != null) {
                try {
                    var cp = directHandler.Reader.GetCurrentPlayer();
                    uint tid = cp?.Entity?.MapTerritory ?? 0;
                    if (tid != 0) {
                        var territory = lumina.Excel.GetSheet<global::Lumina.Excel.Sheets.TerritoryType>();
                        var place     = lumina.Excel.GetSheet<global::Lumina.Excel.Sheets.PlaceName>();
                        if (territory.HasRow(tid)) {
                            uint pnId = territory.GetRow(tid).PlaceName.RowId;
                            if (place.HasRow(pnId)) {
                                Log($"    Current territory[{tid}].PlaceName = \"{place.GetRow(pnId).Name.ExtractText()}\"");
                            }
                        }
                    }
                }
                catch { /* one of the sheets above failed to resolve; already logged */ }
            }
        }
        Log(string.Empty);


        Log("====== END HARNESS ======");

        if (outFilePath != null) {
            await File.WriteAllTextAsync(outFilePath, report.ToString());
            Console.WriteLine($"\nReport written to {outFilePath}");
        }

        // Live refresh — the one-shot report above is the snapshot for the file;
        // this loop lets the user watch eye-check values change in-game. Skipped
        // when --once is passed (CI / single-shot usage).
        if (runOnce || directHandler == null) {
            return 0;
        }

        await RunLiveRefreshLoop(directHandler, liveIntervalMs);
        return 0;
    }

    private static async Task RunLiveRefreshLoop(MemoryHandler handler, int intervalMs) {
        using CancellationTokenSource cts = new();
        ConsoleCancelEventHandler onCancel = (_, e) => {
            e.Cancel = true; // suppress default termination; exit loop cleanly
            cts.Cancel();
        };
        Console.CancelKeyPress += onCancel;

        Console.WriteLine();
        Console.WriteLine($"──── LIVE EYE-CHECK (refresh {intervalMs} ms — Ctrl+C to exit) ────");

        LiveRenderer renderer = new LiveRenderer(Console.CursorTop);

        try {
            while (!cts.IsCancellationRequested) {
                Action<string> renderLine = renderer.Begin();
                renderLine($"tick {DateTime.Now:HH:mm:ss.fff}");
                try {
                    RenderEyeCheckAndGameState(handler, renderLine);
                }
                catch (Exception ex) {
                    renderLine($"  ✗ live render failed: {ex.GetType().Name}: {ex.Message}");
                }
                renderer.End();

                try {
                    await Task.Delay(intervalMs, cts.Token);
                }
                catch (TaskCanceledException) {
                    break;
                }
            }
        }
        finally {
            Console.CancelKeyPress -= onCancel;
            // Drop the cursor below the live block so the shell prompt doesn't overwrite it.
            Console.WriteLine();
            Console.WriteLine("──── live refresh stopped ────");
        }
    }

    /// <summary>
    /// Draws the [3c.2] GameState block. Called once from the one-shot report pass
    /// (writes to console + report file) and then repeatedly from the live-refresh
    /// loop (writes padded lines in place). The earlier [3c] eye-check section is
    /// kept in source but commented out so the harness focuses on GameState only.
    /// </summary>
    private static void RenderEyeCheckAndGameState(MemoryHandler handler, Action<string> log) {
        // [3c] Eye-check values — commented out; re-enable to surface LocalPlayer / actors /
        // target / hotbar / job-resource fields for manual UI verification.
        
        try {
            var dp = handler.Reader.GetCurrentPlayer()?.Entity;
            if (dp != null) {
                log(string.Empty);
                log("  [3c] EYE-CHECK VALUES");
                log($"    LocalPlayer.Name       = \"{dp.Name}\"  Job={dp.Job}  Lv={dp.Level}  HP={dp.HPCurrent}/{dp.HPMax}  MP={dp.MPCurrent}/{dp.MPMax}");
                log($"    LocalPlayer.Pos        = ({dp.X:F2},{dp.Y:F2},{dp.Z:F2})  Heading={dp.Heading:F2}  MapTerritory={dp.MapTerritory}");
                log($"    LocalPlayer.TargetID   = 0x{dp.TargetID:X8}   (0 when not targeting)");
                log($"    LocalPlayer.ClaimedByID= 0x{dp.ClaimedByID:X8}   (combat-tag owner; 0 when not in combat)");
                log($"    LocalPlayer.NPCID1     = {dp.NPCID1}");
                log($"    LocalPlayer.NPCID2     = {dp.NPCID2}   (NameId — 0 for players, row id for NPCs)");
                log($"    LocalPlayer.Type       = {dp.Type}   (1=Pc, 2=BattleNpc, 3=EventNpc, 4=Treasure, ...)");
                log($"    LocalPlayer.TargetFlags= {dp.TargetFlags}   (targetability; see ObjectTargetableFlags)");
                log($"    LocalPlayer.Title      = {dp.Title}   (TitleId — compare against /title list)");
                log($"    LocalPlayer.Icon       = {dp.Icon}   (nameplate icon id)");
                log($"    LocalPlayer.HitBoxRadius={dp.HitBoxRadius}");
                log($"    LocalPlayer.IsAgroed   = {dp.IsAgroed}   (CharacterData.Flags bit 1 = InCombat; True when engaged)");
                log($"    LocalPlayer.InCombat   = {dp.InCombat}   (same byte, same bit — should match IsAgroed for local player)");
                log($"    LocalPlayer.IsAggressive= {dp.IsAggressive}   (bit 0 = IsHostile)");
                log($"    LocalPlayer.AgroFlags  = 0x{dp.AgroFlags:X2} CombatFlags=0x{dp.CombatFlags:X2}   (bit0=IsHostile bit1=InCombat)");
                log($"    LocalPlayer.InCutscene = {dp.InCutscene}   (ActorItem.InCutscene — RenderFlags bit; true for cutscenes AND teleports)");

                try {
                    if (handler.Scanner.Locations.TryGetValue(Sharlayan.Signatures.CHARMAP_KEY, out var charmap)) {
                        long firstActorAddr = handler.GetInt64(charmap);
                        if (firstActorAddr != 0) {
                            IntPtr actor = new IntPtr(firstActorAddr);
                            byte tgtStatus      = handler.GetByte(actor, 0x95);
                            byte targetable     = handler.GetByte(actor, 0x9A);
                            byte renderFlagsLo  = handler.GetByte(actor, 0x118);
                            byte renderFlagsHi  = handler.GetByte(actor, 0x119);
                            log($"    LocalPlayer raw @ CHARMAP[0]: TargetStatus@0x95=0x{tgtStatus:X2} TargetableStatus@0x9A=0x{targetable:X2} RenderFlags@0x118=0x{renderFlagsLo:X2} RenderFlags@0x119=0x{renderFlagsHi:X2}");
                        }
                    }
                }
                catch (Exception ex) {
                    log($"    ✗ Raw actor byte dump: {ex.GetType().Name}: {ex.Message}");
                }
            }

            var dActors2 = handler.Reader.GetActors();
            var sample = dActors2.CurrentNPCs.Values.Take(3).ToList();
            foreach (var mob in dActors2.CurrentMonsters.Values.Take(2)) sample.Add(mob);
            if (sample.Count > 0) {
                log($"    Sample actors ({sample.Count}):");
                foreach (var a in sample) {
                    log($"      - {a.Name,-24} Type={a.Type} ID=0x{a.ID:X8} HP={a.HPCurrent}/{a.HPMax} IsAgroed={a.IsAgroed} AgroFlags=0x{a.AgroFlags:X2}");
                }
            }

            try {
                var tr = handler.Reader.GetTargetInfo();
                var ti = tr?.TargetInfo;
                if (ti == null) {
                    log("    CurrentTarget: (TargetInfo null)");
                }
                else {
                    void DumpTarget(string label, Sharlayan.Core.ActorItem? t) {
                        if (t == null) { log($"    {label}: (none)"); return; }
                        log($"    {label}: \"{t.Name}\" Type={t.Type} ID=0x{t.ID:X8} HP={t.HPCurrent}/{t.HPMax}");
                        log($"      IsAgroed={t.IsAgroed}  AgroFlags=0x{t.AgroFlags:X2}  CombatFlags=0x{t.CombatFlags:X2}  ClaimedByID=0x{t.ClaimedByID:X8}");
                        log($"      IsCasting={t.IsCasting1}  CastingID={t.CastingID}  CastingProgress={t.CastingProgress:F2}/{t.CastingTime:F2}s  CastTargetID=0x{t.CastingTargetID:X8}");
                        log($"      Pos=({t.X:F1},{t.Y:F1},{t.Z:F1})  TargetID=0x{t.TargetID:X8}  NPCID2={t.NPCID2}");
                    }
                    DumpTarget("CurrentTarget", ti.CurrentTarget);
                    if (ti.FocusTarget != null) DumpTarget("FocusTarget",   ti.FocusTarget);
                    if (ti.MouseOverTarget != null) DumpTarget("MouseOverTarget", ti.MouseOverTarget);
                    if (ti.CurrentTarget == null && ti.FocusTarget == null && ti.MouseOverTarget == null) {
                        log($"    CurrentTargetID = 0x{ti.CurrentTargetID:X8}  (no resolved target — target offscreen or between actor frames)");
                    }
                    if (ti.EnmityItems.Count > 0) {
                        log($"    EnmityItems ({ti.EnmityItems.Count}):");
                        foreach (var e in ti.EnmityItems) {
                            log($"      ID=0x{e.ID:X8}  Enmity={e.Enmity,8}  Name=\"{e.Name}\"");
                        }
                    }
                }
            }
            catch (Exception ex) {
                log($"    ✗ Target: {ex.GetType().Name}: {ex.Message}");
            }

            if (handler.Reader.CanGetActions()) {
                var actions = handler.Reader.GetActions();
                foreach (var target in new[] { Sharlayan.Core.Enums.Action.Container.HOTBAR_1, Sharlayan.Core.Enums.Action.Container.HOTBAR_2 }) {
                    var bar = actions.ActionContainers.FirstOrDefault(c => c.ContainerType == target);
                    if (bar == null || bar.ActionItems.Count == 0) {
                        log($"    Hotbar {target}: (no populated slots)");
                        continue;
                    }
                    log($"    Hotbar {target} ({bar.ActionItems.Count} populated slots):");
                    foreach (var item in bar.ActionItems) {
                        string keybind = string.IsNullOrEmpty(item.KeyBinds) ? "<unbound>" : item.KeyBinds;
                        log($"      slot {item.Slot}: \"{item.Name}\" ID={item.ID} keybind=\"{keybind}\" Avail={item.IsAvailable} InRange={item.InRange} CD={item.CoolDownPercent}% Proc={item.IsProcOrCombo} Charges={item.ChargeReady}/{item.ChargesRemaining}");
                    }
                }
            }
            else {
                log("    Hotbar: CanGetActions=false (HOTBAR or RECAST signature missing)");
            }

            try {
                if (handler.Reader.CanGetJobResources()) {
                    var jr = handler.Reader.GetJobResources();
                    var c = jr?.JobResourcesContainer;
                    if (c != null) {
                        log($"    JobResources populated=true  (current job: {dp?.Job})");
                        log($"      Bard: ActiveSong={c.Bard.ActiveSong} Timer={c.Bard.Timer}ms Repertoire={c.Bard.Repertoire} SoulVoice={c.Bard.SoulVoice} RadiantFinaleCoda=0x{c.Bard.RadiantFinaleCoda:X2}(b{c.Bard.RadiantFinaleCoda:b3})");
                    }
                    else {
                        log("    JobResources: Container is null (reader returned empty)");
                    }
                }
                else {
                    log("    JobResources: CanGetJobResources=false (JOBRESOURCES signature missing)");
                }
            }
            catch (Exception ex) {
                log($"    ✗ JobResources: {ex.GetType().Name}: {ex.Message}");
            }
        }
        catch (Exception ex) {
            log($"    ✗ EyeCheck: {ex.GetType().Name}: {ex.Message}");
        }

        // [3c.2] GameState — validates GAMEMAIN / CONDITIONS / CONTENTSFINDER / WEATHER /
        // BGMSYSTEM signatures + Lumina name lookups in one shot.
        try {
            if (handler.Reader.CanGetGameState()) {
                var gs = handler.Reader.GetGameState();
                log(string.Empty);
                log("  [3c.2] GAMESTATE");
                log($"    IsLoggedIn={gs.IsLoggedIn}  IsReadyToRead={gs.IsReadyToRead}  TerritoryLoadState={gs.TerritoryLoadState}  (1=loading,2=loaded,3=unloading)");
                log($"    WatchingCutscene={gs.WatchingCutscene}  IsTeleporting={gs.IsTeleporting}");
                log($"    ContentsFinderQueueState={gs.ContentsFinderQueueState}  (DutyFinderBellPopped={gs.DutyFinderBellPopped}  InInstance={gs.InInstance})");
                log($"    Weather id={gs.CurrentWeatherId}  name=\"{gs.CurrentWeatherName ?? "(no lumina)"}\"");
                log($"    BGM playing={gs.CurrentBgmId}  target={gs.CurrentBgmTargetId}  scene={gs.CurrentBgmSceneId}  file=\"{gs.CurrentBgmFile ?? "(no lumina)"}\"");
                log($"    BGM fadeIn={gs.BgmFadeIn:F2}s  resume={gs.BgmResume:F2}s   (highest non-zero preset across all scenes; fadeIn = delay before audio starts on rising edge)");
                log($"    IsBgmAudible={gs.IsBgmAudible}   (post-fader — speaker output; muting BGM in-game keeps this false)");
                if (gs.BgmScenes != null) {
                    log($"    Scenes ({gs.BgmScenes.Count}):");
                    foreach (var s in gs.BgmScenes) {
                        string playing = s.IsPlaying ? s.PlayingBgmId.ToString() : "-";
                        string target  = s.BgmId == 0 ? "-" : s.BgmId.ToString();
                        string file    = s.PlayingBgmFile ?? "";
                        log($"      [{s.Index,2}] {s.SceneType,-10}  playing={playing,-5}  target={target,-5}  isPlaying={s.IsPlaying,-5}  file=\"{file}\"");
                        // Fade row: custom override first (only set when EnableCustomFade=true),
                        // then the Lumina preset fallback. PresetFadeInStartTime is the
                        // delay-before-audio-starts the user is chasing.
                        log($"           custom: enable={s.EnableCustomFade}  fadeOut={s.FadeOutTime}ms  fadeIn={s.FadeInTime}ms");
                        log($"           preset: fadeOut={s.PresetFadeOutTime:F2}s  fadeIn={s.PresetFadeInTime:F2}s  fadeInStart={s.PresetFadeInStartTime:F2}s  resume={s.PresetResumeFadeInTime:F2}s");
                    }
                }
            }
            else {
                log("    GameState: CanGetGameState=false (GAMEMAIN signature didn't resolve)");
            }
        }
        catch (Exception ex) {
            log($"    ✗ GameState: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static int ParseIntArg(string[] args, string prefix, int fallback) {
        string? v = args.FirstOrDefault(a => a.StartsWith(prefix))?.Substring(prefix.Length);
        return int.TryParse(v, out int n) && n > 0 ? n : fallback;
    }

    private static void DumpSize<T>(Action<string> log, int? expected = null) where T : unmanaged {
        int actual = Marshal.SizeOf<T>();
        string marker = expected is null ? "-" : (actual == expected.Value ? "✓" : "⚠");
        string exp = expected is null ? string.Empty : $" (declared 0x{expected:X})";
        log($"  {marker} {typeof(T).Name,-35} sizeof=0x{actual:X}{exp}");
    }

    /// <summary>
    /// In-place line renderer for the live-refresh loop. Captures the cursor row
    /// once, then on each frame seeks back to it and overwrites lines with
    /// space-padded content. Pads trailing slots if the current frame is shorter
    /// than the previous, so ghost lines from a longer prior frame are cleared.
    /// Each emitted line is truncated to Console.WindowWidth - 1 to prevent
    /// terminal wrap (which would throw off row math for subsequent lines).
    /// </summary>
    private sealed class LiveRenderer {
        private readonly int _startRow;
        private int _previousLineCount;
        private int _thisFrameLineCount;

        public LiveRenderer(int startRow) {
            _startRow = startRow;
        }

        public Action<string> Begin() {
            try { Console.SetCursorPosition(0, _startRow); } catch { /* buffer scrolled; give up seeking */ }
            Console.CursorVisible = false;
            _thisFrameLineCount = 0;
            return WriteLine;
        }

        public void End() {
            for (int i = _thisFrameLineCount; i < _previousLineCount; i++) {
                WriteLine(string.Empty);
            }
            _previousLineCount = Math.Max(_previousLineCount, _thisFrameLineCount);
            Console.CursorVisible = true;
        }

        private void WriteLine(string line) {
            int width = Math.Max(1, Console.WindowWidth - 1);
            if (line.Length > width) {
                line = line.Substring(0, width);
            }
            Console.Write(line.PadRight(width));
            Console.WriteLine();
            _thisFrameLineCount++;
        }
    }
}
