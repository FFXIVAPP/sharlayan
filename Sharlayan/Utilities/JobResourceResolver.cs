// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResolver.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResolver.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System;
    using System.Collections.Generic;

    using NLog;

    using Sharlayan.Core.JobResources;
    using Sharlayan.Core.JobResources.Enums;

    internal class JobResourceResolver {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private MemoryHandler _memoryHandler;

        public JobResourceResolver(MemoryHandler memoryHandler) {
            this._memoryHandler = memoryHandler;
        }

        internal AstrologianResources ResolveAstrologianFromBytes(byte[] sourceBytes) {
            AstrologianResources resource = new AstrologianResources();

            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Astrologian.Timer));
            resource.Arcana = (AstrologianCard) sourceBytes[this._memoryHandler.Structures.JobResources.Astrologian.Arcana];
            resource.Seals = new List<AstrologianSeal> {
                (AstrologianSeal) sourceBytes[this._memoryHandler.Structures.JobResources.Astrologian.Seal1],
                (AstrologianSeal) sourceBytes[this._memoryHandler.Structures.JobResources.Astrologian.Seal2],
                (AstrologianSeal) sourceBytes[this._memoryHandler.Structures.JobResources.Astrologian.Seal3],
            };

            return resource;
        }

        internal BardResources ResolveBardFromBytes(byte[] sourceBytes) {
            BardResources resource = new BardResources();

            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Bard.Timer));
            resource.Repertoire = sourceBytes[this._memoryHandler.Structures.JobResources.Bard.Repertoire];
            resource.SoulVoice = sourceBytes[this._memoryHandler.Structures.JobResources.Bard.SoulVoice];
            BardSong activeSong = (BardSong) sourceBytes[this._memoryHandler.Structures.JobResources.Bard.ActiveSong];
            if (activeSong != BardSong.MagesBallad && activeSong != BardSong.ArmysPaeon && activeSong != BardSong.WanderersMinuet) {
                resource.ActiveSong = activeSong;
            }
            else {
                resource.ActiveSong = BardSong.None;
            }

            return resource;
        }

        internal BlackMageResources ResolveBlackMageFromBytes(byte[] sourceBytes) {
            BlackMageResources resource = new BlackMageResources();

            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.BlackMage.Timer));
            resource.UmbralHearts = sourceBytes[this._memoryHandler.Structures.JobResources.BlackMage.UmbralHearts];
            sbyte stacks = (sbyte) sourceBytes[this._memoryHandler.Structures.JobResources.BlackMage.Stacks];
            resource.UmbralStacks = stacks >= 0
                                        ? 0
                                        : stacks * -1;
            resource.AstralStacks = stacks <= 0
                                        ? 0
                                        : stacks;
            resource.PolyglotCount = sourceBytes[this._memoryHandler.Structures.JobResources.BlackMage.PolyglotCount];
            resource.Enochian = sourceBytes[this._memoryHandler.Structures.JobResources.BlackMage.Enochian] != 0;

            return resource;
        }

        internal DancerResources ResolveDancerFromBytes(byte[] sourceBytes) {
            DancerResources resource = new DancerResources();

            resource.FourFoldFeathers = sourceBytes[this._memoryHandler.Structures.JobResources.Dancer.FourFoldFeathers];
            resource.Esprit = sourceBytes[this._memoryHandler.Structures.JobResources.Dancer.Esprit];
            resource.StepIndex = sourceBytes[this._memoryHandler.Structures.JobResources.Dancer.StepIndex];

            DanceStep[] steps = {
                (DanceStep) sourceBytes[this._memoryHandler.Structures.JobResources.Dancer.Step1],
                (DanceStep) sourceBytes[this._memoryHandler.Structures.JobResources.Dancer.Step2],
                (DanceStep) sourceBytes[this._memoryHandler.Structures.JobResources.Dancer.Step3],
                (DanceStep) sourceBytes[this._memoryHandler.Structures.JobResources.Dancer.Step4],
            };

            resource.Steps = steps[2] > 0
                                 ? new List<DanceStep> {
                                     steps[0],
                                     steps[1],
                                     steps[2],
                                     steps[3],
                                     0,
                                 }
                                 : new List<DanceStep> {
                                     steps[0],
                                     steps[1],
                                     0,
                                 };

            return resource;
        }

        internal DarkKnightResources ResolveDarkKnightFromBytes(byte[] sourceBytes) {
            DarkKnightResources resource = new DarkKnightResources();

            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.DarkKnight.Timer));
            resource.BlackBlood = sourceBytes[this._memoryHandler.Structures.JobResources.DarkKnight.BlackBlood];
            resource.DarkArts = sourceBytes[this._memoryHandler.Structures.JobResources.DarkKnight.DarkArts] != 0;

            return resource;
        }

        internal DragoonResources ResolveDragoonFromBytes(byte[] sourceBytes) {
            DragoonResources resource = new DragoonResources();

            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Dragoon.Timer));
            resource.Mode = (DragoonMode) sourceBytes[this._memoryHandler.Structures.JobResources.Dragoon.Mode];
            resource.DragonGaze = sourceBytes[this._memoryHandler.Structures.JobResources.Dragoon.DragonGaze];

            return resource;
        }

        internal GunBreakerResources ResolveGunBreakerFromBytes(byte[] sourceBytes) {
            GunBreakerResources resource = new GunBreakerResources();

            resource.Cartridge = sourceBytes[this._memoryHandler.Structures.JobResources.GunBreaker.Cartridge];
            resource.ComboStep = sourceBytes[this._memoryHandler.Structures.JobResources.GunBreaker.ComboStep];

            return resource;
        }

        internal MachinistResources ResolveMachinistFromBytes(byte[] sourceBytes) {
            MachinistResources resource = new MachinistResources();

            resource.OverheatTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Machinist.OverheatTimer));
            resource.SummonTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Machinist.SummonTimer));
            resource.Heat = sourceBytes[this._memoryHandler.Structures.JobResources.Machinist.Heat];
            resource.Battery = sourceBytes[this._memoryHandler.Structures.JobResources.Machinist.Battery];

            return resource;
        }

        internal MonkResources ResolveMonkFromBytes(byte[] sourceBytes) {
            MonkResources resource = new MonkResources();

            resource.Chakra = sourceBytes[this._memoryHandler.Structures.JobResources.Monk.Chakra];

            return resource;
        }

        internal NinjaResources ResolveNinjaFromBytes(byte[] sourceBytes) {
            NinjaResources resource = new NinjaResources();

            ushort time = BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Ninja.Timer);
            byte timerFlag = sourceBytes[this._memoryHandler.Structures.JobResources.Ninja.TimerFlag];
            resource.Timer = TimeSpan.FromMilliseconds(
                timerFlag == 1
                    ? ushort.MaxValue + time
                    : time);
            resource.NinkiGauge = sourceBytes[this._memoryHandler.Structures.JobResources.Ninja.NinkiGauge];

            return resource;
        }

        internal PaladinResources ResolvePaladinFromBytes(byte[] sourceBytes) {
            PaladinResources resource = new PaladinResources();

            resource.OathGauge = sourceBytes[this._memoryHandler.Structures.JobResources.Paladin.OathGauge];

            return resource;
        }

        internal RedMageResources ResolveRedMageFromBytes(byte[] sourceBytes) {
            RedMageResources resource = new RedMageResources();

            resource.WhiteMana = sourceBytes[this._memoryHandler.Structures.JobResources.RedMage.WhiteMana];
            resource.BlackMana = sourceBytes[this._memoryHandler.Structures.JobResources.RedMage.BlackMana];

            return resource;
        }

        internal SamuraiResources ResolveSamuraiFromBytes(byte[] sourceBytes) {
            SamuraiResources resource = new SamuraiResources();

            resource.Kenki = sourceBytes[this._memoryHandler.Structures.JobResources.Samurai.Kenki];
            resource.Meditation = sourceBytes[this._memoryHandler.Structures.JobResources.Samurai.Meditation];
            resource.Sen = (Iaijutsu) sourceBytes[this._memoryHandler.Structures.JobResources.Samurai.Sen];

            return resource;
        }

        internal ScholarResources ResolveScholarFromBytes(byte[] sourceBytes) {
            ScholarResources resource = new ScholarResources();

            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Scholar.Timer));
            resource.Aetherflow = sourceBytes[this._memoryHandler.Structures.JobResources.Scholar.Aetherflow];
            resource.FaerieGauge = sourceBytes[this._memoryHandler.Structures.JobResources.Scholar.FaerieGauge];

            return resource;
        }

        internal SummonerResources ResolveSummonerFromBytes(byte[] sourceBytes) {
            SummonerResources resource = new SummonerResources();

            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Summoner.Timer));
            resource.Aether = (AetherFlags) sourceBytes[this._memoryHandler.Structures.JobResources.Summoner.Aether];

            return resource;
        }

        internal WarriorResources ResolveWarriorFromBytes(byte[] sourceBytes) {
            WarriorResources resource = new WarriorResources();

            resource.BeastGauge = sourceBytes[this._memoryHandler.Structures.JobResources.Warrior.BeastGauge];

            return resource;
        }

        internal WhiteMageResources ResolveWhiteMageFromBytes(byte[] sourceBytes) {
            WhiteMageResources resource = new WhiteMageResources();

            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.WhiteMage.Timer));
            resource.Lily = sourceBytes[this._memoryHandler.Structures.JobResources.WhiteMage.Lily];
            resource.BloodLily = sourceBytes[this._memoryHandler.Structures.JobResources.WhiteMage.BloodLily];

            return resource;
        }
    }
}