// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultSet.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ResultSet.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.SharlayanWrappers {
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using Sharlayan.Core;
    using Sharlayan.Core.JobResources;

    public class ResultSet {
        public List<ActionContainer> ActionContainers { get; internal set; } = new List<ActionContainer>();
        public AstrologianResources Astrologian { get; internal set; } = new AstrologianResources();
        public BardResources Bard { get; internal set; } = new BardResources();
        public BlackMageResources BlackMage { get; internal set; } = new BlackMageResources();

        /// <summary>
        /// ChatLogItems is continuous and will be appended to
        /// </summary>
        public List<ChatLogItem> ChatLogItems { get; internal set; } = new List<ChatLogItem>();

        /// <summary>
        /// CurrentMonsters static ref from Sharlayan and will not lose reference
        /// </summary>
        public ConcurrentDictionary<uint, ActorItem> CurrentMonsters { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();

        /// <summary>
        /// CurrentNPCs static ref from Sharlayan and will not lose reference
        /// </summary>
        public ConcurrentDictionary<uint, ActorItem> CurrentNPCs { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();

        /// <summary>
        /// CurrentPCs static ref from Sharlayan and will not lose reference
        /// </summary>
        public ConcurrentDictionary<uint, ActorItem> CurrentPCs { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();

        public DancerResources Dancer { get; internal set; } = new DancerResources();
        public DarkKnightResources DarkKnight { get; internal set; } = new DarkKnightResources();
        public DragoonResources Dragoon { get; internal set; } = new DragoonResources();
        public ActorItem Entity { get; internal set; }
        public GunBreakerResources GunBreaker { get; internal set; } = new GunBreakerResources();
        public List<InventoryContainer> InventoryContainers { get; internal set; } = new List<InventoryContainer>();
        public MachinistResources Machinist { get; internal set; } = new MachinistResources();
        public MonkResources Monk { get; internal set; } = new MonkResources();
        public ConcurrentDictionary<uint, ActorItem> NewMonsters { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();
        public ConcurrentDictionary<uint, ActorItem> NewNPCs { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();
        public ConcurrentDictionary<uint, PartyMember> NewPartyMembers { get; internal set; } = new ConcurrentDictionary<uint, PartyMember>();
        public ConcurrentDictionary<uint, ActorItem> NewPCs { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();
        public NinjaResources Ninja { get; internal set; } = new NinjaResources();
        public PaladinResources Paladin { get; internal set; } = new PaladinResources();
        public ConcurrentDictionary<uint, PartyMember> PartyMembers { get; internal set; } = new ConcurrentDictionary<uint, PartyMember>();
        public PlayerInfo PlayerInfo { get; internal set; } = new PlayerInfo();
        public RedMageResources RedMage { get; internal set; } = new RedMageResources();
        public ConcurrentDictionary<uint, ActorItem> RemovedMonsters { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();
        public ConcurrentDictionary<uint, ActorItem> RemovedNPCs { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();
        public ConcurrentDictionary<uint, PartyMember> RemovedPartyMembers { get; internal set; } = new ConcurrentDictionary<uint, PartyMember>();
        public ConcurrentDictionary<uint, ActorItem> RemovedPCs { get; internal set; } = new ConcurrentDictionary<uint, ActorItem>();
        public SamuraiResources Samurai { get; internal set; } = new SamuraiResources();
        public ScholarResources Scholar { get; internal set; } = new ScholarResources();
        public SummonerResources Summoner { get; internal set; } = new SummonerResources();
        public TargetInfo TargetInfo { get; internal set; } = new TargetInfo();
        public bool TargetsFound { get; internal set; }
        public WarriorResources Warrior { get; internal set; } = new WarriorResources();
        public WhiteMageResources WhiteMage { get; internal set; } = new WhiteMageResources();
    }
}