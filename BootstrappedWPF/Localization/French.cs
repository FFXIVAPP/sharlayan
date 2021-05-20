// --------------------------------------------------------------------------------------------------------------------
// <copyright file="French.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   French.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.Localization {
    using System.Windows;

    public class French {
        private static readonly ResourceDictionary _translations = new ResourceDictionary();

        public static ResourceDictionary Translations() {
            _translations.Clear();
            _translations.Add("home", "Home");

            return _translations;
        }
    }
}