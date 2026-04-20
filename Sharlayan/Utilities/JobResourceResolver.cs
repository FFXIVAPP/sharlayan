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

            short cards = BitConverter.ToInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Astrologian.Cards);
            resource.DrawnCards = new List<AstrologianCard> {
                (AstrologianCard)(0xF & (cards >> 0)),
                (AstrologianCard)(0xF & (cards >> 4)),
                (AstrologianCard)(0xF & (cards >> 8)),
            };
            resource.CurrentArcana = (AstrologianCard)(0xF & (cards >> 12));
            resource.DrawType = (AstrologianDraw)sourceBytes[this._memoryHandler.Structures.JobResources.Astrologian.CurrentDraw];
            resource.Timer = TimeSpan.Zero;

            return resource;
        }

        internal BardResources ResolveBardFromBytes(byte[] sourceBytes) {
            BardResources resource = new BardResources();

            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Bard.Timer));
            resource.Repertoire = sourceBytes[this._memoryHandler.Structures.JobResources.Bard.Repertoire];
            resource.SoulVoice = sourceBytes[this._memoryHandler.Structures.JobResources.Bard.SoulVoice];
            resource.RadiantFinaleCoda = sourceBytes[this._memoryHandler.Structures.JobResources.Bard.RadiantFinaleCoda];
            SongFlags activeSong = ((SongFlags)sourceBytes[this._memoryHandler.Structures.JobResources.Bard.ActiveSong] & (SongFlags.MagesBallad | SongFlags.ArmysPaeon | SongFlags.WanderersMinuet));
            if (activeSong != resource.ActiveSong) {
                resource.ActiveSong = activeSong;
            }

            return resource;
        }

        internal BlackMageResources ResolveBlackMageFromBytes(byte[] sourceBytes) {
            BlackMageResources resource = new BlackMageResources();

            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.BlackMage.Timer));
            resource.UmbralHearts = sourceBytes[this._memoryHandler.Structures.JobResources.BlackMage.UmbralHearts];
            resource.PolyglotCount = sourceBytes[this._memoryHandler.Structures.JobResources.BlackMage.PolyglotCount];
            sbyte stacks = (sbyte)sourceBytes[this._memoryHandler.Structures.JobResources.BlackMage.Stacks];
            resource.UmbralStacks = stacks >= 0 ? 0 : stacks * -1;
            resource.AstralStacks = stacks <= 0 ? 0 : stacks;
            byte enochianFlags = sourceBytes[this._memoryHandler.Structures.JobResources.BlackMage.Enochian];
            resource.Enochian = (enochianFlags & 1) != 0;
            resource.ParadoxActive = (enochianFlags & 2) != 0;
            resource.AstralSoulStacks = (enochianFlags >> 2) & 7;

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
            resource.ShadowTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.DarkKnight.ShadowTimer));
            resource.DeliriumStep = BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.DarkKnight.DeliriumStep);

            return resource;
        }

        internal DragoonResources ResolveDragoonFromBytes(byte[] sourceBytes) {
            DragoonResources resource = new DragoonResources();

            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Dragoon.Timer));
            resource.Mode = (DragoonMode)sourceBytes[this._memoryHandler.Structures.JobResources.Dragoon.Mode];
            resource.DragonGaze = sourceBytes[this._memoryHandler.Structures.JobResources.Dragoon.DragonGaze];
            resource.FirstmindsFocusCount = sourceBytes[this._memoryHandler.Structures.JobResources.Dragoon.FirstmindsFocusCount];

            return resource;
        }

        internal GunBreakerResources ResolveGunBreakerFromBytes(byte[] sourceBytes) {
            GunBreakerResources resource = new GunBreakerResources();

            resource.Cartridge = sourceBytes[this._memoryHandler.Structures.JobResources.GunBreaker.Cartridge];
            resource.ComboStep = sourceBytes[this._memoryHandler.Structures.JobResources.GunBreaker.ComboStep];
            resource.MaxTimerDuration = TimeSpan.FromMilliseconds(BitConverter.ToInt16(sourceBytes, this._memoryHandler.Structures.JobResources.GunBreaker.MaxTimerDuration));

            return resource;
        }

        internal MachinistResources ResolveMachinistFromBytes(byte[] sourceBytes) {
            MachinistResources resource = new MachinistResources();

            resource.OverheatTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Machinist.OverheatTimer));
            resource.SummonTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Machinist.SummonTimer));
            resource.Heat = sourceBytes[this._memoryHandler.Structures.JobResources.Machinist.Heat];
            resource.Battery = sourceBytes[this._memoryHandler.Structures.JobResources.Machinist.Battery];
            resource.LastSummonBatteryPower = sourceBytes[this._memoryHandler.Structures.JobResources.Machinist.LastSummonBatteryPower];
            resource.TimerActive = sourceBytes[this._memoryHandler.Structures.JobResources.Machinist.TimerActive] != 0;

            return resource;
        }

        internal MonkResources ResolveMonkFromBytes(byte[] sourceBytes) {
            MonkResources resource = new MonkResources();

            resource.Chakra = sourceBytes[this._memoryHandler.Structures.JobResources.Monk.Chakra];
            resource.BeastChakra1 = (BeastChakraType)sourceBytes[this._memoryHandler.Structures.JobResources.Monk.BeastChakra1];
            resource.BeastChakra2 = (BeastChakraType)sourceBytes[this._memoryHandler.Structures.JobResources.Monk.BeastChakra2];
            resource.BeastChakra3 = (BeastChakraType)sourceBytes[this._memoryHandler.Structures.JobResources.Monk.BeastChakra3];
            resource.BeastChakra = new[] { resource.BeastChakra1, resource.BeastChakra2, resource.BeastChakra3 };
            byte beastStacks = sourceBytes[this._memoryHandler.Structures.JobResources.Monk.BeastChakraStacks];
            resource.OpoOpoStacks = beastStacks & 3;
            resource.RaptorStacks = (beastStacks >> 2) & 3;
            resource.CoeurlStacks = (beastStacks >> 4) & 3;
            resource.Nadi = (NadiFlags)sourceBytes[this._memoryHandler.Structures.JobResources.Monk.Nadi];
            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Monk.Timer));

            return resource;
        }

        internal NinjaResources ResolveNinjaFromBytes(byte[] sourceBytes) {
            NinjaResources resource = new NinjaResources();

            resource.NinkiGauge = sourceBytes[this._memoryHandler.Structures.JobResources.Ninja.NinkiGauge];
            resource.Kazematoi = sourceBytes[this._memoryHandler.Structures.JobResources.Ninja.Kazematoi];
            resource.Timer = TimeSpan.Zero;

            return resource;
        }

        internal PaladinResources ResolvePaladinFromBytes(byte[] sourceBytes) {
            PaladinResources resource = new PaladinResources();

            resource.OathGauge = sourceBytes[this._memoryHandler.Structures.JobResources.Paladin.OathGauge];
            resource.ConfiteorComboTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Paladin.ConfiteorComboTimer));
            resource.ConfiteorComboStep = sourceBytes[this._memoryHandler.Structures.JobResources.Paladin.ConfiteorComboStep];

            return resource;
        }

        internal RedMageResources ResolveRedMageFromBytes(byte[] sourceBytes) {
            RedMageResources resource = new RedMageResources();

            resource.WhiteMana = sourceBytes[this._memoryHandler.Structures.JobResources.RedMage.WhiteMana];
            resource.BlackMana = sourceBytes[this._memoryHandler.Structures.JobResources.RedMage.BlackMana];
            resource.ManaStacks = sourceBytes[this._memoryHandler.Structures.JobResources.RedMage.ManaStacks];

            return resource;
        }

        internal SamuraiResources ResolveSamuraiFromBytes(byte[] sourceBytes) {
            SamuraiResources resource = new SamuraiResources();

            resource.Kenki = sourceBytes[this._memoryHandler.Structures.JobResources.Samurai.Kenki];
            resource.Meditation = sourceBytes[this._memoryHandler.Structures.JobResources.Samurai.Meditation];
            resource.Sen = (Iaijutsu)sourceBytes[this._memoryHandler.Structures.JobResources.Samurai.Sen];
            resource.Kaeshi = (KaeshiAction)sourceBytes[this._memoryHandler.Structures.JobResources.Samurai.Kaeshi];

            return resource;
        }

        internal ScholarResources ResolveScholarFromBytes(byte[] sourceBytes) {
            ScholarResources resource = new ScholarResources();

            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Scholar.Timer));
            resource.Aetherflow = sourceBytes[this._memoryHandler.Structures.JobResources.Scholar.Aetherflow];
            resource.FaerieGauge = sourceBytes[this._memoryHandler.Structures.JobResources.Scholar.FaerieGauge];
            resource.DismissedFairy = sourceBytes[this._memoryHandler.Structures.JobResources.Scholar.DismissedFairy] != 0;

            return resource;
        }

        internal SummonerResources ResolveSummonerFromBytes(byte[] sourceBytes) {
            SummonerResources resource = new SummonerResources();

            resource.SummonTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Summoner.SummonTimer));
            resource.AttunementTimer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Summoner.AttunementTimer));
            resource.Aether = (AetherFlags)sourceBytes[this._memoryHandler.Structures.JobResources.Summoner.Aether];
            resource.Attunement = sourceBytes[this._memoryHandler.Structures.JobResources.Summoner.Attunement];
            resource.ReturnSummon = sourceBytes[this._memoryHandler.Structures.JobResources.Summoner.ReturnSummon];
            resource.ReturnSummonGlam = sourceBytes[this._memoryHandler.Structures.JobResources.Summoner.ReturnSummonGlam];

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

            resource.Timer = TimeSpan.FromMilliseconds(BitConverter.ToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Viper.Timer));
            resource.RattlingCoilStacks = sourceBytes[this._memoryHandler.Structures.JobResources.Viper.RattlingCoilStacks];
            resource.SerpentOffering = sourceBytes[this._memoryHandler.Structures.JobResources.Viper.SerpentOffering];
            resource.AnguineTribute = sourceBytes[this._memoryHandler.Structures.JobResources.Viper.AnguineTribute];
            resource.DreadCombo = (DreadCombo)sourceBytes[this._memoryHandler.Structures.JobResources.Viper.DreadCombo];
            resource.SerpentCombo = (SerpentCombo)(sourceBytes[this._memoryHandler.Structures.JobResources.Viper.SerpentComboState] >> 2);

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