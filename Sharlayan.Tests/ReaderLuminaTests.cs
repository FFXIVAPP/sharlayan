// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReaderLuminaTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Unit coverage for the pure logic inside Reader.Lumina — specifically the language-code
//   mapping that the public helpers delegate to. The actual Lumina sheet reads require a
//   live FFXIV sqpack and are exercised by Sharlayan.Harness section [6], not here.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests {
    using Lumina.Data;

    using Sharlayan;

    using Xunit;

    public class ReaderLuminaTests {
        [Theory]
        [InlineData("en", Language.English)]
        [InlineData("EN", Language.English)]   // case-insensitive
        [InlineData(" en ", Language.English)] // whitespace-trimmed
        [InlineData("de", Language.German)]
        [InlineData("fr", Language.French)]
        [InlineData("ja", Language.Japanese)]
        [InlineData("jp", Language.Japanese)]  // alias — Chromatics passes "ja"/"jp" interchangeably
        public void MapLanguage_SupportedCodes_ResolveToLuminaLanguage(string code, Language expected) {
            Assert.Equal(expected, Reader.MapLanguage(code));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("zh")] // Global sqpack doesn't ship Chinese sheets — fallback to English.
        [InlineData("ko")] // Same for Korean.
        [InlineData("xyz")]
        public void MapLanguage_UnsupportedOrEmpty_FallsBackToEnglish(string code) {
            // Returning English for unknown / unshipped locales is deliberate: callers using
            // GetZoneName/GetWeatherName against a Global install won't get a null name just
            // because they passed a regional language code.
            Assert.Equal(Language.English, Reader.MapLanguage(code));
        }
    }
}
