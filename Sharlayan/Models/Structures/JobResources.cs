// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResources.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResources.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public class JobResources {
        public AstrologianResources Astrologian { get; set; } = new AstrologianResources();

        public BardResources Bard { get; set; } = new BardResources();
        public BlackMageResources BlackMage { get; set; } = new BlackMageResources();
        public DancerResources Dancer { get; set; } = new DancerResources();
        public DarkknightResources Darkknight { get; set; } = new DarkknightResources();
        public DragoonResources Dragoon { get; set; } = new DragoonResources();
        public GunbreakerResources Gunbreaker { get; set; } = new GunbreakerResources();
        public MachinistResources Machinist { get; set; } = new MachinistResources();

        public MonkResources Monk { get; set; } = new MonkResources();
        public NinjaResources Ninja { get; set; } = new NinjaResources();

        public PaladinResources Paladin { get; set; } = new PaladinResources();
        public RedMageResources RedMage { get; set; } = new RedMageResources();
        public SamuraiResources Samurai { get; set; } = new SamuraiResources();
        public ScholarResources Scholar { get; set; } = new ScholarResources();
        public int SourceSize { get; set; }

        public SummonerResources Summoner { get; set; } = new SummonerResources();
        public WarriorResources Warrior { get; set; } = new WarriorResources();

        public WhiteMageResources WhiteMage { get; set; } = new WhiteMageResources();

        public sealed class AstrologianResources {
            public int Arcana { get; set; }
            public int Seal1 { get; set; }
            public int Seal2 { get; set; }
            public int Seal3 { get; set; }
            public int Timer { get; set; }
        }

        public sealed class BardResources {
            public int ActiveSong { get; set; }
            public int Repertoire { get; set; }
            public int SoulVoice { get; set; }
            public int Timer { get; set; }
        }

        public sealed class BlackMageResources {
            public int Enochian { get; set; }
            public int PolyglotCount { get; set; }
            public int Stacks { get; set; }
            public int Timer { get; set; }
            public int UmbralHearts { get; set; }
        }

        public sealed class DancerResources {
            public int Esprit { get; set; }
            public int FourFoldFeathers { get; set; }

            public int Step1 { get; set; }
            public int Step2 { get; set; }
            public int Step3 { get; set; }
            public int Step4 { get; set; }
            public int StepIndex { get; set; }
        }

        public sealed class DarkknightResources {
            public int BlackBlood { get; set; }
            public int DarkArts { get; set; }
            public int Timer { get; set; }
        }

        public sealed class DragoonResources {
            public int DragonGaze { get; set; }
            public int Mode { get; set; }
            public int Timer { get; set; }
        }

        public sealed class GunbreakerResources {
            public int Cartridge { get; set; }
            public int ComboStep { get; set; }
        }

        public sealed class MachinistResources {
            public int Battery { get; set; }
            public int Heat { get; set; }
            public int OverheatTimer { get; set; }
            public int SummonTimer { get; set; }
        }

        public sealed class MonkResources {
            public int Chakra { get; set; }
        }

        public sealed class NinjaResources {
            public int NinkiGauge { get; set; }
            public int Timer { get; set; }
            public int TimerFlag { get; set; }
        }

        public sealed class PaladinResources {
            public int OathGauge { get; set; }
        }

        public sealed class RedMageResources {
            public int BlackMana { get; set; }
            public int WhiteMana { get; set; }
        }

        public sealed class SamuraiResources {
            public int Kenki { get; set; }
            public int Meditation { get; set; }
            public int Sen { get; set; }
        }

        public sealed class ScholarResources {
            public int Aetherflow { get; set; }
            public int FaerieGauge { get; set; }
            public int Timer { get; set; }
        }

        public sealed class SummonerResources {
            public int Aether { get; set; }
            public int Timer { get; set; }
        }

        public sealed class WarriorResources {
            public int BeastGauge { get; set; }
        }

        public sealed class WhiteMageResources {
            public int BloodLily { get; set; }
            public int Lily { get; set; }
            public int Timer { get; set; }
        }
    }
}