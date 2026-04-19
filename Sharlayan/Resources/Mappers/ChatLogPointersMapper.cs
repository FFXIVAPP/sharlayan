// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatLogPointersMapper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Maps Sharlayan's ChatLogPointers to offsets inside Component::Log::LogModule's two
//   StdVector fields: LogMessageIndex (the int-offset table) and LogMessageData (the
//   message byte buffer). Each StdVector has the layout (First*, Last*, End*) at 0/8/16
//   on x64 — so Start/Pos(=Last)/End bracket the in-use range.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Component.Log;

    using Sharlayan.Models.Structures;

    internal static class ChatLogPointersMapper {
        // StdVector<T> layout is (T* First, T* Last, T* End), sequential, x64 — 8 bytes per pointer.
        private const int StdVectorFirstOffset = 0;
        private const int StdVectorLastOffset = 8;
        private const int StdVectorEndOffset = 16;

        public static ChatLogPointers Build() {
            int indexVectorOffset = (int)Marshal.OffsetOf<LogModule>(nameof(LogModule.LogMessageIndex));
            int dataVectorOffset = (int)Marshal.OffsetOf<LogModule>(nameof(LogModule.LogMessageData));

            return new ChatLogPointers {
                OffsetArrayStart = indexVectorOffset + StdVectorFirstOffset,
                OffsetArrayPos = indexVectorOffset + StdVectorLastOffset,
                OffsetArrayEnd = indexVectorOffset + StdVectorEndOffset,

                LogStart = dataVectorOffset + StdVectorFirstOffset,
                LogNext = dataVectorOffset + StdVectorLastOffset,
                LogEnd = dataVectorOffset + StdVectorEndOffset,
            };
        }
    }
}
