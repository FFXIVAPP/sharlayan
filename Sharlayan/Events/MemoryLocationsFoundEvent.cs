// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryLocationsFoundEvent.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   MemoryLocationsFoundEvent.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Events {
    using System;
    using System.Collections.Generic;

    using NLog;

    public class MemoryLocationsFoundEvent : EventArgs {
        public MemoryLocationsFoundEvent(object sender, Logger logger, Dictionary<string, MemoryLocation> memoryLocations, long processingTime) {
            this.Sender = sender;
            this.Logger = logger;
            this.MemoryLocations = memoryLocations;
            this.ProcessingTime = processingTime;
        }

        public Logger Logger { get; set; }

        public Dictionary<string, MemoryLocation> MemoryLocations { get; }

        public long ProcessingTime { get; set; }

        public object Sender { get; set; }
    }
}