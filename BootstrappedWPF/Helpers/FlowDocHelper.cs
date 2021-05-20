// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlowDocHelper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   FlowDocHelper.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.Helpers {
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    using BootstrappedWPF.Converters;

    using Sharlayan;
    using Sharlayan.Core;

    public static class FlowDocHelper {
        private static readonly StringToBrushConverter _converter = new StringToBrushConverter();

        public static void AppendChatLogItem(MemoryHandler memoryHandler, ChatLogItem chatLogItem, FlowDocumentReader reader) {
            Task.Run(
                () => DispatcherHelper.Invoke(
                    () => {
                        Span process = new Span(new Run($"[{memoryHandler.Configuration.ProcessModel.ProcessID}] ")) {
                            Foreground = (Brush) _converter.Convert("#FF000000"),
                            FontWeight = FontWeights.Bold,
                        };
                        Span time = new Span(new Run(chatLogItem.TimeStamp.ToString("[HH:mm:ss] "))) {
                            Foreground = (Brush) _converter.Convert("#FF0000FF"),
                            FontWeight = FontWeights.Bold,
                        };
                        Span line = new Span(new Run(chatLogItem.Message)) {
                            Foreground = (Brush) _converter.Convert("#FF000000"),
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
                    }));
        }

        public static void AppendMessage(MemoryHandler memoryHandler, string message, FlowDocumentReader reader) {
            string newMessage = $"[{memoryHandler.Configuration.ProcessModel.ProcessID}] {message}";
            AppendMessage(newMessage, reader);
        }

        public static void AppendMessage(string message, FlowDocumentReader reader) {
            Task.Run(
                () => DispatcherHelper.Invoke(
                    () => {
                        Paragraph paragraph = new Paragraph();
                        paragraph.Inlines.Add(new Span(new Run(message)));
                        reader.Document.Blocks.Add(paragraph);
                        if (reader.Document.Blocks.LastBlock != null) {
                            reader.Document.Blocks.LastBlock.Loaded += MessageAdded;
                        }
                    }));
        }

        private static void MessageAdded(object sender, RoutedEventArgs e) {
            Block block = (Block) sender;
            block.BringIntoView();
            block.Loaded -= MessageAdded;
        }
    }
}