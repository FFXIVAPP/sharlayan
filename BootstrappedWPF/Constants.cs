namespace BootstrappedWPF {
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Xml.Linq;

    using BootstrappedWPF.Helpers;
    using BootstrappedWPF.Models;

    public class Constants : PropertyChangedBase {
        public const string AppPack = "pack://application:,,,/BootstrappedWPF;component/";

        private static Lazy<Constants> _instance = new Lazy<Constants>(() => new Constants());

        private ObservableCollection<ChatCode> _chatCodes;

        private CultureInfo _cultureInfo;

        private XDocument _xChatCodes;

        public static Constants Instance => _instance.Value;

        public string CachePath {
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

        public ObservableCollection<ChatCode> ChatCodes {
            get => this._chatCodes ??= new ObservableCollection<ChatCode>();
            set => this.SetProperty(ref this._chatCodes, value);
        }

        public string ConfigurationsPath => Path.Combine(this.CachePath, "Configurations");

        public CultureInfo CultureInfo {
            get => this._cultureInfo ??= new CultureInfo("en");
            set => this.SetProperty(ref this._cultureInfo, value);
        }

        public string LogsPath => Path.Combine(this.CachePath, "Logs");

        public string SettingsPath => Path.Combine(this.CachePath, "Settings");

        public XDocument XChatCodes {
            get {
                if (this._xChatCodes is not null) {
                    return this._xChatCodes;
                }

                string path = Path.Combine(this.CachePath, "Configurations", "ChatCodes.xml");
                try {
                    this._xChatCodes = File.Exists(path)
                                           ? XDocument.Load(path)
                                           : ResourceHelper.LoadXML($"{AppPack}Resources/ChatCodes.xml");
                }
                catch (Exception) {
                    this._xChatCodes = ResourceHelper.LoadXML($"{AppPack}Resources/ChatCodes.xml");
                }

                return this._xChatCodes;
            }
            set => this.SetProperty(ref this._xChatCodes, value);
        }
    }
}