// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggedInStateLatch.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2026 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Debounces transient null reads of the local-player ActorItem produced by
//   <see cref="Reader.GetCurrentPlayer"/>. FFXIV briefly zeroes the local-player slot
//   in GameObjectManager during any zone transition (teleport, instance entry, Gold
//   Saucer mini-game entry, etc.) while the actor table rebuilds — typically 1–3
//   poll ticks. Without latching, every downstream consumer sees Entity flip to null
//   for that window and has to debounce it themselves. The latch is purely
//   defensive: once it has seen a non-null Entity, it keeps returning that cached
//   value for up to N consecutive null reads before letting null surface again, on
//   the assumption that a sustained absence is a real logout / disconnect.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System.Threading;

    using Sharlayan.Core;

    internal sealed class LoggedInStateLatch {
        private readonly int _maxConsecutiveAbsent;
        private ActorItem _cached;
        private int _consecutiveAbsent;

        public LoggedInStateLatch(int maxConsecutiveAbsent) {
            this._maxConsecutiveAbsent = maxConsecutiveAbsent;
        }

        // Exposed for tests; production code reads the count through Apply()'s return value.
        internal int ConsecutiveAbsent => Volatile.Read(ref this._consecutiveAbsent);

        internal ActorItem Cached => this._cached;

        /// <summary>
        /// Returns <paramref name="live"/> when it represents a present local player
        /// (non-null Entity with non-zero ID). Otherwise returns the most recently cached
        /// present Entity if the latch hasn't expired, falling through to <paramref name="live"/>
        /// (typically null) once the underlying read has stayed absent for more than
        /// <c>maxConsecutiveAbsent</c> consecutive calls.
        /// </summary>
        public ActorItem Apply(ActorItem live) {
            // maxConsecutiveAbsent <= 0 disables latching entirely — handy as a kill-switch
            // for downstream consumers that want the raw underlying read.
            if (this._maxConsecutiveAbsent <= 0) {
                return live;
            }

            if (live != null && live.ID != 0u) {
                this._cached = live;
                Volatile.Write(ref this._consecutiveAbsent, 0);
                return live;
            }

            if (this._cached != null) {
                int n = Interlocked.Increment(ref this._consecutiveAbsent);
                if (n <= this._maxConsecutiveAbsent) {
                    return this._cached;
                }
                this._cached = null;
            }

            return live;
        }
    }
}
