// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerInfoInterfaceParityTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   PlayerInfo carries one Level + one CurrentEXP property per job, and IPlayerInfo is the
//   abstraction consumers program against. Adding a job means touching both — and because a
//   class may legally expose members its interface doesn't, forgetting the interface compiles
//   clean and silently hides the new job from every interface-based consumer. That is exactly
//   what happened when BST was first added, so pin the parity here.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Core {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Sharlayan.Core;
    using Sharlayan.Core.Interfaces;

    using Xunit;

    public class PlayerInfoInterfaceParityTests {
        // Known, pre-existing divergence: MPMax is exposed on the concrete type only.
        // Documented rather than silently tolerated — if it is ever added to IPlayerInfo,
        // drop it from this set.
        private static readonly HashSet<string> KnownClassOnlyMembers = new(StringComparer.Ordinal) {
            "MPMax",
        };

        [Fact]
        public void IPlayerInfo_Exposes_EveryPlayerInfoProperty() {
            HashSet<string> classProps = Names(typeof(PlayerInfo));
            HashSet<string> interfaceProps = Names(typeof(IPlayerInfo));

            classProps.ExceptWith(interfaceProps);
            classProps.ExceptWith(KnownClassOnlyMembers);

            Assert.True(
                classProps.Count == 0,
                $"PlayerInfo exposes properties missing from IPlayerInfo: {string.Join(", ", classProps.OrderBy(n => n))}. "
                + "Add them to the interface (or to KnownClassOnlyMembers with a reason).");
        }

        [Fact]
        public void IPlayerInfo_DeclaresLevelAndExp_ForEveryJob() {
            // Every job that has a Level property must also have the matching _CurrentEXP,
            // on the interface. Catches a half-wired job addition.
            HashSet<string> interfaceProps = Names(typeof(IPlayerInfo));

            string[] missing = interfaceProps
                .Where(n => n.EndsWith("_CurrentEXP", StringComparison.Ordinal))
                .Select(n => n.Substring(0, n.Length - "_CurrentEXP".Length))
                .Where(job => !interfaceProps.Contains(job))
                .OrderBy(n => n)
                .ToArray();

            Assert.True(missing.Length == 0, $"IPlayerInfo has _CurrentEXP without a matching level property for: {string.Join(", ", missing)}");
        }

        [Fact]
        public void IPlayerInfo_Includes_BeastmasterMembers() {
            HashSet<string> interfaceProps = Names(typeof(IPlayerInfo));

            Assert.Contains("BST", interfaceProps);
            Assert.Contains("BST_CurrentEXP", interfaceProps);
        }

        private static HashSet<string> Names(Type type) {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Select(p => p.Name)
                       .ToHashSet(StringComparer.Ordinal);
        }
    }
}
