// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   MainWindowViewModel.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.ViewModels {
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;

    using BootstrappedWPF.Controls;
    using BootstrappedWPF.Models;
    using BootstrappedWPF.Properties;

    public class MainWindowViewModel : PropertyChangedBase {
        private ObservableCollection<LanguageItem> _interfaceLanguages;

        private int _selectedIndex;

        private LanguageItem? _selectedInterfaceLanguage;

        private ViewItem? _selectedItem;

        public MainWindowViewModel() {
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

            this.HomeView = new ViewItem("Home", typeof(HomeTabItem));
            this.SettingsView = new ViewItem("Settings", typeof(SettingsTabItem));
            this.ChatView = new ViewItem("Chat", typeof(ChatTabItem));
            this.DebugView = new ViewItem("Debug", typeof(DebugTabItem));

            this.SelectedItem = this.HomeView;
            this.SelectedInterfaceLanguage = this.InterfaceLanguages.FirstOrDefault(item => string.Equals(item.Language, Settings.Default.InterfaceLanguage, StringComparison.OrdinalIgnoreCase));

            this.HomeCommand = new DelegatedCommand(
                _ => {
                    this.SelectedIndex = 0;
                    this.SelectedItem = this.HomeView;
                });
            this.SettingsCommand = new DelegatedCommand(
                _ => {
                    this.SelectedIndex = 1;
                    this.SelectedItem = this.SettingsView;
                });
            this.ChatCommand = new DelegatedCommand(
                _ => {
                    this.SelectedIndex = 2;
                    this.SelectedItem = this.ChatView;
                });
            this.DebugCommand = new DelegatedCommand(
                _ => {
                    this.SelectedIndex = 3;
                    this.SelectedItem = this.DebugView;
                });

            this.UpdateInterfaceLanguage = new DelegatedCommand(
                value => {
                    if (value is LanguageItem item) {
                        this.SelectedInterfaceLanguage = item;
                        Settings.Default.InterfaceLanguage = item.Language;
                        Settings.Default.Culture = item.CultureInfo;
                    }
                });
        }

        public DelegatedCommand ChatCommand { get; }
        public ViewItem ChatView { get; set; }

        public DelegatedCommand DebugCommand { get; }
        public ViewItem DebugView { get; set; }

        public DelegatedCommand HomeCommand { get; }

        public ViewItem HomeView { get; set; }

        public ObservableCollection<LanguageItem> InterfaceLanguages {
            get => this._interfaceLanguages ??= new ObservableCollection<LanguageItem>();
            set => this.SetProperty(ref this._interfaceLanguages, value);
        }

        public int SelectedIndex {
            get => this._selectedIndex;
            set => this.SetProperty(ref this._selectedIndex, value);
        }

        public LanguageItem? SelectedInterfaceLanguage {
            get => this._selectedInterfaceLanguage;
            set => this.SetProperty(ref this._selectedInterfaceLanguage, value);
        }

        public ViewItem? SelectedItem {
            get => this._selectedItem;
            set {
                this.SetProperty(ref this._selectedItem, value);
                AppViewModel.Instance.AppTitle = value?.Name;
            }
        }

        public DelegatedCommand SettingsCommand { get; }
        public ViewItem SettingsView { get; set; }

        public DelegatedCommand UpdateInterfaceLanguage { get; }
    }
}