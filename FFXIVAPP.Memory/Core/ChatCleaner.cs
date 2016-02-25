// FFXIVAPP.Memory ~ ChatCleaner.cs
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using FFXIVAPP.Memory.Helpers;

namespace FFXIVAPP.Memory.Core
{
    internal class ChatCleaner : INotifyPropertyChanged
    {
        private const RegexOptions DefaultOptions = RegexOptions.Compiled | RegexOptions.ExplicitCapture;

        #region Declarations

        private static readonly Regex Checks = new Regex(@"^00(20|21|23|27|28|46|47|48|49|5C)$", DefaultOptions);

        #endregion

        private Regex PlayerRegEx = new Regex(@"(?<full>\[[A-Z0-9]{10}(?<first>[A-Z0-9]{3,})20(?<last>[A-Z0-9]{3,})\](?<short>[\w']+\.? [\w']+\.?)\[[A-Z0-9]{12}\])", DefaultOptions);

        /// <summary>
        /// </summary>
        /// <param name="line"></param>
        public ChatCleaner(string line)
        {
            Result = ProcessName(line);
        }

        /// <summary>
        /// </summary>
        /// <param name="bytes"></param>
        public ChatCleaner(byte[] bytes)
        {
            Result = ProcessFullLine(bytes)
                .Trim();
        }

        /// <summary>
        /// </summary>
        /// <param name="bytes"> </param>
        /// <returns> </returns>
        private string ProcessFullLine(byte[] bytes)
        {
            var line = HttpUtility.HtmlDecode(Encoding.UTF8.GetString(bytes.ToArray()))
                                  .Replace("  ", " ");
            try
            {
                var autoTranslateList = new List<byte>();
                var newList = new List<byte>();
                for (var x = 0; x < bytes.Count(); x++)
                {
                    if (bytes[x] == 2)
                    {
                        var byteString = String.Format("{0}{1}{2}{3}", bytes[x], bytes[x + 1], bytes[x + 2], bytes[x + 3]);
                        switch (byteString)
                        {
                            case "22913":
                            case "21613":
                            case "22213":
                                x += 4;
                                break;
                        }
                    }
                    switch (bytes[x])
                    {
                        case 2:
                            //2 46 5 7 242 2 210 3
                            //2 29 1 3
                            var length = bytes[x + 2];
                            var limit = length - 1;
                            if (length > 1)
                            {
                                x = x + 3;
                                autoTranslateList.Add(Convert.ToByte('['));
                                var translated = new byte[limit];
                                Buffer.BlockCopy(bytes, x, translated, 0, limit);
                                foreach (var b in translated)
                                {
                                    autoTranslateList.AddRange(Encoding.UTF8.GetBytes(b.ToString("X2")));
                                }
                                autoTranslateList.Add(Convert.ToByte(']'));
                                var aCheckStr = "";
                                var checkedAt = autoTranslateList.GetRange(1, autoTranslateList.Count - 1)
                                                                 .ToArray();
                                if (String.IsNullOrWhiteSpace(aCheckStr))
                                {
                                    // TODO: implement showing or using in the chatlog
                                }
                                else
                                {
                                    newList.AddRange(Encoding.UTF8.GetBytes(aCheckStr));
                                }
                                autoTranslateList.Clear();
                                x += limit;
                            }
                            else
                            {
                                x = x + 4;
                                newList.Add(32);
                                newList.Add(bytes[x]);
                            }
                            break;
                        default:
                            newList.Add(bytes[x]);
                            break;
                    }
                }
                //var cleanedList = newList.Where(v => (v >= 0x0020 && v <= 0xD7FF) || (v >= 0xE000 && v <= 0xFFFD) || v == 0x0009 || v == 0x000A || v == 0x000D);
                var cleaned = HttpUtility.HtmlDecode(Encoding.UTF8.GetString(newList.ToArray()))
                                         .Replace("  ", " ");
                autoTranslateList.Clear();
                newList.Clear();
                cleaned = Regex.Replace(cleaned, @"", "⇒");
                cleaned = Regex.Replace(cleaned, @"", "[HQ]");
                cleaned = Regex.Replace(cleaned, @"", "");
                cleaned = Regex.Replace(cleaned, @"�", "");
                cleaned = Regex.Replace(cleaned, @"\[+0([12])010101([\w]+)?\]+", "");
                cleaned = Regex.Replace(cleaned, @"\[+CF010101([\w]+)?\]+", "");
                cleaned = Regex.Replace(cleaned, @"\[+..FF\w{6}\]+|\[+EC\]+", "");
                cleaned = Regex.Replace(cleaned, @"\[\]+", "");
                line = cleaned;
            }
            catch (Exception ex)
            {
            }
            return line;
        }

        /// <summary>
        /// </summary>
        /// <param name="cleaned"></param>
        /// <returns></returns>
        private string ProcessName(string cleaned)
        {
            var line = cleaned;
            try
            {
                // cleanup name if using other settings
                var playerMatch = PlayerRegEx.Match(line);
                if (playerMatch.Success)
                {
                    var fullName = playerMatch.Groups[1].Value;
                    var firstName = StringHelper.HexToString(playerMatch.Groups[2].Value);
                    var lastName = StringHelper.HexToString(playerMatch.Groups[3].Value);
                    var player = String.Format("{0} {1}", firstName, lastName);
                    // remove double placement
                    cleaned = line.Replace(String.Format("{0}:{1}", fullName, fullName), "•name•");
                    // remove single placement
                    cleaned = cleaned.Replace(fullName, "•name•");
                    switch (Regex.IsMatch(cleaned, @"^([Vv]ous|[Dd]u|[Yy]ou)"))
                    {
                        case true:
                            cleaned = cleaned.Substring(1)
                                             .Replace("•name•", "");
                            break;
                        case false:
                            cleaned = cleaned.Replace("•name•", player);
                            break;
                    }
                }
                cleaned = Regex.Replace(cleaned, @"[\r\n]+", "");
                cleaned = Regex.Replace(cleaned, @"[\x00-\x1F]+", "");
                line = cleaned;
            }
            catch (Exception ex)
            {
            }
            return line;
        }

        #region Property Bindings

        private static bool _colorFound;
        private string _result;

        private bool ColorFound
        {
            get { return _colorFound; }
            set
            {
                _colorFound = value;
                RaisePropertyChanged();
            }
        }

        public string Result
        {
            get { return _result; }
            private set
            {
                _result = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}
