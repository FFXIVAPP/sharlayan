// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizationHelper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Pure helper that picks the right field off a <see cref="Sharlayan.Models.Localization"/>
//   record given a <see cref="Sharlayan.Enums.GameLanguage"/>. Extracted from the duplicated
//   switch blocks in ActorItemResolver / PartyMemberResolver so the language-selection logic
//   can be unit tested without spinning up a MemoryHandler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using Sharlayan.Enums;
    using Sharlayan.Models;

    internal static class LocalizationHelper {
        /// <summary>
        /// Returns the localised string for <paramref name="language"/>. Falls back to
        /// English when the requested language slot is null or when the language enum is
        /// <see cref="GameLanguage.English"/> (the default). Returns null only if the
        /// <paramref name="names"/> argument itself is null.
        /// </summary>
        public static string SelectLocalized(Localization names, GameLanguage language) {
            if (names == null) {
                return null;
            }
            switch (language) {
                case GameLanguage.French:   return names.French   ?? names.English;
                case GameLanguage.Japanese: return names.Japanese ?? names.English;
                case GameLanguage.German:   return names.German   ?? names.English;
                case GameLanguage.Chinese:  return names.Chinese  ?? names.English;
                case GameLanguage.Korean:   return names.Korean   ?? names.English;
                default:                    return names.English;
            }
        }
    }
}
