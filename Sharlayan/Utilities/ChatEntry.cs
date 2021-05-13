// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatEntry.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ChatEntry.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using Sharlayan.Core;

    public class ChatEntry {
        public static ChatLogItem Process(byte[] raw) {
            ChatLogItem chatLogEntry = new ChatLogItem();
            try {
                chatLogEntry.Bytes = raw;
                chatLogEntry.TimeStamp = UnixTimeStampToDateTime(int.Parse(ByteArrayToString(raw.Take(4).Reverse().ToArray()), NumberStyles.HexNumber));
                chatLogEntry.Code = ByteArrayToString(raw.Skip(4).Take(2).Reverse().ToArray());
                chatLogEntry.Raw = Encoding.UTF8.GetString(raw.ToArray());
                byte[] cleanable = raw.Skip(8).ToArray();
                string cleaned = ChatCleaner.ProcessFullLine(chatLogEntry.Code, cleanable);
                int cut = cleaned.Substring(1, 1) == ":"
                              ? 2
                              : 1;
                chatLogEntry.Line = XMLCleaner.SanitizeXmlString(cleaned.Substring(cut));
                chatLogEntry.JP = IsJapanese(chatLogEntry.Line);

                chatLogEntry.Combined = $"{chatLogEntry.Code}:{chatLogEntry.Line}";
            }
            catch (Exception) {
                chatLogEntry.Bytes = Array.Empty<byte>();
                chatLogEntry.Raw = string.Empty;
                chatLogEntry.Line = string.Empty;
                chatLogEntry.Code = string.Empty;
                chatLogEntry.Combined = string.Empty;
            }

            return chatLogEntry;
        }

        private static string ByteArrayToString(byte[] raw) {
            StringBuilder hex = new StringBuilder(raw.Length * 2);
            foreach (byte b in raw) {
                hex.AppendFormat($"{b:X2}");
            }

            return hex.ToString();
        }

        private static bool IsJapanese(string line) {
            // 0x3040 -> 0x309F === Hirigana
            // 0x30A0 -> 0x30FF === Katakana
            // 0x4E00 -> 0x9FBF === Kanji
            return line.Any(c => c >= 0x3040 && c <= 0x309F) || line.Any(c => c >= 0x30A0 && c <= 0x30FF) || line.Any(c => c >= 0x4E00 && c <= 0x9FBF);
        }

        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            time = time.AddSeconds(unixTimeStamp).ToLocalTime();
            return time;
        }
    }
}