// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LegacyJsonProvider.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Wraps the historical sharlayan-resources JSON fetch path. Kept as a fallback
//   when SharlayanConfiguration.ResourceProvider is LegacySharlayanResources.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Providers {
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    using Sharlayan.Models;
    using Sharlayan.Models.Structures;
    using Sharlayan.Models.XIVDatabase;
    using Sharlayan.Utilities;

    using StatusItem = Sharlayan.Models.XIVDatabase.StatusItem;

    // APIHelper is [Obsolete] for external callers, but this provider IS the legacy path —
    // it intentionally forwards to the deprecated methods. Suppress the warning here only.
#pragma warning disable CS0618 // Type or member is obsolete
    internal sealed class LegacyJsonProvider : IResourceProvider {
        public Task<StructuresContainer> GetStructuresAsync(SharlayanConfiguration configuration) {
            return APIHelper.GetStructures(configuration);
        }

        public Task<Signature[]> GetSignaturesAsync(SharlayanConfiguration configuration) {
            return APIHelper.GetSignatures(configuration);
        }

        public Task GetActionsAsync(ConcurrentDictionary<uint, ActionItem> actions, SharlayanConfiguration configuration) {
            return APIHelper.GetActions(actions, configuration);
        }

        public Task GetStatusEffectsAsync(ConcurrentDictionary<uint, StatusItem> statusEffects, SharlayanConfiguration configuration) {
            return APIHelper.GetStatusEffects(statusEffects, configuration);
        }

        public Task GetZonesAsync(ConcurrentDictionary<uint, MapItem> zones, SharlayanConfiguration configuration) {
            return APIHelper.GetZones(zones, configuration);
        }
    }
#pragma warning restore CS0618
}
