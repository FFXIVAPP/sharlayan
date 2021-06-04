namespace BootstrappedWPF.Launcher {
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public static MainWindow Instance;

        public MainWindow() {
            this.InitializeComponent();

            Instance = this;
        }

        private void CleanupTemporary() {
            try {
                string path = Directory.GetCurrentDirectory();
                FileInfo[] files = new DirectoryInfo(path).GetFiles();
                foreach (FileInfo file in files) {
                    if (file.Extension == ".tmp" || file.Extension == ".PendingOverwrite") {
                        file.Delete();
                    }
                }
            }
            catch (Exception) {
                // IGNORED
            }
        }

        private void CloseUpdater_OnClick(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown(0);
        }

        private void MainWindow_OnClosed(object? sender, EventArgs e) {
            this.CleanupTemporary();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e) {
            this.CleanupTemporary();

            Process[] processes = Process.GetProcessesByName("BootstrappedWPF");
            foreach (Process process in processes) {
                try {
                    process.Kill();
                }
                catch (Exception) {
                    // IGNORED
                }
            }

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new ThreadStart(UpdateManager.DownloadUpdate));
        }

        private void UIElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}