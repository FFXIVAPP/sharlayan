// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.JobResource.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
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
        private byte[] _jobResourcesMap;

        public bool CanGetJobResources() {
            return this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.JobResourceKey);
        }

        public JobResourceResult GetJobResources() {
            JobResources structure = this._memoryHandler.Structures.JobResources;
            JobResourceResult result = new JobResourceResult();

            if (!this.CanGetJobResources() || !this._memoryHandler.IsAttached) {
                return result;
            }

            IntPtr resourcePtr = this._memoryHandler.Scanner.Locations[Signatures.JobResourceKey];
            if (resourcePtr == IntPtr.Zero) {
                return result;
            }

            IntPtr jobResourcesAddress = new IntPtr(this._memoryHandler.GetInt64(resourcePtr));
            if (jobResourcesAddress == IntPtr.Zero) {
                return result;
            }

            if (this._jobResourcesMap == null) {
                this._jobResourcesMap = new byte[this._memoryHandler.Structures.JobResources.SourceSize];
            }

            result.JobResourcesContainer = new JobResourcesContainer();

            try {
                this._memoryHandler.GetByteArray(jobResourcesAddress, this._jobResourcesMap);

                result.JobResourcesContainer.Astrologian = this._jobResourceResolver.ResolveAstrologianFromBytes(this._jobResourcesMap);
                result.JobResourcesContainer.Bard = this._jobResourceResolver.ResolveBardFromBytes(this._jobResourcesMap);
                result.JobResourcesContainer.BlackMage = this._jobResourceResolver.ResolveBlackMageFromBytes(this._jobResourcesMap);
                result.JobResourcesContainer.Dancer = this._jobResourceResolver.ResolveDancerFromBytes(this._jobResourcesMap);
                result.JobResourcesContainer.DarkKnight = this._jobResourceResolver.ResolveDarkKnightFromBytes(this._jobResourcesMap);
                result.JobResourcesContainer.Dragoon = this._jobResourceResolver.ResolveDragoonFromBytes(this._jobResourcesMap);
                result.JobResourcesContainer.GunBreaker = this._jobResourceResolver.ResolveGunBreakerFromBytes(this._jobResourcesMap);
                result.JobResourcesContainer.Machinist = this._jobResourceResolver.ResolveMachinistFromBytes(this._jobResourcesMap);
                result.JobResourcesContainer.Monk = this._jobResourceResolver.ResolveMonkFromBytes(this._jobResourcesMap);
                result.JobResourcesContainer.Ninja = this._jobResourceResolver.ResolveNinjaFromBytes(this._jobResourcesMap);
                result.JobResourcesContainer.Paladin = this._jobResourceResolver.ResolvePaladinFromBytes(this._jobResourcesMap);
                result.JobResourcesContainer.RedMage = this._jobResourceResolver.ResolveRedMageFromBytes(this._jobResourcesMap);
                result.JobResourcesContainer.Samurai = this._jobResourceResolver.ResolveSamuraiFromBytes(this._jobResourcesMap);
                result.JobResourcesContainer.Scholar = this._jobResourceResolver.ResolveScholarFromBytes(this._jobResourcesMap);
                result.JobResourcesContainer.Summoner = this._jobResourceResolver.ResolveSummonerFromBytes(this._jobResourcesMap);
                result.JobResourcesContainer.Warrior = this._jobResourceResolver.ResolveWarriorFromBytes(this._jobResourcesMap);
                result.JobResourcesContainer.WhiteMage = this._jobResourceResolver.ResolveWhiteMageFromBytes(this._jobResourcesMap);
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(ex);
            }

            return result;
        }
    }
}