// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Pictomancer.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Pictomancer.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;

    using Sharlayan.Core.Interfaces;
    using Sharlayan.Core.JobResources.Enums;

    public sealed class PictomancerResources : IJobResource {
        public byte PalleteGauge { get; set; }

        public byte WhitePaint { get; set; }

        public CanvasFlags CanvasFlags { get; set; }

        public CreatureFlags CreatureFlags { get; set; }

        public bool CreatureMotifDrawn => CanvasFlags.HasFlag(CanvasFlags.Pom) || CanvasFlags.HasFlag(CanvasFlags.Wing) || CanvasFlags.HasFlag(CanvasFlags.Claw) || CanvasFlags.HasFlag(CanvasFlags.Maw);

        public bool WeaponMotifDrawn => CanvasFlags.HasFlag(CanvasFlags.Weapon);

        public bool LandscapeMotifDrawn => CanvasFlags.HasFlag(CanvasFlags.Landscape);

        public bool MooglePortraitReady => CreatureFlags.HasFlag(CreatureFlags.MooglePortait);

        public bool MadeenPortraitReady => CreatureFlags.HasFlag(CreatureFlags.MadeenPortrait);

        public TimeSpan Timer { get; set; }

    }
}