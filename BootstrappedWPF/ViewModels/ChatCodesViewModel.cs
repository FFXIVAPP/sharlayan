namespace BootstrappedWPF.ViewModels {
    using System;

    using BootstrappedWPF.Controls;

    public class ChatCodesViewModel {
        private static Lazy<ChatCodesViewModel> _instance = new Lazy<ChatCodesViewModel>(() => new ChatCodesViewModel());

        public ChatCodesViewModel() {
            this.SaveChatCodesCommand = new DelegatedCommand(
                _ => {
                    ChatCodes.Instance?.ChatCodesDataGrid.CommitEdit();
                });
        }

        public static ChatCodesViewModel Instance => _instance.Value;

        public DelegatedCommand SaveChatCodesCommand { get; }
    }
}