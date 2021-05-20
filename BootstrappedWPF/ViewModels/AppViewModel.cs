// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppViewModel.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   AppViewModel.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class AppViewModel : PropertyChangedBase {
        private static Lazy<AppViewModel> _instance = new Lazy<AppViewModel>(() => new AppViewModel());

        private string _appTitle;

        private string _cachePath;

        private string _configurationsPath;

        private Dictionary<string, string> _locale;

        private string _logsPath;

        private string _settingsPath;

        public static AppViewModel Instance => _instance.Value;

        public string AppTitle {
            get => this._appTitle;
            set {
                string appTitle = "BootstrappedWPF";
                string title = string.IsNullOrWhiteSpace(value)
                                   ? appTitle
                                   : $"{appTitle}: {value}";
                this.SetProperty(ref this._appTitle, title);
            }
        }

        public string CachePath {
            get => this._cachePath;
            set {
                if (!Directory.Exists(value)) {
                    Directory.CreateDirectory(value);
                }

                this.SetProperty(ref this._cachePath, value);
            }
        }

        public string ConfigurationsPath {
            get => this._configurationsPath;
            set {
                if (!Directory.Exists(value)) {
                    Directory.CreateDirectory(value);
                }

                this.SetProperty(ref this._configurationsPath, value);
            }
        }

        public Dictionary<string, string> Locale {
            get => this._locale ??= new Dictionary<string, string>();
            set => this.SetProperty(ref this._locale, value);
        }

        public string LogsPath {
            get => this._logsPath;
            set {
                if (!Directory.Exists(value)) {
                    Directory.CreateDirectory(value);
                }

                this.SetProperty(ref this._logsPath, value);
            }
        }

        public string SettingsPath {
            get => this._settingsPath;
            set {
                if (!Directory.Exists(value)) {
                    Directory.CreateDirectory(value);
                }

                this.SetProperty(ref this._settingsPath, value);
            }
        }
    }
}