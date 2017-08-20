// Sharlayan ~ Reader.cs
// 
// Copyright © 2007 - 2017 Ryan Wilson - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using NLog;
using Sharlayan.Core;

namespace Sharlayan
{
    public static partial class Reader
    {
        #region Logger

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        public static bool CanGetAgroEntities()
        {
            var canRead = Scanner.Instance.Locations.ContainsKey(Signatures.AgroCountKey) && Scanner.Instance.Locations.ContainsKey(Signatures.AgroMapKey);
            if (canRead)
            {
                // OTHER STUFF?
            }
            return canRead;
        }

        public static bool CanGetEnmityEntities()
        {
            var canRead = Scanner.Instance.Locations.ContainsKey(Signatures.EnmityCountKey) && Scanner.Instance.Locations.ContainsKey(Signatures.EnmityMapKey);
            if (canRead)
            {
                // OTHER STUFF?
            }
            return canRead;
        }

        private static void EnsureMapAndZone(ActorEntity entry)
        {
            if (Scanner.Instance.Locations.ContainsKey(Signatures.MapInformationKey))
            {
                try
                {
                    entry.MapTerritory = (uint) MemoryHandler.Instance.GetPlatformUInt(Scanner.Instance.Locations[Signatures.MapInformationKey]);
                    entry.MapID = (uint) MemoryHandler.Instance.GetPlatformUInt(Scanner.Instance.Locations[Signatures.MapInformationKey], 8);
                }
                catch (Exception ex)
                {
                    MemoryHandler.Instance.RaiseException(Logger, ex, true);
                }
            }

            if (Scanner.Instance.Locations.ContainsKey(Signatures.ZoneInformationKey))
            {
                try
                {
                    entry.MapIndex = (uint) MemoryHandler.Instance.GetPlatformUInt(Scanner.Instance.Locations[Signatures.ZoneInformationKey], 8);

                    // current map is 0 if the map the actor is in does not have more than 1 layer.
                    // if the map has more than 1 layer, overwrite the map id.
                    var currentActiveMapID = (uint) MemoryHandler.Instance.GetPlatformUInt(Scanner.Instance.Locations[Signatures.ZoneInformationKey]);
                    if (currentActiveMapID > 0)
                    {
                        entry.MapID = currentActiveMapID;
                    }
                }
                catch (Exception ex)
                {
                    MemoryHandler.Instance.RaiseException(Logger, ex, true);
                }
            }
        }
    }
}
