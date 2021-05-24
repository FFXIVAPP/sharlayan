namespace BootstrappedWPF.Launcher {
    using System.Collections.Generic;

    public class GitHubRelease {
        public List<ReleaseAsset> assets { get; set; }
        public string tag_name { get; set; }
        public string target_commitish { get; set; }
    }

    public class ReleaseAsset {
        public string browser_download_url { get; set; }
        public string name { get; set; }
    }
}
