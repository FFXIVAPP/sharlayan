// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Localization.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Localization.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models {
    using System;

    public class Localization {
        public string Chinese { get; set; }

        public string English { get; set; }

        public string French { get; set; }

        public string German { get; set; }

        public string Japanese { get; set; }

        public string Korean { get; set; }

        public bool Matches(string name) {
            return string.Equals(this.English, name, StringComparison.InvariantCultureIgnoreCase) || string.Equals(this.French, name, StringComparison.InvariantCultureIgnoreCase) || string.Equals(this.Japanese, name, StringComparison.InvariantCultureIgnoreCase) || string.Equals(this.German, name, StringComparison.InvariantCultureIgnoreCase) || string.Equals(this.Chinese, name, StringComparison.InvariantCultureIgnoreCase) || string.Equals(this.Korean, name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}