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
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Xml.Linq;

    using BootstrappedElectron.Models;

    public class Constants {
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
            set => this._chatCodes = value;
        }

        public string ConfigurationsPath => Path.Combine(this.CachePath, "Configurations");

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