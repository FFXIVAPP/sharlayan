// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatLogPointers.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ChatLogPointers.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models {
    internal class ChatLogPointers {
        public uint LineCount { get; set; }

        public long LogEnd { get; set; }

        public long LogNext { get; set; }

        public long LogStart { get; set; }

        public long OffsetArrayEnd { get; set; }

        public long OffsetArrayPos { get; set; }

        public long OffsetArrayStart { get; set; }
    }
}