// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatLogItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ChatLogItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core {
    using System;

    using Sharlayan.Core.Interfaces;

    public class ChatLogItem : IChatLogItem, ICloneable {
        /// <summary>
        /// character name of player that was loaded when log was detected
        /// </summary>
        public string PlayerCharacterName { get; set; }

        /// <summary>
        /// raw bytes from memory
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// alphanumeric chat code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// [Code]:[Line] combination string
        /// </summary>
        public string Combined { get; set; }

        /// <summary>
        /// does the text contain non-roman text
        /// </summary>
        public bool IsInternational { get; set; }

        /// <summary>
        /// "(playerName )?messsage" combination string
        /// </summary>
        public string Line { get; set; }

        /// <summary>
        /// message without name of player
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// player name from message
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// raw bytes decoded to string
        /// </summary>
        public string Raw { get; set; }

        /// <summary>
        /// message timestamp
        /// </summary>
        public DateTime TimeStamp { get; set; }

        public object Clone() {
            byte[] bytes = new byte[this.Bytes.Length];
            Buffer.BlockCopy(this.Bytes, 0, bytes, 0, this.Bytes.Length);
            return new ChatLogItem {
                PlayerCharacterName = this.PlayerCharacterName,
                Bytes = bytes,
                Code = this.Code,
                Combined = this.Combined,
                IsInternational = this.IsInternational,
                Line = this.Line,
                Message = this.Message,
                PlayerName = this.PlayerName,
                Raw = this.Raw,
                TimeStamp = this.TimeStamp,
            };
        }
    }
}