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

    public static class StringExtensions {
        private static ConcurrentDictionary<string, string> _fromHexLookup = new ConcurrentDictionary<string, string>();

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
    }
}