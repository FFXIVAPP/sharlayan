// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Validation harness for Sharlayan v9. Run while FFXIV is running; produces a
//   structured report validating the FFXIVClientStructsDirect provider against
//   live game state and Lumina xivdatabase content.
//   Intended to be pasted back to the session that's developing the refactor.
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
            foreach (string key in directHandler.Scanner.Locations.Keys.OrderBy(k => k)) {
                IntPtr addr = directHandler.Scanner.Locations[key];
                Log($"    ✓ {key,-20} 0x{addr.ToInt64():X12}");
            }

            // [3b] End-to-end Reader check. Addresses in [3] resolving correctly is
            // necessary but not sufficient — the Reader also needs the StructuresContainer
            // offsets to align with the live game layout. This block reads the same fields
            // Chromatics consumes and logs them for eye-verification against the in-game UI.
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

            // [3c] Eye-check values — read fields from the local player and nearby actors
            // so the developer can compare against the in-game UI. These need manual verification.
            try {
                var dp = directHandler.Reader.GetCurrentPlayer()?.Entity;
                if (dp != null) {
                    Log(string.Empty);
                    Log("  [3c] EYE-CHECK VALUES");
                    Log($"    LocalPlayer.TargetID   = 0x{dp.TargetID:X8}   (0 when not targeting)");
                    Log($"    LocalPlayer.ClaimedByID= 0x{dp.ClaimedByID:X8}   (combat-tag owner; 0 when not in combat)");
                    Log($"    LocalPlayer.NPCID1     = {dp.NPCID1}");
                    Log($"    LocalPlayer.NPCID2     = {dp.NPCID2}   (NameId — 0 for players, row id for NPCs)");
                    Log($"    LocalPlayer.Type       = {dp.Type}   (1=Pc, 2=BattleNpc, 3=EventNpc, 4=Treasure, ...)");
                    Log($"    LocalPlayer.TargetFlags= {dp.TargetFlags}   (targetability; see ObjectTargetableFlags)");
                    Log($"    LocalPlayer.Title      = {dp.Title}   (TitleId — compare against /title list)");
                    Log($"    LocalPlayer.Icon       = {dp.Icon}   (nameplate icon id)");
                    Log($"    LocalPlayer.HitBoxRadius={dp.HitBoxRadius}");
                    Log($"    LocalPlayer.IsAgroed   = {dp.IsAgroed}   (CharacterData.Flags bit 1 = InCombat; True when engaged)");
                    Log($"    LocalPlayer.InCombat   = {dp.InCombat}   (same byte, same bit — should match IsAgroed for local player)");
                    Log($"    LocalPlayer.IsAggressive= {dp.IsAggressive}   (bit 0 = IsHostile)");
                    Log($"    LocalPlayer.AgroFlags  = 0x{dp.AgroFlags:X2} CombatFlags=0x{dp.CombatFlags:X2}   (bit0=IsHostile bit1=InCombat)");
                    Log($"    LocalPlayer.InCutscene = {dp.InCutscene}   (ActorItem.InCutscene — currently unmapped in direct, see raw RenderFlags below)");

                    try {
                        if (directHandler.Scanner.Locations.TryGetValue(Sharlayan.Signatures.CHARMAP_KEY, out var charmap)) {
                            long firstActorAddr = directHandler.GetInt64(charmap);
                            if (firstActorAddr != 0) {
                                IntPtr actor = new IntPtr(firstActorAddr);
                                byte tgtStatus      = directHandler.GetByte(actor, 0x95);
                                byte targetable     = directHandler.GetByte(actor, 0x9A);
                                byte renderFlagsLo  = directHandler.GetByte(actor, 0x118);
                                byte renderFlagsHi  = directHandler.GetByte(actor, 0x119);
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
                var sample = dActors2.CurrentNPCs.Values.Take(3).ToList();
                foreach (var mob in dActors2.CurrentMonsters.Values.Take(2)) sample.Add(mob);
                if (sample.Count > 0) {
                    Log($"    Sample actors ({sample.Count}):");
                    foreach (var a in sample) {
                        Log($"      - {a.Name,-24} Type={a.Type} ID=0x{a.ID:X8} HP={a.HPCurrent}/{a.HPMax} IsAgroed={a.IsAgroed} AgroFlags=0x{a.AgroFlags:X2}");
                    }
                }

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
                        if (ti.EnmityItems.Count > 0) {
                            Log($"    EnmityItems ({ti.EnmityItems.Count}):");
                            foreach (var e in ti.EnmityItems) {
                                Log($"      ID=0x{e.ID:X8}  Enmity={e.Enmity,8}  Name=\"{e.Name}\"");
                            }
                        }
                        else {
                            Log($"    EnmityItems: (none — target a mob while in combat to populate)");
                        }
                    }
                }
                catch (Exception ex) {
                    Log($"    ✗ Target: {ex.GetType().Name}: {ex.Message}");
                }

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

            // [3c.2] GameState — validates GAMEMAIN / CONDITIONS / CONTENTSFINDER / WEATHER /
            // BGMSYSTEM signatures + Lumina name lookups in one shot.
            try {
                if (directHandler.Reader.CanGetGameState()) {
                    var gs = directHandler.Reader.GetGameState();
                    Log(string.Empty);
                    Log("  [3c.2] GAMESTATE");
                    Log($"    IsLoggedIn={gs.IsLoggedIn}  IsReadyToRead={gs.IsReadyToRead}  TerritoryLoadState={gs.TerritoryLoadState}  (1=loading,2=loaded,3=unloading)");
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

            // [3d] Chat log — validates Framework→UIModule→RaptureLogModule multi-hop
            // signature AND FCS-derived ChatLogPointers offsets. First call primes the cursor;
            // (0,0) replays the historical ring buffer; last poll proves the cursor advances.
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

        // [4] Lumina xivdatabase smoke test -----------------------------------
        Log("[4] LUMINA XIVDATABASE VALIDATION");
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

            if (action.HasRow(7)) {
                Log($"    Action[7].Name     : \"{action.GetRow(7).Name.ExtractText()}\"");
            }
            if (directHandler != null) {
                try {
                    var cp = directHandler.Reader.GetCurrentPlayer();
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
