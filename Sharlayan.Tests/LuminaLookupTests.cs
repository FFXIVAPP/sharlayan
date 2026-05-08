// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LuminaLookupTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Structural tests for Sharlayan.LuminaLookup. CI doesn't have an FFXIV sqpack to read,
//   so the assertions here are limited to what we can verify without one:
//   - Each helper exists with the expected public static signature.
//   - Each helper degrades gracefully (null result) when sqpack can't be resolved.
//   - The generic FindRowId entry point handles null/empty inputs cleanly.
//   - ClearCache resets the cache without throwing.
//   The harness exercises the live-sqpack path with known fixtures.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests {
    using System;
    using System.Reflection;

    using Sharlayan;

    using Xunit;

    public class LuminaLookupTests {
        // A configuration with no GameInstallPath and no ProcessModel — LuminaGameDataCache
        // will fail to resolve sqpack and return null, exercising the graceful-degradation
        // path through every public helper.
        private static SharlayanConfiguration ConfigWithoutSqpack() => new SharlayanConfiguration {
            GameInstallPath = null,
            ProcessModel = null,
        };

        // ------------------------------------------------------------------------------
        // Per-helper graceful-degradation: null sqpack → null result for every method.
        // ------------------------------------------------------------------------------

        [Fact]
        public void WeatherIdFromName_NoSqpack_ReturnsNull() {
            Assert.Null(LuminaLookup.WeatherIdFromName(ConfigWithoutSqpack(), "Moon Dust"));
        }

        [Fact]
        public void ActionIdFromName_NoSqpack_ReturnsNull() {
            Assert.Null(LuminaLookup.ActionIdFromName(ConfigWithoutSqpack(), "Refulgent Arrow"));
        }

        [Fact]
        public void StatusIdFromName_NoSqpack_ReturnsNull() {
            Assert.Null(LuminaLookup.StatusIdFromName(ConfigWithoutSqpack(), "Sleep"));
        }

        [Fact]
        public void PlaceNameIdFromName_NoSqpack_ReturnsNull() {
            Assert.Null(LuminaLookup.PlaceNameIdFromName(ConfigWithoutSqpack(), "Limsa Lominsa Lower Decks"));
        }

        [Fact]
        public void ContentFinderConditionIdFromName_NoSqpack_ReturnsNull() {
            Assert.Null(LuminaLookup.ContentFinderConditionIdFromName(ConfigWithoutSqpack(), "The Aery"));
        }

        [Fact]
        public void ItemIdFromName_NoSqpack_ReturnsNull() {
            Assert.Null(LuminaLookup.ItemIdFromName(ConfigWithoutSqpack(), "Potion"));
        }

        [Fact]
        public void BNpcNameIdFromName_NoSqpack_ReturnsNull() {
            Assert.Null(LuminaLookup.BNpcNameIdFromName(ConfigWithoutSqpack(), "striking dummy"));
        }

        [Fact]
        public void ENpcResidentIdFromName_NoSqpack_ReturnsNull() {
            Assert.Null(LuminaLookup.ENpcResidentIdFromName(ConfigWithoutSqpack(), "Hancock"));
        }

        [Fact]
        public void MountIdFromName_NoSqpack_ReturnsNull() {
            Assert.Null(LuminaLookup.MountIdFromName(ConfigWithoutSqpack(), "Company Chocobo"));
        }

        [Fact]
        public void CompanionIdFromName_NoSqpack_ReturnsNull() {
            Assert.Null(LuminaLookup.CompanionIdFromName(ConfigWithoutSqpack(), "Wind-up Cursor"));
        }

        // ------------------------------------------------------------------------------
        // Input-validation: empty / null / null-config / null-selector all return null
        // without throwing.
        // ------------------------------------------------------------------------------

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void WeatherIdFromName_NullOrEmpty_ReturnsNull(string? name) {
            Assert.Null(LuminaLookup.WeatherIdFromName(ConfigWithoutSqpack(), name!));
        }

        [Fact]
        public void FindRowId_NullConfiguration_ReturnsNull() {
            Assert.Null(LuminaLookup.FindRowId<Lumina.Excel.Sheets.Weather>(null!, "Moon Dust", r => r.Name.ExtractText()));
        }

        [Fact]
        public void FindRowId_NullNameSelector_ReturnsNull() {
            Assert.Null(LuminaLookup.FindRowId<Lumina.Excel.Sheets.Weather>(ConfigWithoutSqpack(), "Moon Dust", null!));
        }

        // ------------------------------------------------------------------------------
        // Cache cycle — ClearCache must not throw whether the cache is empty or populated.
        // ------------------------------------------------------------------------------

        [Fact]
        public void ClearCache_DoesNotThrow_WhenEmpty() {
            // No prior calls. Just verify the method is callable.
            LuminaLookup.ClearCache();
        }

        [Fact]
        public void ClearCache_DoesNotThrow_AfterUse() {
            // Prime an entry then clear; should be a no-op equivalent.
            _ = LuminaLookup.WeatherIdFromName(ConfigWithoutSqpack(), "Moon Dust");
            LuminaLookup.ClearCache();
        }

        // ------------------------------------------------------------------------------
        // Surface contract — each public helper exists with the expected signature.
        // Catches accidental rename / signature drift that the call-site tests above
        // wouldn't (since those compile against the public surface anyway).
        // ------------------------------------------------------------------------------

        [Theory]
        [InlineData(nameof(LuminaLookup.WeatherIdFromName))]
        [InlineData(nameof(LuminaLookup.ActionIdFromName))]
        [InlineData(nameof(LuminaLookup.StatusIdFromName))]
        [InlineData(nameof(LuminaLookup.PlaceNameIdFromName))]
        [InlineData(nameof(LuminaLookup.ContentFinderConditionIdFromName))]
        [InlineData(nameof(LuminaLookup.ItemIdFromName))]
        [InlineData(nameof(LuminaLookup.BNpcNameIdFromName))]
        [InlineData(nameof(LuminaLookup.ENpcResidentIdFromName))]
        [InlineData(nameof(LuminaLookup.MountIdFromName))]
        [InlineData(nameof(LuminaLookup.CompanionIdFromName))]
        public void Helper_HasExpectedSignature(string methodName) {
            MethodInfo? method = typeof(LuminaLookup).GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: new[] { typeof(SharlayanConfiguration), typeof(string) },
                modifiers: null);
            Assert.NotNull(method);
            Assert.Equal(typeof(uint?), method!.ReturnType);
        }
    }
}
