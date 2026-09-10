// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatLogResumePoint.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2026 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Decides where GetChatLog() resumes reading on its FIRST successful poll after attaching.
//
//   The client keeps a rolling buffer of recent chat lines, so on attach there is already a
//   backlog sitting in memory. Historically the reader always jumped past it to the newest
//   entry, which means every line logged before the reader's first successful read is dropped.
//   That is usually what you want mid-session, but it silently eats the login banner
//   ("Welcome to <World>!" and everything that follows) because CHATLOG resolves through
//   Framework -> UIModule -> RaptureLogModule and that chain only becomes valid partway
//   through the login sequence — by which time those lines are already buffered.
//
//   Pulled out as a pure function so both behaviours are directly testable without standing
//   up a Reader or a live process.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System.Collections.Generic;

    internal static class ChatLogResumePoint {
        /// <summary>
        /// Resolves the (array index, byte offset) pair the reader should resume from on its
        /// first successful poll.
        /// </summary>
        /// <param name="readBacklog">
        /// When false (the historical default) the already-buffered lines are skipped. When true
        /// the caller-supplied resume point is honoured instead, so the backlog is delivered.
        /// </param>
        /// <param name="callerArrayIndex">Resume index supplied by the caller; 0 means "from the start of the buffer".</param>
        /// <param name="callerOffset">Resume byte offset supplied by the caller.</param>
        /// <param name="currentArrayIndex">Index one past the newest entry, already clamped to <paramref name="indexes"/>.</param>
        /// <param name="indexes">The client's entry-offset table.</param>
        public static (int ArrayIndex, int Offset) ForFirstRun(bool readBacklog, int callerArrayIndex, int callerOffset, int currentArrayIndex, IReadOnlyList<int> indexes) {
            if (indexes == null || indexes.Count == 0 || currentArrayIndex <= 0) {
                return (0, 0);
            }

            if (currentArrayIndex > indexes.Count) {
                currentArrayIndex = indexes.Count;
            }

            if (!readBacklog) {
                // Historical behaviour: park on the newest entry so nothing already buffered
                // is replayed. The next poll emits only lines that arrive after this point.
                int newest = currentArrayIndex - 1;
                return (newest, indexes[newest]);
            }

            // Backlog mode: start where the caller asked. The default (0, 0) walks the whole
            // buffer, which is what recovers the login lines.
            if (callerArrayIndex < 0) {
                callerArrayIndex = 0;
            }

            if (callerArrayIndex > currentArrayIndex) {
                callerArrayIndex = currentArrayIndex;
            }

            if (callerOffset < 0) {
                callerOffset = 0;
            }

            // A caller resuming mid-buffer supplies its own offset; starting from index 0 must
            // start at byte 0 regardless of what was passed, or the first entry's length
            // (offset -> Indexes[0]) is wrong and the line comes out truncated.
            //
            // "index 0 means byte 0" is the same convention Reader.ChatLog's ring-wrap branch
            // already relies on — when the entry table wraps it resets BOTH PreviousArrayIndex
            // and PreviousOffset to 0, i.e. the client restarts its data buffer alongside the
            // index table. This mode inherits that assumption rather than introducing a new one.
            return (callerArrayIndex, callerArrayIndex == 0 ? 0 : callerOffset);
        }
    }
}
