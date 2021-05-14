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
    using Sharlayan.Core.JobResources;

    public class JobResourceResult {
        public AstrologianResources Astrologian { get; set; } = new AstrologianResources();
        public BardResources Bard { get; set; } = new BardResources();
        public BlackMageResources BlackMage { get; set; } = new BlackMageResources();
        public DancerResources Dancer { get; set; } = new DancerResources();
        public DarkKnightResources DarkKnight { get; set; } = new DarkKnightResources();
        public DragoonResources Dragoon { get; set; } = new DragoonResources();
        public GunBreakerResources GunBreaker { get; set; } = new GunBreakerResources();
        public MachinistResources Machinist { get; set; } = new MachinistResources();
        public MonkResources Monk { get; set; } = new MonkResources();
        public NinjaResources Ninja { get; set; } = new NinjaResources();
        public PaladinResources Paladin { get; set; } = new PaladinResources();
        public RedMageResources RedMage { get; set; } = new RedMageResources();
        public SamuraiResources Samurai { get; set; } = new SamuraiResources();
        public ScholarResources Scholar { get; set; } = new ScholarResources();
        public SummonerResources Summoner { get; set; } = new SummonerResources();
        public WarriorResources Warrior { get; set; } = new WarriorResources();
        public WhiteMageResources WhiteMage { get; set; } = new WhiteMageResources();
    }
}