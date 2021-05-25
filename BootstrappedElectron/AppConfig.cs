namespace BootstrappedElectron {
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Xml.Linq;

    using BootstrappedElectron.Models;

    public class AppConfig {
        private static Lazy<AppConfig> _instance = new Lazy<AppConfig>(() => new AppConfig());

        private string _cachePath;

        private ObservableCollection<ChatCode> _chatCodes;

        private string _configurationsPath;

        private CultureInfo _cultureInfo;

        private string _logsPath;

        private string _settingsPath;

        private XDocument _xChatCodes;

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

        public ObservableCollection<ChatCode> ChatCodes {
            get => this._chatCodes ??= new ObservableCollection<ChatCode>();
            set => this._chatCodes = value;
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

        public XDocument XChatCodes {
            get {
                if (this._xChatCodes is not null) {
                    return this._xChatCodes;
                }

                string path = Path.Combine(this.CachePath, "Configurations", "ChatCodes.xml");
                try {
                    this._xChatCodes = File.Exists(path)
                                           ? XDocument.Load(path)
                                           : XDocument.Load(Path.Combine(Directory.GetCurrentDirectory(), "ChatCodes.xml"));
                }
                catch (Exception) {
                    this._xChatCodes = XDocument.Load(Path.Combine(Directory.GetCurrentDirectory(), "ChatCodes.xml"));
                }

                return this._xChatCodes;
            }
            set => this._xChatCodes = value;
        }
    }
}