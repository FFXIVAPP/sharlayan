// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppConfig.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   AppConfig.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedElectron {
    using System;
    using System.IO;

    public class AppConfig {
        private static Lazy<AppConfig> _instance = new Lazy<AppConfig>(() => new AppConfig());

        private string _cachePath;

        private string _configurationsPath;

        private string _logsPath;

        private string _settingsPath;

        public static AppConfig Instance => _instance.Value;

        public string CachePath {
            get => this._cachePath;
            set {
                if (!Directory.Exists(value)) {
                    Directory.CreateDirectory(value);
                }

                this._cachePath = value;
            }
        }

        public string ConfigurationsPath {
            get => this._configurationsPath;
            set {
                if (!Directory.Exists(value)) {
                    Directory.CreateDirectory(value);
                }

                this._configurationsPath = value;
            }
        }

        public string LogsPath {
            get => this._logsPath;
            set {
                if (!Directory.Exists(value)) {
                    Directory.CreateDirectory(value);
                }

                this._logsPath = value;
            }
        }

        public string SettingsPath {
            get => this._settingsPath;
            set {
                if (!Directory.Exists(value)) {
                    Directory.CreateDirectory(value);
                }

                this._settingsPath = value;
            }
        }
    }
}