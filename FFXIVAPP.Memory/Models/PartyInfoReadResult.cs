using System.Collections.Concurrent;
using System.Collections.Generic;
using FFXIVAPP.Memory.Core;
using FFXIVAPP.Memory.Delegates;

namespace FFXIVAPP.Memory.Models
{
    public class PartyInfoReadResult
    {
        public PartyInfoReadResult()
        {
            PreviousParty = new Dictionary<uint, uint>();

            NewParty = new List<uint>();
        }

        public ConcurrentDictionary<uint, PartyEntity> PartyEntities => PartyInfoWorkerDelegate.EntitiesDictionary;
        public Dictionary<uint, uint> PreviousParty { get; set; }
        public List<uint> NewParty { get; set; }
    }
}