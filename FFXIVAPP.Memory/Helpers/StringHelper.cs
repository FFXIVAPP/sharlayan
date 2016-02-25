// FFXIVAPP.Memory ~ StringHelper.cs
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
using System.Text;
using System.Text.RegularExpressions;

namespace FFXIVAPP.Memory.Helpers
{
    public static class StringHelper
    {
        private const RegexOptions DefaultOptions = RegexOptions.Compiled | RegexOptions.ExplicitCapture;
        private static readonly Regex Romans = new Regex(@"(?<roman>\b[IVXLCDM]+\b)", DefaultOptions);
        private static readonly Regex Titles = new Regex(@"(?<num>\d+)(?<designator>\w+)", DefaultOptions | RegexOptions.IgnoreCase);
        private static readonly Regex CleanSpaces = new Regex(@"[ ]+", RegexOptions.Compiled);

        /// <summary>
        /// </summary>
        /// <param name="s"> </param>
        /// <param name="all"> </param>
        /// <returns> </returns>
        public static string TitleCase(string s, bool all = true)
        {
            if (String.IsNullOrWhiteSpace(s.Trim()))
            {
                return "";
            }
            s = TrimAndCleanSpaces(s);
            var result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(all ? s.ToLower() : s);
            var reg = Romans.Match(s);
            if (reg.Success)
            {
                var replace = Convert.ToString(reg.Groups["roman"].Value);
                var original = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(replace.ToLower());
                result = result.Replace(original, replace.ToUpper());
            }
            var titles = Titles.Matches(result);
            foreach (Match title in titles)
            {
                var num = Convert.ToString(title.Groups["num"].Value);
                var designator = Convert.ToString(title.Groups["designator"].Value);
                result = result.Replace(String.Format("{0}{1}", num, designator), String.Format("{0}{1}", num, designator.ToLower()));
            }
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string TrimAndCleanSpaces(string name)
        {
            return CleanSpaces.Replace(name, " ")
                              .Trim();
        }

        /// <summary>
        /// </summary>
        /// <param name="hexValue"></param>
        /// <returns></returns>
        public static string HexToString(string hexValue)
        {
            var sb = new StringBuilder();
            for (var i = 0; i <= hexValue.Length - 2; i += 2)
            {
                sb.Append(Convert.ToChar(Int32.Parse(hexValue.Substring(i, 2), NumberStyles.HexNumber)));
            }
            return sb.ToString();
        }
    }
}
