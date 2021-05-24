namespace BootstrappedWPF.Launcher {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Cache;
    using System.Windows;

    using Newtonsoft.Json;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public App() {
            this.Startup += this.OnStartup;

            this.InitializeComponent();
        }

        private GitHubRelease GetCurrentReleaseInfo() {
            try {
                string url = "https://api.github.com/repos/FFXIVAPP/xivlog/releases";

                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                request.UserAgent = "FFXIVAPP";
                request.Headers.Add("Accept-Language", "en;q=0.8");
                request.ContentType = "application/json; charset=utf-8";
                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

                using HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                using Stream stream = response.GetResponseStream();
                using StreamReader reader = new StreamReader(stream);

                List<GitHubRelease> releases = JsonConvert.DeserializeObject<List<GitHubRelease>>(reader.ReadToEnd());

                if (releases is not null && releases.Any()) {
                    GitHubRelease release = releases.FirstOrDefault(item => item.target_commitish == "main");
                    if (release is not null) {
                        Version releasedVersion = new Version(release.tag_name);
                        FileVersionInfo localFileVersionInfo = FileVersionInfo.GetVersionInfo("BootstrappedWPF.exe");
                        string productVersions = localFileVersionInfo.FileVersion;
                        if (!string.IsNullOrWhiteSpace(productVersions)) {
                            Version localVersion = new Version(productVersions);
                            if (localVersion.CompareTo(releasedVersion) < 0) {
                                return release;
                            }
                        }
                    }
                }
            }
            catch (Exception) {
                // IGNORED JUST LAUNCH EXISTING
            }

            return null;
        }

        private void LaunchApplication() {
            try {
                Process process = new Process {
                    StartInfo = {
                        FileName = "BootstrappedWPF.exe",
                    },
                };
                process.Start();
            }
            catch (Exception ex) {
                MessageBox.Show($"{ex.Message} [BootstrappedWPF.exe]", "Exception");
            }
            finally {
                Environment.Exit(0);
            }
        }

        private void OnStartup(object sender, StartupEventArgs e) {
            GitHubRelease currentReleaseInfo = this.GetCurrentReleaseInfo();
            if (currentReleaseInfo is null) {
                this.LaunchApplication();
            }
            else {
                AppContext.Instance.ReleaseInfo = currentReleaseInfo;
            }
        }
    }
}
