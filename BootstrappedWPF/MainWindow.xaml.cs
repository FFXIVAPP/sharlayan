namespace BootstrappedWPF {
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Threading;

    using BootstrappedWPF.Helpers;
    using BootstrappedWPF.Properties;
    using BootstrappedWPF.ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
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
            SavedLogsHelper.SaveCurrentLog();

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