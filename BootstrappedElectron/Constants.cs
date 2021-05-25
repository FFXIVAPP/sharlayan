namespace BootstrappedElectron {
    using System;
    using System.IO;

    public static class Constants {
        public static string CachePath {
            get {
                try {
                    string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    return Path.Combine(documentsFolder, "BootstrappedWPF");
                }
                catch (Exception) {
                    return Path.Combine(Directory.GetCurrentDirectory(), "AppCache");
                }
            }
        }

        public static string ConfigurationsPath => Path.Combine(CachePath, "Configurations");

        public static string LogsPath => Path.Combine(CachePath, "Logs");

        public static string SettingsPath => Path.Combine(CachePath, "Settings");
    }
}