// Sharlayan ~ Reader.PartyInfo.cs
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
using Sharlayan.Core;
using Sharlayan.Delegates;
using Sharlayan.Helpers;
using Sharlayan.Models;
using BitConverter = Sharlayan.Helpers.BitConverter;

namespace Sharlayan
{
    public static partial class Reader
    {
        public static bool CanGetPartyMembers()
        {
            var canRead = Scanner.Instance.Locations.ContainsKey(Signatures.CharacterMapKey) && Scanner.Instance.Locations.ContainsKey(Signatures.PartyMapKey) && Scanner.Instance.Locations.ContainsKey(Signatures.PartyCountKey);
            if (canRead)
            {
                // OTHER STUFF?
            }
            return canRead;
        }

        public static PartyInfoReadResult GetPartyMembers()
        {
            var result = new PartyInfoReadResult();

            if (!CanGetPartyMembers() || !MemoryHandler.Instance.IsAttached)
            {
                return result;
            }

            var PartyInfoMap = (IntPtr) Scanner.Instance.Locations[Signatures.PartyMapKey];
            var PartyCountMap = Scanner.Instance.Locations[Signatures.PartyCountKey];

            try
            {
                var partyCount = MemoryHandler.Instance.GetByte(PartyCountMap);

                if (partyCount > 1 && partyCount < 9)
                {
                    for (uint i = 0; i < partyCount; i++)
                    {
                        var size = (uint) MemoryHandler.Instance.Structures.PartyInfo.Size;
                        var address = PartyInfoMap.ToInt64() + i * size;
                        var source = MemoryHandler.Instance.GetByteArray(new IntPtr(address), (int) size);
                        var ID = BitConverter.TryToUInt32(source, MemoryHandler.Instance.Structures.PartyEntity.ID);
                        ActorEntity existing = null;
                        if (result.RemovedParty.ContainsKey(ID))
                        {
                            result.RemovedParty.Remove(ID);
                            if (MonsterWorkerDelegate.EntitiesDictionary.ContainsKey(ID))
                            {
                                existing = MonsterWorkerDelegate.GetEntity(ID);
                            }
                            if (PCWorkerDelegate.EntitiesDictionary.ContainsKey(ID))
                            {
                                existing = PCWorkerDelegate.GetEntity(ID);
                            }
                        }
                        else
                        {
                            result.NewParty.Add(ID);
                        }
                        var entry = PartyEntityHelper.ResolvePartyMemberFromBytes(source, existing);
                        if (!entry.IsValid)
                        {
                            continue;
                        }
                        if (existing != null)
                        {
                            continue;
                        }
                        PartyInfoWorkerDelegate.EnsureEntity(entry.ID, entry);
                    }
                }
                else if (partyCount == 0 || partyCount == 1)
                {
                    var entry = PartyEntityHelper.ResolvePartyMemberFromBytes(new byte[0], PCWorkerDelegate.CurrentUser);
                    if (entry.IsValid)
                    {
                        var exists = false;
                        if (result.RemovedParty.ContainsKey(entry.ID))
                        {
                            result.RemovedParty.Remove(entry.ID);
                            exists = true;
                        }
                        else
                        {
                            result.NewParty.Add(entry.ID);
                        }
                        if (!exists)
                        {
                            PartyInfoWorkerDelegate.EnsureEntity(entry.ID, entry);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MemoryHandler.Instance.RaiseException(Logger, ex, true);
            }

            try
            {
                // REMOVE OLD PARTY MEMBERS FROM LIVE CURRENT DICTIONARY
                foreach (var kvp in result.RemovedParty)
                {
                    PartyInfoWorkerDelegate.RemoveEntity(kvp.Key);
                }
            }
            catch (Exception ex)
            {
                MemoryHandler.Instance.RaiseException(Logger, ex, true);
            }

            return result;
        }
    }
}
