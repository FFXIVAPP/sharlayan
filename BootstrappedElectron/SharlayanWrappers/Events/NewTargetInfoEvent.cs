// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewTargetInfoEvent.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   NewTargetInfoEvent.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedElectron.SharlayanWrappers.Events {
    using Sharlayan;
    using Sharlayan.Core;

    public class NewTargetInfoEvent : SharlayanDataEvent<TargetInfo> {
        public NewTargetInfoEvent(object sender, MemoryHandler memoryHandler, TargetInfo eventData) : base(sender, memoryHandler, eventData) { }
    }
}