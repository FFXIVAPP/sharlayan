// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using Sharlayan.Models.Structures;

    public partial class JobResourceResult {
        public AstrologianResources Astrologian => new AstrologianResources(this);
        public BardResources Bard => new BardResources(this);
        public BlackMageResources BlackMage => new BlackMageResources(this);
        public DancerResources Dancer => new DancerResources(this);
        public DarkKnightResources DarkKnight => new DarkKnightResources(this);
        public DragoonResources Dragoon => new DragoonResources(this);
        public GunBreakerResources GunBreaker => new GunBreakerResources(this);
        public MachinistResources Machinist => new MachinistResources(this);
        public MonkResources Monk => new MonkResources(this);
        public NinjaResources Ninja => new NinjaResources(this);

        public PaladinResources Paladin => new PaladinResources(this);
        public RedMageResources RedMage => new RedMageResources(this);
        public SamuraiResources Samurai => new SamuraiResources(this);
        public ScholarResources Scholar => new ScholarResources(this);
        public SummonerResources Summoner => new SummonerResources(this);
        public WarriorResources Warrior => new WarriorResources(this);
        public WhiteMageResources WhiteMage => new WhiteMageResources(this);
        internal byte[] Data { get; set; }
        internal JobResources Offsets { get; set; }
    }
}