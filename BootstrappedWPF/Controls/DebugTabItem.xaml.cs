namespace BootstrappedWPF.Controls {
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for DebugTabItem.xaml
    /// </summary>
    public partial class DebugTabItem : UserControl {
        public static DebugTabItem Instance;

        public DebugTabItem() {
            this.InitializeComponent();

            Instance = this;
        }
    }
}