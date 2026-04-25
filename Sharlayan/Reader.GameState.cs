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

    using FCSGame = FFXIVClientStructs.FFXIV.Client.Game;
    using FCSGameUI = FFXIVClientStructs.FFXIV.Client.Game.UI;
    using FCSSound = FFXIVClientStructs.FFXIV.Client.Sound;

    using Sharlayan.Models.ReadResults;
    using Sharlayan.Resources.Mappers;
    using Sharlayan.Resources.Providers;

    public partial class Reader {
        // Every inner offset used by GetGameState is derived at class init via
        // FieldOffsetReader reading [FieldOffset] attributes on FCS structs. Caching as
        // static readonly avoids reflecting on every frame — GetGameState is typically
        // polled at UI refresh rates.
        private static readonly int GameMainTerritoryLoadStateOffset = FieldOffsetReader.OffsetOf<FCSGame.GameMain>(nameof(FCSGame.GameMain.TerritoryLoadState));
        private static readonly int ConditionsOccupiedInCutSceneEventOffset = FieldOffsetReader.OffsetOf<FCSGame.Conditions>(nameof(FCSGame.Conditions.OccupiedInCutSceneEvent));
        private static readonly int ConditionsWatchingCutsceneOffset = FieldOffsetReader.OffsetOf<FCSGame.Conditions>(nameof(FCSGame.Conditions.WatchingCutscene));
        private static readonly int ConditionsWatchingCutscene78Offset = FieldOffsetReader.OffsetOf<FCSGame.Conditions>(nameof(FCSGame.Conditions.WatchingCutscene78));
        private static readonly int ConditionsBetweenAreasOffset = FieldOffsetReader.OffsetOf<FCSGame.Conditions>(nameof(FCSGame.Conditions.BetweenAreas));
        private static readonly int ConditionsBetweenAreas51Offset = FieldOffsetReader.OffsetOf<FCSGame.Conditions>(nameof(FCSGame.Conditions.BetweenAreas51));
        private static readonly int ContentsFinderQueueStateOffset =
            FieldOffsetReader.OffsetOf<FCSGameUI.ContentsFinder>(nameof(FCSGameUI.ContentsFinder.QueueInfo)) +
            FieldOffsetReader.OffsetOf<FCSGameUI.ContentsFinderQueueInfo>(nameof(FCSGameUI.ContentsFinderQueueInfo.QueueState));
        private static readonly int WeatherManagerWeatherIdOffset = FieldOffsetReader.OffsetOf<FCSGame.WeatherManager>(nameof(FCSGame.WeatherManager.WeatherId));
        private static readonly int BGMSystemScenesOffset = FieldOffsetReader.OffsetOf<FCSGame.BGMSystem>(nameof(FCSGame.BGMSystem.Scenes));
        private static readonly int BGMSceneSize = FieldOffsetReader.SizeOf<FCSGame.BGMSystem.Scene>();
        private static readonly int BGMScenePlayingBgmIdOffset = FieldOffsetReader.OffsetOf<FCSGame.BGMSystem.Scene>(nameof(FCSGame.BGMSystem.Scene.PlayingBgmId));
        private static readonly int BGMSceneBgmIdOffset = FieldOffsetReader.OffsetOf<FCSGame.BGMSystem.Scene>(nameof(FCSGame.BGMSystem.Scene.BgmId));
        private static readonly int BGMScenePreviousBgmIdOffset = FieldOffsetReader.OffsetOf<FCSGame.BGMSystem.Scene>(nameof(FCSGame.BGMSystem.Scene.PreviousBgmId));
        private static readonly int BGMScenePlayStateOffset = FieldOffsetReader.OffsetOf<FCSGame.BGMSystem.Scene>(nameof(FCSGame.BGMSystem.Scene.PlayState));
        private static readonly int BGMSceneEnableCustomFadeOffset = FieldOffsetReader.OffsetOf<FCSGame.BGMSystem.Scene>(nameof(FCSGame.BGMSystem.Scene.EnableCustomFade));
        private static readonly int BGMSceneFadeOutTimeOffset = FieldOffsetReader.OffsetOf<FCSGame.BGMSystem.Scene>(nameof(FCSGame.BGMSystem.Scene.FadeOutTime));
        private static readonly int BGMSceneFadeInTimeOffset = FieldOffsetReader.OffsetOf<FCSGame.BGMSystem.Scene>(nameof(FCSGame.BGMSystem.Scene.FadeInTime));
        private static readonly int SoundManagerActiveSoundDataListHeadOffset = FieldOffsetReader.OffsetOf<FCSSound.SoundManager>(nameof(FCSSound.SoundManager.ActiveSoundDataListHead));
        private static readonly int ISoundDataNextOffset = FieldOffsetReader.OffsetOf<FCSSound.ISoundData>(nameof(FCSSound.ISoundData.Next));
        private static readonly int SoundDataIsLoadingSoundResourceOffset = FieldOffsetReader.OffsetOf<FCSSound.SoundData>(nameof(FCSSound.SoundData.IsLoadingSoundResource));
        // FCS labels SoundManager's FFT arrays by colour: Red=System, Green=SE/Voice/Instruments,
        // Blue=Music. Each is a 256-element float array of frequency-domain energy refreshed
        // every game frame; Blue1 carries the post-mix output for the music bus, so reading any
        // bin > 0 means audible music. We sample only the first 8 low-frequency bins (32 bytes)
        // — music carries most of its energy there and SFX-bleed-through stays out. Field is
        // private (`_FFTBlue1`) in FCS so we resolve it via string-name lookup.
        private static readonly int SoundManagerFFTBlue1Offset = FieldOffsetReader.OffsetOf<FCSSound.SoundManager>("_FFTBlue1");
        private const int BgmAudibleBinCount = 8;
        private const float BgmAudibleEpsilon = 0.0001f;
        // StdVector<T> layout is fixed C++ ABI contract: (T* First, T* Last, T* End) at 0/8/16.
        private const int StdVectorFirstOffset = 0;
        private const int StdVectorLastOffset = 8;

        // Sheet refs resolved once via LuminaGameDataCache and cached for the Reader's
        // lifetime. Per-frame calls to GetSheet<T>() would still hit Lumina's internal
        // dictionary + generic dispatch; field reads are effectively free. `_sheetsAttempted`
        // prevents re-probing sqpack every frame when GameInstallPath is unset or the
        // Lumina open failed.
        private Lumina.Excel.ExcelSheet<Lumina.Excel.Sheets.Weather> _weatherSheetEn;
        private Lumina.Excel.ExcelSheet<Lumina.Excel.Sheets.BGM> _bgmSheetEn;
        // Per-scene fade preset cache: scene index 0..11 → BGMFadeType row id (0 = none).
        // Populated on first sheet resolution by walking BGMFade rows and matching SceneIn.
        // BGMFade is small (a few dozen rows) and static, so this is one-shot work.
        private uint[] _scenePresetFadeTypeRowIds;
        private Lumina.Excel.ExcelSheet<Lumina.Excel.Sheets.BGMFadeType> _bgmFadeTypeSheet;
        private bool _sheetsAttempted;

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

            // --- GameMain.TerritoryLoadState (1=loading, 2=loaded, 3=unloading) ---
            if (locations.ContainsKey(Signatures.GAMEMAIN_KEY)) {
                IntPtr gameMain = locations[Signatures.GAMEMAIN_KEY];
                try { result.TerritoryLoadState = this._memoryHandler.GetByte(gameMain, GameMainTerritoryLoadStateOffset); } catch { /* ignore, leave default */ }
            }

            // IsLoggedIn: CurrentPlayer.Entity resolves with a non-zero entity ID.
            try {
                var cp = this.GetCurrentPlayer()?.Entity;
                result.IsLoggedIn = cp != null && cp.ID != 0u;
            }
            catch { /* not-yet-loaded characters raise; treat as logged-out */ }
            // IsReadyToRead: territory is loaded AND the scanner resolved the actor pointer array.
            result.IsReadyToRead = result.TerritoryLoadState == 2 && locations.ContainsKey(Signatures.CHARMAP_KEY);

            // --- Conditions: cutscene and teleport flags live on the same struct. ---
            //   OccupiedInCutSceneEvent — explicit cutscene-event condition (quest/story).
            //   WatchingCutscene / 78   — generic cutscene flag; 78 fires for instance cutscenes.
            //   BetweenAreas / 51       — set during zone transitions and instance entry/exit.
            //
            // IsTeleporting uses BetweenAreas|51 only. InCutscene on ActorItem (RenderFlags bit)
            // fires for both cutscenes AND zone transitions; using IsTeleporting lets callers
            // distinguish the two: a real cutscene has WatchingCutscene=true, IsTeleporting=false.
            if (locations.ContainsKey(Signatures.CONDITIONS_KEY)) {
                IntPtr conditions = locations[Signatures.CONDITIONS_KEY];
                try {
                    byte occCutscene  = this._memoryHandler.GetByte(conditions, ConditionsOccupiedInCutSceneEventOffset);
                    byte watching58   = this._memoryHandler.GetByte(conditions, ConditionsWatchingCutsceneOffset);
                    byte watching78   = this._memoryHandler.GetByte(conditions, ConditionsWatchingCutscene78Offset);
                    byte betweenAreas = this._memoryHandler.GetByte(conditions, ConditionsBetweenAreasOffset);
                    byte between51    = this._memoryHandler.GetByte(conditions, ConditionsBetweenAreas51Offset);
                    result.WatchingCutscene = occCutscene != 0 || watching58 != 0 || watching78 != 0;
                    result.IsTeleporting    = betweenAreas != 0 || between51 != 0;
                }
                catch { /* ignore */ }
            }

            // --- ContentsFinder.QueueInfo.QueueState
            // FCS ContentsFinderQueueState: 0=None, 1=Pending, 2=Queued, 3=Ready (popped!), 4=Accepted, 5=InContent.
            if (locations.ContainsKey(Signatures.CONTENTSFINDER_KEY)) {
                IntPtr contentsFinder = locations[Signatures.CONTENTSFINDER_KEY];
                try {
                    byte state = this._memoryHandler.GetByte(contentsFinder, ContentsFinderQueueStateOffset);
                    result.ContentsFinderQueueState = state;
                    result.DutyFinderBellPopped = state == 3; // Ready — DF popped but not yet Accepted.
                    result.InInstance = state >= 4;           // Accepted (entering) or InContent.
                }
                catch { /* ignore */ }
            }

            // --- WeatherManager.WeatherId (byte, row id in the Weather sheet) ---
            if (locations.ContainsKey(Signatures.WEATHER_KEY)) {
                IntPtr weather = locations[Signatures.WEATHER_KEY];
                try { result.CurrentWeatherId = this._memoryHandler.GetByte(weather, WeatherManagerWeatherIdOffset); } catch { /* ignore */ }
            }

            // --- BGMSystem.Scenes StdVector, iterate 0..N ---
            // Scenes are priority-ordered — scene 0 (Event) wins over scene 11 (Territory),
            // so the first non-silence PlayingBgmId as we walk the vector is what the user is
            // hearing (CurrentBgmId/SceneId/TargetId). The full per-scene snapshot lands in
            // result.BgmScenes for downstream consumers that need every layer.
            if (locations.ContainsKey(Signatures.BGMSYSTEM_KEY)) {
                IntPtr bgmSystem = locations[Signatures.BGMSYSTEM_KEY];
                try {
                    long first = this._memoryHandler.GetInt64(bgmSystem, BGMSystemScenesOffset + StdVectorFirstOffset);
                    long last  = this._memoryHandler.GetInt64(bgmSystem, BGMSystemScenesOffset + StdVectorLastOffset);
                    if (first != 0 && last > first) {
                        int sceneCount = (int)((last - first) / BGMSceneSize);
                        if (sceneCount > 12) sceneCount = 12; // cap — 12 documented scene types.
                        var scenes = new BgmSceneInfo[sceneCount];
                        for (int i = 0; i < sceneCount; i++) {
                            IntPtr sceneAddr = new IntPtr(first + i * BGMSceneSize);
                            BgmSceneInfo info = new BgmSceneInfo {
                                Index = i,
                                SceneType = (BgmSceneType)i,
                            };
                            try { info.PlayingBgmId      = this._memoryHandler.GetUInt16(sceneAddr, BGMScenePlayingBgmIdOffset); } catch { }
                            try { info.BgmId             = this._memoryHandler.GetUInt16(sceneAddr, BGMSceneBgmIdOffset); }        catch { }
                            try { info.PreviousBgmId    = this._memoryHandler.GetUInt16(sceneAddr, BGMScenePreviousBgmIdOffset); } catch { }
                            try { info.PlayState         = (byte)this._memoryHandler.GetUInt32(sceneAddr, BGMScenePlayStateOffset); } catch { }
                            try { info.EnableCustomFade  = this._memoryHandler.GetByte(sceneAddr, BGMSceneEnableCustomFadeOffset) != 0; } catch { }
                            try { info.FadeOutTime       = this._memoryHandler.GetUInt32(sceneAddr, BGMSceneFadeOutTimeOffset); } catch { }
                            try { info.FadeInTime        = this._memoryHandler.GetUInt32(sceneAddr, BGMSceneFadeInTimeOffset); }  catch { }
                            scenes[i] = info;

                            // BGM id 1 is FFXIV's silence sentinel — a scene playing id=1 is
                            // actively suppressing audio, not "nothing playing". Skip it so the
                            // priority walk falls through to the next-priority scene with real
                            // audio (e.g. an Event scene parked on silence during a fight would
                            // otherwise mask the Battle scene's actual track).
                            if (result.CurrentBgmId == 0 && info.PlayingBgmId != 0 && info.PlayingBgmId != 1) {
                                result.CurrentBgmId      = info.PlayingBgmId;
                                result.CurrentBgmSceneId = i;
                                result.CurrentBgmTargetId = info.BgmId;
                            }
                        }
                        result.BgmScenes = scenes;
                    }
                }
                catch { /* ignore */ }
            }

            // --- SoundManager.ActiveSoundDataListHead walk for AnySoundLoading ---
            // Each list node is a SoundData (which inherits ISoundData; ISoundData.Next is at
            // +0x18 and lives at the same offset within SoundData by single-inheritance layout).
            // We OR all IsLoadingSoundResource bytes — coarse "audio engine is loading" signal,
            // most useful as a "BGM transitioning" proxy during zone changes / scene swaps.
            if (locations.ContainsKey(Signatures.SOUNDMANAGER_KEY)) {
                IntPtr soundManager = locations[Signatures.SOUNDMANAGER_KEY];
                try {
                    long head = this._memoryHandler.GetInt64(soundManager, SoundManagerActiveSoundDataListHeadOffset);
                    // Pool is bounded at 256 SoundData entries (SoundDataMemory). Cap loop at 512
                    // as a safety belt against corrupted Next pointers / list cycles.
                    int safety = 512;
                    while (head != 0 && safety-- > 0) {
                        IntPtr soundData = new IntPtr(head);
                        byte loading = this._memoryHandler.GetByte(soundData, SoundDataIsLoadingSoundResourceOffset);
                        if (loading != 0) {
                            result.AnySoundLoading = true;
                            break;
                        }
                        head = this._memoryHandler.GetInt64(soundData, ISoundDataNextOffset);
                    }
                }
                catch { /* ignore */ }

                // IsBgmAudible — speaker output energy on the music bus. POST-master-volume,
                // so a muted music slider keeps this false; downstream apps that need a
                // pre-fader signal can consume Scene.PlayingBgmId on result.BgmScenes instead.
                try {
                    IntPtr fftAddr = new IntPtr(soundManager.ToInt64() + SoundManagerFFTBlue1Offset);
                    byte[] fftBytes = this._memoryHandler.GetByteArray(fftAddr, BgmAudibleBinCount * sizeof(float));
                    float energy = 0f;
                    for (int i = 0; i < BgmAudibleBinCount; i++) {
                        energy += Math.Abs(BitConverter.ToSingle(fftBytes, i * sizeof(float)));
                    }
                    result.IsBgmAudible = energy > BgmAudibleEpsilon;
                }
                catch { /* leave false */ }
            }

            // --- Lumina name resolution (best-effort; silently skipped if sqpack not found) ---
            this.EnsureLuminaSheets();
            try {
                if (this._weatherSheetEn != null && result.CurrentWeatherId != 0 && this._weatherSheetEn.HasRow(result.CurrentWeatherId)) {
                    result.CurrentWeatherName = this._weatherSheetEn.GetRow(result.CurrentWeatherId).Name.ExtractText();
                }
                if (this._bgmSheetEn != null && result.CurrentBgmId != 0 && this._bgmSheetEn.HasRow(result.CurrentBgmId)) {
                    result.CurrentBgmFile = this._bgmSheetEn.GetRow(result.CurrentBgmId).File.ExtractText();
                }
                // Per-scene file + preset-fade resolution.
                if (result.BgmScenes != null) {
                    foreach (BgmSceneInfo info in result.BgmScenes) {
                        if (info == null) continue;
                        if (this._bgmSheetEn != null && info.IsPlaying && this._bgmSheetEn.HasRow(info.PlayingBgmId)) {
                            info.PlayingBgmFile = this._bgmSheetEn.GetRow(info.PlayingBgmId).File.ExtractText();
                        }
                        // BGMFade preset lookup — scene → first matching BGMFadeType.
                        if (this._bgmFadeTypeSheet != null && this._scenePresetFadeTypeRowIds != null
                            && info.Index >= 0 && info.Index < this._scenePresetFadeTypeRowIds.Length) {
                            uint fadeTypeId = this._scenePresetFadeTypeRowIds[info.Index];
                            if (fadeTypeId != 0 && this._bgmFadeTypeSheet.HasRow(fadeTypeId)) {
                                var ft = this._bgmFadeTypeSheet.GetRow(fadeTypeId);
                                info.PresetFadeOutTime      = ft.FadeOutTime;
                                info.PresetFadeInTime       = ft.FadeInTime;
                                info.PresetFadeInStartTime  = ft.FadeInStartTime;
                                info.PresetResumeFadeInTime = ft.ResumeFadeInTime;
                            }
                        }
                    }

                    // Highest non-zero preset values across every scene, surfaced flat on the
                    // result so consumers don't have to re-walk BgmScenes. Zero entries
                    // (scenes with no Lumina preset) are skipped — only meaningful values
                    // contribute, so a single scene with a 0.5s start delay wins over many
                    // scenes left at 0.
                    foreach (BgmSceneInfo info in result.BgmScenes) {
                        if (info == null) continue;
                        if (info.PresetFadeInStartTime > result.BgmFadeIn) {
                            result.BgmFadeIn = info.PresetFadeInStartTime;
                        }
                        if (info.PresetResumeFadeInTime > result.BgmResume) {
                            result.BgmResume = info.PresetResumeFadeInTime;
                        }
                    }
                }
            }
            catch { /* Lumina failures shouldn't break the numeric result */ }

            return result;
        }

        // Resolve the EN Weather and BGM sheets once, lazily. Per-frame GetGameState reads
        // them as fields — no dictionary lookup, no generic dispatch.
        private void EnsureLuminaSheets() {
            if (this._sheetsAttempted) {
                return;
            }
            this._sheetsAttempted = true;
            Lumina.GameData lumina = LuminaGameDataCache.GetOrNull(this._memoryHandler.Configuration);
            if (lumina == null) {
                return;
            }
            try {
                this._weatherSheetEn = lumina.Excel.GetSheet<Lumina.Excel.Sheets.Weather>();
                this._bgmSheetEn = lumina.Excel.GetSheet<Lumina.Excel.Sheets.BGM>();
                this._bgmFadeTypeSheet = lumina.Excel.GetSheet<Lumina.Excel.Sheets.BGMFadeType>();

                // Build the scene → BGMFadeType lookup once. BGMFade has (SceneOut, SceneIn,
                // BGMFadeType) triples — we pick the FIRST row whose SceneIn matches each
                // scene index 0..11. Multiple rows may match (different SceneOut origins);
                // first match is a pragmatic default that surfaces a representative preset.
                var bgmFadeSheet = lumina.Excel.GetSheet<Lumina.Excel.Sheets.BGMFade>();
                this._scenePresetFadeTypeRowIds = new uint[12];
                foreach (var row in bgmFadeSheet) {
                    int sceneIn = row.SceneIn;
                    if (sceneIn >= 0 && sceneIn < this._scenePresetFadeTypeRowIds.Length
                        && this._scenePresetFadeTypeRowIds[sceneIn] == 0) {
                        this._scenePresetFadeTypeRowIds[sceneIn] = row.BGMFadeType.RowId;
                    }
                }
            }
            catch { /* leave fields null; subsequent calls skip the lookup */ }
        }

        // Thin accessor retained so Reader.Lumina can resolve language-specific sheets
        // (zone / weather / exp-table helpers) via the same shared cache.
        private Lumina.GameData GetLumina() => LuminaGameDataCache.GetOrNull(this._memoryHandler.Configuration);
    }
}
