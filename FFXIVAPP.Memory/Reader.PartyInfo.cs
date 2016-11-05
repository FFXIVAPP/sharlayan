// FFXIVAPP.Memory ~ Reader.PartyInfo.cs
// 
// Copyright © 2007 - 2016 Ryan Wilson - All Rights Reserved
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
using FFXIVAPP.Memory.Core;
using FFXIVAPP.Memory.Delegates;
using FFXIVAPP.Memory.Helpers;
using FFXIVAPP.Memory.Models;

namespace FFXIVAPP.Memory
{
    public static partial class Reader
    {
        public static IntPtr PartyInfoMap { get; set; }
        public static IntPtr PartyCountMap { get; set; }

        public static PartyInfoReadResult GetPartyMembers()
        {
            var result = new PartyInfoReadResult();

            if (Scanner.Instance.Locations.ContainsKey("CHARMAP"))
            {
                if (Scanner.Instance.Locations.ContainsKey("PARTYMAP"))
                {
                    if (Scanner.Instance.Locations.ContainsKey("PARTYCOUNT"))
                    {
                        PartyInfoMap = Scanner.Instance.Locations["PARTYMAP"];
                        PartyCountMap = Scanner.Instance.Locations["PARTYCOUNT"];
                        try
                        {
                            var partyCount = MemoryHandler.Instance.GetByte(PartyCountMap);

                            if (partyCount > 1 && partyCount < 9)
                            {
                                for (uint i = 0; i < partyCount; i++)
                                {
                                    var size = (uint) MemoryHandler.Instance.Structures.PartyInfo.Size;
                                    var address = PartyInfoMap.ToInt64() + (i * size);
                                    var source = MemoryHandler.Instance.GetByteArray(new IntPtr(address), (int) size);
                                    var ID = BitConverter.ToUInt32(source, MemoryHandler.Instance.Structures.PartyEntity.ID);
                                    ActorEntity existing = null;
                                    if (result.PreviousParty.ContainsKey(ID))
                                    {
                                        result.PreviousParty.Remove(ID);
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
                                    if (result.PreviousParty.ContainsKey(entry.ID))
                                    {
                                        result.PreviousParty.Remove(entry.ID);
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
                        }
                    }
                }
            }

            return result;
        }
    }
}
