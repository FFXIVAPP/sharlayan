// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LuminaXivDatabaseProvider.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reads Action / Status / TerritoryType data directly from the player's installed FFXIV
//   client via Lumina, replacing the xivdatabase JSON slice of sharlayan-resources.
//   Struct/signature methods throw NotSupportedException — call ClientStructsYamlProvider
//   for those. Only EN/JP/DE/FR languages are populated (CN/KR out of scope per
//   the 2026-04-19 decisions).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Providers {
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;

    using Lumina;
    using Lumina.Data;
    using Lumina.Excel;
    using Lumina.Excel.Sheets;

    using Sharlayan.Models;
    using Sharlayan.Models.Structures;
    using Sharlayan.Models.XIVDatabase;

    using NativeAction = Lumina.Excel.Sheets.Action;
    using NativeStatus = Lumina.Excel.Sheets.Status;
    using NativeTerritoryType = Lumina.Excel.Sheets.TerritoryType;
    using StatusItem = Sharlayan.Models.XIVDatabase.StatusItem;

    internal sealed class LuminaXivDatabaseProvider : IResourceProvider {
        public Task<StructuresContainer> GetStructuresAsync(SharlayanConfiguration configuration) {
            throw new NotSupportedException($"{nameof(LuminaXivDatabaseProvider)} does not supply memory structures — use {nameof(FFXIVClientStructsDirectProvider)}.");
        }

        public Task<Signature[]> GetSignaturesAsync(SharlayanConfiguration configuration) {
            throw new NotSupportedException($"{nameof(LuminaXivDatabaseProvider)} does not supply memory signatures — use {nameof(FFXIVClientStructsDirectProvider)}.");
        }

        public Task GetActionsAsync(ConcurrentDictionary<uint, ActionItem> actions, SharlayanConfiguration configuration) {
            return Task.Run(() => {
                GameData gameData = OpenGameData(configuration);
                ExcelSheet<NativeAction> en = gameData.Excel.GetSheet<NativeAction>(Language.English);
                ExcelSheet<NativeAction> jp = gameData.Excel.GetSheet<NativeAction>(Language.Japanese);
                ExcelSheet<NativeAction> de = gameData.Excel.GetSheet<NativeAction>(Language.German);
                ExcelSheet<NativeAction> fr = gameData.Excel.GetSheet<NativeAction>(Language.French);

                foreach (NativeAction row in en) {
                    uint id = row.RowId;
                    ActionItem item = new ActionItem {
                        Name = BuildLocalization(
                            row.Name.ExtractText(),
                            jp.HasRow(id) ? jp.GetRow(id).Name.ExtractText() : string.Empty,
                            de.HasRow(id) ? de.GetRow(id).Name.ExtractText() : string.Empty,
                            fr.HasRow(id) ? fr.GetRow(id).Name.ExtractText() : string.Empty),
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
                GameData gameData = OpenGameData(configuration);
                ExcelSheet<NativeStatus> en = gameData.Excel.GetSheet<NativeStatus>(Language.English);
                ExcelSheet<NativeStatus> jp = gameData.Excel.GetSheet<NativeStatus>(Language.Japanese);
                ExcelSheet<NativeStatus> de = gameData.Excel.GetSheet<NativeStatus>(Language.German);
                ExcelSheet<NativeStatus> fr = gameData.Excel.GetSheet<NativeStatus>(Language.French);

                foreach (NativeStatus row in en) {
                    uint id = row.RowId;
                    StatusItem item = new StatusItem {
                        Name = BuildLocalization(
                            row.Name.ExtractText(),
                            jp.HasRow(id) ? jp.GetRow(id).Name.ExtractText() : string.Empty,
                            de.HasRow(id) ? de.GetRow(id).Name.ExtractText() : string.Empty,
                            fr.HasRow(id) ? fr.GetRow(id).Name.ExtractText() : string.Empty),
                        CompanyAction = row.IsFcBuff,
                    };
                    statusEffects.AddOrUpdate(id, item, (_, _) => item);
                }
            });
        }

        public Task GetZonesAsync(ConcurrentDictionary<uint, MapItem> zones, SharlayanConfiguration configuration) {
            return Task.Run(() => {
                GameData gameData = OpenGameData(configuration);
                ExcelSheet<NativeTerritoryType> territories = gameData.Excel.GetSheet<NativeTerritoryType>();

                ExcelSheet<PlaceName> placeNameEn = gameData.Excel.GetSheet<PlaceName>(Language.English);
                ExcelSheet<PlaceName> placeNameJp = gameData.Excel.GetSheet<PlaceName>(Language.Japanese);
                ExcelSheet<PlaceName> placeNameDe = gameData.Excel.GetSheet<PlaceName>(Language.German);
                ExcelSheet<PlaceName> placeNameFr = gameData.Excel.GetSheet<PlaceName>(Language.French);

                foreach (NativeTerritoryType row in territories) {
                    uint id = row.RowId;
                    uint placeNameId = row.PlaceName.RowId;

                    MapItem item = new MapItem {
                        Index = id,
                        IsDungeonInstance = row.ExclusiveType == 2,  // 2 = instanced content
                        Name = BuildLocalization(
                            placeNameEn.HasRow(placeNameId) ? placeNameEn.GetRow(placeNameId).Name.ExtractText() : string.Empty,
                            placeNameJp.HasRow(placeNameId) ? placeNameJp.GetRow(placeNameId).Name.ExtractText() : string.Empty,
                            placeNameDe.HasRow(placeNameId) ? placeNameDe.GetRow(placeNameId).Name.ExtractText() : string.Empty,
                            placeNameFr.HasRow(placeNameId) ? placeNameFr.GetRow(placeNameId).Name.ExtractText() : string.Empty),
                    };
                    zones.AddOrUpdate(id, item, (_, _) => item);
                }
            });
        }

        private static GameData OpenGameData(SharlayanConfiguration configuration) {
            string sqpackPath = ResolveSqpackPath(configuration);
            return new GameData(sqpackPath);
        }

        /// <summary>
        /// Resolves the sqpack directory path either from the explicit
        /// <see cref="SharlayanConfiguration.GameInstallPath"/> override, or (falling back)
        /// from the running game's MainModule file path (ffxiv_dx11.exe sits next to sqpack/).
        /// </summary>
        private static string ResolveSqpackPath(SharlayanConfiguration configuration) {
            if (!string.IsNullOrWhiteSpace(configuration.GameInstallPath)) {
                string explicitSqpack = Path.Combine(configuration.GameInstallPath, "game", "sqpack");
                if (Directory.Exists(explicitSqpack)) {
                    return explicitSqpack;
                }
                // Also accept if GameInstallPath is already the game\ folder
                explicitSqpack = Path.Combine(configuration.GameInstallPath, "sqpack");
                if (Directory.Exists(explicitSqpack)) {
                    return explicitSqpack;
                }
                throw new DirectoryNotFoundException($"sqpack directory not found under configured GameInstallPath '{configuration.GameInstallPath}'.");
            }

            string? exePath = configuration.ProcessModel?.Process?.MainModule?.FileName
                              ?? throw new InvalidOperationException("Cannot locate game sqpack: no GameInstallPath set and no running ProcessModel available.");

            string? gameDir = Path.GetDirectoryName(exePath);
            if (string.IsNullOrEmpty(gameDir)) {
                throw new InvalidOperationException($"Cannot derive game directory from exe path '{exePath}'.");
            }

            string sqpack = Path.Combine(gameDir, "sqpack");
            if (!Directory.Exists(sqpack)) {
                throw new DirectoryNotFoundException($"sqpack directory not found at '{sqpack}'.");
            }

            return sqpack;
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
