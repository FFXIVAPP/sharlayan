// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatLogPointers.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ChatLogPointers.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public class ChatLogPointers {
        public int LogEnd { get; set; }

        public int LogNext { get; set; }

        public int LogStart { get; set; }

        public int OffsetArrayEnd { get; set; }

        public int OffsetArrayPos { get; set; }

        public int OffsetArrayStart { get; set; }
    }
}