// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggedInStateLatchTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2026 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Covers Sharlayan.Utilities.LoggedInStateLatch — the debouncer that holds the
//   last-known local-player Entity across the transient null reads FFXIV produces
//   during zone transitions. Direct unit tests here prove the present → transient →
//   present and present → sustained-absent transitions without standing up a Reader
//   or memory backend.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests {
    using Sharlayan.Core;
    using Sharlayan.Utilities;

    using Xunit;

    public class LoggedInStateLatchTests {
        private static ActorItem Present(uint id = 0x10000001u) {
            return new ActorItem { ID = id };
        }

        [Fact]
        public void Apply_LiveEntity_PassesThrough() {
            LoggedInStateLatch latch = new LoggedInStateLatch(10);
            ActorItem live = Present();

            ActorItem result = latch.Apply(live);

            Assert.Same(live, result);
            Assert.Equal(0, latch.ConsecutiveAbsent);
        }

        [Fact]
        public void Apply_NullBeforeAnyLive_ReturnsNull() {
            LoggedInStateLatch latch = new LoggedInStateLatch(10);

            ActorItem result = latch.Apply(null);

            Assert.Null(result);
        }

        [Fact]
        public void Apply_ZoneTransition_HoldsLastKnownEntity() {
            // Simulates the FFXIV pattern: live → transient null for a few reads (zone
            // change) → live again. The latch should hide the null window from callers.
            LoggedInStateLatch latch = new LoggedInStateLatch(maxConsecutiveAbsent: 10);
            ActorItem player = Present();

            // Steady state.
            Assert.Same(player, latch.Apply(player));

            // Zone transition: actor table briefly null for 3 ticks.
            for (int i = 0; i < 3; i++) {
                ActorItem latched = latch.Apply(null);
                Assert.Same(player, latched);
                Assert.NotEqual(0u, latched.ID);
            }

            // Actor table populated again.
            Assert.Same(player, latch.Apply(player));
            Assert.Equal(0, latch.ConsecutiveAbsent);
        }

        [Fact]
        public void Apply_SustainedAbsence_EventuallyReturnsNull() {
            // Simulates a real logout: live → underlying read stays null forever. The
            // latch must give up after maxConsecutiveAbsent calls and let null surface,
            // so consumers' "logged out" detection still fires.
            LoggedInStateLatch latch = new LoggedInStateLatch(maxConsecutiveAbsent: 5);
            ActorItem player = Present();
            Assert.Same(player, latch.Apply(player));

            // Within the latch window — still returns cached.
            for (int i = 1; i <= 5; i++) {
                Assert.Same(player, latch.Apply(null));
            }

            // One past threshold — cache is cleared, null surfaces.
            Assert.Null(latch.Apply(null));
            // Subsequent nulls also surface (cache stays cleared).
            Assert.Null(latch.Apply(null));
        }

        [Fact]
        public void Apply_LivePresentResetsCounter_AfterPartialAbsence() {
            // If a transient absence ends BEFORE the threshold is hit, the absence
            // counter must reset so the next zone transition gets the full window.
            LoggedInStateLatch latch = new LoggedInStateLatch(maxConsecutiveAbsent: 5);
            ActorItem player = Present();
            latch.Apply(player);

            // 3 transient nulls (not enough to clear cache).
            Assert.Same(player, latch.Apply(null));
            Assert.Same(player, latch.Apply(null));
            Assert.Same(player, latch.Apply(null));

            // Live again — counter resets.
            Assert.Same(player, latch.Apply(player));
            Assert.Equal(0, latch.ConsecutiveAbsent);

            // Another 5 transient nulls — must still latch all 5 (proves the counter
            // started over, not continued from 3).
            for (int i = 1; i <= 5; i++) {
                Assert.Same(player, latch.Apply(null));
            }
        }

        [Fact]
        public void Apply_ZeroIdEntity_TreatedAsAbsent() {
            // GameObjectManager occasionally returns a slot with a half-initialised
            // Character struct (ID still 0) mid-rebuild. We treat ID==0 the same as
            // null — neither is a "real" present player.
            LoggedInStateLatch latch = new LoggedInStateLatch(maxConsecutiveAbsent: 5);
            ActorItem player = Present();
            latch.Apply(player);

            ActorItem halfInit = new ActorItem { ID = 0u };
            Assert.Same(player, latch.Apply(halfInit));
            Assert.Equal(1, latch.ConsecutiveAbsent);
        }

        [Fact]
        public void Apply_NewLivePresent_ReplacesCache() {
            // If a different character logs in (or the player ID changes for any
            // reason), the latch should hand back the freshest live value, not the
            // stale cached one.
            LoggedInStateLatch latch = new LoggedInStateLatch(maxConsecutiveAbsent: 10);
            ActorItem original = Present(0x10000001u);
            ActorItem replacement = Present(0x20000002u);

            latch.Apply(original);
            ActorItem result = latch.Apply(replacement);

            Assert.Same(replacement, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Apply_NonPositiveThreshold_DisablesLatch(int threshold) {
            // Threshold 0 (or negative) is the documented kill-switch — consumers who
            // want the raw underlying read should see null pass straight through.
            LoggedInStateLatch latch = new LoggedInStateLatch(threshold);
            ActorItem player = Present();

            Assert.Same(player, latch.Apply(player));
            Assert.Null(latch.Apply(null));
            Assert.Null(latch.Apply(null));
        }

        [Fact]
        public void Apply_RepeatedZoneTransitions_EachGetsFullLatchWindow() {
            // Real-world scenario: a teleport, then a few seconds of play, then another
            // teleport. Each transient window should get the full latch budget — this
            // is the integration-level guarantee on top of the partial-absence reset.
            LoggedInStateLatch latch = new LoggedInStateLatch(maxConsecutiveAbsent: 3);
            ActorItem player = Present();

            for (int round = 0; round < 4; round++) {
                latch.Apply(player);
                for (int i = 1; i <= 3; i++) {
                    Assert.Same(player, latch.Apply(null));
                }
                latch.Apply(player); // back to steady state
            }
        }
    }
}
