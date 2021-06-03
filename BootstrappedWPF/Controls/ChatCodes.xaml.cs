namespace BootstrappedWPF.Controls {
    using System.Windows.Controls;

    using BootstrappedWPF.ViewModels;

    /// <summary>
    /// Interaction logic for ChatCodes.xaml
    /// </summary>
    public partial class ChatCodes : UserControl {
        public ChatCodes() {
            this.InitializeComponent();

            Instance = this;

            this.DataContext = ChatCodesViewModel.Instance;
        }

        public static ChatCodes Instance { get; set; }
    }
}