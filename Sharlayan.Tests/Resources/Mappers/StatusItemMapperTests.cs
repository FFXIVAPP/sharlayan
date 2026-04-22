// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusItemMapperTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game;

    using Sharlayan.Models.Structures;
    using Sharlayan.Resources.Mappers;

    using Xunit;

    public class StatusItemMapperTests {
        [Fact]
        public void Build_StatusID_IsAtOffsetZero() {
            // Status.StatusId is the first field of the struct.
            StatusItem statusItem = StatusItemMapper.Build();
            Assert.Equal(0, statusItem.StatusID);
            Assert.Equal((int)Marshal.OffsetOf<Status>(nameof(Status.StatusId)), statusItem.StatusID);
        }

        [Fact]
        public void Build_Duration_MatchesRemainingTimeOffset() {
            StatusItem statusItem = StatusItemMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<Status>(nameof(Status.RemainingTime)), statusItem.Duration);
        }

        [Fact]
        public void Build_CasterID_MatchesSourceObjectOffset() {
            StatusItem statusItem = StatusItemMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<Status>(nameof(Status.SourceObject)), statusItem.CasterID);
        }

        [Fact]
        public void Build_SourceSize_MatchesStatusStructSize() {
            StatusItem statusItem = StatusItemMapper.Build();
            Assert.Equal(Marshal.SizeOf<Status>(), statusItem.SourceSize);
        }

        [Fact]
        public void Build_MappedFields_Sanity() {
            StatusItem statusItem = StatusItemMapper.Build();
            // StatusID legitimately maps to offset 0 (first field), so > 0 assertion would be wrong.
            Assert.Equal(0, statusItem.StatusID);
            Assert.True(statusItem.Stacks > 0, nameof(StatusItem.Stacks));
            Assert.True(statusItem.Duration > 0, nameof(StatusItem.Duration));
            Assert.True(statusItem.CasterID > 0, nameof(StatusItem.CasterID));
            Assert.True(statusItem.SourceSize > 0, nameof(StatusItem.SourceSize));
        }
    }
}
