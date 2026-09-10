// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatLogResumePointTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2026 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Covers Sharlayan.Utilities.ChatLogResumePoint — where GetChatLog() picks up on its first
//   successful poll. Issue #130: the reader has always skipped whatever was already buffered,
//   which drops the login banner because CHATLOG only resolves partway through login. These
//   tests pin both the historical skip and the opt-in backlog behaviour without needing a live
//   client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Core {
    using System.Collections.Generic;

    using Sharlayan.Utilities;

    using Xunit;

    public class ChatLogResumePointTests {
        // Five buffered entries; values are the byte offsets the client records per line.
        private static List<int> Indexes() {
            return new List<int> { 40, 95, 130, 190, 260 };
        }

        [Fact]
        public void Default_SkipsEverythingAlreadyBuffered() {
            // Historical behaviour: park on the newest entry so the backlog is not replayed.
            (int index, int offset) = ChatLogResumePoint.ForFirstRun(false, 0, 0, 5, Indexes());

            Assert.Equal(4, index);
            Assert.Equal(260, offset);
        }

        [Fact]
        public void Backlog_StartsFromBeginningWhenCallerPassesDefaults() {
            // This is the issue #130 case — the whole buffer, so the login lines survive.
            (int index, int offset) = ChatLogResumePoint.ForFirstRun(true, 0, 0, 5, Indexes());

            Assert.Equal(0, index);
            Assert.Equal(0, offset);
        }

        [Fact]
        public void Backlog_HonoursCallerResumePoint() {
            // A consumer persisting its position across restarts continues where it left off.
            (int index, int offset) = ChatLogResumePoint.ForFirstRun(true, 2, 95, 5, Indexes());

            Assert.Equal(2, index);
            Assert.Equal(95, offset);
        }

        [Fact]
        public void Backlog_IndexZero_ForcesOffsetZero() {
            // Starting at entry 0 must start at byte 0, otherwise the first entry's computed
            // length (offset -> Indexes[0]) is wrong and the line is truncated or dropped.
            (int index, int offset) = ChatLogResumePoint.ForFirstRun(true, 0, 500, 5, Indexes());

            Assert.Equal(0, index);
            Assert.Equal(0, offset);
        }

        [Fact]
        public void Backlog_ClampsCallerIndexPastTheBuffer() {
            // A stale persisted index (log since wrapped/shrunk) must not walk past the end.
            (int index, _) = ChatLogResumePoint.ForFirstRun(true, 999, 0, 5, Indexes());

            Assert.Equal(5, index);
        }

        [Fact]
        public void Backlog_ClampsNegativeCallerValues() {
            (int index, int offset) = ChatLogResumePoint.ForFirstRun(true, -3, -7, 5, Indexes());

            Assert.Equal(0, index);
            Assert.Equal(0, offset);
        }

        [Fact]
        public void CurrentIndexBeyondTable_ClampsToTable() {
            // Torn read of the two pointer fields can yield a count past the real table.
            (int index, int offset) = ChatLogResumePoint.ForFirstRun(false, 0, 0, 99, Indexes());

            Assert.Equal(4, index);
            Assert.Equal(260, offset);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void EmptyOrUnreadyBuffer_ReturnsOrigin(bool readBacklog) {
            Assert.Equal((0, 0), ChatLogResumePoint.ForFirstRun(readBacklog, 0, 0, 0, Indexes()));
            Assert.Equal((0, 0), ChatLogResumePoint.ForFirstRun(readBacklog, 0, 0, 5, new List<int>()));
            Assert.Equal((0, 0), ChatLogResumePoint.ForFirstRun(readBacklog, 0, 0, 5, null));
        }

        [Fact]
        public void Default_IsUnaffectedByCallerResumePoint() {
            // Skip mode ignores whatever the caller passed — that is the pre-existing contract.
            (int index, int offset) = ChatLogResumePoint.ForFirstRun(false, 2, 95, 5, Indexes());

            Assert.Equal(4, index);
            Assert.Equal(260, offset);
        }
    }
}
