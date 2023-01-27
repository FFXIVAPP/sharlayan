// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XMLCleaner.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   XMLCleaner.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System.Text;

    public static class XMLCleaner {
        public static string SanitizeXmlString(string xValue) {
            if (xValue == null) {
                return string.Empty;
            }

            StringBuilder buffer = new StringBuilder(xValue.Length);
            foreach (char c in xValue) {
                if (IsLegalXmlChar(c)) {
                    buffer.Append(c);
                }
            }

            return buffer.ToString();
        }

        private static bool IsLegalXmlChar(int xChar) {
            return xChar == 0x9 || xChar == 0xA || xChar == 0xD || xChar >= 0x20 && xChar <= 0xD7FF || xChar >= 0xE000 && xChar <= 0xFFFD || xChar >= 0x10000 && xChar <= 0x10FFFF;
        }
    }
}