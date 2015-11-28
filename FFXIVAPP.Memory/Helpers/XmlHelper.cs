// FFXIVAPP.Memory
// XmlHelper.cs
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
