// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatTabItem.xaml.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ChatTabItem.xaml.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.Controls {
    using System.Windows.Controls;

    using BootstrappedWPF.ViewModels;

    /// <summary>
    /// Interaction logic for ChatTabItem.xaml
    /// </summary>
    public partial class ChatTabItem : UserControl {
        public static ChatTabItem TabItem;

        public ChatTabItem() {
            this.InitializeComponent();

            this.DataContext = new ChatTabItemViewModel();

            TabItem = this;
        }
    }
}