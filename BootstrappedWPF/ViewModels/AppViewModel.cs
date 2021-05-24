namespace BootstrappedWPF.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;

    using BootstrappedWPF.Models;

    public class AppViewModel : PropertyChangedBase {
        private static Lazy<AppViewModel> _instance = new Lazy<AppViewModel>(() => new AppViewModel());

        private string _appTitle;

        private string _cachePath;

        private string _configurationsPath;

        private ObservableCollection<LanguageItem> _interfaceLanguages;

        private Dictionary<string, string> _locale;

        private string _logsPath;

        private string _settingsPath;

        public AppViewModel() {
            this.InterfaceLanguages.Add(
                new LanguageItem {
                    Language = "English",
                    ImageURI = "pack://application:,,,/Resources/EN.png",
                    Title = "English",
                    CultureInfo = new CultureInfo("en"),
                });

            this.InterfaceLanguages.Add(
                new LanguageItem {
                    Language = "Japanese",
                    ImageURI = "pack://application:,,,/Resources/JA.png",
                    Title = "日本語",
                    CultureInfo = new CultureInfo("ja"),
                });

            this.InterfaceLanguages.Add(
                new LanguageItem {
                    Language = "French",
                    ImageURI = "pack://application:,,,/Resources/FR.png",
                    Title = "Français",
                    CultureInfo = new CultureInfo("fr"),
                });

            this.InterfaceLanguages.Add(
                new LanguageItem {
                    Language = "German",
                    ImageURI = "pack://application:,,,/Resources/DE.png",
                    Title = "Deutsch",
                    CultureInfo = new CultureInfo("de"),
                });

            this.InterfaceLanguages.Add(
                new LanguageItem {
                    Language = "Chinese",
                    ImageURI = "pack://application:,,,/Resources/ZH.png",
                    Title = "中國",
                    CultureInfo = new CultureInfo("zh"),
                });

            this.InterfaceLanguages.Add(
                new LanguageItem {
                    Language = "Korean",
                    ImageURI = "pack://application:,,,/Resources/KO.png",
                    Title = "한국어",
                    CultureInfo = new CultureInfo("ko"),
                });
        }

        public static AppViewModel Instance => _instance.Value;

        public string AppTitle {
            get => this._appTitle;
            set {
                string appTitle = "XIVLOG";
                string title = string.IsNullOrWhiteSpace(value)
                                   ? appTitle
                                   : $"{appTitle}: {value}";
                this.SetProperty(ref this._appTitle, title.ToUpperInvariant());
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

        public ObservableCollection<LanguageItem> InterfaceLanguages {
            get => this._interfaceLanguages ??= new ObservableCollection<LanguageItem>();
            set => this.SetProperty(ref this._interfaceLanguages, value);
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