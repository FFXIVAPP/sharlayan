// --------------------------------------------------------------------------------------------------------------------
// <copyright file="German.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   German.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.Localization {
    using System.Windows;

    public class German {
        private static readonly ResourceDictionary _translations = new ResourceDictionary();

        public static ResourceDictionary Translations() {
            _translations.Clear();

            _translations.Add("home", "Home");
            _translations.Add("HomeTabItem_WelcomeText", "Welcome to Sharlayan's WPF Example");

            return _translations;
        }
    }
}