// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logging.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Logging.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.Utilities {
    using System;

    using BootstrappedWPF.Controls;
    using BootstrappedWPF.Helpers;
    using BootstrappedWPF.Models;

    using NLog;

    public static class Logging {
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

            FlowDocHelper.AppendMessage(logItem.Message, DebugTabItem.TabItem.DebugLogReader._FDR);
        }
    }
}