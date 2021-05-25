// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Constants.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedElectron {
    using System;
    using System.IO;

    public static class Constants {
        public static string CachePath {
            get {
                try {
                    string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    return Path.Combine(documentsFolder, "BootstrappedWPF");
                }
                catch (Exception) {
                    return Path.Combine(Directory.GetCurrentDirectory(), "AppCache");
                }
            }
        }

        public static string ConfigurationsPath => Path.Combine(CachePath, "Configurations");

        public static string LogsPath => Path.Combine(CachePath, "Logs");

        public static string SettingsPath => Path.Combine(CachePath, "Settings");
    }
}