// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceProviderKind.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources {
    using System;

    public enum ResourceProviderKind {
        [Obsolete("sharlayan-resources is no longer maintained and its signatures/offsets drift with every FFXIV patch. Use FFXIVClientStructsDirect instead — it's now the default.")]
        LegacySharlayanResources,
        FFXIVClientStructsDirect,
    }
}
