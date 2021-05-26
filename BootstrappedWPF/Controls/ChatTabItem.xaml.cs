namespace BootstrappedWPF.Controls {
    using System;
    using System.Windows.Controls;

    using BootstrappedWPF.Helpers;
    using BootstrappedWPF.SharlayanWrappers;

    using Sharlayan;
    using Sharlayan.Core;

    /// <summary>
    /// Interaction logic for ChatTabItem.xaml
    /// </summary>
    public partial class ChatTabItem : UserControl, IDisposable {
        public static ChatTabItem Instance;

        public ChatTabItem() {
            this.InitializeComponent();

            Instance = this;

            EventHost.Instance.OnNewChatLogItem += this.OnNewChatLogItem;
        }

        public void Dispose() {
            EventHost.Instance.OnNewChatLogItem -= this.OnNewChatLogItem;
        }

        ~ChatTabItem() {
            this.Dispose();
        }

        private void OnNewChatLogItem(object? sender, MemoryHandler memoryHandler, ChatLogItem chatLogItem) {
            FlowDocHelper.AppendChatLogItem(memoryHandler, chatLogItem, this.ChatLogReader._FDR);
        }
    }
}