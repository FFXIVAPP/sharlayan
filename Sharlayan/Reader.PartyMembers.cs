// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.PartyMembers.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.PartyMembers.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Collections.Generic;

    using Sharlayan.Core;
    using Sharlayan.Models.ReadResults;

    using BitConverter = Sharlayan.Utilities.BitConverter;

    public partial class Reader {
        public bool CanGetPartyMembers() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.CharacterMapKey) && this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.PartyMapKey) && this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.PartyCountKey);
            if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        public PartyResult GetPartyMembers() {
            PartyResult result = new PartyResult();

            if (!this.CanGetPartyMembers() || !this._memoryHandler.IsAttached) {
                return result;
            }

            IntPtr PartyInfoMap = (IntPtr) this._memoryHandler.Scanner.Locations[Signatures.PartyMapKey];
            MemoryLocation PartyCountMap = this._memoryHandler.Scanner.Locations[Signatures.PartyCountKey];

            foreach (KeyValuePair<uint, PartyMember> kvp in this._partyWorkerDelegate.PartyMembers) {
                result.RemovedPartyMembers.TryAdd(kvp.Key, kvp.Value.Clone());
            }

            try {
                byte partyCount = this._memoryHandler.GetByte(PartyCountMap);
                int sourceSize = this._memoryHandler.Structures.PartyMember.SourceSize;

                if (partyCount > 1 && partyCount < 9) {
                    for (uint i = 0; i < partyCount; i++) {
                        long address = PartyInfoMap.ToInt64() + i * (uint) sourceSize;
                        byte[] source = this._memoryHandler.GetByteArray(new IntPtr(address), sourceSize);
                        uint ID = BitConverter.TryToUInt32(source, this._memoryHandler.Structures.PartyMember.ID);
                        ActorItem existing = null;
                        bool newEntry = false;

                        if (result.RemovedPartyMembers.ContainsKey(ID)) {
                            result.RemovedPartyMembers.TryRemove(ID, out PartyMember removedPartyMember);
                            if (this._monsterWorkerDelegate.ActorItems.ContainsKey(ID)) {
                                existing = this._monsterWorkerDelegate.GetActorItem(ID);
                            }

                            if (this._pcWorkerDelegate.ActorItems.ContainsKey(ID)) {
                                existing = this._pcWorkerDelegate.GetActorItem(ID);
                            }
                        }
                        else {
                            newEntry = true;
                        }

                        PartyMember entry = this._partyMemberResolver.ResolvePartyMemberFromBytes(source, existing);
                        if (!entry.IsValid) {
                            continue;
                        }

                        if (existing != null) {
                            continue;
                        }

                        if (newEntry) {
                            this._partyWorkerDelegate.EnsurePartyMember(entry.ID, entry);
                            result.NewPartyMembers.TryAdd(entry.ID, entry.Clone());
                        }
                    }
                }

                if (partyCount <= 1 && this._pcWorkerDelegate.CurrentUser != null) {
                    PartyMember entry = this._partyMemberResolver.ResolvePartyMemberFromBytes(Array.Empty<byte>(), this._pcWorkerDelegate.CurrentUser);
                    if (result.RemovedPartyMembers.ContainsKey(entry.ID)) {
                        result.RemovedPartyMembers.TryRemove(entry.ID, out PartyMember removedPartyMember);
                    }

                    this._partyWorkerDelegate.EnsurePartyMember(entry.ID, entry);
                }
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex, true);
            }

            try {
                // REMOVE OLD PARTY MEMBERS FROM LIVE CURRENT DICTIONARY
                foreach (KeyValuePair<uint, PartyMember> kvp in result.RemovedPartyMembers) {
                    this._partyWorkerDelegate.RemovePartyMember(kvp.Key);
                }
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex, true);
            }

            result.PartyMembers = this._partyWorkerDelegate.PartyMembers;

            return result;
        }
    }
}