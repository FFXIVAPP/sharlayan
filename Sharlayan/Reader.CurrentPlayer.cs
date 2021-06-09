// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.CurrentPlayer.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.CurrentPlayer.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;

    using Sharlayan.Core;
    using Sharlayan.Models.ReadResults;
    using Sharlayan.Utilities;

    public partial class Reader {
        private bool CanGetPlayerInfo() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.PLAYERINFO_KEY);
            if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        private bool CanGetCurrentUser() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.CHARMAP_KEY);
            if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        public CurrentPlayerResult GetCurrentPlayer() {
            CurrentPlayerResult result = new CurrentPlayerResult();

            result.Entity = this.GetCurrentPlayerEntity();
            result.PlayerInfo = this.GetPlayerInfo();

            return result;
        }

        private ActorItem GetCurrentPlayerEntity() {
            ActorItem result = null;

            if (!this.CanGetCurrentUser() || !this._memoryHandler.IsAttached) {
                return result;
            }

            IntPtr targetAddress = IntPtr.Zero;

            int limit = this._memoryHandler.Structures.ActorItem.EntityCount;
            int sourceSize = this._memoryHandler.Structures.ActorItem.SourceSize;

            byte[] characterAddressMap = this._memoryHandler.BufferPool.Rent(8);
            byte[] sourceMap = this._memoryHandler.BufferPool.Rent(sourceSize);
            byte[] targetInfoMap = this._memoryHandler.BufferPool.Rent(128);

            try {
                this._memoryHandler.GetByteArray(this._memoryHandler.Scanner.Locations[Signatures.CHARMAP_KEY], characterAddressMap);

                IntPtr characterAddress = new IntPtr(SharlayanBitConverter.TryToInt64(characterAddressMap, 0));

                if (characterAddress != IntPtr.Zero) {
                    this._memoryHandler.GetByteArray(characterAddress, sourceMap);

                    result = this._actorItemResolver.ResolveActorFromBytes(sourceMap, true);

                    (uint mapID, uint mapIndex, uint mapTerritory) = this.GetMapInfo();

                    result.MapID = mapID;
                    result.MapIndex = mapIndex;
                    result.MapTerritory = mapTerritory;

                    if (targetAddress.ToInt64() > 0) {
                        this._memoryHandler.GetByteArray(targetAddress, targetInfoMap);
                        result.TargetID = (int) SharlayanBitConverter.TryToUInt32(targetInfoMap, this._memoryHandler.Structures.ActorItem.ID);
                    }
                }
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }
            finally {
                this._memoryHandler.BufferPool.Return(characterAddressMap);
                this._memoryHandler.BufferPool.Return(sourceMap);
                this._memoryHandler.BufferPool.Return(targetInfoMap);
            }

            return result;
        }

        private PlayerInfo GetPlayerInfo() {
            PlayerInfo result = new PlayerInfo();

            if (!this.CanGetPlayerInfo() || !this._memoryHandler.IsAttached) {
                return result;
            }

            IntPtr playerInfoAddress = this._memoryHandler.Scanner.Locations[Signatures.PLAYERINFO_KEY];

            if (playerInfoAddress.ToInt64() <= 6496) {
                return result;
            }

            byte[] playerMap = this._memoryHandler.BufferPool.Rent(this._memoryHandler.Structures.PlayerInfo.SourceSize);

            try {
                this._memoryHandler.GetByteArray(playerInfoAddress, playerMap);

                try {
                    result = this._playerInfoResolver.ResolvePlayerFromBytes(playerMap);
                }
                catch (Exception ex) {
                    this._memoryHandler.RaiseException(Logger, ex);
                }

                if (this.CanGetAgroEntities()) {
                    short agroCount = this._memoryHandler.GetInt16(this._memoryHandler.Scanner.Locations[Signatures.AGRO_COUNT_KEY]);
                    IntPtr agroStructure = (IntPtr) this._memoryHandler.Scanner.Locations[Signatures.AGROMAP_KEY];

                    if (agroCount > 0 && agroCount < 32 && agroStructure.ToInt64() > 0) {
                        int agroSourceSize = this._memoryHandler.Structures.EnmityItem.SourceSize;
                        for (uint i = 0; i < agroCount; i++) {
                            IntPtr address = new IntPtr(agroStructure.ToInt64() + i * agroSourceSize);
                            EnmityItem agroEntry = new EnmityItem {
                                ID = this._memoryHandler.GetUInt32(address, this._memoryHandler.Structures.EnmityItem.ID),
                                Name = this._memoryHandler.GetString(address + this._memoryHandler.Structures.EnmityItem.Name),
                                Enmity = this._memoryHandler.GetUInt32(address + this._memoryHandler.Structures.EnmityItem.Enmity),
                            };
                            if (agroEntry.ID > 0) {
                                result.EnmityItems.Add(agroEntry);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }
            finally {
                this._memoryHandler.BufferPool.Return(playerMap);
            }

            return result;
        }
    }
}