// FFXIVAPP.Memory
// StringHelper.cs
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
