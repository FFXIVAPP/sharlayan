// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryHandlerConfiguration.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   MemoryHandlerConfiguration.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using Sharlayan.Enums;
    using Sharlayan.Models;

    public class MemoryHandlerConfiguration {
        public GameLanguage GameLanguage { get; set; } = GameLanguage.English;
        public GameRegion GameRegion { get; set; } = GameRegion.Global;
        public string PatchVersion { get; set; } = "latest";
        public ProcessModel ProcessModel { get; set; }
        public bool ScanAlMemoryRegions { get; set; } = false;
        public bool UseLocalCache { get; set; } = true;
    }
}