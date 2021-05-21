// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeTabItem.xaml.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   HomeTabItem.xaml.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.Controls {
    using System.Windows;
    using System.Windows.Controls;

    using BootstrappedWPF.Utilities;

    /// <summary>
    /// Interaction logic for HomeTabItem.xaml
    /// </summary>
    public partial class HomeTabItem : UserControl {
        public HomeTabItem() {
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