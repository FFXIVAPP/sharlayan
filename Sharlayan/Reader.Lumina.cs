// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Lumina.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Thin Lumina-backed lookup helpers exposed on the public Reader surface so downstream
//   consumers (e.g. Chromatics) can resolve zone names, exp tables, weather names, etc.
//   without importing Lumina types themselves — keeps the dependency chain linear:
//       consumer → Sharlayan → { FFXIVClientStructs, Lumina }
//
//   Each helper reuses the lazy GameData cache managed by Reader.GameState (same sqpack
//   resolution: explicit GameInstallPath → running process's MainModule → null). If
//   Lumina can't be opened, the helpers return sensible defaults (null / empty dict)
//   rather than throwing, so consumers can compose them without null-guarding every call.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Collections.Generic;

    public partial class Reader {
        /// <summary>
        /// Resolve a TerritoryType row id to its PlaceName in the requested language.
        /// Accepts "en" / "de" / "fr" / "ja" (case-insensitive). Returns null if Lumina
        /// isn't available, the row doesn't exist, or the language isn't supported.
        /// </summary>
        public string GetZoneName(uint territoryId, string language = "en") {
            Lumina.GameData lumina = this.GetLumina();
            if (lumina == null || territoryId == 0) return null;
            try {
                Lumina.Data.Language lang = MapLanguage(language);
                var territorySheet = lumina.Excel.GetSheet<Lumina.Excel.Sheets.TerritoryType>(lang);
                if (!territorySheet.HasRow(territoryId)) return null;
                var terri = territorySheet.GetRow(territoryId);
                uint placeNameId = terri.PlaceName.RowId;
                if (placeNameId == 0) return null;
                var placeSheet = lumina.Excel.GetSheet<Lumina.Excel.Sheets.PlaceName>(lang);
                if (!placeSheet.HasRow(placeNameId)) return null;
                return placeSheet.GetRow(placeNameId).Name.ExtractText();
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// Returns a dictionary mapping character level → total EXP required to reach the
        /// next level (ParamGrow.ExpToNext). Empty dictionary if Lumina isn't available.
        /// The table is stable across play sessions so callers can cache the result.
        /// </summary>
        public Dictionary<int, int> GetExpTable() {
            var table = new Dictionary<int, int>();
            Lumina.GameData lumina = this.GetLumina();
            if (lumina == null) return table;
            try {
                var sheet = lumina.Excel.GetSheet<Lumina.Excel.Sheets.ParamGrow>();
                foreach (var row in sheet) {
                    table[(int)row.RowId] = row.ExpToNext;
                }
            }
            catch {
                // Swallow — partial dictionary is better than throwing in a frame loop.
            }
            return table;
        }

        /// <summary>
        /// Resolve a Weather row id (typically from <see cref="Models.ReadResults.GameStateResult.CurrentWeatherId"/>)
        /// to its display name in the requested language. Useful if a consumer already has
        /// a weather id from somewhere other than <see cref="GetGameState"/>.
        /// </summary>
        public string GetWeatherName(byte weatherId, string language = "en") {
            Lumina.GameData lumina = this.GetLumina();
            if (lumina == null || weatherId == 0) return null;
            try {
                var sheet = lumina.Excel.GetSheet<Lumina.Excel.Sheets.Weather>(MapLanguage(language));
                if (!sheet.HasRow(weatherId)) return null;
                return sheet.GetRow(weatherId).Name.ExtractText();
            }
            catch {
                return null;
            }
        }

        // Internal for xunit coverage (Sharlayan.Tests carries the InternalsVisibleTo).
        internal static Lumina.Data.Language MapLanguage(string code) {
            if (string.IsNullOrWhiteSpace(code)) return Lumina.Data.Language.English;
            switch (code.Trim().ToLowerInvariant()) {
                case "en": return Lumina.Data.Language.English;
                case "de": return Lumina.Data.Language.German;
                case "fr": return Lumina.Data.Language.French;
                case "ja":
                case "jp": return Lumina.Data.Language.Japanese;
                // zh and ko aren't shipped in the Global sqpack that Lumina reads by default;
                // fall back to English so callers don't get a null for unsupported regions.
                default: return Lumina.Data.Language.English;
            }
        }
    }
}
