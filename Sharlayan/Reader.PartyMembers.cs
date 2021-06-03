// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.PartyMembers.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
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
    using Sharlayan.Utilities;

    public partial class Reader {
        public bool CanGetPartyMembers() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.CHARMAP_KEY) && this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.PARTYMAP_KEY) && this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.PARTYCOUNT_KEY);
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

            IntPtr partInfoAddress = (IntPtr) this._memoryHandler.Scanner.Locations[Signatures.PARTYMAP_KEY];
            IntPtr partyCountyAddress = this._memoryHandler.Scanner.Locations[Signatures.PARTYCOUNT_KEY];

            foreach (KeyValuePair<uint, PartyMember> kvp in this._partyWorkerDelegate.PartyMembers) {
                result.RemovedPartyMembers.TryAdd(kvp.Key, kvp.Value.Clone());
            }

            try {
                byte partyCount = this._memoryHandler.GetByte(partyCountyAddress);
                int sourceSize = this._memoryHandler.Structures.PartyMember.SourceSize;

                byte[] partyMemberMap = this._memoryHandler.BufferPool.Rent(sourceSize);

                try {
                    if (partyCount > 1 && partyCount < 9) {
                        for (uint i = 0; i < partyCount; i++) {
                            long address = partInfoAddress.ToInt64() + i * (uint) sourceSize;
                            this._memoryHandler.GetByteArray(new IntPtr(address), partyMemberMap);
                            uint ID = SharlayanBitConverter.TryToUInt32(partyMemberMap, this._memoryHandler.Structures.PartyMember.ID);
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

                            PartyMember entry = this._partyMemberResolver.ResolvePartyMemberFromBytes(partyMemberMap, existing);
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
                    this._memoryHandler.RaiseException(Logger, ex);
                }
                finally {
                    this._memoryHandler.BufferPool.Return(partyMemberMap);
                }
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }

            try {
                // REMOVE OLD PARTY MEMBERS FROM LIVE CURRENT DICTIONARY
                foreach (KeyValuePair<uint, PartyMember> kvp in result.RemovedPartyMembers) {
                    this._partyWorkerDelegate.RemovePartyMember(kvp.Key);
                }
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }

            result.PartyMembers = this._partyWorkerDelegate.PartyMembers;

            return result;
        }
    }
}