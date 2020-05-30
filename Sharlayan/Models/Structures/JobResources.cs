namespace Sharlayan.Models.Structures {
    public class JobResources {
        public int SourceSize { get; set; }
        #region Tank

        public PaladinResources Paladin { get; set; } = new PaladinResources();
        public WarriorResources Warrior { get; set; } = new WarriorResources();
        public DarkknightResources Darkknight { get; set; } = new DarkknightResources();
        public GunbreakerResources Gunbreaker { get; set; } = new GunbreakerResources();

        public sealed class PaladinResources {
            public int OathGauge { get; set; }
        }

        public sealed class WarriorResources {
            public int BeastGauge { get; set; }
        }

        public sealed class DarkknightResources {
            public int Timer { get; set; }
            public int BlackBlood { get; set; }
            public int DarkArts { get; set; }
        }

        public sealed class GunbreakerResources {
            public int Cartridge { get; set; }
            public int ComboStep { get; set; }
        }

        #endregion
        #region MeleeDps

        public MonkResources Monk { get; set; } = new MonkResources();
        public DragoonResources Dragoon { get; set; } = new DragoonResources();
        public NinjaResources Ninja { get; set; } = new NinjaResources();
        public SamuraiResources Samurai { get; set; } = new SamuraiResources();

        public sealed class MonkResources {
            public int Timer { get; set; }
            public int GreasedLightning { get; set; }
            public int Chakra { get; set; }
        }

        public sealed class DragoonResources {
            public int Timer { get; set; }
            public int Mode { get; set; }
            public int DragonGaze { get; set; }
        }

        public sealed class NinjaResources {
            public int Timer { get; set; }
            public int TimerFlag { get; set; }
            public int NinkiGauge { get; set; }
        }

        public sealed class SamuraiResources {
            public int Kenki { get; set; }
            public int Meditation { get; set; }
            public int Sen { get; set; }
        }

        #endregion
        #region RangedDps

        public BardResources Bard { get; set; } = new BardResources();
        public MachinistResources Machinist { get; set; } = new MachinistResources();
        public DancerResources Dancer { get; set; } = new DancerResources();

        public sealed class BardResources {
            public int Timer { get; set; }
            public int Repertoire { get; set; }
            public int SoulVoice { get; set; }
            public int ActiveSong { get; set; }
        }

        public sealed class MachinistResources {
            public int OverheatTimer { get; set; }
            public int SummonTimer { get; set; }
            public int Heat { get; set; }
            public int Battery { get; set; }
        }

        public sealed class DancerResources {
            public int FourFoldFeathers { get; set; }
            public int Esprit { get; set; }
            public int StepIndex { get; set; }
            
            public int Step1 { get; set; }
            public int Step2 { get; set; }
            public int Step3 { get; set; }
            public int Step4 { get; set; }
        }

        #endregion
        #region MagicDps

        public SummonerResources Summoner { get; set; } = new SummonerResources();
        public BlackMageResources BlackMage { get; set; } = new BlackMageResources();
        public RedMageResources RedMage { get; set; } = new RedMageResources();

        public sealed class SummonerResources {
            public int Timer { get; set; }
            public int Aether { get; set; }
        }

        public sealed class BlackMageResources {
            public int Timer { get; set; }
            public int Stacks { get; set; }
            public int UmbralHearts { get; set; }
            public int PolyglotCount { get; set; }
            public int Enochian { get; set; }
        }

        public sealed class RedMageResources {
            public int WhiteMana { get; set; }
            public int BlackMana { get; set; }
        }

        #endregion
        #region Healer

        public WhiteMageResources WhiteMage { get; set; } = new WhiteMageResources();
        public ScholarResources Scholar { get; set; } = new ScholarResources();
        public AstrologianResources Astrologian { get; set; } = new AstrologianResources();

        public sealed class WhiteMageResources {
            public int Timer { get; set; }
            public int Lily { get; set; }
            public int BloodLily { get; set; }
        }

        public sealed class ScholarResources {
            public int Timer { get; set; }
            public int Aetherflow { get; set; }
            public int FaerieGauge { get; set; }
        }

        public sealed class AstrologianResources {
            public int Timer { get; set; }
            public int Arcana { get; set; }
            public int Seal1 { get; set; }
            public int Seal2 { get; set; }
            public int Seal3 { get; set; }
        }

        #endregion
    }
}