// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResourceProvider.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources {
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    using Sharlayan.Models;
    using Sharlayan.Models.Structures;
    using Sharlayan.Models.XIVDatabase;

    using StatusItem = Sharlayan.Models.XIVDatabase.StatusItem;

    public interface IResourceProvider {
        Task<StructuresContainer> GetStructuresAsync(SharlayanConfiguration configuration);

        Task<Signature[]> GetSignaturesAsync(SharlayanConfiguration configuration);

        Task GetActionsAsync(ConcurrentDictionary<uint, ActionItem> actions, SharlayanConfiguration configuration);

        Task GetStatusEffectsAsync(ConcurrentDictionary<uint, StatusItem> statusEffects, SharlayanConfiguration configuration);

        Task GetZonesAsync(ConcurrentDictionary<uint, MapItem> zones, SharlayanConfiguration configuration);
    }
}
