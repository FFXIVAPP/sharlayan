// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatLogPointersMapperTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Component.Log;

    using Sharlayan.Models.Structures;
    using Sharlayan.Resources.Mappers;

    using Xunit;

    public class ChatLogPointersMapperTests {
        [Fact]
        public void Build_OffsetArrayStart_MatchesLogMessageIndexFirstOffset() {
            ChatLogPointers pointers = ChatLogPointersMapper.Build();
            int expected = (int)Marshal.OffsetOf<LogModule>(nameof(LogModule.LogMessageIndex));
            Assert.Equal(expected, pointers.OffsetArrayStart);
        }

        [Fact]
        public void Build_OffsetArrayPos_IsEightBytesAfterStart() {
            // StdVector.Last sits at +8 from First on x64.
            ChatLogPointers pointers = ChatLogPointersMapper.Build();
            Assert.Equal(pointers.OffsetArrayStart + 8, pointers.OffsetArrayPos);
        }

        [Fact]
        public void Build_OffsetArrayEnd_IsSixteenBytesAfterStart() {
            ChatLogPointers pointers = ChatLogPointersMapper.Build();
            Assert.Equal(pointers.OffsetArrayStart + 16, pointers.OffsetArrayEnd);
        }

        [Fact]
        public void Build_LogStart_MatchesLogMessageDataFirstOffset() {
            ChatLogPointers pointers = ChatLogPointersMapper.Build();
            int expected = (int)Marshal.OffsetOf<LogModule>(nameof(LogModule.LogMessageData));
            Assert.Equal(expected, pointers.LogStart);
        }

        [Fact]
        public void Build_AllFields_AreNonZero() {
            ChatLogPointers pointers = ChatLogPointersMapper.Build();
            Assert.True(pointers.LogStart > 0);
            Assert.True(pointers.LogNext > pointers.LogStart);
            Assert.True(pointers.LogEnd > pointers.LogNext);
            Assert.True(pointers.OffsetArrayStart > 0);
            Assert.True(pointers.OffsetArrayPos > pointers.OffsetArrayStart);
            Assert.True(pointers.OffsetArrayEnd > pointers.OffsetArrayPos);
        }
    }
}
