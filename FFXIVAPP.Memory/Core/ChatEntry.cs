// FFXIVAPP.Memory ~ ChatEntry.cs
// 
// Copyright © 2007 - 2016 Ryan Wilson - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using FFXIVAPP.Memory.Helpers;

namespace FFXIVAPP.Memory.Core
{
    public static class ChatEntry
    {
        public static ChatLogEntry Process(byte[] raw)
        {
            var chatLogEntry = new ChatLogEntry();
            try
            {
                chatLogEntry.Bytes = raw;
                chatLogEntry.TimeStamp = UnixTimeStampToDateTime(Int32.Parse(ByteArrayToString(raw.Take(4)
                                                                                                  .Reverse()
                                                                                                  .ToArray()), NumberStyles.HexNumber));
                chatLogEntry.Code = ByteArrayToString(raw.Skip(4)
                                                         .Take(2)
                                                         .Reverse()
                                                         .ToArray());
                chatLogEntry.Raw = Encoding.UTF8.GetString(raw.ToArray());
                var cleanable = raw.Skip(8)
                                   .ToArray();
                var cleaned = new ChatCleaner(cleanable).Result;
                var cut = (cleaned.Substring(1, 1) == ":") ? 2 : 1;
                chatLogEntry.Line = XmlHelper.SanitizeXmlString(cleaned.Substring(cut));
                chatLogEntry.Line = new ChatCleaner(chatLogEntry.Line).Result;
                chatLogEntry.JP = Encoding.UTF8.GetBytes(chatLogEntry.Line)
                                          .Any(b => b > 128);

                chatLogEntry.Combined = String.Format("{0}:{1}", chatLogEntry.Code, chatLogEntry.Line);
            }
            catch (Exception ex)
            {
                chatLogEntry.Bytes = new byte[0];
                chatLogEntry.Raw = "";
                chatLogEntry.Line = "";
                chatLogEntry.Code = "";
                chatLogEntry.Combined = "";
            }
            return chatLogEntry;
        }

        private static string ByteArrayToString(byte[] raw)
        {
            var hex = new StringBuilder(raw.Length * 2);
            foreach (var b in raw)
            {
                hex.AppendFormat("{0:X2}", b);
            }
            return hex.ToString();
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp)
                                   .ToLocalTime();
            return dtDateTime;
        }
    }
}
