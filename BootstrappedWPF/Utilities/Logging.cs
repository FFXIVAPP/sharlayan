namespace BootstrappedWPF.Utilities {
    using System;

    using BootstrappedWPF.Controls;
    using BootstrappedWPF.Helpers;
    using BootstrappedWPF.Models;

    using NLog;

    public static class Logging {
        public static void Debug(Logger logger, string message, Exception exception = null, bool levelIsError = false) {
            Debug(logger, new LogItem(message, exception, levelIsError));
        }

        public static void Debug(Logger logger, LogItem logItem) {
            Log(logger, logItem);

            // handle pre rendered cases in App.xaml.cs
            if (DebugTabItem.Instance is not null) {
                FlowDocHelper.AppendMessage(logItem.Message, DebugTabItem.Instance.DebugLogReader._FDR);
            }
        }

        public static void Log(Logger logger, string message, Exception exception = null, bool levelIsError = false) {
            Log(logger, new LogItem(message, exception, levelIsError));
        }

        public static void Log(Logger logger, LogItem logItem) {
            if (logItem.Exception == null) {
                logger.Log(logItem.LogLevel, logItem.Message);
            }
            else {
                logger.Log(logItem.LogLevel, logItem.Exception, logItem.Message);
            }
        }
    }
}