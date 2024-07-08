// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatLogResult.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ChatLogResult.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System.Collections.Concurrent;

    using Sharlayan.Core;

    public class ChatLogResult {
        public ConcurrentQueue<ChatLogItem> ChatLogItems { get; internal set; } = new ConcurrentQueue<ChatLogItem>();

        public int PreviousArrayIndex { get; internal set; }

        public int PreviousOffset { get; internal set; }
    }
}