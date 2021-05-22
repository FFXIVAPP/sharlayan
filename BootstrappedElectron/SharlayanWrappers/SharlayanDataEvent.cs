// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharlayanDataEvent.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   SharlayanDataEvent.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedElectron.SharlayanWrappers {
    using System;

    using Sharlayan;

    public class SharlayanDataEvent<T> : EventArgs {
        public SharlayanDataEvent(object sender, MemoryHandler memoryHandler, T eventData) {
            this.Sender = sender;
            this.MemoryHandler = memoryHandler;
            this.EventData = eventData;
        }

        public T EventData { get; }

        public MemoryHandler MemoryHandler { get; }

        public object Sender { get; }
    }
}