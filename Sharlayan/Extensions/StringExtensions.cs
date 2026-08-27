// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
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

    public static class StringExtensions {
        // F94: keys come from chat-derived strings (other players' names), which are
        // externally controlled — cap the cache so a long busy session can't grow it
        // without bound.
        private const int FromHexLookupMaxEntries = 4096;

        private static ConcurrentDictionary<string, string> _fromHexLookup = new ConcurrentDictionary<string, string>();

        public static string FromHex(this string source) {
            if (string.IsNullOrEmpty(source)) {
                return string.Empty;
            }

            if (_fromHexLookup.TryGetValue(source, out string existing)) {
                return existing;
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i <= source.Length - 2; i += 2) {
                // F94: callers feed regex captures that allow any [A-Z0-9], not just
                // hex digits (e.g. a name segment containing 'G') — fail closed to
                // empty instead of throwing FormatException out of the extension.
                if (!int.TryParse(source.AsSpan(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int value)) {
                    return string.Empty;
                }

                builder.Append(Convert.ToChar(value));
            }

            string result = builder.ToString();

            if (_fromHexLookup.Count >= FromHexLookupMaxEntries) {
                _fromHexLookup.Clear();
            }

            _fromHexLookup.AddOrUpdate(source, result, (k, v) => result);

            return result;
        }
    }
}