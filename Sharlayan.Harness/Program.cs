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
            ResourceProvider = ResourceProviderKind.LegacySharlayanResources,
            UseLocalCache = false,
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
        Log("[4] LEGACY PROVIDER — MEMORY HANDLER + READER");
        MemoryHandler? handler = null;
        try {
            handler = SharlayanMemoryManager.Instance.AddHandler(legacyConfig);
            // Give Scanner time to pattern-scan.
            for (int i = 0; i < 20 && handler.Scanner.IsScanning; i++) {
                await Task.Delay(500);
            }
            Log($"  ✓ Attached handler; Scanner locations found: {handler.Scanner.Locations.Count}");
            foreach (string key in handler.Scanner.Locations.Keys.OrderBy(k => k)) {
                IntPtr addr = handler.Scanner.Locations[key];
                Log($"    {key,-15} 0x{addr.ToInt64():X}");
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
                Check("  Actors.CurrentPCs >= 1 (self)", pcs >= 1, Log);
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
            Log(string.Empty);
        }

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
