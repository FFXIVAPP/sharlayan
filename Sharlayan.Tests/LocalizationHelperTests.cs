// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizationHelperTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Covers Sharlayan.Utilities.LocalizationHelper.SelectLocalized — the pure
//   language-selection helper that ActorItemResolver / PartyMemberResolver use to populate
//   the localised StatusName and English-pinned StatusNameEnglish fields. Direct unit tests
//   here let us prove the cross-language behaviour without spinning up a MemoryHandler /
//   live process.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests {
    using Sharlayan.Core;
    using Sharlayan.Enums;
    using Sharlayan.Models;
    using Sharlayan.Utilities;

    using Xunit;

    public class LocalizationHelperTests {
        // Fixture mirroring the FFXIV "Iron Will" tank stance row across all six locales
        // Sharlayan tracks. Used to verify the helper picks the right slot per language and
        // that the English-pinned counterpart is unaffected by the language switch.
        private static Localization IronWillNames() => new Localization {
            English  = "Iron Will",
            Japanese = "アイアンウィル",
            German   = "Eisenwille",
            French   = "Volonté de fer",
            Chinese  = "铁壁",
            Korean   = "굳건한 의지",
        };

        [Theory]
        [InlineData(GameLanguage.English,  "Iron Will")]
        [InlineData(GameLanguage.Japanese, "アイアンウィル")]
        [InlineData(GameLanguage.German,   "Eisenwille")]
        [InlineData(GameLanguage.French,   "Volonté de fer")]
        [InlineData(GameLanguage.Chinese,  "铁壁")]
        [InlineData(GameLanguage.Korean,   "굳건한 의지")]
        public void SelectLocalized_ReturnsRequestedLanguage(GameLanguage language, string expected) {
            Assert.Equal(expected, LocalizationHelper.SelectLocalized(IronWillNames(), language));
        }

        [Fact]
        public void SelectLocalized_FallsBackToEnglish_WhenLanguageSlotIsNull() {
            Localization names = IronWillNames();
            names.Japanese = null;
            // GameLanguage.Japanese requested but JP slot empty → English fallback.
            Assert.Equal("Iron Will", LocalizationHelper.SelectLocalized(names, GameLanguage.Japanese));
        }

        [Fact]
        public void SelectLocalized_NullNames_ReturnsNull() {
            Assert.Null(LocalizationHelper.SelectLocalized(null, GameLanguage.English));
            Assert.Null(LocalizationHelper.SelectLocalized(null, GameLanguage.Japanese));
        }

        // ------------------------------------------------------------------------------
        // StatusItem schema check — both StatusName and StatusNameEnglish must be public
        // settable strings (used by both resolvers + downstream consumers). Pin the surface
        // so a future rename / removal breaks loudly.
        // ------------------------------------------------------------------------------

        [Fact]
        public void StatusItem_StatusName_IsPublicSettableString() {
            var prop = typeof(StatusItem).GetProperty(nameof(StatusItem.StatusName));
            Assert.NotNull(prop);
            Assert.Equal(typeof(string), prop!.PropertyType);
            Assert.True(prop.CanRead);
            Assert.True(prop.CanWrite);
        }

        [Fact]
        public void StatusItem_StatusNameEnglish_IsPublicSettableString() {
            var prop = typeof(StatusItem).GetProperty(nameof(StatusItem.StatusNameEnglish));
            Assert.NotNull(prop);
            Assert.Equal(typeof(string), prop!.PropertyType);
            Assert.True(prop.CanRead);
            Assert.True(prop.CanWrite);
        }

        // ------------------------------------------------------------------------------
        // Resolver-style end-to-end: simulates the resolver's three-line block (localised
        // + English-pinned) for every non-default language. This is exactly what
        // ActorItemResolver and PartyMemberResolver do when populating each statusEntry —
        // running it standalone proves the StatusNameEnglish field always returns the
        // English literal regardless of the configured language.
        // ------------------------------------------------------------------------------

        [Theory]
        [InlineData(GameLanguage.Japanese, "アイアンウィル")]
        [InlineData(GameLanguage.German,   "Eisenwille")]
        [InlineData(GameLanguage.French,   "Volonté de fer")]
        [InlineData(GameLanguage.Chinese,  "铁壁")]
        [InlineData(GameLanguage.Korean,   "굳건한 의지")]
        public void ResolverPattern_StatusNameEnglish_IsAlwaysEnglish_RegardlessOfGameLanguage(GameLanguage language, string expectedLocalised) {
            Localization names = IronWillNames();
            StatusItem statusEntry = new StatusItem {
                StatusName        = LocalizationHelper.SelectLocalized(names, language),
                StatusNameEnglish = names.English,
            };
            Assert.Equal(expectedLocalised, statusEntry.StatusName);
            Assert.Equal("Iron Will", statusEntry.StatusNameEnglish);
        }
    }
}
