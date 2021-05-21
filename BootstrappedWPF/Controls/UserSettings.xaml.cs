// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserSettings.xaml.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   UserSettings.xaml.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.Controls {
    using System.Windows;
    using System.Windows.Controls;

    using BootstrappedWPF.Properties;

    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class UserSettings : UserControl {
        public UserSettings() {
            this.InitializeComponent();
        }

        private void ResetSettingsButton_OnClick(object sender, RoutedEventArgs e) {
            Settings.Default.Reset();
        }
    }
}