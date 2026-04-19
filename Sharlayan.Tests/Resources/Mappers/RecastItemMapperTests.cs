// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecastItemMapperTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.UI.Arrays.Common;

    using Sharlayan.Models.Structures;
    using Sharlayan.Resources.Mappers;

    using Xunit;

    public class RecastItemMapperTests {
        // RECAST is resolved via AtkStage → ActionBarNumberArray._bars[0], so every field
        // offset is relative to ActionBarSlotNumberArray (17 ints / 68 bytes per slot).
        [Fact]
        public void Build_ID_MatchesActionIdOffset() {
            RecastItem item = RecastItemMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.ActionId)), item.ID);
        }

        [Fact]
        public void Build_IsAvailable_MatchesExecutableOffset() {
            RecastItem item = RecastItemMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.Executable)), item.IsAvailable);
        }

        [Fact]
        public void Build_InRange_MatchesInRangeOffset() {
            RecastItem item = RecastItemMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.InRange)), item.InRange);
        }

        [Fact]
        public void Build_CoolDownPercent_MatchesGlobalCoolDownPercentageOffset() {
            RecastItem item = RecastItemMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.GlobalCoolDownPercentage)), item.CoolDownPercent);
        }

        [Fact]
        public void Build_Icon_MatchesIconIdOffset() {
            RecastItem item = RecastItemMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.IconId)), item.Icon);
        }

        [Fact]
        public void Build_ActionProc_MatchesGlowsOffset() {
            // Glows is the steady proc/combo highlight. Pulses is a transient animation
            // trigger and polling it unreliably reads false even on proc'd actions.
            RecastItem item = RecastItemMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.Glows)), item.ActionProc);
        }

        [Fact]
        public void Build_ChargeReadyAndChargesRemaining_BothPointAtCurrentCharges() {
            // The same int field answers both questions: Reader reads it twice — once as
            // bool (ChargeReady) and once as int (ChargesRemaining).
            RecastItem item = RecastItemMapper.Build();
            int expected = (int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.CurrentCharges));
            Assert.Equal(expected, item.ChargeReady);
            Assert.Equal(expected, item.ChargesRemaining);
        }

        [Fact]
        public void Build_TypeAndCategory_BothPointAtActionType() {
            // FCS names the +0 field ActionType; the game actually writes the ActionCategory
            // row id there (BRD weaponskills = 47, role actions / LB = 56). Sharlayan's
            // Type and Category both surface that value.
            RecastItem item = RecastItemMapper.Build();
            int expected = (int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.ActionType));
            Assert.Equal(expected, item.Type);
            Assert.Equal(expected, item.Category);
        }

        [Fact]
        public void Build_RemainingCost_MatchesManaCostOffset() {
            RecastItem item = RecastItemMapper.Build();
            Assert.Equal((int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.ManaCost)), item.RemainingCost);
        }

        [Fact]
        public void Build_ItemSize_MatchesActionBarSlotNumberArraySize() {
            RecastItem item = RecastItemMapper.Build();
            Assert.Equal(Marshal.SizeOf<ActionBarSlotNumberArray>(), item.ItemSize);
        }

        [Fact]
        public void Build_ContainerSize_MatchesActionBarBarNumberArraySize() {
            // Each ActionBarBarNumberArray is 272 × 4 bytes (12 slots × 68 + other bar state).
            // RECAST_KEY + type × ContainerSize must land at bars[type]._slots base.
            RecastItem item = RecastItemMapper.Build();
            Assert.Equal(272 * sizeof(int), item.ContainerSize);
        }

        [Fact]
        public void Build_UnmappedFields_StayZero() {
            // Amount has no direct equivalent on ActionBarSlotNumberArray; stays at default.
            RecastItem item = RecastItemMapper.Build();
            Assert.Equal(0, item.Amount);
        }
    }
}
