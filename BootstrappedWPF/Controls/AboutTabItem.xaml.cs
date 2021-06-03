namespace BootstrappedWPF.Controls {
    using System.Windows;
    using System.Windows.Controls;

    using BootstrappedWPF.Utilities;

    /// <summary>
    /// Interaction logic for HomeTabItem.xaml
    /// </summary>
    public partial class AboutTabItem : UserControl {
        public AboutTabItem() {
            this.InitializeComponent();
        }

        private void ChatButton_OnClick(object sender, RoutedEventArgs e) {
            Link.OpenInBrowser("https://discord.gg/aCzSANp");
        }

        private void DonateButton_OnClick(object sender, RoutedEventArgs e) {
            Link.OpenInBrowser("https://github.com/sponsors/Icehunter");
        }

        private void GitHubButton_OnClick(object sender, RoutedEventArgs e) {
            Link.OpenInBrowser("https://github.com/FFXIVAPP");
        }
    }
}