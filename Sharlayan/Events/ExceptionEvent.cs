﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionEvent.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ExceptionEvent.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Events {
    using System;

    using NLog;

    public class ExceptionEvent : EventArgs {
        public ExceptionEvent(object sender, Logger logger, Exception exception, bool levelIsError = false) {
            this.Sender = sender;
            this.Logger = logger;
            this.Exception = exception;
            this.LevelIsError = levelIsError;
        }

        public Exception Exception { get; set; }

        public bool LevelIsError { get; set; }

        public Logger Logger { get; set; }

        public object Sender { get; set; }
    }
}