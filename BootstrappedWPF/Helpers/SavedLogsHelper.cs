namespace BootstrappedWPF.Helpers {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    using BootstrappedWPF.Models;
    using BootstrappedWPF.Utilities;
    using BootstrappedWPF.ViewModels;

    using NLog;

    using Sharlayan.Core;

    public static class SavedLogsHelper {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static StringBuilder _crossWorldLinkShellStringBuilder = new StringBuilder();

        private static StringBuilder _linkShellStringBuilder = new StringBuilder();

        private static StringBuilder _tellStringBuilder = new StringBuilder();

        private static Dictionary<string, StringBuilder> _textLogBuilders = new Dictionary<string, StringBuilder> {
            {
                "000A", new StringBuilder()
            }, {
                "000B", new StringBuilder()
            }, {
                "000C", _tellStringBuilder
            }, {
                "000D", _tellStringBuilder
            }, {
                "000E", new StringBuilder()
            }, {
                "0010", _linkShellStringBuilder
            }, {
                "0011", _linkShellStringBuilder
            }, {
                "0012", _linkShellStringBuilder
            }, {
                "0013", _linkShellStringBuilder
            }, {
                "0014", _linkShellStringBuilder
            }, {
                "0015", _linkShellStringBuilder
            }, {
                "0016", _linkShellStringBuilder
            }, {
                "0017", _linkShellStringBuilder
            }, {
                "0018", new StringBuilder()
            }, {
                "001E", new StringBuilder()
            }, {
                "0025", _crossWorldLinkShellStringBuilder
            }, {
                "0026", _crossWorldLinkShellStringBuilder
            }, {
                "0027", _crossWorldLinkShellStringBuilder
            }, {
                "0028", _crossWorldLinkShellStringBuilder
            }, {
                "0029", _crossWorldLinkShellStringBuilder
            }, {
                "002A", _crossWorldLinkShellStringBuilder
            }, {
                "002B", _crossWorldLinkShellStringBuilder
            }, {
                "002C", _crossWorldLinkShellStringBuilder
            },
        };

        public static void SaveCurrentLog() {
            if (AppViewModel.Instance.ChatHistory.Any()) {
                try {
                    // clear current builders
                    foreach ((string _, StringBuilder builder) in _textLogBuilders) {
                        builder.Clear();
                    }

                    // setup full xml log file
                    XDocument xChatHistory = ResourceHelper.LoadXML($"{Constants.AppPack}Resources/ChatHistory.xml");

                    foreach ((string playerName, List<ChatLogItem> chatLogItems) in AppViewModel.Instance.ChatHistory) {
                        foreach (ChatLogItem chatLogItem in chatLogItems) {
                            // process text logging
                            try {
                                if (_textLogBuilders.ContainsKey(chatLogItem.Code)) {
                                    string prefix = $"[{playerName}]";
                                    if (Constants.ChatLS.Contains(chatLogItem.Code)) {
                                        prefix = $"{prefix}[LS{Array.IndexOf(Constants.ChatCWLS, chatLogItem.Code) + 1}] ";
                                    }

                                    if (Constants.ChatCWLS.Contains(chatLogItem.Code)) {
                                        prefix = $"{prefix}[CWLS{Array.IndexOf(Constants.ChatCWLS, chatLogItem.Code) + 1}] ";
                                    }

                                    _textLogBuilders[chatLogItem.Code].AppendLine($"{prefix} {chatLogItem.TimeStamp} {chatLogItem.Line}");
                                }
                            }
                            catch (Exception ex) {
                                Logging.Log(Logger, new LogItem(ex));
                            }

                            // process xml log
                            try {
                                string xTimeStamp = chatLogItem.TimeStamp.ToString("[HH:mm:ss]");
                                string xCode = chatLogItem.Code;
                                string xBytes = chatLogItem.Bytes.Aggregate(string.Empty, (current, bytes) => current + bytes + " ").Trim();
                                string xLine = chatLogItem.Line;

                                List<KeyValuePair<string, string>> keyPairList = new List<KeyValuePair<string, string>>();

                                keyPairList.Add(new KeyValuePair<string, string>("PlayerCharacterName", playerName));
                                keyPairList.Add(new KeyValuePair<string, string>("Bytes", xBytes));
                                keyPairList.Add(new KeyValuePair<string, string>("Line", xLine));
                                keyPairList.Add(new KeyValuePair<string, string>("TimeStamp", xTimeStamp));

                                XMLHelper.SaveXMLNode(xChatHistory, "History", "Entry", xCode, keyPairList);
                            }
                            catch (Exception ex) {
                                Logging.Log(Logger, new LogItem(ex));
                            }
                        }
                    }

                    // save text logs
                    try {
                        string textLogName = $"{DateTime.Now:yyyy_MM_dd_HH.mm.ss}.txt";

                        foreach ((string key, StringBuilder builder) in _textLogBuilders) {
                            if (Constants.ChatSay.Contains(key) && builder.Length > 0) {
                                File.WriteAllText(Path.Combine(AppViewModel.Instance.LogsPath, "Say", textLogName), builder.ToString());
                            }

                            if (Constants.ChatShout.Contains(key) && builder.Length > 0) {
                                File.WriteAllText(Path.Combine(AppViewModel.Instance.LogsPath, "Shout", textLogName), builder.ToString());
                            }

                            if (Constants.ChatParty.Contains(key) && builder.Length > 0) {
                                File.WriteAllText(Path.Combine(AppViewModel.Instance.LogsPath, "Party", textLogName), builder.ToString());
                            }

                            if (Constants.ChatTell.Contains(key) && builder.Length > 0) {
                                File.WriteAllText(Path.Combine(AppViewModel.Instance.LogsPath, "Tell", textLogName), builder.ToString());
                            }

                            if (Constants.ChatLS.Contains(key) && builder.Length > 0) {
                                File.WriteAllText(Path.Combine(AppViewModel.Instance.LogsPath, "LS", textLogName), builder.ToString());
                            }

                            if (Constants.ChatCWLS.Contains(key) && builder.Length > 0) {
                                File.WriteAllText(Path.Combine(AppViewModel.Instance.LogsPath, "CWLS", textLogName), builder.ToString());
                            }

                            if (Constants.ChatFC.Contains(key) && builder.Length > 0) {
                                File.WriteAllText(Path.Combine(AppViewModel.Instance.LogsPath, "FC", textLogName), builder.ToString());
                            }

                            if (Constants.ChatYell.Contains(key) && builder.Length > 0) {
                                File.WriteAllText(Path.Combine(AppViewModel.Instance.LogsPath, "Yell", textLogName), builder.ToString());
                            }
                        }
                    }
                    catch (Exception ex) {
                        Logging.Log(Logger, new LogItem(ex));
                    }

                    // save xml log
                    try {
                        xChatHistory.Save(Path.Combine(AppViewModel.Instance.LogsPath, $"{DateTime.Now:yyyy_MM_dd_HH.mm.ss}_ChatHistory.xml"));
                    }
                    catch (Exception ex) {
                        Logging.Log(Logger, new LogItem(ex));
                    }
                }
                catch (Exception ex) {
                    Logging.Log(Logger, new LogItem(ex));
                }
            }
        }
    }
}