// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewChatLogItemEvent.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   NewChatLogItemEvent.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.SharlayanWrappers.Events {
    using Sharlayan;
    using Sharlayan.Core;

    public class NewChatLogItemEvent : SharlayanDataEvent<ChatLogItem> {
        public NewChatLogItemEvent(object sender, MemoryHandler memoryHandler, ChatLogItem eventData) : base(sender, memoryHandler, eventData) { }
    }
}