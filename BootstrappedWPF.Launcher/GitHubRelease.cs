namespace BootstrappedWPF.Launcher {
    using System.Collections.Generic;

    public class GitHubRelease {
        public List<GitHubReleaseAsset> assets { get; set; }
        public string tag_name { get; set; }
        public string target_commitish { get; set; }
    }
}