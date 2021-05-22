// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   LogItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedElectron.Models {
    using System;

    using NLog;

    public class LogItem {
        public LogItem(string message) {
            this.Message = string.IsNullOrWhiteSpace(message)
                               ? "LogItem: Called Without Message"
                               : message;
        }

        public LogItem(Exception exception, bool levelIsError = false) {
            this.Message = exception?.Message ?? "LogItem: Called Without Exception";
            this.Exception = exception;
            if (levelIsError) {
                this.LogLevel = LogLevel.Error;
            }
        }

        public LogItem(string message, Exception exception, bool levelIsError = false) {
            this.Message = string.IsNullOrWhiteSpace(message)
                               ? exception?.Message ?? "LogItem: Called Without Message"
                               : message;
            this.Exception = exception;
            if (this.Exception != null && levelIsError) {
                this.LogLevel = LogLevel.Error;
            }
        }

        public Exception Exception { get; set; }

        public LogLevel LogLevel { get; set; } = LogLevel.Trace;

        public string Message { get; set; }
    }
}