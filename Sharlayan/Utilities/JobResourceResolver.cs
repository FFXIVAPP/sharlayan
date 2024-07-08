// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResolver.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResolver.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;

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
                (AstrologianSeal)(3 & (sourceBytes[this._memoryHandler.Structures.JobResources.Astrologian.Seals] >> 0)),
                (AstrologianSeal)(3 & (sourceBytes[this._memoryHandler.Structures.JobResources.Astrologian.Seals] >> 2)),
                (AstrologianSeal)(3 & (sourceBytes[this._memoryHandler.Structures.JobResources.Astrologian.Seals] >> 4))
            };

            return resource;
        }

        internal BardResources ResolveBardFromBytes(byte[] sourceBytes) {
            BardResources resource = new BardResources();

            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Bard.Timer));
            resource.Repertoire = sourceBytes[this._memoryHandler.Structures.JobResources.Bard.Repertoire];
            resource.SoulVoice = sourceBytes[this._memoryHandler.Structures.JobResources.Bard.SoulVoice];
            //SongFlags activeSong = (SongFlags) sourceBytes[this._memoryHandler.Structures.JobResources.Bard.ActiveSong];
            SongFlags activeSong = ((SongFlags)sourceBytes[this._memoryHandler.Structures.JobResources.Bard.ActiveSong] & (SongFlags.MagesBallad | SongFlags.ArmysPaeon | SongFlags.WanderersMinuet));
            
            if (activeSong != resource.ActiveSong) {
                resource.ActiveSong = activeSong;
            }

            return resource;
        }

        internal BlackMageResources ResolveBlackMageFromBytes(byte[] sourceBytes) {
            BlackMageResources resource = new BlackMageResources();

            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.BlackMage.Timer));
            resource.AstralTimer = sourceBytes[this._memoryHandler.Structures.JobResources.BlackMage.AstralTimer]; //TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.BlackMage.AstralTimer));
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
            resource.BeastChakra1 = (BeastChakraType)sourceBytes[this._memoryHandler.Structures.JobResources.Monk.BeastChakra1];
            resource.BeastChakra2 = (BeastChakraType)sourceBytes[this._memoryHandler.Structures.JobResources.Monk.BeastChakra2];
            resource.BeastChakra3 = (BeastChakraType)sourceBytes[this._memoryHandler.Structures.JobResources.Monk.BeastChakra3];
            resource.Nadi = (NadiFlags)sourceBytes[this._memoryHandler.Structures.JobResources.Monk.Nadi];
            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Monk.Timer));
            resource.BeastChakra = new[] {resource.BeastChakra1, resource.BeastChakra2, resource.BeastChakra3};

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

            resource.SummonTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Summoner.SummonTimer));
            resource.AttunementTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Summoner.AttunementTimer));
            resource.Aether = (AetherFlags) sourceBytes[this._memoryHandler.Structures.JobResources.Summoner.Aether];
            resource.Attunement = sourceBytes[this._memoryHandler.Structures.JobResources.Summoner.Attunement];

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

        internal SageResources ResolveSageFromBytes(byte[] sourceBytes) {
            SageResources resource = new SageResources();

            resource.AddersgallTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Sage.AddersgallTimer));
            resource.Addersgall = sourceBytes[this._memoryHandler.Structures.JobResources.Sage.Addersgall];
            resource.Addersting = sourceBytes[this._memoryHandler.Structures.JobResources.Sage.Addersting];
            resource.Eukrasia = sourceBytes[this._memoryHandler.Structures.JobResources.Sage.Eukrasia];
            resource.EukrasiaActive = sourceBytes[this._memoryHandler.Structures.JobResources.Sage.Eukrasia] > 0;

            return resource;
        }

        internal ReaperResources ResolveReaperFromBytes(byte[] sourceBytes) {
            ReaperResources resource = new ReaperResources();

            resource.EnshroudedTimeRemaining = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Reaper.EnshroudedTimeRemaining));
            resource.Soul = sourceBytes[this._memoryHandler.Structures.JobResources.Reaper.Soul];
            resource.Shroud = sourceBytes[this._memoryHandler.Structures.JobResources.Reaper.Shroud];
            resource.LemureShroud = sourceBytes[this._memoryHandler.Structures.JobResources.Reaper.LemureShroud];
            resource.VoidShroud = sourceBytes[this._memoryHandler.Structures.JobResources.Reaper.VoidShroud];

            return resource;
        }

        internal ViperResources ResolveViperFromBytes(byte[] sourceBytes) {
            ViperResources resource = new ViperResources();

            resource.RattlingCoilStacks = sourceBytes[this._memoryHandler.Structures.JobResources.Viper.RattlingCoilStacks];
            resource.SerpentOffering = sourceBytes[this._memoryHandler.Structures.JobResources.Viper.SerpentOffering];
            resource.AnguineTribute = sourceBytes[this._memoryHandler.Structures.JobResources.Viper.AnguineTribute];
            resource.DreadCombo = (DreadCombo) sourceBytes[this._memoryHandler.Structures.JobResources.Viper.DreadCombo];


            return resource;
        }

        internal PictomancerResources ResolvePictomancerFromBytes(byte[] sourceBytes) {
            PictomancerResources resource = new PictomancerResources();

            resource.PalleteGauge = sourceBytes[this._memoryHandler.Structures.JobResources.Pictomancer.PalleteGauge];
            resource.WhitePaint = sourceBytes[this._memoryHandler.Structures.JobResources.Pictomancer.WhitePaint];
            resource.CanvasFlags = (CanvasFlags) sourceBytes[this._memoryHandler.Structures.JobResources.Pictomancer.CanvasFlags];
            resource.CreatureFlags = (CreatureFlags) sourceBytes[this._memoryHandler.Structures.JobResources.Pictomancer.CreatureFlags];

            return resource;
        }
    }
}