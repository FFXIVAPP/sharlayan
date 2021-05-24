namespace BootstrappedWPF.Controls {
    using System;
    using System.Windows.Controls;

    using BootstrappedWPF.Helpers;
    using BootstrappedWPF.SharlayanWrappers;
    using BootstrappedWPF.SharlayanWrappers.Events;

    /// <summary>
    /// Interaction logic for ChatTabItem.xaml
    /// </summary>
    public partial class ChatTabItem : UserControl, IDisposable {
        public static ChatTabItem Instance;

        public ChatTabItem() {
            this.InitializeComponent();

            Instance = this;

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