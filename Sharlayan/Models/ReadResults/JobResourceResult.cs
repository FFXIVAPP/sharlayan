// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

using Sharlayan.Models.Structures;

namespace Sharlayan.Models.ReadResults {
    public class JobResourceResult {
        internal JobResourceResult(byte[] sourceBytes) {
            this.Data = sourceBytes ?? new byte[MemoryHandler.Instance.Structures.JobResources.SourceSize];
        }

        public AstrologianResources Astrologian => new AstrologianResources(this);
        public BardResources Bard => new BardResources(this);
        public BlackMageResources BlackMage => new BlackMageResources(this);
        public DancerResources Dancer => new DancerResources(this);
        public DarkknightResources Darkknight => new DarkknightResources(this);
        public DragoonResources Dragoon => new DragoonResources(this);
        public GunbreakerResources Gunbreaker => new GunbreakerResources(this);
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
        internal byte[] Data { get; }
        internal JobResources Offsets => MemoryHandler.Instance.Structures.JobResources;

        public sealed class AstrologianResources {
            internal AstrologianResources(JobResourceResult result) {
                this.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Astrologian.Timer));
                this.Arcana = (AstrologianCard) result.Data[result.Offsets.Astrologian.Arcana];
                this.Seals = new[] {
                    (AstrologianSeal) result.Data[result.Offsets.Astrologian.Seal1],
                    (AstrologianSeal) result.Data[result.Offsets.Astrologian.Seal2],
                    (AstrologianSeal) result.Data[result.Offsets.Astrologian.Seal3],
                };
            }

            public enum AstrologianCard : byte {
                None,

                Balance,

                Bole,

                Arrow,

                Spear,

                Ewer,

                Spire,

                LordofCrowns = 112,

                LadyofCrowns = 128,
            }

            public enum AstrologianSeal : byte {
                None,

                SolarSeal,

                LunarSeal,

                CelestialSeal,
            }

            public AstrologianCard Arcana { get; }
            public AstrologianSeal[] Seals { get; }
            public TimeSpan Timer { get; }
        }

        public sealed class BardResources {
            private readonly byte _song;

            internal BardResources(JobResourceResult result) {
                this.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Bard.Timer));
                this.Repertoire = result.Data[result.Offsets.Bard.Repertoire];
                this.SoulVoice = result.Data[result.Offsets.Bard.SoulVoice];
                this._song = result.Data[result.Offsets.Bard.ActiveSong];
            }

            public enum BardSong : byte {
                None,

                MagesBallad = 5,

                ArmysPaeon = 10,

                WanderersMinuet = 15,
            }

            public BardSong ActiveSong {
                get {
                    var song = (BardSong) this._song;
                    if (song != BardSong.MagesBallad && song != BardSong.ArmysPaeon && song != BardSong.WanderersMinuet) {
                        return BardSong.None;
                    }

                    return song;
                }
            }

            public int Repertoire { get; }
            public int SoulVoice { get; }
            public TimeSpan Timer { get; }
        }

        public sealed class BlackMageResources {
            private readonly sbyte _stacks;

            internal BlackMageResources(JobResourceResult result) {
                this.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.BlackMage.Timer));
                this._stacks = (sbyte) result.Data[result.Offsets.BlackMage.Stacks];
                this.UmbralHearts = result.Data[result.Offsets.BlackMage.UmbralHearts];
                this.PolyglotCount = result.Data[result.Offsets.BlackMage.PolyglotCount];
                this.Enochian = result.Data[result.Offsets.BlackMage.Enochian] != 0;
            }

            public int AstralStacks =>
                this._stacks <= 0
                    ? 0
                    : this._stacks;

            public bool Enochian { get; }
            public int PolyglotCount { get; }
            public TimeSpan Timer { get; }
            public int UmbralHearts { get; }

            public int UmbralStacks =>
                this._stacks >= 0
                    ? 0
                    : this._stacks * -1;
        }

        public sealed class DancerResources {
            private readonly DanceStep[] _steps;

            internal DancerResources(JobResourceResult result) {
                this.FourFoldFeathers = result.Data[result.Offsets.Dancer.FourFoldFeathers];
                this.Esprit = result.Data[result.Offsets.Dancer.Esprit];
                this.StepIndex = result.Data[result.Offsets.Dancer.StepIndex];
                this._steps = new[] {
                    (DanceStep) result.Data[result.Offsets.Dancer.Step1],
                    (DanceStep) result.Data[result.Offsets.Dancer.Step2],
                    (DanceStep) result.Data[result.Offsets.Dancer.Step3],
                    (DanceStep) result.Data[result.Offsets.Dancer.Step4],
                };
            }

            public enum DanceStep : byte {
                Finish,

                Emboite,

                Entrechat,

                Jete,

                Pirouette,
            }

            public DanceStep CurrentStep => this.Steps[this.StepIndex];
            public int Esprit { get; }
            public int FourFoldFeathers { get; }
            public int StepIndex { get; }

            public DanceStep[] Steps {
                get {
                    if (this._steps[2] > 0) {
                        return new[] {
                            this._steps[0],
                            this._steps[1],
                            this._steps[2],
                            this._steps[3],
                            (DanceStep) 0,
                        };
                    }

                    return new[] {
                        this._steps[0],
                        this._steps[1],
                        (DanceStep) 0,
                    };
                }
            }
        }

        public sealed class DarkknightResources {
            internal DarkknightResources(JobResourceResult result) {
                this.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Darkknight.Timer));
                this.BlackBlood = result.Data[result.Offsets.Darkknight.BlackBlood];
                this.DarkArts = result.Data[result.Offsets.Darkknight.DarkArts] != 0;
            }

            public int BlackBlood { get; }
            public bool DarkArts { get; }
            public TimeSpan Timer { get; }
        }

        public sealed class DragoonResources {
            internal DragoonResources(JobResourceResult result) {
                this.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Dragoon.Timer));
                this.Mode = (DragoonMode) result.Data[result.Offsets.Dragoon.Mode];
                this.DragonGaze = result.Data[result.Offsets.Dragoon.DragonGaze];
            }

            public enum DragoonMode : byte {
                None,

                Blood,

                Life,
            }

            public int DragonGaze { get; }
            public DragoonMode Mode { get; }
            public TimeSpan Timer { get; }
        }

        public sealed class GunbreakerResources {
            internal GunbreakerResources(JobResourceResult result) {
                this.Cartridge = result.Data[result.Offsets.Gunbreaker.Cartridge];
                this.ComboStep = result.Data[result.Offsets.Gunbreaker.ComboStep];
            }

            public int Cartridge { get; }
            public int ComboStep { get; }
        }

        public sealed class MachinistResources {
            internal MachinistResources(JobResourceResult result) {
                this.OverheatTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Machinist.OverheatTimer));
                this.SummonTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Machinist.SummonTimer));
                this.Heat = result.Data[result.Offsets.Machinist.Heat];
                this.Battery = result.Data[result.Offsets.Machinist.Battery];
            }

            public int Battery { get; }
            public int Heat { get; }
            public TimeSpan OverheatTimer { get; }
            public TimeSpan SummonTimer { get; }
        }

        public sealed class MonkResources {
            internal MonkResources(JobResourceResult result) {
                this.Chakra = result.Data[result.Offsets.Monk.Chakra];
            }

            public int Chakra { get; }
        }

        public sealed class NinjaResources {
            private readonly ushort _time;

            private readonly byte _timerFlag;

            internal NinjaResources(JobResourceResult result) {
                this._time = BitConverter.ToUInt16(result.Data, result.Offsets.Ninja.Timer);
                this._timerFlag = result.Data[result.Offsets.Ninja.TimerFlag];
                this.NinkiGauge = result.Data[result.Offsets.Ninja.NinkiGauge];
            }

            public int NinkiGauge { get; }

            public TimeSpan Timer =>
                TimeSpan.FromMilliseconds(
                    this._timerFlag == 1
                        ? ushort.MaxValue + this._time
                        : this._time);
        }

        public sealed class PaladinResources {
            internal PaladinResources(JobResourceResult result) {
                this.OathGauge = result.Data[result.Offsets.Paladin.OathGauge];
            }

            public int OathGauge { get; }
        }

        public sealed class RedMageResources {
            internal RedMageResources(JobResourceResult result) {
                this.WhiteMana = result.Data[result.Offsets.RedMage.WhiteMana];
                this.BlackMana = result.Data[result.Offsets.RedMage.BlackMana];
            }

            public int BlackMana { get; }
            public int WhiteMana { get; }
        }

        public sealed class SamuraiResources {
            internal SamuraiResources(JobResourceResult result) {
                this.Kenki = result.Data[result.Offsets.Samurai.Kenki];
                this.Meditation = result.Data[result.Offsets.Samurai.Meditation];
                this.Sen = (Iaijutsu) result.Data[result.Offsets.Samurai.Sen];
            }

            [Flags]
            public enum Iaijutsu : byte {
                Setsu = 1,

                Getsu = 2,

                Ka = 4,
            }

            public int Kenki { get; }
            public int Meditation { get; }
            public Iaijutsu Sen { get; }
        }

        public sealed class ScholarResources {
            internal ScholarResources(JobResourceResult result) {
                this.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Scholar.Timer));
                this.Aetherflow = result.Data[result.Offsets.Scholar.Aetherflow];
                this.FaerieGauge = result.Data[result.Offsets.Scholar.FaerieGauge];
            }

            public int Aetherflow { get; }
            public int FaerieGauge { get; }
            public TimeSpan Timer { get; }
        }

        public sealed class SummonerResources {
            internal SummonerResources(JobResourceResult result) {
                this.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Summoner.Timer));
                this.Aether = (AetherFlags) result.Data[result.Offsets.Summoner.Aether];
            }

            [Flags]
            public enum AetherFlags : byte {
                Empty = 0,

                AetherFlow1 = 1,

                AetherFlow2 = 2,

                Dreadwyrm1 = 4,

                Dreadwyrm2 = 8,
            }

            public AetherFlags Aether { get; }

            public int AetherFlow =>
                this.Aether.HasFlag(AetherFlags.AetherFlow1)
                    ? 1
                    : this.Aether.HasFlag(AetherFlags.AetherFlow2)
                        ? 2
                        : 0;

            public int DreadwyrmAether =>
                this.Aether.HasFlag(AetherFlags.Dreadwyrm1)
                    ? 1
                    : this.Aether.HasFlag(AetherFlags.Dreadwyrm2)
                        ? 2
                        : 0;

            public TimeSpan Timer { get; }
        }

        public sealed class WarriorResources {
            internal WarriorResources(JobResourceResult result) {
                this.BeastGauge = result.Data[result.Offsets.Warrior.BeastGauge];
            }

            public int BeastGauge { get; }
        }

        public sealed class WhiteMageResources {
            internal WhiteMageResources(JobResourceResult result) {
                this.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.WhiteMage.Timer));
                this.Lily = result.Data[result.Offsets.WhiteMage.Lily];
                this.BloodLily = result.Data[result.Offsets.WhiteMage.BloodLily];
            }

            public int BloodLily { get; }
            public int Lily { get; }
            public TimeSpan Timer { get; }
        }
    }
}