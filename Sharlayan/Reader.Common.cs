// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Common.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.Common.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;

    using NLog;

    using Sharlayan.Core.Enums;

    public partial class Reader {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool CanGetAgroEntities() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.AGRO_COUNT_KEY) && this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.AGROMAP_KEY);
            if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        public bool CanGetEnmityEntities() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.ENMITY_COUNT_KEY) && this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.ENMITYMAP_KEY);
            if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        private (ushort EventObjectTypeID, Actor.EventObjectType EventObjectType) GetEventObjectType(IntPtr address) {
            IntPtr eventObjectTypePointer = IntPtr.Add(address, this._memoryHandler.Structures.ActorItem.EventObjectType);
            IntPtr eventObjectTypeAddress = this._memoryHandler.ReadPointer(eventObjectTypePointer, 4);

            ushort eventObjectTypeID = this._memoryHandler.GetUInt16(eventObjectTypeAddress);

            return (eventObjectTypeID, (Actor.EventObjectType) eventObjectTypeID);
        }

        private (uint mapID, uint mapIndex, uint mapTerritory) GetMapInfo() {
            uint mapID = 0;
            uint mapIndex = 0;
            uint mapTerritory = 0;

            if (this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.MAPINFO_KEY)) {
                try {
                    mapTerritory = this._memoryHandler.GetUInt32(this._memoryHandler.Scanner.Locations[Signatures.MAPINFO_KEY]);
                    mapID = this._memoryHandler.GetUInt32(this._memoryHandler.Scanner.Locations[Signatures.MAPINFO_KEY], 8);
                }
                catch (Exception ex) {
                    this._memoryHandler.RaiseException(Logger, ex);
                }
            }

            if (this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.ZONEINFO_KEY)) {
                try {
                    mapIndex = this._memoryHandler.GetUInt32(this._memoryHandler.Scanner.Locations[Signatures.ZONEINFO_KEY], 8);

                    // current map is 0 if the map the actor is in does not have more than 1 layer.
                    // if the map has more than 1 layer, overwrite the map id.
                    uint currentActiveMapID = this._memoryHandler.GetUInt32(this._memoryHandler.Scanner.Locations[Signatures.ZONEINFO_KEY]);
                    if (currentActiveMapID > 0) {
                        mapID = currentActiveMapID;
                    }
                }
                catch (Exception ex) {
                    this._memoryHandler.RaiseException(Logger, ex);
                }
            }

            return (mapID, mapIndex, mapTerritory);
        }
    }
}