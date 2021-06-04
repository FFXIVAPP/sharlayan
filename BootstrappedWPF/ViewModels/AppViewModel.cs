namespace BootstrappedWPF.ViewModels {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Xml.Linq;

    using BootstrappedWPF.Helpers;
    using BootstrappedWPF.Models;

    using Sharlayan.Core;

    public class AppViewModel : PropertyChangedBase {
        private static Lazy<AppViewModel> _instance = new Lazy<AppViewModel>(() => new AppViewModel());

        private string _appTitle;

        private string _cachePath;

        private ObservableCollection<ChatCode> _chatCodes;

        private string _configurationsPath;

        private CultureInfo _cultureInfo;

        private ObservableCollection<LanguageItem> _interfaceLanguages;

        private ConcurrentDictionary<string, string> _locale;

        private string _logsPath;

        private List<string> _savedLogsDirectoryList;

        private string _settingsPath;

        private XDocument _xChatCodes;

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
                string appTitle = "BootstrappedWPF";
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

        public ObservableCollection<ChatCode> ChatCodes {
            get => this._chatCodes ??= new ObservableCollection<ChatCode>();
            set => this.SetProperty(ref this._chatCodes, value);
        }

        public ConcurrentDictionary<string, List<ChatLogItem>> ChatHistory { get; } = new ConcurrentDictionary<string, List<ChatLogItem>>();

        public string ConfigurationsPath {
            get => this._configurationsPath;
            set {
                if (!Directory.Exists(value)) {
                    Directory.CreateDirectory(value);
                }

                this.SetProperty(ref this._configurationsPath, value);
            }
        }

        public CultureInfo CultureInfo {
            get => this._cultureInfo ??= new CultureInfo("en");
            set => this.SetProperty(ref this._cultureInfo, value);
        }

        public ObservableCollection<LanguageItem> InterfaceLanguages {
            get => this._interfaceLanguages ??= new ObservableCollection<LanguageItem>();
            set => this.SetProperty(ref this._interfaceLanguages, value);
        }

        public ConcurrentDictionary<string, string> Locale {
            get => this._locale ??= new ConcurrentDictionary<string, string>();
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

        public List<string> SavedLogsDirectoryList {
            get => this._savedLogsDirectoryList ??= new List<string>();
            set {
                List<string> directoryPaths = value;
                foreach (string directoryPath in directoryPaths) {
                    string path = Path.Combine(this.LogsPath, directoryPath);
                    if (!Directory.Exists(path)) {
                        Directory.CreateDirectory(path);
                    }
                }

                this.SetProperty(ref this._savedLogsDirectoryList, value);
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

        public XDocument XChatCodes {
            get {
                if (this._xChatCodes is not null) {
                    return this._xChatCodes;
                }

                string path = Path.Combine(this.CachePath, "Configurations", "ChatCodes.xml");
                try {
                    this._xChatCodes = File.Exists(path)
                                           ? XDocument.Load(path)
                                           : ResourceHelper.LoadXML($"{Constants.AppPack}Resources/ChatCodes.xml");
                }
                catch (Exception) {
                    this._xChatCodes = ResourceHelper.LoadXML($"{Constants.AppPack}Resources/ChatCodes.xml");
                }

                return this._xChatCodes;
            }
            set => this.SetProperty(ref this._xChatCodes, value);
        }

        public ConcurrentDictionary<int, Process> GameInstances { get; set; } = new ConcurrentDictionary<int, Process>();
    }
}