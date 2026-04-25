// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameStateResult.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Aggregates the lightweight "what is the game doing right now" fields that downstream
//   apps (Chromatics) used to resolve via ad-hoc memory signatures. Every field is derived
//   from an FFXIVClientStructs singleton (GameMain / Conditions / ContentsFinder /
//   WeatherManager / BGMSystem); see Reader.GameState.cs for the actual read sites.
//   Name lookups (Weather / BGM) are populated only when the configuration's
//   GameInstallPath is set so Lumina can resolve them from sqpack.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System.Collections.Generic;

    public class GameStateResult {
        // --- Session & readiness -------------------------------------------------------
        public bool IsLoggedIn { get; internal set; }
        public bool IsReadyToRead { get; internal set; }

        /// <summary>GameMain.TerritoryLoadState raw byte: 1 = loading, 2 = loaded, 3 = unloading.</summary>
        public byte TerritoryLoadState { get; internal set; }

        // --- Cutscene & teleport -------------------------------------------------------
        /// <summary>True while a cutscene is playing (Conditions.WatchingCutscene / 78).</summary>
        public bool WatchingCutscene { get; internal set; }

        /// <summary>
        /// True while the player is travelling between zones or entering/exiting an instance
        /// (Conditions.BetweenAreas or BetweenAreas51). Distinct from WatchingCutscene — a
        /// genuine cutscene has WatchingCutscene=true and IsTeleporting=false. ActorItem.InCutscene
        /// (from RenderFlags) fires for both; use IsTeleporting to tell them apart.
        /// </summary>
        public bool IsTeleporting { get; internal set; }

        // --- Duty Finder & instances ---------------------------------------------------
        /// <summary>Raw ContentsFinderQueueInfo.QueueState byte (0=None, 1=Pending, 2=Queued, 3=Ready (popped), 4=Accepted, 5=InContent).</summary>
        public byte ContentsFinderQueueState { get; internal set; }

        /// <summary>QueueState == 3 — Duty Finder popped but not accepted yet.</summary>
        public bool DutyFinderBellPopped { get; internal set; }

        /// <summary>QueueState >= 4 — Accepted (entering) or already inside the instance.</summary>
        public bool InInstance { get; internal set; }

        /// <summary>Alias preserved for parity with the pre-rebuild Chromatics extension, which exposed this byte directly.</summary>
        public byte InstanceState => this.ContentsFinderQueueState;

        // --- Weather --------------------------------------------------------------------
        /// <summary>WeatherManager.WeatherId — row id in the Weather Excel sheet.</summary>
        public byte CurrentWeatherId { get; internal set; }

        /// <summary>Localised name from the Weather sheet; null if GameInstallPath wasn't set or the row is missing.</summary>
        public string CurrentWeatherName { get; internal set; }

        // --- Music ---------------------------------------------------------------------
        /// <summary>
        /// Sourced from <c>Scene.PlayingBgmId</c> on the highest-priority scene currently
        /// playing audible audio (walks BGMSystem.Scenes 0..N skipping scenes parked on
        /// id=0 or id=1 silence sentinel). 0 if no scene has audible audio.
        /// </summary>
        public ushort CurrentBgmId { get; internal set; }

        /// <summary>
        /// Sourced from <c>Scene.BgmId</c> (the "intended" track) on the same winning scene
        /// as <see cref="CurrentBgmId"/>. When <c>CurrentBgmTargetId != CurrentBgmId</c> the
        /// scene is mid-BGMSwitch — useful as a "BGM transition in progress" signal that
        /// settles at the new value once the swap completes.
        /// </summary>
        public ushort CurrentBgmTargetId { get; internal set; }

        /// <summary>
        /// One <see cref="BgmSceneInfo"/> per slot in <c>BGMSystem.Scenes</c> (currently 12,
        /// indexed via <see cref="BgmSceneType"/>). Scenes are priority-ordered: index 0
        /// (Event) wins over index 11 (Territory). The "winning" scene's values are also
        /// exposed flat as <see cref="CurrentBgmId"/> / <see cref="CurrentBgmTargetId"/> /
        /// <see cref="CurrentBgmSceneId"/> / <see cref="CurrentBgmFile"/>; this array gives
        /// downstream consumers visibility into every layer (e.g. the Battle scene track
        /// while an Event scene is also active).
        /// <para>
        /// Empty if BGMSYSTEM didn't resolve. Always 12 elements when populated.
        /// </para>
        /// </summary>
        public IReadOnlyList<BgmSceneInfo> BgmScenes { get; internal set; }

        /// <summary>BGMScene row index that's currently playing (0 = Event, 1 = Battle, ..., 11 = Territory). -1 if none.</summary>
        public int CurrentBgmSceneId { get; internal set; } = -1;

        /// <summary>BGM file path from the BGM sheet (e.g. "music/ffxiv/BGM_Field_Gri_Day.scd"); null if unavailable.</summary>
        public string CurrentBgmFile { get; internal set; }

        /// <summary>
        /// Highest non-zero <c>BGMFadeType.FadeInStartTime</c> (seconds) across every entry
        /// in <see cref="BgmScenes"/>. This is the *delay* between a scene's PlayingBgmId
        /// becoming non-zero and audio actually starting its volume ramp; the longest such
        /// delay among all scenes is what determines when the user will hear the new track.
        /// Zero scenes don't contribute — a single scene with a 0.5s start delay wins over
        /// many scenes at 0. 0 if all scenes are 0 or sqpack/Lumina isn't available.
        /// </summary>
        public float BgmFadeIn { get; internal set; }

        /// <summary>
        /// Highest non-zero <c>BGMFadeType.ResumeFadeInTime</c> (seconds) across every
        /// entry in <see cref="BgmScenes"/>. Used by the game when resuming a previously
        /// paused track rather than starting fresh. Zero entries are skipped. 0 if all
        /// scenes are 0 or sqpack/Lumina isn't available.
        /// </summary>
        public float BgmResume { get; internal set; }

        /// <summary>
        /// True if any active SoundData entry has IsLoadingSoundResource set. Walked from
        /// SoundManager.ActiveSoundDataListHead following ISoundData.Next. This is a coarse
        /// "audio engine is loading something right now" signal — during a BGM transition
        /// (zone change, scene swap) the new BGM SoundData briefly raises this flag, so it
        /// works as a proxy for "BGM is transitioning". Returns false during steady-state
        /// playback. Set to false if SoundManager isn't resolved.
        /// </summary>
        public bool AnySoundLoading { get; internal set; }

        /// <summary>
        /// True when audible audio energy is present on the *music* bus specifically
        /// (sourced from <c>SoundManager._FFTBlue1</c> — FCS labels Blue as the Music bus,
        /// distinct from Red=System and Green=SE/Voice/Instruments).
        /// <para>
        /// <b>Post-master-volume</b> — this is the speaker output, so muting BGM in-game or
        /// dropping the slider to 0 will keep this false even when music is playing in the
        /// audio engine. For a pre-fader signal that ignores user volume settings, consume
        /// <see cref="BgmScenes"/> directly (a non-zero <c>PlayingBgmId</c> on any scene means
        /// the audio engine has selected a track regardless of user volume).
        /// </para>
        /// Set to false if SoundManager isn't resolved.
        /// </summary>
        public bool IsBgmAudible { get; internal set; }
    }
}
