// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LuminaXivDatabaseProvider.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reads action / status / zone data directly from the player's installed FFXIV client
//   via Lumina. Replaces the xivdatabase JSON slice of sharlayan-resources.
//   Only services xivdatabase methods; struct/signature methods throw.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Providers {
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    using Sharlayan.Models;
    using Sharlayan.Models.Structures;
    using Sharlayan.Models.XIVDatabase;

    using StatusItem = Sharlayan.Models.XIVDatabase.StatusItem;

    internal sealed class LuminaXivDatabaseProvider : IResourceProvider {
        public Task<StructuresContainer> GetStructuresAsync(SharlayanConfiguration configuration) {
            throw new NotSupportedException("LuminaXivDatabaseProvider does not supply memory structures — use ClientStructsYamlProvider.");
        }

        public Task<Signature[]> GetSignaturesAsync(SharlayanConfiguration configuration) {
            throw new NotSupportedException("LuminaXivDatabaseProvider does not supply memory signatures — use ClientStructsYamlProvider.");
        }

        public Task GetActionsAsync(ConcurrentDictionary<uint, ActionItem> actions, SharlayanConfiguration configuration) {
            throw new NotImplementedException("LuminaXivDatabaseProvider.GetActionsAsync is pending implementation (P3-09).");
        }

        public Task GetStatusEffectsAsync(ConcurrentDictionary<uint, StatusItem> statusEffects, SharlayanConfiguration configuration) {
            throw new NotImplementedException("LuminaXivDatabaseProvider.GetStatusEffectsAsync is pending implementation (P3-09).");
        }

        public Task GetZonesAsync(ConcurrentDictionary<uint, MapItem> zones, SharlayanConfiguration configuration) {
            throw new NotImplementedException("LuminaXivDatabaseProvider.GetZonesAsync is pending implementation (P3-09).");
        }
    }
}
