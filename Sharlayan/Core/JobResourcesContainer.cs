// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourcesContainer.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourcesContainer.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core {
    using Sharlayan.Core.JobResources;

    public class JobResourcesContainer {
        public AstrologianResources Astrologian { get; internal set; } = new AstrologianResources();
        public BardResources Bard { get; internal set; } = new BardResources();
        public BlackMageResources BlackMage { get; internal set; } = new BlackMageResources();
        public DancerResources Dancer { get; internal set; } = new DancerResources();
        public DarkKnightResources DarkKnight { get; internal set; } = new DarkKnightResources();
        public DragoonResources Dragoon { get; internal set; } = new DragoonResources();
        public GunBreakerResources GunBreaker { get; internal set; } = new GunBreakerResources();
        public MachinistResources Machinist { get; internal set; } = new MachinistResources();
        public MonkResources Monk { get; internal set; } = new MonkResources();
        public NinjaResources Ninja { get; internal set; } = new NinjaResources();
        public PaladinResources Paladin { get; internal set; } = new PaladinResources();
        public RedMageResources RedMage { get; internal set; } = new RedMageResources();
        public SamuraiResources Samurai { get; internal set; } = new SamuraiResources();
        public ScholarResources Scholar { get; internal set; } = new ScholarResources();
        public SummonerResources Summoner { get; internal set; } = new SummonerResources();
        public WarriorResources Warrior { get; internal set; } = new WarriorResources();
        public WhiteMageResources WhiteMage { get; internal set; } = new WhiteMageResources();
    }
}