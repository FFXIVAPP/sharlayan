namespace BootstrappedWPF.Controls {
    using System.Windows.Controls;

    using BootstrappedWPF.ViewModels;

    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SharlayanSettings : UserControl {
        public SharlayanSettings() {
            this.InitializeComponent();

            this.DataContext = SharlayanSettingsViewModel.Instance;
        }
    }
}