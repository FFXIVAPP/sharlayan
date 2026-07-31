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

    using NLog;

    using Sharlayan.Core.JobResources;
    using Sharlayan.Core.JobResources.Enums;

    internal class JobResourceResolver {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private MemoryHandler _memoryHandler;

        public JobResourceResolver(MemoryHandler memoryHandler) {
            this._memoryHandler = memoryHandler;
        }

        // F86: every gauge read is total — a bad offset (e.g. after an FCS layout bump)
        // returns 0 for that one field instead of throwing out of the shared try/catch
        // in GetJobResources() and zeroing every gauge resolved after it.
        private static byte ReadByte(byte[] source, int offset) {
            return offset >= 0 && offset < source.Length ? source[offset] : (byte) 0;
        }

        internal AstrologianResources ResolveAstrologianFromBytes(byte[] sourceBytes) {
            AstrologianResources resource = new AstrologianResources();

            short cards = SharlayanBitConverter.TryToInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Astrologian.Cards);
            resource.DrawnCards = new[] {
                (AstrologianCard)(0xF & (cards >> 0)),
                (AstrologianCard)(0xF & (cards >> 4)),
                (AstrologianCard)(0xF & (cards >> 8)),
            };
            resource.CurrentArcana = (AstrologianCard)(0xF & (cards >> 12));
            resource.DrawType = (AstrologianDraw)ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Astrologian.CurrentDraw);
            resource.Timer = TimeSpan.Zero;

            return resource;
        }

        internal BardResources ResolveBardFromBytes(byte[] sourceBytes) {
            BardResources resource = new BardResources();

            resource.Timer = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Bard.Timer));
            resource.Repertoire = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Bard.Repertoire);
            resource.SoulVoice = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Bard.SoulVoice);
            resource.RadiantFinaleCoda = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Bard.RadiantFinaleCoda);
            SongFlags activeSong = ((SongFlags)ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Bard.ActiveSong) & (SongFlags.MagesBallad | SongFlags.ArmysPaeon | SongFlags.WanderersMinuet));
            if (activeSong != resource.ActiveSong) {
                resource.ActiveSong = activeSong;
            }

            return resource;
        }

        internal BlackMageResources ResolveBlackMageFromBytes(byte[] sourceBytes) {
            BlackMageResources resource = new BlackMageResources();

            resource.Timer = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.BlackMage.Timer));
            resource.UmbralHearts = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.BlackMage.UmbralHearts);
            resource.PolyglotCount = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.BlackMage.PolyglotCount);
            sbyte stacks = (sbyte)ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.BlackMage.Stacks);
            resource.UmbralStacks = stacks >= 0 ? 0 : stacks * -1;
            resource.AstralStacks = stacks <= 0 ? 0 : stacks;
            byte enochianFlags = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.BlackMage.Enochian);
            resource.Enochian = (enochianFlags & 1) != 0;
            resource.ParadoxActive = (enochianFlags & 2) != 0;
            resource.AstralSoulStacks = (enochianFlags >> 2) & 7;

            return resource;
        }

        internal DancerResources ResolveDancerFromBytes(byte[] sourceBytes) {
            DancerResources resource = new DancerResources();

            resource.FourFoldFeathers = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Dancer.FourFoldFeathers);
            resource.Esprit = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Dancer.Esprit);
            resource.StepIndex = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Dancer.StepIndex);

            DanceStep step1 = (DanceStep) ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Dancer.Step1);
            DanceStep step2 = (DanceStep) ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Dancer.Step2);
            DanceStep step3 = (DanceStep) ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Dancer.Step3);
            DanceStep step4 = (DanceStep) ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Dancer.Step4);

            resource.Steps = step3 > 0
                                 ? new DanceStep[] { step1, step2, step3, step4, 0 }
                                 : new DanceStep[] { step1, step2, 0 };

            return resource;
        }

        internal DarkKnightResources ResolveDarkKnightFromBytes(byte[] sourceBytes) {
            DarkKnightResources resource = new DarkKnightResources();

            resource.Timer = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.DarkKnight.Timer));
            resource.BlackBlood = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.DarkKnight.BlackBlood);
            resource.DarkArts = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.DarkKnight.DarkArts) != 0;
            resource.ShadowTimer = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.DarkKnight.ShadowTimer));
            resource.DeliriumStep = SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.DarkKnight.DeliriumStep);

            return resource;
        }

        internal DragoonResources ResolveDragoonFromBytes(byte[] sourceBytes) {
            DragoonResources resource = new DragoonResources();

            resource.Timer = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Dragoon.Timer));
            resource.Mode = (DragoonMode)ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Dragoon.Mode);
            resource.DragonGaze = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Dragoon.DragonGaze);
            resource.FirstmindsFocusCount = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Dragoon.FirstmindsFocusCount);

            return resource;
        }

        internal GunBreakerResources ResolveGunBreakerFromBytes(byte[] sourceBytes) {
            GunBreakerResources resource = new GunBreakerResources();

            resource.Cartridge = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.GunBreaker.Cartridge);
            resource.ComboStep = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.GunBreaker.ComboStep);
            resource.MaxTimerDuration = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToInt16(sourceBytes, this._memoryHandler.Structures.JobResources.GunBreaker.MaxTimerDuration));

            return resource;
        }

        internal MachinistResources ResolveMachinistFromBytes(byte[] sourceBytes) {
            MachinistResources resource = new MachinistResources();

            resource.OverheatTimer = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Machinist.OverheatTimer));
            resource.SummonTimer = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Machinist.SummonTimer));
            resource.Heat = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Machinist.Heat);
            resource.Battery = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Machinist.Battery);
            resource.LastSummonBatteryPower = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Machinist.LastSummonBatteryPower);
            resource.TimerActive = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Machinist.TimerActive) != 0;

            return resource;
        }

        internal MonkResources ResolveMonkFromBytes(byte[] sourceBytes) {
            MonkResources resource = new MonkResources();

            resource.Chakra = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Monk.Chakra);
            resource.BeastChakra1 = (BeastChakraType)ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Monk.BeastChakra1);
            resource.BeastChakra2 = (BeastChakraType)ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Monk.BeastChakra2);
            resource.BeastChakra3 = (BeastChakraType)ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Monk.BeastChakra3);
            byte beastStacks = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Monk.BeastChakraStacks);
            resource.OpoOpoStacks = beastStacks & 3;
            resource.RaptorStacks = (beastStacks >> 2) & 3;
            resource.CoeurlStacks = (beastStacks >> 4) & 3;
            resource.Nadi = (NadiFlags)ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Monk.Nadi);
            resource.Timer = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Monk.Timer));

            return resource;
        }

        internal NinjaResources ResolveNinjaFromBytes(byte[] sourceBytes) {
            NinjaResources resource = new NinjaResources();

            resource.NinkiGauge = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Ninja.NinkiGauge);
            resource.Kazematoi = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Ninja.Kazematoi);
            resource.Timer = TimeSpan.Zero;

            return resource;
        }

        internal PaladinResources ResolvePaladinFromBytes(byte[] sourceBytes) {
            PaladinResources resource = new PaladinResources();

            resource.OathGauge = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Paladin.OathGauge);
            resource.ConfiteorComboTimer = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Paladin.ConfiteorComboTimer));
            resource.ConfiteorComboStep = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Paladin.ConfiteorComboStep);

            return resource;
        }

        internal RedMageResources ResolveRedMageFromBytes(byte[] sourceBytes) {
            RedMageResources resource = new RedMageResources();

            resource.WhiteMana = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.RedMage.WhiteMana);
            resource.BlackMana = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.RedMage.BlackMana);
            resource.ManaStacks = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.RedMage.ManaStacks);

            return resource;
        }

        internal SamuraiResources ResolveSamuraiFromBytes(byte[] sourceBytes) {
            SamuraiResources resource = new SamuraiResources();

            resource.Kenki = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Samurai.Kenki);
            resource.Meditation = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Samurai.Meditation);
            resource.Sen = (Iaijutsu)ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Samurai.Sen);
            resource.Kaeshi = (KaeshiAction)ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Samurai.Kaeshi);

            return resource;
        }

        internal ScholarResources ResolveScholarFromBytes(byte[] sourceBytes) {
            ScholarResources resource = new ScholarResources();

            resource.Timer = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Scholar.Timer));
            resource.Aetherflow = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Scholar.Aetherflow);
            resource.FaerieGauge = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Scholar.FaerieGauge);
            resource.DismissedFairy = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Scholar.DismissedFairy) != 0;

            return resource;
        }

        internal SummonerResources ResolveSummonerFromBytes(byte[] sourceBytes) {
            SummonerResources resource = new SummonerResources();

            resource.SummonTimer = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Summoner.SummonTimer));
            resource.AttunementTimer = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Summoner.AttunementTimer));
            resource.Aether = (AetherFlags)ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Summoner.Aether);
            resource.Attunement = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Summoner.Attunement);
            resource.ReturnSummon = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Summoner.ReturnSummon);
            resource.ReturnSummonGlam = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Summoner.ReturnSummonGlam);

            return resource;
        }

        internal WarriorResources ResolveWarriorFromBytes(byte[] sourceBytes) {
            WarriorResources resource = new WarriorResources();

            resource.BeastGauge = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Warrior.BeastGauge);

            return resource;
        }

        internal WhiteMageResources ResolveWhiteMageFromBytes(byte[] sourceBytes) {
            WhiteMageResources resource = new WhiteMageResources();

            resource.Timer = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.WhiteMage.Timer));
            resource.Lily = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.WhiteMage.Lily);
            resource.BloodLily = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.WhiteMage.BloodLily);

            return resource;
        }

        internal SageResources ResolveSageFromBytes(byte[] sourceBytes) {
            SageResources resource = new SageResources();

            resource.AddersgallTimer = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Sage.AddersgallTimer));
            resource.Addersgall = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Sage.Addersgall);
            resource.Addersting = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Sage.Addersting);
            resource.Eukrasia = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Sage.Eukrasia);
            resource.EukrasiaActive = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Sage.Eukrasia) > 0;

            return resource;
        }

        internal ReaperResources ResolveReaperFromBytes(byte[] sourceBytes) {
            ReaperResources resource = new ReaperResources();

            resource.EnshroudedTimeRemaining = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Reaper.EnshroudedTimeRemaining));
            resource.Soul = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Reaper.Soul);
            resource.Shroud = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Reaper.Shroud);
            resource.LemureShroud = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Reaper.LemureShroud);
            resource.VoidShroud = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Reaper.VoidShroud);

            return resource;
        }

        internal ViperResources ResolveViperFromBytes(byte[] sourceBytes) {
            ViperResources resource = new ViperResources();

            resource.Timer = TimeSpan.FromMilliseconds(SharlayanBitConverter.TryToUInt16(sourceBytes, this._memoryHandler.Structures.JobResources.Viper.Timer));
            resource.RattlingCoilStacks = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Viper.RattlingCoilStacks);
            resource.SerpentOffering = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Viper.SerpentOffering);
            resource.AnguineTribute = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Viper.AnguineTribute);
            resource.DreadCombo = (DreadCombo)ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Viper.DreadCombo);
            resource.SerpentCombo = (SerpentCombo)(ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Viper.SerpentComboState) >> 2);

            return resource;
        }

        internal PictomancerResources ResolvePictomancerFromBytes(byte[] sourceBytes) {
            PictomancerResources resource = new PictomancerResources();

            resource.PalleteGauge = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Pictomancer.PalleteGauge);
            resource.WhitePaint = ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Pictomancer.WhitePaint);
            resource.CanvasFlags = (CanvasFlags) ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Pictomancer.CanvasFlags);
            resource.CreatureFlags = (CreatureFlags) ReadByte(sourceBytes, this._memoryHandler.Structures.JobResources.Pictomancer.CreatureFlags);

            return resource;
        }
    }
}