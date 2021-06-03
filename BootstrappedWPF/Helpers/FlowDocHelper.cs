namespace BootstrappedWPF.Helpers {
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    using BootstrappedWPF.Converters;
    using BootstrappedWPF.ViewModels;

    using Sharlayan;
    using Sharlayan.Core;

    public static class FlowDocHelper {
        private static readonly StringToBrushConverter _converter = new StringToBrushConverter();

        public static void AppendChatLogItem(MemoryHandler memoryHandler, ChatLogItem chatLogItem, FlowDocumentReader reader) {
            DispatcherHelper.Invoke(
                () => {
                    Span process = new Span(new Run($"[{memoryHandler.Configuration.ProcessModel.ProcessID}] ")) {
                        Foreground = (Brush) _converter.Convert("#FFFFFFFF"),
                        FontWeight = FontWeights.Bold,
                    };
                    Span time = new Span(new Run(chatLogItem.TimeStamp.ToString("[HH:mm:ss] "))) {
                        Foreground = Brushes.MediumPurple,
                        FontWeight = FontWeights.Bold,
                    };
                    string lineColor = AppViewModel.Instance.ChatCodes.FirstOrDefault(code => code.Code.Equals(chatLogItem.Code))?.Color ?? "FFFFFF";
                    Span line = new Span(new Run(chatLogItem.Message)) {
                        Foreground = (Brush) _converter.Convert($@"#{lineColor}"),
                    };
                    Paragraph paragraph = new Paragraph();
                    paragraph.Inlines.Add(process);
                    paragraph.Inlines.Add(time);
                    if (!string.IsNullOrWhiteSpace(chatLogItem.PlayerName)) {
                        Span playerLine = new Span(new Run($"[{chatLogItem.PlayerName}] ")) {
                            Foreground = (Brush) _converter.Convert("#FFFF00FF"),
                        };
                        paragraph.Inlines.Add(playerLine);
                    }

                    paragraph.Inlines.Add(line);
                    reader.Document.Blocks.Add(paragraph);
                    if (reader.Document.Blocks.LastBlock != null) {
                        reader.Document.Blocks.LastBlock.Loaded += MessageAdded;
                    }
                });
        }

        public static void AppendMessage(MemoryHandler memoryHandler, string message, FlowDocumentReader reader) {
            string newMessage = $"[{memoryHandler.Configuration.ProcessModel.ProcessID}] {message}";
            AppendMessage(newMessage, reader);
        }

        public static void AppendMessage(string message, FlowDocumentReader reader) {
            DispatcherHelper.Invoke(
                () => {
                    Paragraph paragraph = new Paragraph();
                    paragraph.Inlines.Add(new Span(new Run(message)));
                    reader.Document.Blocks.Add(paragraph);
                    if (reader.Document.Blocks.LastBlock != null) {
                        reader.Document.Blocks.LastBlock.Loaded += MessageAdded;
                    }
                });
        }

        private static void MessageAdded(object sender, RoutedEventArgs e) {
            Block block = (Block) sender;
            block.BringIntoView();
            block.Loaded -= MessageAdded;
        }
    }
}