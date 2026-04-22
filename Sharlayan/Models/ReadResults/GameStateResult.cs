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
        /// <summary>Highest-priority scene's PlayingBgmId (walks BGMSystem.Scenes 0..N). 0 if no scene is playing.</summary>
        public ushort CurrentBgmId { get; internal set; }

        /// <summary>BGMScene row index that's currently playing (0 = Event, 1 = Battle, ..., 11 = Territory). -1 if none.</summary>
        public int CurrentBgmSceneId { get; internal set; } = -1;

        /// <summary>BGM file path from the BGM sheet (e.g. "music/ffxiv/BGM_Field_Gri_Day.scd"); null if unavailable.</summary>
        public string CurrentBgmFile { get; internal set; }
    }
}
