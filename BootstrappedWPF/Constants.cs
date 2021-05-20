// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Constants.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF {
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Xml.Linq;

    using BootstrappedWPF.Helpers;
    using BootstrappedWPF.Models;

    public class Constants : PropertyChangedBase {
        public const string AppPack = "pack://application:,,,/BootstrappedWPF;component/";

        private static Lazy<Constants> _instance = new Lazy<Constants>(() => new Constants());

        private ObservableCollection<ChatCode> _chatCodes;

        private ObservableCollection<ChatColor> _chatColors;

        private XDocument _xChatCodes;

        private XDocument _xChatColors;

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

        public ObservableCollection<ChatColor> ChatColors {
            get => this._chatColors ??= new ObservableCollection<ChatColor>();
            set => this.SetProperty(ref this._chatColors, value);
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
                                           : ResourceHelper.LoadXML($"{AppPack}Resources/ChatCodes.xml");
                }
                catch (Exception) {
                    this._xChatCodes = ResourceHelper.LoadXML($"{AppPack}Resources/ChatCodes.xml");
                }

                return this._xChatCodes;
            }
            set => this.SetProperty(ref this._xChatCodes, value);
        }

        public XDocument XChatColors {
            get {
                if (this._xChatColors is not null) {
                    return this._xChatColors;
                }

                string path = Path.Combine(this.CachePath, "Configurations", "ChatColors.xml");
                try {
                    this._xChatColors = File.Exists(path)
                                            ? XDocument.Load(path)
                                            : ResourceHelper.LoadXML($"{AppPack}Resources/ChatColors.xml");
                }
                catch (Exception) {
                    this._xChatColors = ResourceHelper.LoadXML($"{AppPack}Resources/ChatColors.xml");
                }

                return this._xChatColors;
            }
            set => this.SetProperty(ref this._xChatColors, value);
        }
    }
}