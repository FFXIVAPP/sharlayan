// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewPlayerInfoEvent.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   NewPlayerInfoEvent.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.SharlayanWrappers.Events {
    using Sharlayan;
    using Sharlayan.Core;

    public class NewPlayerInfoEvent : SharlayanDataEvent<PlayerInfo> {
        public NewPlayerInfoEvent(object sender, MemoryHandler memoryHandler, PlayerInfo eventData) : base(sender, memoryHandler, eventData) { }
    }
}