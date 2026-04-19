// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.GameState.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Lightweight "what is the game doing right now" reader. Pulls session, cutscene,
//   duty-finder, weather, and music state from FFXIVClientStructs singletons
//   (GameMain / Conditions / ContentsFinder / WeatherManager / BGMSystem). Name lookups
//   for weather & BGM resolve via Lumina against the installed sqpack; if the sqpack
//   path can't be derived, the IDs are still returned and the Name / File fields stay
//   null.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.IO;

    using Sharlayan.Models.ReadResults;

    public partial class Reader {
        // Lumina GameData is lazily constructed on first GetGameState call and cached
        // for the Reader's lifetime. `_luminaAttempted` prevents re-probing the sqpack
        // path every frame when GameInstallPath is unset or the sqpack directory can't
        // be located.
        private Lumina.GameData _luminaGameData;
        private bool _luminaAttempted;

        public bool CanGetGameState() {
            // At minimum we need GameMain to read territory-load state. The other keys are
            // optional — the result's Current* fields fall back to defaults if a key didn't
            // scan.
            return this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.GAMEMAIN_KEY);
        }

        public GameStateResult GetGameState() {
            GameStateResult result = new GameStateResult();
            if (!this._memoryHandler.IsAttached) {
                return result;
            }

            var locations = this._memoryHandler.Scanner.Locations;

            // --- GameMain: TerritoryLoadState @ +0x4100 (1=loading, 2=loaded, 3=unloading) ---
            if (locations.ContainsKey(Signatures.GAMEMAIN_KEY)) {
                IntPtr gameMain = locations[Signatures.GAMEMAIN_KEY];
                try { result.TerritoryLoadState = this._memoryHandler.GetByte(gameMain, 0x4100); } catch { /* ignore, leave default */ }
            }

            // IsLoggedIn: CurrentPlayer.Entity resolves with a non-zero entity ID.
            try {
                var cp = this.GetCurrentPlayer()?.Entity;
                result.IsLoggedIn = cp != null && cp.ID != 0u;
            }
            catch { /* not-yet-loaded characters raise; treat as logged-out */ }
            // IsReadyToRead: territory is loaded AND the scanner resolved the actor pointer array.
            result.IsReadyToRead = result.TerritoryLoadState == 2 && locations.ContainsKey(Signatures.CHARMAP_KEY);

            // --- Conditions: FCS exposes three cutscene-related flags on the same struct.
            //   +35 OccupiedInCutSceneEvent — explicit cutscene-event condition (quest/story/etc).
            //   +58 WatchingCutscene        — generic "watching a cutscene" flag.
            //   +78 WatchingCutscene78      — secondary flag set during instance cutscenes.
            // OR all three so short unskippable cutscenes (which only trip one) still register.
            if (locations.ContainsKey(Signatures.CONDITIONS_KEY)) {
                IntPtr conditions = locations[Signatures.CONDITIONS_KEY];
                try {
                    byte occCutscene = this._memoryHandler.GetByte(conditions, 35);
                    byte watching58  = this._memoryHandler.GetByte(conditions, 58);
                    byte watching78  = this._memoryHandler.GetByte(conditions, 78);
                    result.WatchingCutscene = occCutscene != 0 || watching58 != 0 || watching78 != 0;
                }
                catch { /* ignore */ }
            }

            // --- ContentsFinder.QueueInfo.QueueState @ +0x75 (QueueInfo @ ContentsFinder+0x20, QueueState @ QueueInfo+0x55) ---
            // FCS ContentsFinderQueueState: 0=None, 1=Pending, 2=Queued, 3=Ready (popped!), 4=Accepted, 5=InContent.
            if (locations.ContainsKey(Signatures.CONTENTSFINDER_KEY)) {
                IntPtr contentsFinder = locations[Signatures.CONTENTSFINDER_KEY];
                try {
                    byte state = this._memoryHandler.GetByte(contentsFinder, 0x20 + 0x55);
                    result.ContentsFinderQueueState = state;
                    result.DutyFinderBellPopped = state == 3; // Ready — DF popped but not yet Accepted.
                    result.InInstance = state >= 4;           // Accepted (entering) or InContent.
                }
                catch { /* ignore */ }
            }

            // --- WeatherManager.WeatherId @ +0x64 (byte, row id in the Weather sheet) ---
            if (locations.ContainsKey(Signatures.WEATHER_KEY)) {
                IntPtr weather = locations[Signatures.WEATHER_KEY];
                try { result.CurrentWeatherId = this._memoryHandler.GetByte(weather, 0x64); } catch { /* ignore */ }
            }

            // --- BGMSystem.Scenes StdVector @ +0xC0, iterate 0..N ---
            // Scene layout: SceneId @ +0x00, BgmId @ +0x0C, PlayingBgmId @ +0x0E (ushort),
            // stride 0x60. Scenes are priority-ordered — scene 0 (Event) wins over scene 11
            // (Territory), so first non-zero PlayingBgmId is what the user is hearing.
            if (locations.ContainsKey(Signatures.BGMSYSTEM_KEY)) {
                IntPtr bgmSystem = locations[Signatures.BGMSYSTEM_KEY];
                try {
                    long first = this._memoryHandler.GetInt64(bgmSystem, 0xC0);
                    long last  = this._memoryHandler.GetInt64(bgmSystem, 0xC0 + 8);
                    if (first != 0 && last > first) {
                        int sceneCount = (int)((last - first) / 0x60);
                        if (sceneCount > 12) sceneCount = 12; // cap — 12 documented scene types.
                        for (int i = 0; i < sceneCount; i++) {
                            IntPtr sceneAddr = new IntPtr(first + i * 0x60);
                            ushort playing = this._memoryHandler.GetUInt16(sceneAddr, 0x0E);
                            if (playing != 0) {
                                result.CurrentBgmId = playing;
                                result.CurrentBgmSceneId = i;
                                break;
                            }
                        }
                    }
                }
                catch { /* ignore */ }
            }

            // --- Lumina name resolution (best-effort; silently skipped if sqpack not found) ---
            try {
                Lumina.GameData lumina = this.GetLumina();
                if (lumina != null) {
                    if (result.CurrentWeatherId != 0) {
                        var weatherSheet = lumina.Excel.GetSheet<Lumina.Excel.Sheets.Weather>();
                        if (weatherSheet.HasRow(result.CurrentWeatherId)) {
                            result.CurrentWeatherName = weatherSheet.GetRow(result.CurrentWeatherId).Name.ExtractText();
                        }
                    }
                    if (result.CurrentBgmId != 0) {
                        var bgmSheet = lumina.Excel.GetSheet<Lumina.Excel.Sheets.BGM>();
                        if (bgmSheet.HasRow(result.CurrentBgmId)) {
                            result.CurrentBgmFile = bgmSheet.GetRow(result.CurrentBgmId).File.ExtractText();
                        }
                    }
                }
            }
            catch { /* Lumina failures shouldn't break the numeric result */ }

            return result;
        }

        private Lumina.GameData GetLumina() {
            if (this._luminaAttempted) {
                return this._luminaGameData;
            }
            this._luminaAttempted = true;
            try {
                string sqpack = ResolveSqpackPath(this._memoryHandler.Configuration);
                if (sqpack != null) {
                    this._luminaGameData = new Lumina.GameData(sqpack);
                }
            }
            catch { /* leave cache null; subsequent calls return null without retrying */ }
            return this._luminaGameData;
        }

        // Mirrors LuminaXivDatabaseProvider.ResolveSqpackPath but returns null on any failure
        // instead of throwing — GetGameState must work even without xivdatabase access.
        private static string ResolveSqpackPath(SharlayanConfiguration configuration) {
            if (!string.IsNullOrWhiteSpace(configuration?.GameInstallPath)) {
                string gameSqpack = Path.Combine(configuration.GameInstallPath, "game", "sqpack");
                if (Directory.Exists(gameSqpack)) return gameSqpack;
                string rootSqpack = Path.Combine(configuration.GameInstallPath, "sqpack");
                if (Directory.Exists(rootSqpack)) return rootSqpack;
            }
            string exePath = configuration?.ProcessModel?.Process?.MainModule?.FileName;
            if (string.IsNullOrWhiteSpace(exePath)) return null;
            string gameDir = Path.GetDirectoryName(exePath);
            if (string.IsNullOrWhiteSpace(gameDir)) return null;
            string guess = Path.Combine(gameDir, "sqpack");
            return Directory.Exists(guess) ? guess : null;
        }
    }
}
