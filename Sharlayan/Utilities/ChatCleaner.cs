// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatCleaner.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ChatCleaner.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    using NLog;

    using Sharlayan.Extensions;

    internal class ChatCleaner {
        private const RegexOptions DefaultOptions = RegexOptions.Compiled | RegexOptions.ExplicitCapture;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Regex PlayerChatCodesRegex = new Regex(@"^00(0[A-F]|1[0-9A-F])$", DefaultOptions);

        private static readonly Regex PlayerRegEx = new Regex(@"(?<full>\[[A-Z0-9]{10}(?<first>[A-Z0-9]{3,})20(?<last>[A-Z0-9]{3,})\](?<short>[\w']+\.? [\w']+\.?)\[[A-Z0-9]{12}\])", DefaultOptions);

        private static readonly Regex ArrowRegex = new Regex(@"", RegexOptions.Compiled);

        private static readonly Regex HQRegex = new Regex(@"", RegexOptions.Compiled);

        private static readonly Regex NewLineRegex = new Regex(@"[\r\n]+", RegexOptions.Compiled);

        private static readonly Regex NoPrintingCharactersRegex = new Regex(@"[\x00-\x1F]+", RegexOptions.Compiled);

        private static readonly Regex SpecialPurposeUnicodeRegex = new Regex(@"[\uE000-\uF8FF]", RegexOptions.Compiled);

        private static readonly Regex SpecialReplacementRegex = new Regex(@"[�]", RegexOptions.Compiled);

        public static string ProcessFullLine(string code, byte[] bytes) {
            string line = HttpUtility.HtmlDecode(Encoding.UTF8.GetString(bytes.ToArray())).Replace("  ", " ");
            try {
                List<byte> newList = new List<byte>();
                for (int x = 0; x < bytes.Count(); x++) {
                    switch (bytes[x]) {
                        case 2:
                            // special in-game replacements/wrappers
                            // 2 46 5 7 242 2 210 3
                            // 2 29 1 3
                            // remove them
                            byte length = bytes[x + 2];
                            int limit = length - 1;
                            if (length > 1) {
                                x = x + 3 + limit;
                            }
                            else {
                                x = x + 4;
                                newList.Add(32);
                                newList.Add(bytes[x]);
                            }

                            break;
                        // unit separator
                        case 31:
                            // TODO: this breaks in some areas like NOVICE chat
                            // if (PlayerChatCodesRegex.IsMatch(code)) {
                            //     newList.Add(58);
                            // }
                            // else {
                            //     newList.Add(31);
                            // }
                            newList.Add(58);
                            if (PlayerChatCodesRegex.IsMatch(code)) {
                                newList.Add(32);
                            }

                            break;
                        default:
                            newList.Add(bytes[x]);
                            break;
                    }
                }

                string cleaned = HttpUtility.HtmlDecode(Encoding.UTF8.GetString(newList.ToArray())).Replace("  ", " ");

                newList.Clear();

                // replace right arrow in chat (parsing)
                cleaned = ArrowRegex.Replace(cleaned, "⇒");
                // replace HQ symbol
                cleaned = HQRegex.Replace(cleaned, "[HQ]");
                // replace all Extended special purpose unicode with empty string
                cleaned = SpecialPurposeUnicodeRegex.Replace(cleaned, string.Empty);
                // cleanup special replacement character bytes: 239 191 189
                cleaned = SpecialReplacementRegex.Replace(cleaned, string.Empty);
                // remove new lines
                cleaned = NewLineRegex.Replace(cleaned, string.Empty);
                // remove characters 0-31
                cleaned = NoPrintingCharactersRegex.Replace(cleaned, string.Empty);

                line = cleaned;
            }
            catch (Exception ex) {
                // TODO: figure out how to raise exception
            }

            return ProcessName(line);
        }

        private static string ProcessName(string cleaned) {
            string line = cleaned;
            try {
                // cleanup name if using other settings
                Match playerMatch = PlayerRegEx.Match(line);
                if (playerMatch.Success) {
                    string fullName = playerMatch.Groups[1].Value;
                    string firstName = playerMatch.Groups[2].Value.FromHex();
                    string lastName = playerMatch.Groups[3].Value.FromHex();
                    string player = $"{firstName} {lastName}";

                    // remove double placement
                    cleaned = line.Replace($"{fullName}:{fullName}", "•name•");

                    // remove single placement
                    cleaned = cleaned.Replace(fullName, "•name•");
                    switch (Regex.IsMatch(cleaned, @"^([Vv]ous|[Dd]u|[Yy]ou)")) {
                        case true:
                            cleaned = cleaned.Substring(1).Replace("•name•", string.Empty);
                            break;
                        case false:
                            cleaned = cleaned.Replace("•name•", player);
                            break;
                    }
                }

                cleaned = Regex.Replace(cleaned, @"[\r\n]+", string.Empty);
                cleaned = Regex.Replace(cleaned, @"[\x00-\x1F]+", string.Empty);
                line = cleaned;
            }
            catch (Exception ex) {
                // TODO: figure out how to raise exception
            }

            return line;
        }
    }
}