// FFXIVAPP.Memory
// ChatEntry.cs
// 
// Copyright © 2007 - 2015 Ryan Wilson - All Rights Reserved
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions are met: 
// 
//  * Redistributions of source code must retain the above copyright notice, 
//    this list of conditions and the following disclaimer. 
//  * Redistributions in binary form must reproduce the above copyright 
//    notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution. 
//  * Neither the name of SyndicatedLife nor the names of its contributors may 
//    be used to endorse or promote products derived from this software 
//    without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE. 

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
