namespace BootstrappedWPF.Helpers {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    using NLog;

    using Sharlayan.Core;

    using BootstrappedWPF.Models;
    using BootstrappedWPF.Utilities;
    using BootstrappedWPF.ViewModels;

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
                    foreach (KeyValuePair<string, StringBuilder> textLogBuilder in _textLogBuilders) {
                        textLogBuilder.Value.Clear();
                    }

                    // setup full chatlog xml file
                    XDocument xChatHistory = ResourceHelper.LoadXML($"{Constants.AppPack}Resources/ChatHistory.xml");

                    foreach (ChatLogItem chatLogItem in AppViewModel.Instance.ChatHistory) {
                        // process text logging
                        try {
                            switch (chatLogItem.Code) {
                                case "000A":
                                case "000B":
                                case "000E":
                                case "000C":
                                case "000D":
                                case "0010":
                                case "0011":
                                case "0012":
                                case "0013":
                                case "0014":
                                case "0015":
                                case "0016":
                                case "0017":
                                case "0018":
                                case "001E":
                                case "0025":
                                case "0026":
                                case "0027":
                                case "0028":
                                case "0029":
                                case "002A":
                                case "002B":
                                case "002C":
                                    string prefix = string.Empty;
                                    if (Constants.ChatLS.Contains(chatLogItem.Code)) {
                                        prefix = $"[LS{Array.IndexOf(Constants.ChatCWLS, chatLogItem.Code) + 1}] ";
                                    }

                                    if (Constants.ChatCWLS.Contains(chatLogItem.Code)) {
                                        prefix = $"[CWLS{Array.IndexOf(Constants.ChatCWLS, chatLogItem.Code) + 1}] ";
                                    }

                                    _textLogBuilders[chatLogItem.Code].AppendLine($"{chatLogItem.TimeStamp} {prefix}{chatLogItem.Line}");
                                    break;
                            }
                        }
                        catch (Exception ex) {
                            Logging.Log(Logger, new LogItem(ex, true));
                        }

                        // process xml log
                        try {
                            string xTimeStamp = chatLogItem.TimeStamp.ToString("[HH:mm:ss]");
                            string xCode = chatLogItem.Code;
                            string xBytes = chatLogItem.Bytes.Aggregate(string.Empty, (current, bytes) => current + bytes + " ").Trim();
                            string xLine = chatLogItem.Line;

                            List<KeyValuePair<string, string>> keyPairList = new List<KeyValuePair<string, string>>();

                            keyPairList.Add(new KeyValuePair<string, string>("Bytes", xBytes));
                            keyPairList.Add(new KeyValuePair<string, string>("Line", xLine));
                            keyPairList.Add(new KeyValuePair<string, string>("TimeStamp", xTimeStamp));

                            XMLHelper.SaveXMLNode(xChatHistory, "History", "Entry", xCode, keyPairList);
                        }
                        catch (Exception ex) {
                            Logging.Log(Logger, new LogItem(ex, true));
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
                        Logging.Log(Logger, new LogItem(ex, true));
                    }

                    // save xml log
                    try {
                        xChatHistory.Save(Path.Combine(AppViewModel.Instance.LogsPath, $"{DateTime.Now:yyyy_MM_dd_HH.mm.ss}_ChatHistory.xml"));
                    }
                    catch (Exception ex) {
                        Logging.Log(Logger, new LogItem(ex, true));
                    }
                }
                catch (Exception ex) {
                    Logging.Log(Logger, new LogItem(ex, true));
                }
            }
        }
    }
}
