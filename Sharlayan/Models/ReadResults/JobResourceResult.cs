namespace Sharlayan.Models.ReadResults {
    using System;
    using System.Runtime.InteropServices;

    public class JobResourceResult {
        private static JobResourceStruct _resourceStruct;

        internal static JobResourceResult Empty => new JobResourceResult(new JobResourceStruct());

        public PaladinResources Paladin { get; } = new PaladinResources();
        public WarriorResources Warrior { get; } = new WarriorResources();
        public DarkknightResources Darkknight { get; } = new DarkknightResources();
        public GunbreakerResources Gunbreaker { get; } = new GunbreakerResources();
        public MonkResources Monk { get; } = new MonkResources();
        public DragoonResources Dragoon { get; } = new DragoonResources();
        public NinjaResources Ninja { get; } = new NinjaResources();
        public SamuraiResources Samurai { get; } = new SamuraiResources();
        public BardResources Bard { get; } = new BardResources();
        public MachinistResources Machinist { get; } = new MachinistResources();
        public DancerResources Dancer { get; } = new DancerResources();
        public SummonerResources Summoner { get; } = new SummonerResources();
        public BlackMageResources BlackMage { get; } = new BlackMageResources();
        public RedMageResources RedMage { get; } = new RedMageResources();
        public WhiteMageResources WhiteMage { get; } = new WhiteMageResources();
        public ScholarResources Scholar { get; } = new ScholarResources();
        public AstrologianResources Astrologian { get; } = new AstrologianResources();

        internal JobResourceResult(JobResourceStruct resourceStruct) {
            _resourceStruct = resourceStruct;
        }
        
        #region Tank

        public sealed class PaladinResources {
            public int OathGauge => _resourceStruct.Resource08;
        }

        public sealed class WarriorResources {
            public int BeastGauge => _resourceStruct.Resource08;
        }

        public sealed class DarkknightResources {
            public TimeSpan Timer => TimeSpan.FromMilliseconds(_resourceStruct.Timer2);
            public byte BlackBlood => _resourceStruct.Resource08;
            public bool DarkArts => _resourceStruct.Resource0C != 0;
        }

        public sealed class GunbreakerResources {
            public byte Cartridge => _resourceStruct.Resource08;
            public byte ComboStep => _resourceStruct.Resource0C;
        }

        #endregion
        #region MeleeDps

        public sealed class MonkResources {
            public TimeSpan Timer => TimeSpan.FromMilliseconds(_resourceStruct.Timer1);
            public byte GreasedLightning => _resourceStruct.Resource0A;
            public byte Chakra => _resourceStruct.Resource0B;
        }

        public sealed class DragoonResources {
            public TimeSpan Timer => TimeSpan.FromMilliseconds(_resourceStruct.Timer1);
            public DragoonMode Mode => (DragoonMode)_resourceStruct.Resource0A;
            public byte DragonGaze => _resourceStruct.Resource0B;
            public enum DragoonMode : byte { None, Blood, Life }
        }

        public sealed class NinjaResources {
            private static ushort Time => _resourceStruct.Timer1;
            private static byte TimerFlag => _resourceStruct.Resource0A;
            public TimeSpan Timer => TimeSpan.FromMilliseconds(TimerFlag == 1 ? ushort.MaxValue + Time : Time);
            public byte NinkiGauge => _resourceStruct.Resource0B;
        }

        public sealed class SamuraiResources {
            public byte Kenki => _resourceStruct.Resource0B;
            public byte Meditation => _resourceStruct.Resource0C;
            public Iaijutsu Sen => (Iaijutsu)_resourceStruct.Resource0D;
            [Flags] public enum Iaijutsu : byte { Setsu = 1, Getsu = 2, Ka = 4 }
        }

        #endregion
        #region RangedDps

        public sealed class BardResources {
            public TimeSpan Timer => TimeSpan.FromMilliseconds(_resourceStruct.Timer1);
            public byte Repertoire => _resourceStruct.Resource0A;
            public byte SoulVoice => _resourceStruct.Resource0B;
            public BardSong ActiveSong {
                get {
                    var song = (BardSong)_resourceStruct.Resource0C;
                    if (song != BardSong.MagesBallad && song != BardSong.ArmysPaeon && song != BardSong.WanderersMinuet)
                        return BardSong.None;
                    return song;
                }
            }
            public enum BardSong : byte { None, MagesBallad = 5, ArmysPaeon = 10, WanderersMinuet = 15 }
        }

        public sealed class MachinistResources {
            public TimeSpan OverheatTimer => TimeSpan.FromMilliseconds(_resourceStruct.Timer1);
            public TimeSpan SummonTimer => TimeSpan.FromMilliseconds(_resourceStruct.Timer2);
            public byte Heat => _resourceStruct.Resource0C;
            public byte Battery => _resourceStruct.Resource0D;
        }

        public sealed class DancerResources {
            public byte FourFoldFeathers => _resourceStruct.Resource08;
            public byte Esprit => _resourceStruct.Resource09;
            public byte StepIndex => _resourceStruct.Resource0E;

            public DanceStep CurrentStep => Steps[StepIndex];
            
            public DanceStep[] Steps {
                get {
                    var s = _resourceStruct;
                    if (s.Resource0C > 0) return new [] {
                        (DanceStep)s.Resource0A,
                        (DanceStep)s.Resource0B,
                        (DanceStep)s.Resource0C,
                        (DanceStep)s.Resource0D,
                        (DanceStep)0
                    };
                    return new [] {(DanceStep)s.Resource0A, (DanceStep)s.Resource0B, (DanceStep)0};
                }
            }
            public enum DanceStep : byte { Finish, Emboite, Entrechat, Jete, Pirouette }
        }

        #endregion
        #region MagicDps

        public sealed class SummonerResources {
            public TimeSpan Timer => TimeSpan.FromMilliseconds(_resourceStruct.Timer1);
            public AetherFlags Aether => (AetherFlags)_resourceStruct.Resource0C;
            public int AetherFlow => Aether.HasFlag(AetherFlags.AetherFlow1) ? 1 : Aether.HasFlag(AetherFlags.AetherFlow2) ? 2 : 0;
            public int DreadwyrmAether => Aether.HasFlag(AetherFlags.Dreadwyrm1) ? 1 : Aether.HasFlag(AetherFlags.Dreadwyrm2) ? 2 : 0;
            [Flags] public enum AetherFlags : byte { Empty = 0, AetherFlow1 = 1, AetherFlow2 = 2, Dreadwyrm1 = 4, Dreadwyrm2 = 8 }
        }

        public sealed class BlackMageResources {
            public TimeSpan Timer => TimeSpan.FromMilliseconds(_resourceStruct.Timer1);
            private sbyte Stacks => (sbyte)_resourceStruct.Resource0C;
            public byte UmbralHearts => _resourceStruct.Resource0D;
            public byte PolyglotCount => _resourceStruct.Resource0E;
            public bool Enochian => _resourceStruct.Resource0F != 0;
            public int UmbralStacks => Stacks >= 0 ? 0 : Stacks * -1;
            public int AstralStacks => Stacks <= 0 ? 0 : Stacks;
        }

        public sealed class RedMageResources {
            public byte WhiteMana => _resourceStruct.Resource08;
            public byte BlackMana => _resourceStruct.Resource09;
        }

        #endregion
        #region Healer

        public sealed class WhiteMageResources {
            public TimeSpan Timer => TimeSpan.FromMilliseconds(_resourceStruct.Timer1);
            public byte Lily => _resourceStruct.Resource0C;
            public byte BloodLily => _resourceStruct.Resource0D;
        }

        public sealed class ScholarResources {
            public TimeSpan Timer => TimeSpan.FromMilliseconds(_resourceStruct.Timer1);
            public byte Aetherflow => _resourceStruct.Resource0A;
            public byte FaerieGauge => _resourceStruct.Resource0B;
        }

        public sealed class AstrologianResources {
            public TimeSpan Timer => TimeSpan.FromMilliseconds(_resourceStruct.Timer1);
            public AstrologianCard Arcana => (AstrologianCard)_resourceStruct.Resource0C;
            public AstrologianSeal[] Seals => new[] {
                (AstrologianSeal)_resourceStruct.Resource0D,
                (AstrologianSeal)_resourceStruct.Resource0E,
                (AstrologianSeal)_resourceStruct.Resource0F
            };
            public enum AstrologianCard : byte { None, Balance, Bole, Arrow, Spear, Ewer, Spire, LordofCrowns = 112, LadyofCrowns = 128 }
            public enum AstrologianSeal : byte { None, SolarSeal, LunarSeal, CelestialSeal }
        }

        #endregion

        [StructLayout(LayoutKind.Explicit, Size = 16)]
        internal struct JobResourceStruct {
            [FieldOffset(0x08)] public ushort Timer1;
            [FieldOffset(0x0A)] public ushort Timer2;

            [FieldOffset(0x08)] public byte Resource08;
            [FieldOffset(0x09)] public byte Resource09;
            [FieldOffset(0x0A)] public byte Resource0A;
            [FieldOffset(0x0B)] public byte Resource0B;
            [FieldOffset(0x0C)] public byte Resource0C;
            [FieldOffset(0x0D)] public byte Resource0D;
            [FieldOffset(0x0E)] public byte Resource0E;
            [FieldOffset(0x0F)] public byte Resource0F;
        }
    }
}