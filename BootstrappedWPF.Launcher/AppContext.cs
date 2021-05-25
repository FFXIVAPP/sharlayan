namespace BootstrappedWPF.Launcher {
    using System;

    public class AppContext {
        private static Lazy<AppContext> _instance = new Lazy<AppContext>(() => new AppContext());

        public static AppContext Instance => _instance.Value;

        public GitHubRelease ReleaseInfo { get; set; }
    }
}