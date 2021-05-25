// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   StringExtensions.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Extensions {
    using System;
    using System.Collections.Concurrent;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class StringExtensions {
        private const RegexOptions DefaultOptions = RegexOptions.Compiled | RegexOptions.ExplicitCapture;

        private static readonly Regex Romans = new Regex(@"(?<roman>\b[IVXLCDM]+\b)", DefaultOptions);

        private static readonly Regex Titles = new Regex(@"(?<num>\d+)(?<designator>\w+)", DefaultOptions | RegexOptions.IgnoreCase);

        private static ConcurrentDictionary<string, string> _fromHexLookup = new ConcurrentDictionary<string, string>();

        private static ConcurrentDictionary<string, string> _titleCaseLookup = new ConcurrentDictionary<string, string>();

        public static string FromHex(this string source) {
            if (_fromHexLookup.TryGetValue(source, out string existing)) {
                return existing;
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i <= source.Length - 2; i += 2) {
                builder.Append(Convert.ToChar(int.Parse(source.Substring(i, 2), NumberStyles.HexNumber)));
            }

            string result = builder.ToString();

            _fromHexLookup.AddOrUpdate(source, result, (k, v) => result);

            return result;
        }

        public static string ToTitleCase(this string source, bool all = true) {
            if (string.IsNullOrWhiteSpace(source)) {
                return string.Empty;
            }

            if (_titleCaseLookup.TryGetValue(source, out string existing)) {
                return existing;
            }

            string result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                all
                    ? source.ToLower()
                    : source);
            Match reg = Romans.Match(source);
            if (reg.Success) {
                string replace = Convert.ToString(reg.Groups["roman"].Value);
                string original = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(replace.ToLower());
                result = result.Replace(original, replace.ToUpper());
            }

            MatchCollection titles = Titles.Matches(result);
            foreach (Match title in titles) {
                string num = Convert.ToString(title.Groups["num"].Value);
                string designator = Convert.ToString(title.Groups["designator"].Value);
                result = result.Replace($"{num}{designator}", $"{num}{designator.ToLower()}");
            }

            _titleCaseLookup.AddOrUpdate(source, result, (k, v) => result);

            return result;
        }
    }
}