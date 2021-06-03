namespace BootstrappedWPF.Controls {
    using System.Windows.Controls;

    using BootstrappedWPF.ViewModels;

    /// <summary>
    /// Interaction logic for PaletteSelector.xaml
    /// </summary>
    public partial class PaletteSelector : UserControl {
        public PaletteSelector() {
            this.InitializeComponent();

            this.DataContext = PaletteSelectorViewModel.Instance;
        }
    }
}