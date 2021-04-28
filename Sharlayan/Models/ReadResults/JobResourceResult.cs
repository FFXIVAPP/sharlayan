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
        internal byte[] Data { get; }
        internal JobResources Offsets => MemoryHandler.Instance.Structures.JobResources;

        public PaladinResources Paladin => new PaladinResources(this);
        public WarriorResources Warrior => new WarriorResources(this);
        public DarkknightResources Darkknight => new DarkknightResources(this);
        public GunbreakerResources Gunbreaker => new GunbreakerResources(this);
        public MonkResources Monk => new MonkResources(this);
        public DragoonResources Dragoon => new DragoonResources(this);
        public NinjaResources Ninja => new NinjaResources(this);
        public SamuraiResources Samurai => new SamuraiResources(this);
        public BardResources Bard => new BardResources(this);
        public MachinistResources Machinist => new MachinistResources(this);
        public DancerResources Dancer => new DancerResources(this);
        public SummonerResources Summoner => new SummonerResources(this);
        public BlackMageResources BlackMage => new BlackMageResources(this);
        public RedMageResources RedMage => new RedMageResources(this);
        public WhiteMageResources WhiteMage => new WhiteMageResources(this);
        public ScholarResources Scholar => new ScholarResources(this);
        public AstrologianResources Astrologian => new AstrologianResources(this);

        internal JobResourceResult(byte[] sourceBytes) {
            Data = sourceBytes ?? new byte[MemoryHandler.Instance.Structures.JobResources.SourceSize];
        }

        #region Tank

        public sealed class PaladinResources {
            public int OathGauge { get; }

            internal PaladinResources(JobResourceResult result) {
                OathGauge = result.Data[result.Offsets.Paladin.OathGauge];
            }
        }

        public sealed class WarriorResources {
            public int BeastGauge { get; }

            internal WarriorResources(JobResourceResult result) {
                BeastGauge = result.Data[result.Offsets.Warrior.BeastGauge];
            }
        }

        public sealed class DarkknightResources {
            public TimeSpan Timer { get; }
            public int BlackBlood { get; }
            public bool DarkArts { get; }

            internal DarkknightResources(JobResourceResult result) {
                Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Darkknight.Timer));
                BlackBlood = result.Data[result.Offsets.Darkknight.BlackBlood];
                DarkArts = result.Data[result.Offsets.Darkknight.DarkArts] != 0;
            }
        }

        public sealed class GunbreakerResources {
            public int Cartridge { get; }
            public int ComboStep { get; }

            internal GunbreakerResources(JobResourceResult result) {
                Cartridge = result.Data[result.Offsets.Gunbreaker.Cartridge];
                ComboStep = result.Data[result.Offsets.Gunbreaker.ComboStep];
            }
        }

        #endregion

        #region MeleeDps

        public sealed class MonkResources {
            public int Chakra { get; }

            internal MonkResources(JobResourceResult result) {
                Chakra = result.Data[result.Offsets.Monk.Chakra];
            }
        }

        public sealed class DragoonResources {
            public TimeSpan Timer { get; }
            public DragoonMode Mode { get; }
            public int DragonGaze { get; }

            internal DragoonResources(JobResourceResult result) {
                Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Dragoon.Timer));
                Mode = (DragoonMode)result.Data[result.Offsets.Dragoon.Mode];
                DragonGaze = result.Data[result.Offsets.Dragoon.DragonGaze];
            }

            public enum DragoonMode : byte {
                None,
                Blood,
                Life
            }
        }

        public sealed class NinjaResources {
            private readonly ushort _time;
            private readonly byte _timerFlag;
            public TimeSpan Timer => TimeSpan.FromMilliseconds(_timerFlag == 1 ? ushort.MaxValue + _time : _time);
            public int NinkiGauge { get; }

            internal NinjaResources(JobResourceResult result) {
                _time = BitConverter.ToUInt16(result.Data, result.Offsets.Ninja.Timer);
                _timerFlag = result.Data[result.Offsets.Ninja.TimerFlag];
                NinkiGauge = result.Data[result.Offsets.Ninja.NinkiGauge];
            }
        }

        public sealed class SamuraiResources {
            public int Kenki { get; }
            public int Meditation { get; }
            public Iaijutsu Sen { get; }

            internal SamuraiResources(JobResourceResult result) {
                Kenki = result.Data[result.Offsets.Samurai.Kenki];
                Meditation = result.Data[result.Offsets.Samurai.Meditation];
                Sen = (Iaijutsu)result.Data[result.Offsets.Samurai.Sen];
            }

            [Flags]
            public enum Iaijutsu : byte {
                Setsu = 1,
                Getsu = 2,
                Ka = 4
            }
        }

        #endregion

        #region RangedDps

        public sealed class BardResources {
            public TimeSpan Timer { get; }
            public int Repertoire { get; }
            public int SoulVoice { get; }
            private readonly byte _song;

            public BardSong ActiveSong {
                get {
                    var song = (BardSong)_song;
                    if (song != BardSong.MagesBallad && song != BardSong.ArmysPaeon && song != BardSong.WanderersMinuet)
                        return BardSong.None;
                    return song;
                }
            }

            internal BardResources(JobResourceResult result) {
                Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Bard.Timer));
                Repertoire = result.Data[result.Offsets.Bard.Repertoire];
                SoulVoice = result.Data[result.Offsets.Bard.SoulVoice];
                _song = result.Data[result.Offsets.Bard.ActiveSong];
            }

            public enum BardSong : byte {
                None,
                MagesBallad = 5,
                ArmysPaeon = 10,
                WanderersMinuet = 15
            }
        }

        public sealed class MachinistResources {
            public TimeSpan OverheatTimer { get; }
            public TimeSpan SummonTimer { get; }
            public int Heat { get; }
            public int Battery { get; }

            internal MachinistResources(JobResourceResult result) {
                OverheatTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Machinist.OverheatTimer));
                SummonTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Machinist.SummonTimer));
                Heat = result.Data[result.Offsets.Machinist.Heat];
                Battery = result.Data[result.Offsets.Machinist.Battery];
            }
        }

        public sealed class DancerResources {
            public int FourFoldFeathers { get; }
            public int Esprit { get; }
            public int StepIndex { get; }
            public DanceStep CurrentStep => Steps[StepIndex];
            public DanceStep[] Steps {
                get {
                    if (_steps[2] > 0)
                        return new[] {
                            _steps[0],
                            _steps[1],
                            _steps[2],
                            _steps[3],
                            (DanceStep)0
                        };
                    return new[] {_steps[0], _steps[1], (DanceStep)0};
                }
            }
            private readonly DanceStep[] _steps;

            internal DancerResources(JobResourceResult result) {
                FourFoldFeathers = result.Data[result.Offsets.Dancer.FourFoldFeathers];
                Esprit = result.Data[result.Offsets.Dancer.Esprit];
                StepIndex = result.Data[result.Offsets.Dancer.StepIndex];
                _steps = new [] {
                    (DanceStep)result.Data[result.Offsets.Dancer.Step1],
                    (DanceStep)result.Data[result.Offsets.Dancer.Step2],
                    (DanceStep)result.Data[result.Offsets.Dancer.Step3],
                    (DanceStep)result.Data[result.Offsets.Dancer.Step4],
                };
            }

            public enum DanceStep : byte {
                Finish,
                Emboite,
                Entrechat,
                Jete,
                Pirouette
            }
        }

        #endregion

        #region MagicDps

        public sealed class SummonerResources {
            public TimeSpan Timer { get; }
            public AetherFlags Aether { get; }
            public int AetherFlow => Aether.HasFlag(AetherFlags.AetherFlow1) ? 1 : Aether.HasFlag(AetherFlags.AetherFlow2) ? 2 : 0;
            public int DreadwyrmAether => Aether.HasFlag(AetherFlags.Dreadwyrm1) ? 1 : Aether.HasFlag(AetherFlags.Dreadwyrm2) ? 2 : 0;

            internal SummonerResources(JobResourceResult result) {
                Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Summoner.Timer));
                Aether = (AetherFlags)result.Data[result.Offsets.Summoner.Aether];
            }

            [Flags]
            public enum AetherFlags : byte {
                Empty = 0,
                AetherFlow1 = 1,
                AetherFlow2 = 2,
                Dreadwyrm1 = 4,
                Dreadwyrm2 = 8
            }
        }

        public sealed class BlackMageResources {
            public TimeSpan Timer { get; }
            public int UmbralHearts { get; }
            public int PolyglotCount { get; }
            public bool Enochian { get; }
            public int UmbralStacks => _stacks >= 0 ? 0 : _stacks * -1;
            public int AstralStacks => _stacks <= 0 ? 0 : _stacks;
            private readonly sbyte _stacks;

            internal BlackMageResources(JobResourceResult result) {
                Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.BlackMage.Timer));
                _stacks = (sbyte)result.Data[result.Offsets.BlackMage.Stacks];
                UmbralHearts = result.Data[result.Offsets.BlackMage.UmbralHearts];
                PolyglotCount = result.Data[result.Offsets.BlackMage.PolyglotCount];
                Enochian = result.Data[result.Offsets.BlackMage.Enochian] != 0;
            }
        }

        public sealed class RedMageResources {
            public int WhiteMana { get; }
            public int BlackMana { get; }

            internal RedMageResources(JobResourceResult result) {
                WhiteMana = result.Data[result.Offsets.RedMage.WhiteMana];
                BlackMana = result.Data[result.Offsets.RedMage.BlackMana];
            }
        }

        #endregion

        #region Healer

        public sealed class WhiteMageResources {
            public TimeSpan Timer { get; }
            public int Lily { get; }
            public int BloodLily { get; }

            internal WhiteMageResources(JobResourceResult result) {
                Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.WhiteMage.Timer));
                Lily = result.Data[result.Offsets.WhiteMage.Lily];
                BloodLily = result.Data[result.Offsets.WhiteMage.BloodLily];
            }
        }

        public sealed class ScholarResources {
            public TimeSpan Timer { get; }
            public int Aetherflow { get; }
            public int FaerieGauge { get; }

            internal ScholarResources(JobResourceResult result) {
                Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Scholar.Timer));
                Aetherflow = result.Data[result.Offsets.Scholar.Aetherflow];
                FaerieGauge = result.Data[result.Offsets.Scholar.FaerieGauge];
            }
        }

        public sealed class AstrologianResources {
            public TimeSpan Timer { get; }
            public AstrologianCard Arcana { get; }
            public AstrologianSeal[] Seals { get; }

            internal AstrologianResources(JobResourceResult result) {
                Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(result.Data, result.Offsets.Astrologian.Timer));
                Arcana = (AstrologianCard)result.Data[result.Offsets.Astrologian.Arcana];
                Seals = new [] {
                    (AstrologianSeal)result.Data[result.Offsets.Astrologian.Seal1],
                    (AstrologianSeal)result.Data[result.Offsets.Astrologian.Seal2],
                    (AstrologianSeal)result.Data[result.Offsets.Astrologian.Seal3]
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
                LadyofCrowns = 128
            }

            public enum AstrologianSeal : byte {
                None,
                SolarSeal,
                LunarSeal,
                CelestialSeal
            }
        }

        #endregion
    }
} 