// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BgmSceneInfo.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Per-scene BGM snapshot. <see cref="GameStateResult.BgmScenes"/> exposes one entry per
//   slot in <c>BGMSystem.Scenes</c> (currently 12). Scenes are priority-ordered: index 0
//   wins over index 11, so the highest-priority scene with a non-silence PlayingBgmId is
//   what the user is actually hearing — that's what <see cref="GameStateResult.CurrentBgmId"/>
//   already surfaces. This array gives downstream consumers visibility into every scene.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    /// <summary>
    /// Index into <see cref="GameStateResult.BgmScenes"/>. Mirrors the FFXIVClientStructs
    /// <c>BGMSystem.Scene.SceneId</c> documentation. <see cref="Unknown7"/> and
    /// <see cref="Unknown8"/> are present for completeness; FCS doesn't have firm names.
    /// </summary>
    public enum BgmSceneType {
        Event = 0,
        Battle = 1,
        MiniGame = 2,
        Content = 3,
        GFate = 4,
        Duel = 5,
        Mount = 6,
        Unknown7 = 7,
        Unknown8 = 8,
        Wedding = 9,
        Town = 10,
        Territory = 11,
    }

    public class BgmSceneInfo {
        /// <summary>0..11 — same as <see cref="SceneType"/> cast to int.</summary>
        public int Index { get; internal set; }

        /// <summary>Named slot — see <see cref="BgmSceneType"/> for the priority order.</summary>
        public BgmSceneType SceneType { get; internal set; }

        /// <summary>
        /// <c>Scene.BgmId</c> — the *intended* track for this scene. May differ from
        /// <see cref="PlayingBgmId"/> mid-BGMSwitch and converges once the swap completes.
        /// </summary>
        public ushort BgmId { get; internal set; }

        /// <summary>
        /// <c>Scene.PlayingBgmId</c> — the track actually selected on this scene. Note id 0
        /// means "no track" and id 1 is FFXIV's silence sentinel; both are treated as
        /// not-playing by <see cref="IsPlaying"/>.
        /// </summary>
        public ushort PlayingBgmId { get; internal set; }

        /// <summary>
        /// <c>Scene.PreviousBgmId</c>. FCS comment: "holds BgmId until after BGMSwitch
        /// selection" — useful for tracking transitions.
        /// </summary>
        public ushort PreviousBgmId { get; internal set; }

        /// <summary>
        /// True iff <see cref="PlayingBgmId"/> is non-zero AND not the silence sentinel
        /// (id=1). Cheap convenience over the raw id.
        /// </summary>
        public bool IsPlaying => this.PlayingBgmId != 0 && this.PlayingBgmId != 1;

        /// <summary>
        /// Lumina-resolved file path for <see cref="PlayingBgmId"/> (e.g.
        /// "music/ffxiv/BGM_Field_Gri_Day.scd"). Null when <see cref="IsPlaying"/> is false
        /// or when sqpack/Lumina isn't available.
        /// </summary>
        public string PlayingBgmFile { get; internal set; }

        /// <summary>
        /// <c>Scene.PlayState</c> — manual control flag set by <c>BGMSystem.SetBGMPlayState</c>.
        /// 0 = Paused, 1 = Playing. Note the game does *not* write this during normal
        /// playback; expect it to stay 0 for most scenes. Exposed for completeness.
        /// </summary>
        public byte PlayState { get; internal set; }

        /// <summary>
        /// <c>Scene.EnableCustomFade</c> — true when the per-scene fade overrides
        /// (<see cref="FadeOutTime"/> / <see cref="FadeInTime"/>) are populated. When
        /// false, the game uses defaults from the BGMFadeType Excel sheet and the local
        /// fade fields stay at 0.
        /// </summary>
        public bool EnableCustomFade { get; internal set; }

        /// <summary>Per-scene fade-out duration override (ms). Only meaningful when <see cref="EnableCustomFade"/> is true.</summary>
        public uint FadeOutTime { get; internal set; }

        /// <summary>Per-scene fade-in duration override (ms). Only meaningful when <see cref="EnableCustomFade"/> is true.</summary>
        public uint FadeInTime { get; internal set; }

        // --- Lumina-resolved preset fades ---------------------------------------------
        // When EnableCustomFade is false (the common case), the game uses fade durations
        // from the BGMFade Excel sheet, keyed on (SceneOut, SceneIn). The Preset* fields
        // below are filled from the FIRST BGMFade row whose SceneIn matches this scene's
        // Index — i.e. the "fade-in for transitioning *into* this scene." Values are
        // in seconds (Lumina float). Zero when sqpack is unavailable or no row matches.

        /// <summary><c>BGMFadeType.FadeOutTime</c> (seconds) for the preset fade-into-this-scene row.</summary>
        public float PresetFadeOutTime { get; internal set; }

        /// <summary>
        /// <c>BGMFadeType.FadeInTime</c> (seconds) — duration of the volume ramp once fade-in
        /// actually begins.
        /// </summary>
        public float PresetFadeInTime { get; internal set; }

        /// <summary>
        /// <c>BGMFadeType.FadeInStartTime</c> (seconds) — delay between the scene's
        /// PlayingBgmId becoming non-zero and the audio starting its ramp. This is the
        /// "delayed start" you observe when IsPlaying flips true before audio is heard:
        /// audible at <c>(IsPlaying rising edge) + PresetFadeInStartTime</c>, full volume
        /// at <c>(rising edge) + PresetFadeInStartTime + PresetFadeInTime</c>.
        /// </summary>
        public float PresetFadeInStartTime { get; internal set; }

        /// <summary>
        /// <c>BGMFadeType.ResumeFadeInTime</c> (seconds) — used when resuming a previously
        /// paused track rather than starting fresh.
        /// </summary>
        public float PresetResumeFadeInTime { get; internal set; }
    }
}
