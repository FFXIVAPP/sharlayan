// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourcesMapper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Maps each Sharlayan JobResources.<Job>Resources class against its FFXIVClientStructs
//   Client::Game::Gauge::<Job>Gauge. All 21 jobs in one file to keep related mappings
//   together — each gauge is a small, contained struct.
//
//   Offsets are relative to the gauge struct (not JobGaugeManager) because Sharlayan's
//   resolver reads from the gauge pointer directly. JobGaugeManager places all gauges at
//   offset 0x08 via CExporterUnion, so the native field offsets on <Job>Gauge line up
//   with what Sharlayan's resolver expects.
//
//   JobResources.SourceSize reports sizeof(JobGaugeManager) = 0x60, the container size
//   the resolver reads into.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game;
    using FFXIVClientStructs.FFXIV.Client.Game.Gauge;

    using Sharlayan.Models.Structures;

    internal static class JobResourcesMapper {
        public static JobResources Build() {
            return new JobResources {
                SourceSize = Marshal.SizeOf<JobGaugeManager>(),
                Astrologian = BuildAstrologian(),
                Bard = BuildBard(),
                BlackMage = BuildBlackMage(),
                Dancer = BuildDancer(),
                DarkKnight = BuildDarkKnight(),
                Dragoon = BuildDragoon(),
                GunBreaker = BuildGunBreaker(),
                Machinist = BuildMachinist(),
                Monk = BuildMonk(),
                Ninja = BuildNinja(),
                Paladin = BuildPaladin(),
                Pictomancer = BuildPictomancer(),
                Reaper = BuildReaper(),
                RedMage = BuildRedMage(),
                Sage = BuildSage(),
                Samurai = BuildSamurai(),
                Scholar = BuildScholar(),
                Summoner = BuildSummoner(),
                Viper = BuildViper(),
                Warrior = BuildWarrior(),
                WhiteMage = BuildWhiteMage(),
            };
        }

        private static JobResources.AstrologianResources BuildAstrologian() {
            return new JobResources.AstrologianResources {
                Cards = (int)Marshal.OffsetOf<AstrologianGauge>(nameof(AstrologianGauge.Cards)),
                CurrentDraw = (int)Marshal.OffsetOf<AstrologianGauge>(nameof(AstrologianGauge.CurrentDraw)),
            };
        }

        private static JobResources.BardResources BuildBard() {
            return new JobResources.BardResources {
                ActiveSong = (byte)Marshal.OffsetOf<BardGauge>(nameof(BardGauge.SongFlags)),
                Repertoire = (int)Marshal.OffsetOf<BardGauge>(nameof(BardGauge.Repertoire)),
                SoulVoice = (int)Marshal.OffsetOf<BardGauge>(nameof(BardGauge.SoulVoice)),
                RadiantFinaleCoda = (int)Marshal.OffsetOf<BardGauge>(nameof(BardGauge.RadiantFinaleCoda)),
                Timer = (int)Marshal.OffsetOf<BardGauge>(nameof(BardGauge.SongTimer)),
            };
        }

        private static JobResources.BlackMageResources BuildBlackMage() {
            return new JobResources.BlackMageResources {
                Enochian = (int)Marshal.OffsetOf<BlackMageGauge>(nameof(BlackMageGauge.EnochianFlags)),
                PolyglotCount = (int)Marshal.OffsetOf<BlackMageGauge>(nameof(BlackMageGauge.PolyglotStacks)),
                Stacks = (int)Marshal.OffsetOf<BlackMageGauge>(nameof(BlackMageGauge.ElementStance)),
                Timer = (int)Marshal.OffsetOf<BlackMageGauge>(nameof(BlackMageGauge.EnochianTimer)),
                UmbralHearts = (int)Marshal.OffsetOf<BlackMageGauge>(nameof(BlackMageGauge.UmbralHearts)),
                // AstralTimer: BlackMageGauge doesn't carry a separate astral timer; the
                // Enochian timer covers both stances. Left unmapped.
            };
        }

        private static JobResources.DancerResources BuildDancer() {
            // Dance step offsets come from the internal _danceSteps FixedSizeArray at 0x0A.
            int danceStepsBase = FieldOffsetReader.OffsetOf<DancerGauge>("_danceSteps");
            return new JobResources.DancerResources {
                Esprit = (int)Marshal.OffsetOf<DancerGauge>(nameof(DancerGauge.Esprit)),
                FourFoldFeathers = (int)Marshal.OffsetOf<DancerGauge>(nameof(DancerGauge.Feathers)),
                Step1 = danceStepsBase + 0,
                Step2 = danceStepsBase + 1,
                Step3 = danceStepsBase + 2,
                Step4 = danceStepsBase + 3,
                StepIndex = (int)Marshal.OffsetOf<DancerGauge>(nameof(DancerGauge.StepIndex)),
            };
        }

        private static JobResources.DarkKnightResources BuildDarkKnight() {
            return new JobResources.DarkKnightResources {
                BlackBlood = (int)Marshal.OffsetOf<DarkKnightGauge>(nameof(DarkKnightGauge.Blood)),
                DarkArts = (int)Marshal.OffsetOf<DarkKnightGauge>(nameof(DarkKnightGauge.DarkArtsState)),
                Timer = (int)Marshal.OffsetOf<DarkKnightGauge>(nameof(DarkKnightGauge.DarksideTimer)),
                ShadowTimer = (int)Marshal.OffsetOf<DarkKnightGauge>(nameof(DarkKnightGauge.ShadowTimer)),
                DeliriumStep = (int)Marshal.OffsetOf<DarkKnightGauge>(nameof(DarkKnightGauge.DeliriumStep)),
            };
        }

        private static JobResources.DragoonResources BuildDragoon() {
            return new JobResources.DragoonResources {
                DragonGaze = (int)Marshal.OffsetOf<DragoonGauge>(nameof(DragoonGauge.EyeCount)),
                Mode = (int)Marshal.OffsetOf<DragoonGauge>(nameof(DragoonGauge.LotdState)),
                FirstmindsFocusCount = (int)Marshal.OffsetOf<DragoonGauge>(nameof(DragoonGauge.FirstmindsFocusCount)),
                Timer = (int)Marshal.OffsetOf<DragoonGauge>(nameof(DragoonGauge.LotdTimer)),
            };
        }

        private static JobResources.GunBreakerResources BuildGunBreaker() {
            return new JobResources.GunBreakerResources {
                Cartridge = (int)Marshal.OffsetOf<GunbreakerGauge>(nameof(GunbreakerGauge.Ammo)),
                ComboStep = (int)Marshal.OffsetOf<GunbreakerGauge>(nameof(GunbreakerGauge.AmmoComboStep)),
                MaxTimerDuration = (int)Marshal.OffsetOf<GunbreakerGauge>(nameof(GunbreakerGauge.MaxTimerDuration)),
            };
        }

        private static JobResources.MachinistResources BuildMachinist() {
            return new JobResources.MachinistResources {
                Battery = (int)Marshal.OffsetOf<MachinistGauge>(nameof(MachinistGauge.Battery)),
                Heat = (int)Marshal.OffsetOf<MachinistGauge>(nameof(MachinistGauge.Heat)),
                LastSummonBatteryPower = (int)Marshal.OffsetOf<MachinistGauge>(nameof(MachinistGauge.LastSummonBatteryPower)),
                TimerActive = (int)Marshal.OffsetOf<MachinistGauge>(nameof(MachinistGauge.TimerActive)),
                OverheatTimer = (int)Marshal.OffsetOf<MachinistGauge>(nameof(MachinistGauge.OverheatTimeRemaining)),
                SummonTimer = (int)Marshal.OffsetOf<MachinistGauge>(nameof(MachinistGauge.SummonTimeRemaining)),
            };
        }

        private static JobResources.MonkResources BuildMonk() {
            return new JobResources.MonkResources {
                Chakra = (int)Marshal.OffsetOf<MonkGauge>(nameof(MonkGauge.Chakra)),
                BeastChakra1 = (int)Marshal.OffsetOf<MonkGauge>(nameof(MonkGauge.BeastChakra1)),
                BeastChakra2 = (int)Marshal.OffsetOf<MonkGauge>(nameof(MonkGauge.BeastChakra2)),
                BeastChakra3 = (int)Marshal.OffsetOf<MonkGauge>(nameof(MonkGauge.BeastChakra3)),
                BeastChakraStacks = (int)Marshal.OffsetOf<MonkGauge>(nameof(MonkGauge.BeastChakraStacks)),
                Nadi = (int)Marshal.OffsetOf<MonkGauge>(nameof(MonkGauge.Nadi)),
                Timer = (int)Marshal.OffsetOf<MonkGauge>(nameof(MonkGauge.BlitzTimeRemaining)),
            };
        }

        private static JobResources.NinjaResources BuildNinja() {
            return new JobResources.NinjaResources {
                NinkiGauge = (int)Marshal.OffsetOf<NinjaGauge>(nameof(NinjaGauge.Ninki)),
                Kazematoi = (int)Marshal.OffsetOf<NinjaGauge>(nameof(NinjaGauge.Kazematoi)),
            };
        }

        private static JobResources.PaladinResources BuildPaladin() {
            return new JobResources.PaladinResources {
                OathGauge = (int)Marshal.OffsetOf<PaladinGauge>(nameof(PaladinGauge.OathGauge)),
                ConfiteorComboTimer = (int)Marshal.OffsetOf<PaladinGauge>(nameof(PaladinGauge.ConfiteorComboTimer)),
                ConfiteorComboStep = (int)Marshal.OffsetOf<PaladinGauge>(nameof(PaladinGauge.ConfiteorComboStep)),
            };
        }

        private static JobResources.PictomancerResources BuildPictomancer() {
            return new JobResources.PictomancerResources {
                PalleteGauge = (int)Marshal.OffsetOf<PictomancerGauge>(nameof(PictomancerGauge.PalleteGauge)),
                WhitePaint = (int)Marshal.OffsetOf<PictomancerGauge>(nameof(PictomancerGauge.Paint)),
                CanvasFlags = (int)Marshal.OffsetOf<PictomancerGauge>(nameof(PictomancerGauge.CanvasFlags)),
                CreatureFlags = (int)Marshal.OffsetOf<PictomancerGauge>(nameof(PictomancerGauge.CreatureFlags)),
            };
        }

        private static JobResources.ReaperResources BuildReaper() {
            return new JobResources.ReaperResources {
                Soul = (int)Marshal.OffsetOf<ReaperGauge>(nameof(ReaperGauge.Soul)),
                Shroud = (int)Marshal.OffsetOf<ReaperGauge>(nameof(ReaperGauge.Shroud)),
                EnshroudedTimeRemaining = (int)Marshal.OffsetOf<ReaperGauge>(nameof(ReaperGauge.EnshroudedTimeRemaining)),
                LemureShroud = (int)Marshal.OffsetOf<ReaperGauge>(nameof(ReaperGauge.LemureShroud)),
                VoidShroud = (int)Marshal.OffsetOf<ReaperGauge>(nameof(ReaperGauge.VoidShroud)),
            };
        }

        private static JobResources.RedMageResources BuildRedMage() {
            return new JobResources.RedMageResources {
                BlackMana = (int)Marshal.OffsetOf<RedMageGauge>(nameof(RedMageGauge.BlackMana)),
                WhiteMana = (int)Marshal.OffsetOf<RedMageGauge>(nameof(RedMageGauge.WhiteMana)),
                ManaStacks = (int)Marshal.OffsetOf<RedMageGauge>(nameof(RedMageGauge.ManaStacks)),
            };
        }

        private static JobResources.SageResources BuildSage() {
            return new JobResources.SageResources {
                AddersgallTimer = (int)Marshal.OffsetOf<SageGauge>(nameof(SageGauge.AddersgallTimer)),
                Addersgall = (int)Marshal.OffsetOf<SageGauge>(nameof(SageGauge.Addersgall)),
                Addersting = (int)Marshal.OffsetOf<SageGauge>(nameof(SageGauge.Addersting)),
                Eukrasia = (int)Marshal.OffsetOf<SageGauge>(nameof(SageGauge.Eukrasia)),
            };
        }

        private static JobResources.SamuraiResources BuildSamurai() {
            return new JobResources.SamuraiResources {
                Kenki = (int)Marshal.OffsetOf<SamuraiGauge>(nameof(SamuraiGauge.Kenki)),
                Meditation = (int)Marshal.OffsetOf<SamuraiGauge>(nameof(SamuraiGauge.MeditationStacks)),
                Sen = (int)Marshal.OffsetOf<SamuraiGauge>(nameof(SamuraiGauge.SenFlags)),
                Kaeshi = (int)Marshal.OffsetOf<SamuraiGauge>(nameof(SamuraiGauge.Kaeshi)),
            };
        }

        private static JobResources.ScholarResources BuildScholar() {
            return new JobResources.ScholarResources {
                Aetherflow = (int)Marshal.OffsetOf<ScholarGauge>(nameof(ScholarGauge.Aetherflow)),
                FaerieGauge = (int)Marshal.OffsetOf<ScholarGauge>(nameof(ScholarGauge.FairyGauge)),
                DismissedFairy = (int)Marshal.OffsetOf<ScholarGauge>(nameof(ScholarGauge.DismissedFairy)),
                Timer = (int)Marshal.OffsetOf<ScholarGauge>(nameof(ScholarGauge.SeraphTimer)),
            };
        }

        private static JobResources.SummonerResources BuildSummoner() {
            return new JobResources.SummonerResources {
                Aether = (int)Marshal.OffsetOf<SummonerGauge>(nameof(SummonerGauge.AetherFlags)),
                Attunement = (int)Marshal.OffsetOf<SummonerGauge>(nameof(SummonerGauge.Attunement)),
                ReturnSummon = (int)Marshal.OffsetOf<SummonerGauge>(nameof(SummonerGauge.ReturnSummon)),
                ReturnSummonGlam = (int)Marshal.OffsetOf<SummonerGauge>(nameof(SummonerGauge.ReturnSummonGlam)),
                AttunementTimer = (int)Marshal.OffsetOf<SummonerGauge>(nameof(SummonerGauge.AttunementTimer)),
                SummonTimer = (int)Marshal.OffsetOf<SummonerGauge>(nameof(SummonerGauge.SummonTimer)),
            };
        }

        private static JobResources.ViperResources BuildViper() {
            return new JobResources.ViperResources {
                Timer = (int)Marshal.OffsetOf<ViperGauge>(nameof(ViperGauge.ReawakenedTimer)),
                RattlingCoilStacks = (int)Marshal.OffsetOf<ViperGauge>(nameof(ViperGauge.RattlingCoilStacks)),
                SerpentOffering = (int)Marshal.OffsetOf<ViperGauge>(nameof(ViperGauge.SerpentOffering)),
                AnguineTribute = (int)Marshal.OffsetOf<ViperGauge>(nameof(ViperGauge.AnguineTribute)),
                DreadCombo = (int)Marshal.OffsetOf<ViperGauge>(nameof(ViperGauge.DreadCombo)),
                SerpentComboState = (int)Marshal.OffsetOf<ViperGauge>(nameof(ViperGauge.SerpentComboState)),
            };
        }

        private static JobResources.WarriorResources BuildWarrior() {
            return new JobResources.WarriorResources {
                BeastGauge = (int)Marshal.OffsetOf<WarriorGauge>(nameof(WarriorGauge.BeastGauge)),
            };
        }

        private static JobResources.WhiteMageResources BuildWhiteMage() {
            return new JobResources.WhiteMageResources {
                BloodLily = (int)Marshal.OffsetOf<WhiteMageGauge>(nameof(WhiteMageGauge.BloodLily)),
                Lily = (int)Marshal.OffsetOf<WhiteMageGauge>(nameof(WhiteMageGauge.Lily)),
                Timer = (int)Marshal.OffsetOf<WhiteMageGauge>(nameof(WhiteMageGauge.LilyTimer)),
            };
        }
    }
}
