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
    using System;
    using System.Windows.Controls;

    using BootstrappedWPF.Helpers;
    using BootstrappedWPF.SharlayanWrappers;
    using BootstrappedWPF.SharlayanWrappers.Events;
    using BootstrappedWPF.ViewModels;

    /// <summary>
    /// Interaction logic for ChatTabItem.xaml
    /// </summary>
    public partial class ChatTabItem : UserControl, IDisposable {
        public static ChatTabItem TabItem;

        public ChatTabItem() {
            this.InitializeComponent();

            this.DataContext = new ChatTabItemViewModel();

            TabItem = this;

            EventHost.Instance.OnNewChatLogItem += this.OnOnNewChatLogItem;
        }

        ~ChatTabItem() {
            this.Dispose();
        }

        public void Dispose() {
            EventHost.Instance.OnNewChatLogItem -= this.OnOnNewChatLogItem;
        }

        private void OnOnNewChatLogItem(object? sender, NewChatLogItemEvent e) {
            FlowDocHelper.AppendChatLogItem(e.MemoryHandler, e.EventData, this.ChatLogReader._FDR);
        }
    }
}