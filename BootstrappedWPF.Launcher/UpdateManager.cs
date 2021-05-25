namespace BootstrappedWPF.Launcher {
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Cache;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    using Ionic.Zip;

    public static class UpdateManager {
        private static readonly WebClient WebClient = new WebClient {
            CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore),
        };

        public static void DownloadUpdate() {
            try {
                GitHubReleaseAsset asset = AppContext.Instance.ReleaseInfo.assets[0];

                Uri uri = new Uri(asset.browser_download_url);
                string name = asset.name;

                WebClient.DownloadFileCompleted += WebClient_OnDownloadFileCompleted;
                WebClient.DownloadProgressChanged += WebClient_OnDownloadProgressChanged;
                WebClient.DownloadFileAsync(uri, name);
            }
            catch (Exception) {
                Environment.Exit(0);
            }
        }

        private static void ExtractUpdate() {
            GitHubReleaseAsset asset = AppContext.Instance.ReleaseInfo.assets[0];
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

            WebClient.Dispose();

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
                WebClient.DownloadFileCompleted -= WebClient_OnDownloadFileCompleted;
                WebClient.DownloadProgressChanged -= WebClient_OnDownloadProgressChanged;

                Environment.Exit(0);
            }
        }

        private static void WebClient_OnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e) {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new ThreadStart(ExtractUpdate));
        }

        private static void WebClient_OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            double bytesReceived = double.Parse(e.BytesReceived.ToString(CultureInfo.InvariantCulture));
            double totalBytesToReceive = double.Parse(e.TotalBytesToReceive.ToString(CultureInfo.InvariantCulture));

            MainWindow.Instance.ProgressBarSingle.Value = bytesReceived / totalBytesToReceive;
        }
    }
}