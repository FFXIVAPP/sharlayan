// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharlayanConfiguration.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   SharlayanConfiguration.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using Sharlayan.Enums;
    using Sharlayan.Models;
    using Sharlayan.Resources;

    public class SharlayanConfiguration {
        public string CharacterName { get; set; }
        public string GameInstallPath { get; set; }
        public GameLanguage GameLanguage { get; set; } = GameLanguage.English;
        public bool IgnoreGameVersionMismatch { get; set; } = false;
        public ProcessModel ProcessModel { get; set; }
        public ResourceProviderKind ResourceProvider { get; set; } = ResourceProviderKind.FFXIVClientStructsDirect;
        public bool ScanAllRegions { get; set; } = false;

        /// <summary>
        /// Maximum number of consecutive <c>GetCurrentPlayer()</c> calls that may return a
        /// transient null Entity before the Reader stops latching the last-known value and
        /// lets null surface. Zero (or negative) disables the latch entirely.
        /// <para>
        /// FFXIV zeroes the local-player slot in GameObjectManager for 1–3 reads during any
        /// zone transition while the actor table rebuilds. Latching the last-known Entity
        /// over that window keeps <see cref="Models.ReadResults.GameStateResult.IsLoggedIn"/>
        /// and <see cref="Models.ReadResults.CurrentPlayerResult.Entity"/> stable across the
        /// transition. A genuine logout still surfaces once the underlying read has been null
        /// for more than this many consecutive calls.
        /// </para>
        /// <para>
        /// Default is 10. At a 5 Hz poll cadence (Chromatics' default) this gives roughly a
        /// 2-second decay; at 10 Hz roughly 1 second. Tune to your consumer's cadence — you
        /// want enough headroom to cover the 1–3 tick transient window without making a real
        /// logout take noticeably long to register.
        /// </para>
        /// </summary>
        public int LoggedInLatchTicks { get; set; } = 10;
    }
}
