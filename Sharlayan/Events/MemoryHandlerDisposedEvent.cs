// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryHandlerDisposedEvent.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   MemoryHandlerDisposedEvent.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Events {
    using System;

    using NLog;

    public class MemoryHandlerDisposedEvent : EventArgs {
        public MemoryHandlerDisposedEvent(object sender, Logger logger) {
            this.Sender = sender;
            this.Logger = logger;
        }

        public Logger Logger { get; set; }

        public object Sender { get; set; }
    }
}