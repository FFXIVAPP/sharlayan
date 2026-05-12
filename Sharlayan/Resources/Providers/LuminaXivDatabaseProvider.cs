// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LuminaXivDatabaseProvider.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reads Action / Status / TerritoryType data directly from the player's installed FFXIV
//   client via Lumina, replacing the xivdatabase JSON slice of sharlayan-resources.
//   Struct/signature methods throw NotSupportedException - call ClientStructsYamlProvider
//   for those. Only EN/JP/DE/FR languages are populated (CN/KR out of scope per
//   the 2026-04-19 decisions). Installs whose Excel sheets are missing a requested
//   language (CN-region clients with stripped headers, etc.) are tolerated: English
//   is treated as required, the other three are best-effort.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Providers {
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    using Lumina;
    using Lumina.Data;
    using Lumina.Excel;
    using Lumina.Excel.Exceptions;
    using Lumina.Excel.Sheets;

    using NLog;

    using Sharlayan.Models;
    using Sharlayan.Models.Structures;
    using Sharlayan.Models.XIVDatabase;

    using NativeAction = Lumina.Excel.Sheets.Action;
    using NativeStatus = Lumina.Excel.Sheets.Status;
    using NativeTerritoryType = Lumina.Excel.Sheets.TerritoryType;
    using StatusItem = Sharlayan.Models.XIVDatabase.StatusItem;

    internal sealed class LuminaXivDatabaseProvider : IResourceProvider {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Task<StructuresContainer> GetStructuresAsync(SharlayanConfiguration configuration) {
            throw new NotSupportedException($"{nameof(LuminaXivDatabaseProvider)} does not supply memory structures - use {nameof(FFXIVClientStructsDirectProvider)}.");
        }

        public Task<Signature[]> GetSignaturesAsync(SharlayanConfiguration configuration) {
            throw new NotSupportedException($"{nameof(LuminaXivDatabaseProvider)} does not supply memory signatures - use {nameof(FFXIVClientStructsDirectProvider)}.");
        }

        public Task GetActionsAsync(ConcurrentDictionary<uint, ActionItem> actions, SharlayanConfiguration configuration) {
            return Task.Run(() => {
                GameData gameData = LuminaGameDataCache.Get(configuration);
                ExcelSheet<NativeAction> en = TryGetSheet<NativeAction>(gameData, Language.English, "Action");
                if (en == null) {
                    Logger.Warn("[LuminaXivDatabaseProvider] FFXIV install missing English Excel data for sheet 'Action' - bulk lookup skipped");
                    return;
                }
                ExcelSheet<NativeAction> jp = TryGetSheet<NativeAction>(gameData, Language.Japanese, "Action");
                ExcelSheet<NativeAction> de = TryGetSheet<NativeAction>(gameData, Language.German, "Action");
                ExcelSheet<NativeAction> fr = TryGetSheet<NativeAction>(gameData, Language.French, "Action");

                foreach (NativeAction row in en) {
                    uint id = row.RowId;
                    ActionItem item = new ActionItem {
                        Name = BuildLocalization(
                            row.Name.ExtractText(),
                            (jp != null && jp.HasRow(id)) ? jp.GetRow(id).Name.ExtractText() : string.Empty,
                            (de != null && de.HasRow(id)) ? de.GetRow(id).Name.ExtractText() : string.Empty,
                            (fr != null && fr.HasRow(id)) ? fr.GetRow(id).Name.ExtractText() : string.Empty),
                        Icon = row.Icon,
                        ClassJob = row.ClassJob.RowId == 0 ? 0 : (int)row.ClassJob.RowId,
                        ClassJobCategory = (int)row.ClassJobCategory.RowId,
                        Level = row.ClassJobLevel,
                        ActionCategory = row.ActionCategory.RowId == 0 ? 0 : (int)row.ActionCategory.RowId,
                        CastRange = row.Range,
                        EffectRange = row.EffectRange,
                        CastTime = row.Cast100ms / 10m,
                        RecastTime = row.Recast100ms / 10m,
                        // CanTargetDead in legacy Sharlayan is now DeadTargetBehaviour
                        // (sbyte enum). Non-zero means the action can target dead entities.
                        CanTargetDead = row.DeadTargetBehaviour != 0 ? 1 : 0,
                        // Legacy "CanTargetFriendly" maps closest to CanTargetAlly.
                        CanTargetFriendly = row.CanTargetAlly ? 1 : 0,
                        CanTargetHostile = row.CanTargetHostile ? 1 : 0,
                        CanTargetParty = row.CanTargetParty ? 1 : 0,
                        CanTargetSelf = row.CanTargetSelf ? 1 : 0,
                        IsPvp = row.IsPvP ? 1 : 0,
                        IsTargetArea = row.TargetArea ? 1 : 0,
                    };
                    actions.AddOrUpdate(id, item, (_, _) => item);
                }
            });
        }

        public Task GetStatusEffectsAsync(ConcurrentDictionary<uint, StatusItem> statusEffects, SharlayanConfiguration configuration) {
            return Task.Run(() => {
                GameData gameData = LuminaGameDataCache.Get(configuration);
                ExcelSheet<NativeStatus> en = TryGetSheet<NativeStatus>(gameData, Language.English, "Status");
                if (en == null) {
                    Logger.Warn("[LuminaXivDatabaseProvider] FFXIV install missing English Excel data for sheet 'Status' - bulk lookup skipped");
                    return;
                }
                ExcelSheet<NativeStatus> jp = TryGetSheet<NativeStatus>(gameData, Language.Japanese, "Status");
                ExcelSheet<NativeStatus> de = TryGetSheet<NativeStatus>(gameData, Language.German, "Status");
                ExcelSheet<NativeStatus> fr = TryGetSheet<NativeStatus>(gameData, Language.French, "Status");

                foreach (NativeStatus row in en) {
                    uint id = row.RowId;
                    StatusItem item = new StatusItem {
                        Name = BuildLocalization(
                            row.Name.ExtractText(),
                            (jp != null && jp.HasRow(id)) ? jp.GetRow(id).Name.ExtractText() : string.Empty,
                            (de != null && de.HasRow(id)) ? de.GetRow(id).Name.ExtractText() : string.Empty,
                            (fr != null && fr.HasRow(id)) ? fr.GetRow(id).Name.ExtractText() : string.Empty),
                        CompanyAction = row.IsFcBuff,
                        // MaxStacks lets resolvers distinguish stacking debuffs (where
                        // Status.Param = stack count) from non-stacking buffs (where Param
                        // means something else entirely - food/potion id, etc.). Without
                        // this, Stacks ends up showing arbitrary Param bytes for every
                        // status as if everything stacks.
                        MaxStacks = row.MaxStacks,
                    };
                    statusEffects.AddOrUpdate(id, item, (_, _) => item);
                }
            });
        }

        public Task GetZonesAsync(ConcurrentDictionary<uint, MapItem> zones, SharlayanConfiguration configuration) {
            return Task.Run(() => {
                GameData gameData = LuminaGameDataCache.Get(configuration);

                // TerritoryType is the iteration source. The no-language overload picks the
                // sheet's default - usually safe even on stripped-header installs because
                // those sheets default to Language.None - but if it does throw, log and
                // skip the whole pass rather than letting it bubble up as an unobserved
                // background-task fault.
                ExcelSheet<NativeTerritoryType> territories;
                try {
                    territories = gameData.Excel.GetSheet<NativeTerritoryType>();
                }
                catch (UnsupportedLanguageException) {
                    Logger.Warn("[LuminaXivDatabaseProvider] FFXIV install missing Excel data for sheet 'TerritoryType' - bulk lookup skipped");
                    return;
                }

                ExcelSheet<PlaceName> placeNameEn = TryGetSheet<PlaceName>(gameData, Language.English, "PlaceName");
                ExcelSheet<PlaceName> placeNameJp = TryGetSheet<PlaceName>(gameData, Language.Japanese, "PlaceName");
                ExcelSheet<PlaceName> placeNameDe = TryGetSheet<PlaceName>(gameData, Language.German, "PlaceName");
                ExcelSheet<PlaceName> placeNameFr = TryGetSheet<PlaceName>(gameData, Language.French, "PlaceName");

                foreach (NativeTerritoryType row in territories) {
                    uint id = row.RowId;
                    uint placeNameId = row.PlaceName.RowId;

                    MapItem item = new MapItem {
                        Index = id,
                        IsDungeonInstance = row.ExclusiveType == 2,  // 2 = instanced content
                        Name = BuildLocalization(
                            (placeNameEn != null && placeNameEn.HasRow(placeNameId)) ? placeNameEn.GetRow(placeNameId).Name.ExtractText() : string.Empty,
                            (placeNameJp != null && placeNameJp.HasRow(placeNameId)) ? placeNameJp.GetRow(placeNameId).Name.ExtractText() : string.Empty,
                            (placeNameDe != null && placeNameDe.HasRow(placeNameId)) ? placeNameDe.GetRow(placeNameId).Name.ExtractText() : string.Empty,
                            (placeNameFr != null && placeNameFr.HasRow(placeNameId)) ? placeNameFr.GetRow(placeNameId).Name.ExtractText() : string.Empty),
                    };
                    zones.AddOrUpdate(id, item, (_, _) => item);
                }
            });
        }

        /// <summary>
        /// Wraps <see cref="ExcelModule.GetSheet{T}(Language?, string)"/> so callers can decide
        /// per-language whether a missing sheet is fatal. Returns null on
        /// <see cref="UnsupportedLanguageException"/> (the typical "install has stripped Excel
        /// headers" case); any other exception still bubbles since it indicates a real fault.
        /// </summary>
        private static ExcelSheet<T> TryGetSheet<T>(GameData gameData, Language language, string sheetName)
            where T : struct, IExcelRow<T> {
            try {
                return gameData.Excel.GetSheet<T>(language);
            }
            catch (UnsupportedLanguageException) {
                return null;
            }
        }

        private static Localization BuildLocalization(string english, string japanese, string german, string french) {
            return new Localization {
                English = english,
                Japanese = japanese,
                German = german,
                French = french,
                Chinese = Constants.UNKNOWN_LOCALIZED_NAME,
                Korean = Constants.UNKNOWN_LOCALIZED_NAME,
            };
        }
    }
}
