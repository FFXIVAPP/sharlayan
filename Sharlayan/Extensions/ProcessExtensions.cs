// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessExtensions.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ProcessExtensions.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Extensions {
    using System.Diagnostics;
    using System.IO;

    using Sharlayan.Enums;

    public static class ProcessExtensions {
        public static GameRegion GameRegion(this Process source) {
            GameRegion region = Enums.GameRegion.Global;
            if (source.MainModule != null) {
                string gameDirectory = Directory.GetParent(source.MainModule.FileName)?.FullName;
                if (File.Exists($"{gameDirectory}\\boot\\locales\\ko.pak")) {
                    return Enums.GameRegion.Korea;
                }

                if (File.Exists($"{gameDirectory}\\sdo")) {
                    return Enums.GameRegion.China;
                }
            }

            return region;
        }
    }
}