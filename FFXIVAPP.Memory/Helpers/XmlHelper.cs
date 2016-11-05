// FFXIVAPP.Memory ~ XmlHelper.cs
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

using System.Linq;
using System.Text;

namespace FFXIVAPP.Memory.Helpers
{
    public static class XmlHelper
    {
        /// <summary>
        /// </summary>
        /// <param name="xValue"> </param>
        /// <returns> </returns>
        public static string SanitizeXmlString(string xValue)
        {
            if (xValue == null)
            {
                return "";
            }
            var buffer = new StringBuilder(xValue.Length);
            foreach (var xChar in xValue.Where(xChar => IsLegalXmlChar(xChar)))
            {
                buffer.Append(xChar);
            }
            return buffer.ToString();
        }

        /// <summary>
        /// </summary>
        /// <param name="xChar"> </param>
        /// <returns> </returns>
        private static bool IsLegalXmlChar(int xChar)
        {
            return (xChar == 0x9 || xChar == 0xA || xChar == 0xD || (xChar >= 0x20 && xChar <= 0xD7FF) || (xChar >= 0xE000 && xChar <= 0xFFFD) || (xChar >= 0x10000 && xChar <= 0x10FFFF));
        }
    }
}
