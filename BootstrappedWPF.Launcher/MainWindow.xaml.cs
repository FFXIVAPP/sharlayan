namespace BootstrappedWPF.Launcher {
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Ionic.Zip;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private WebClient _webClient = new WebClient();

        public MainWindow() {
            this.InitializeComponent();
        }

        private void CleanupTemporary() {
            try {
                string path = Directory.GetCurrentDirectory();
                FileInfo[] files = new DirectoryInfo(path).GetFiles();
                foreach (FileInfo file in files.Where(t => t.Extension == ".tmp" || t.Extension == ".PendingOverwrite")) {
                    file.Delete();
                }
            }
            catch (Exception) {
                // IGNORED
            }
        }

        private void CloseUpdater_OnClick(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown(0);
        }

        private void DownloadRelease() {
            try {
                ReleaseAsset asset = AppContext.Instance.ReleaseInfo.assets[0];

                Uri uri = new Uri(asset.browser_download_url);
                string name = asset.name;

                this._webClient.DownloadFileCompleted += this.WebClient_OnDownloadFileCompleted;
                this._webClient.DownloadProgressChanged += this.WebClient_OnDownloadProgressChanged;
                this._webClient.DownloadFileAsync(uri, name);
            }
            catch (Exception) {
                Environment.Exit(0);
            }
        }

        private void ExtractRelease() {
            ReleaseAsset asset = AppContext.Instance.ReleaseInfo.assets[0];
            string name = asset.name;

            using ZipFile zipFile = ZipFile.Read(name);
            foreach (ZipEntry entry in zipFile) {
                try {
                    if (File.Exists("BootstrappedWPF.exe.nlog") && entry.FileName.Contains("BootstrappedWPF.exe.nlog")) {
                        continue;
                    }

                    entry.Extract(Directory.GetCurrentDirectory(), ExtractExistingFileAction.OverwriteSilently);
                }
                catch (Exception) {
                    // IGNORED
                }
            }

            this._webClient.Dispose();
            try {
                Process process = new Process {
                    StartInfo = {
                        FileName = "BootstrappedWPF.exe",
                    },
                };
                process.Start();
            }
            catch (Exception) {
                // IGNORED
            }
            finally {
                this._webClient.DownloadFileCompleted -= this.WebClient_OnDownloadFileCompleted;
                this._webClient.DownloadProgressChanged -= this.WebClient_OnDownloadProgressChanged;
                Environment.Exit(0);
            }
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

            Task.Run(() => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new ThreadStart(this.DownloadRelease)));
        }

        private void UIElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void WebClient_OnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e) {
            Task.Run(() => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new ThreadStart(this.ExtractRelease)));
        }

        private void WebClient_OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            double bytesIn = double.Parse(e.BytesReceived.ToString(CultureInfo.InvariantCulture));
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString(CultureInfo.InvariantCulture));
            this.ProgressBarSingle.Value = bytesIn / totalBytes;
        }
    }
}
