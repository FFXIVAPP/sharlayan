// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharlayanConfiguration.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   SharlayanConfiguration.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System.IO;

    using Sharlayan.Enums;
    using Sharlayan.Models;

    public class SharlayanConfiguration {
        public string APIBaseURL { get; set; } = Constants.DEFAULT_API_BASE_URL;
        public string CharacterName { get; set; }
        public GameLanguage GameLanguage { get; set; } = GameLanguage.English;
        public GameRegion GameRegion { get; set; } = GameRegion.Global;
        public string JSONCacheDirectory { get; set; } = Directory.GetCurrentDirectory();
        public string PatchVersion { get; set; } = "latest";
        public ProcessModel ProcessModel { get; set; }
        public bool ScanAllRegions { get; set; } = false;
        public bool UseLocalCache { get; set; } = true;
    }
}