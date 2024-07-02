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

    public partial class Reader {
        public bool CanGetJobResources() {
            return this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.JOBRESOURCES_KEY);
        }

        public JobResourceResult GetJobResources() {
            JobResources structure = this._memoryHandler.Structures.JobResources;
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

                result.JobResourcesContainer.Astrologian = this._jobResourceResolver.ResolveAstrologianFromBytes(jobResourcesMap);
                result.JobResourcesContainer.Bard = this._jobResourceResolver.ResolveBardFromBytes(jobResourcesMap);
                result.JobResourcesContainer.BlackMage = this._jobResourceResolver.ResolveBlackMageFromBytes(jobResourcesMap);
                result.JobResourcesContainer.Dancer = this._jobResourceResolver.ResolveDancerFromBytes(jobResourcesMap);
                result.JobResourcesContainer.DarkKnight = this._jobResourceResolver.ResolveDarkKnightFromBytes(jobResourcesMap);
                result.JobResourcesContainer.Dragoon = this._jobResourceResolver.ResolveDragoonFromBytes(jobResourcesMap);
                result.JobResourcesContainer.GunBreaker = this._jobResourceResolver.ResolveGunBreakerFromBytes(jobResourcesMap);
                result.JobResourcesContainer.Machinist = this._jobResourceResolver.ResolveMachinistFromBytes(jobResourcesMap);
                result.JobResourcesContainer.Monk = this._jobResourceResolver.ResolveMonkFromBytes(jobResourcesMap);
                result.JobResourcesContainer.Ninja = this._jobResourceResolver.ResolveNinjaFromBytes(jobResourcesMap);
                result.JobResourcesContainer.Paladin = this._jobResourceResolver.ResolvePaladinFromBytes(jobResourcesMap);
                result.JobResourcesContainer.RedMage = this._jobResourceResolver.ResolveRedMageFromBytes(jobResourcesMap);
                result.JobResourcesContainer.Samurai = this._jobResourceResolver.ResolveSamuraiFromBytes(jobResourcesMap);
                result.JobResourcesContainer.Scholar = this._jobResourceResolver.ResolveScholarFromBytes(jobResourcesMap);
                result.JobResourcesContainer.Summoner = this._jobResourceResolver.ResolveSummonerFromBytes(jobResourcesMap);
                result.JobResourcesContainer.Warrior = this._jobResourceResolver.ResolveWarriorFromBytes(jobResourcesMap);
                result.JobResourcesContainer.WhiteMage = this._jobResourceResolver.ResolveWhiteMageFromBytes(jobResourcesMap);
                result.JobResourcesContainer.Sage = this._jobResourceResolver.ResolveSageFromBytes(jobResourcesMap);
                result.JobResourcesContainer.Reaper = this._jobResourceResolver.ResolveReaperFromBytes(jobResourcesMap);
                result.JobResourcesContainer.Viper = this._jobResourceResolver.ResolveViperFromBytes(jobResourcesMap);
                result.JobResourcesContainer.Pictomancer = this._jobResourceResolver.ResolvePictomancerFromBytes(jobResourcesMap);

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