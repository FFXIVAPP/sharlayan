// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   MainWindow.xaml.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF {
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Threading;

    using BootstrappedWPF.Helpers;
    using BootstrappedWPF.Properties;
    using BootstrappedWPF.ViewModels;

    using MaterialDesignThemes.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();

        public MainWindow() {
            this.InitializeComponent();

            this.DataContext = new MainWindowViewModel();
        }

        private void CloseApplication() {
            if (Application.Current.MainWindow != null) {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }

            Settings.Default.Save();
            SettingsHelper.SaveChatCodes();
            Environment.Exit(0);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e) {
            e.Cancel = true;
            DispatcherHelper.Invoke(this.CloseApplication, DispatcherPriority.Send);
        }

        private void MainWindow_OnContentRendered(object? sender, EventArgs e) {
            LocaleHelper.UpdateLocale(Settings.Default.Culture);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e) {
            AppContext.Instance.Initialize();
        }
    }
}