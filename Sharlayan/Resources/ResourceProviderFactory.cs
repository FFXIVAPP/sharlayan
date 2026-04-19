// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceProviderFactory.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources {
    using System;

    using Sharlayan.Resources.Providers;

    internal static class ResourceProviderFactory {
        internal static IResourceProvider Create(SharlayanConfiguration configuration) {
#pragma warning disable CS0618 // The factory is the one site that's allowed to reference the obsolete enum value — consumers get the warning at the configuration site.
            return configuration.ResourceProvider switch {
                ResourceProviderKind.LegacySharlayanResources => new LegacyJsonProvider(),
                ResourceProviderKind.FFXIVClientStructsDirect => new FFXIVClientStructsDirectProvider(),
                _ => throw new NotSupportedException($"Unknown resource provider kind: {configuration.ResourceProvider}"),
            };
#pragma warning restore CS0618
        }
    }
}
