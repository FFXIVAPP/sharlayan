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

        /// <summary>
        /// When true, the first <c>GetChatLog()</c> call after attaching returns the lines
        /// already sitting in the client's chat buffer instead of skipping past them to the
        /// newest entry.
        /// <para>
        /// The reader has always jumped to the newest entry on first run, so anything logged
        /// before its first successful read is dropped. That is normally desirable — you don't
        /// want the whole backlog replayed when you attach mid-session — but it also eats the
        /// login banner. <c>CHATLOG</c> resolves through <c>Framework → UIModule →
        /// RaptureLogModule</c>, and that chain only becomes valid partway through the login
        /// sequence, by which point "Welcome to &lt;World&gt;!" and the messages after it are
        /// already buffered.
        /// </para>
        /// <para>
        /// Enable this if you want those lines. The backlog is bounded by the client's own
        /// table (1000 entries), and the resume point passed to <c>GetChatLog()</c> is honoured
        /// — pass a stored index/offset to continue where you left off, or the default (0, 0)
        /// to take the buffer from the start.
        /// </para>
        /// <para>
        /// Default is false, preserving the long-standing behaviour.
        /// </para>
        /// </summary>
        public bool ChatLogFirstRunReadsBacklog { get; set; } = false;
    }
}
