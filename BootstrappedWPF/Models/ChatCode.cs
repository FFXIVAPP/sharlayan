// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatCode.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ChatCode.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.Models {
    public class ChatCode {
        public ChatCode(string code, string color, string description) {
            this.Code = code;
            this.Color = color;
            this.Description = description;
        }

        public string Code { get; }
        public string Color { get; set; }
        public string Description { get; set; }
    }
}