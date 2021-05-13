// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Common.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.Common.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;

    using NLog;

    using Sharlayan.Core;
    using Sharlayan.Core.Enums;

    public partial class Reader {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool CanGetAgroEntities() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.AgroCountKey) && this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.AgroMapKey);
            if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        public bool CanGetEnmityEntities() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.EnmityCountKey) && this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.EnmityMapKey);
            if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        private void EnsureMapAndZone(ActorItem entry) {
            if (this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.MapInformationKey)) {
                try {
                    entry.MapTerritory = this._memoryHandler.GetUInt32(this._memoryHandler.Scanner.Locations[Signatures.MapInformationKey]);
                    entry.MapID = this._memoryHandler.GetUInt32(this._memoryHandler.Scanner.Locations[Signatures.MapInformationKey], 8);
                }
                catch (Exception ex) {
                    this._memoryHandler.RaiseException(Logger, ex, true);
                }
            }

            if (this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.ZoneInformationKey)) {
                try {
                    entry.MapIndex = this._memoryHandler.GetUInt32(this._memoryHandler.Scanner.Locations[Signatures.ZoneInformationKey], 8);

                    // current map is 0 if the map the actor is in does not have more than 1 layer.
                    // if the map has more than 1 layer, overwrite the map id.
                    uint currentActiveMapID = this._memoryHandler.GetUInt32(this._memoryHandler.Scanner.Locations[Signatures.ZoneInformationKey]);
                    if (currentActiveMapID > 0) {
                        entry.MapID = currentActiveMapID;
                    }
                }
                catch (Exception ex) {
                    this._memoryHandler.RaiseException(Logger, ex, true);
                }
            }
        }

        private (ushort EventObjectTypeID, Actor.EventObjectType EventObjectType) GetEventObjectType(IntPtr address) {
            IntPtr eventObjectTypePointer = IntPtr.Add(address, this._memoryHandler.Structures.ActorItem.EventObjectType);
            IntPtr eventObjectTypeAddress = this._memoryHandler.ReadPointer(eventObjectTypePointer, 4);

            ushort eventObjectTypeID = this._memoryHandler.GetUInt16(eventObjectTypeAddress);

            return (eventObjectTypeID, (Actor.EventObjectType) eventObjectTypeID);
        }
    }
}