namespace BootstrappedWPF.Launcher {
    using System;
    using System.Diagnostics;
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public App() {
            this.Startup += this.OnStartup;

            this.InitializeComponent();
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
            GitHubRelease currentRelease = GitHub.GetCurrentRelease();
            if (currentRelease is null) {
                this.LaunchApplication();
            }
            else {
                AppContext.Instance.ReleaseInfo = currentRelease;
            }
        }
    }
}