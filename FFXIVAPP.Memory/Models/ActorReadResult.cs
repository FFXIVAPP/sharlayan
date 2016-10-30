using System.Collections.Concurrent;
using System.Collections.Generic;
using FFXIVAPP.Memory.Core;
using FFXIVAPP.Memory.Delegates;

namespace FFXIVAPP.Memory.Models
{
    public class ActorReadResult
    {
        public ActorReadResult()
        {
            PreviousMonster = new Dictionary<uint, uint>();
            PreviousNPC = new Dictionary<uint, uint>();
            PreviousPC = new Dictionary<uint, uint>();

            NewMonster = new List<uint>();
            NewNPC = new List<uint>();
            NewPC = new List<uint>();
        }

        public ConcurrentDictionary<uint, ActorEntity> MonsterEntities => MonsterWorkerDelegate.EntitiesDictionary;
        public ConcurrentDictionary<uint, ActorEntity> NPCEntities => NPCWorkerDelegate.EntitiesDictionary;
        public ConcurrentDictionary<uint, ActorEntity> PCEntities => PCWorkerDelegate.EntitiesDictionary;
        public Dictionary<uint, uint> PreviousMonster { get; set; }
        public Dictionary<uint, uint> PreviousNPC { get; set; }
        public Dictionary<uint, uint> PreviousPC { get; set; }
        public List<uint> NewMonster { get; set; }
        public List<uint> NewNPC { get; set; }
        public List<uint> NewPC { get; set; }
    }
}