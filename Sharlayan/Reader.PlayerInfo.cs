// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.PlayerInfo.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.PlayerInfo.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;

    using Sharlayan.Core;
    using Sharlayan.Models.ReadResults;

    public partial class Reader {
        private byte[] _playerMap;

        public bool CanGetPlayerInfo() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.CharacterMapKey) && this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.PlayerInformationKey);
            if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        public CurrentPlayerResult GetCurrentPlayer() {
            CurrentPlayerResult result = new CurrentPlayerResult();

            if (!this.CanGetPlayerInfo() || !this._memoryHandler.IsAttached) {
                return result;
            }

            IntPtr PlayerInfoMap = this._memoryHandler.Scanner.Locations[Signatures.PlayerInformationKey];

            if (PlayerInfoMap.ToInt64() <= 6496) {
                return result;
            }

            if (this._playerMap == null) {
                this._playerMap = new byte[this._memoryHandler.Structures.PlayerInfo.SourceSize];
            }

            try {
                this._memoryHandler.GetByteArray(PlayerInfoMap, this._playerMap);

                try {
                    result.PlayerInfo = this._playerInfoResolver.ResolvePlayerFromBytes(this._playerMap);
                }
                catch (Exception ex) {
                    this._memoryHandler.RaiseException(Logger, ex, true);
                }

                if (this.CanGetAgroEntities()) {
                    short agroCount = this._memoryHandler.GetInt16(this._memoryHandler.Scanner.Locations[Signatures.AgroCountKey]);
                    IntPtr agroStructure = (IntPtr) this._memoryHandler.Scanner.Locations[Signatures.AgroMapKey];

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
                                result.PlayerInfo.EnmityItems.Add(agroEntry);
                            }
                        }
                    }
                }

                result.Entity = this._pcWorkerDelegate.CurrentUser;
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex, true);
            }

            return result;
        }
    }
}