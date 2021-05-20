// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatColor.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ChatColor.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.Models {
    public class ChatColor {
        public ChatColor(string code, string color, string description) {
            this.Code = code;
            this.Color = color;
            this.Description = description;
        }

        public string Code { get; }
        public string Color { get; set; }
        public string Description { get; private set; }

        public void SetDescription(string description) {
            this.Description = description;
        }
    }
}