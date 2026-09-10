// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.JobResource.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.JobResource.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;

    using Sharlayan.Core;
    using Sharlayan.Models.ReadResults;
    using Sharlayan.Models.Structures;

    using Actor = Sharlayan.Core.Enums.Actor;

    public partial class Reader {
        public bool CanGetJobResources() {
            return this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.JOBRESOURCES_KEY);
        }

        public JobResourceResult GetJobResources() {
            return this.GetJobResourcesCore(null);
        }

        // F100: single-job overload for polling consumers — the parameterless variant
        // resolves and allocates all 21 gauges every call even though only the current
        // job's gauge is ever relevant. Pass Actor.Job.Unknown/unmatched to get an
        // empty container.
        public JobResourceResult GetJobResources(Actor.Job job) {
            return this.GetJobResourcesCore(job);
        }

        private JobResourceResult GetJobResourcesCore(Actor.Job? job) {
            JobResourceResult result = new JobResourceResult();

            if (!this.CanGetJobResources() || !this._memoryHandler.IsAttached) {
                return result;
            }

            IntPtr resourcePtr = this._memoryHandler.Scanner.Locations[Signatures.JOBRESOURCES_KEY];
            if (resourcePtr == IntPtr.Zero) {
                return result;
            }

            IntPtr jobResourcesAddress = new IntPtr(this._memoryHandler.GetInt64(resourcePtr));
            if (jobResourcesAddress == IntPtr.Zero) {
                return result;
            }

            byte[] jobResourcesMap = this._memoryHandler.BufferPool.Rent(this._memoryHandler.Structures.JobResources.SourceSize);

            result.JobResourcesContainer = new JobResourcesContainer();

            try {
                this._memoryHandler.GetByteArray(jobResourcesAddress, jobResourcesMap);

                JobResourcesContainer c = result.JobResourcesContainer;
                bool all = job == null;

                // Limited jobs (BLU, BST) are absent by design: FCS' JobGaugeManager has
                // no gauge union member for them because the game gives them none. Their
                // level/EXP still surface through PlayerInfo.

                if (all || job == Actor.Job.AST) c.Astrologian = this._jobResourceResolver.ResolveAstrologianFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.BRD) c.Bard = this._jobResourceResolver.ResolveBardFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.BLM) c.BlackMage = this._jobResourceResolver.ResolveBlackMageFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.DNC) c.Dancer = this._jobResourceResolver.ResolveDancerFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.DRK) c.DarkKnight = this._jobResourceResolver.ResolveDarkKnightFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.DRG) c.Dragoon = this._jobResourceResolver.ResolveDragoonFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.GNB) c.GunBreaker = this._jobResourceResolver.ResolveGunBreakerFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.MCH) c.Machinist = this._jobResourceResolver.ResolveMachinistFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.MNK) c.Monk = this._jobResourceResolver.ResolveMonkFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.NIN) c.Ninja = this._jobResourceResolver.ResolveNinjaFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.PLD) c.Paladin = this._jobResourceResolver.ResolvePaladinFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.RDM) c.RedMage = this._jobResourceResolver.ResolveRedMageFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.SAM) c.Samurai = this._jobResourceResolver.ResolveSamuraiFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.SCH) c.Scholar = this._jobResourceResolver.ResolveScholarFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.SMN) c.Summoner = this._jobResourceResolver.ResolveSummonerFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.WAR) c.Warrior = this._jobResourceResolver.ResolveWarriorFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.WHM) c.WhiteMage = this._jobResourceResolver.ResolveWhiteMageFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.SGE) c.Sage = this._jobResourceResolver.ResolveSageFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.RPR) c.Reaper = this._jobResourceResolver.ResolveReaperFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.VPR) c.Viper = this._jobResourceResolver.ResolveViperFromBytes(jobResourcesMap);
                if (all || job == Actor.Job.PCT) c.Pictomancer = this._jobResourceResolver.ResolvePictomancerFromBytes(jobResourcesMap);
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }
            finally {
                this._memoryHandler.BufferPool.Return(jobResourcesMap);
            }

            return result;
        }
    }
}