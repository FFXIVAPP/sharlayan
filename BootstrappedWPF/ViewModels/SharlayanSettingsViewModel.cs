namespace BootstrappedWPF.ViewModels {
    using System;

    using BootstrappedWPF.Properties;

    public class SharlayanSettingsViewModel {
        private static Lazy<SharlayanSettingsViewModel> _instance = new Lazy<SharlayanSettingsViewModel>(() => new SharlayanSettingsViewModel());

        public SharlayanSettingsViewModel() {
            this.ResetSettingsCommand = new DelegatedCommand(
                _ => {
                    Settings.Default.Top = 20;
                    Settings.Default.Left = 20;
                    Settings.Default.Reset();
                });
        }

        public static SharlayanSettingsViewModel Instance => _instance.Value;
        public DelegatedCommand ResetSettingsCommand { get; }
    }
}