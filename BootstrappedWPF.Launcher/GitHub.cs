namespace BootstrappedWPF.Launcher {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Cache;

    using Newtonsoft.Json;

    public static class GitHub {
        public static GitHubRelease GetCurrentRelease() {
            try {
                string url = "https://api.github.com/repos/FFXIVAPP/BootstrappedWPF/releases";

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
                        string fileVersion = localFileVersionInfo.FileVersion;
                        if (!string.IsNullOrWhiteSpace(fileVersion)) {
                            Version localVersion = new Version(fileVersion);
                            if (localVersion.CompareTo(releasedVersion) < 0) {
                                return release;
                            }
                        }
                    }
                }
            }
            catch (Exception) {
                // IGNORED i.e. GitHub Errors
            }

            return null;
        }
    }
}