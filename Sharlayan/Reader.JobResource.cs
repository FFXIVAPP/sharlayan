// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.JobResource.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
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

            IntPtr resource = new IntPtr(this._memoryHandler.GetInt64(resourcePtr));
            if (resource == IntPtr.Zero) {
                return result;
            }

            result.JobResourcesContainer = new JobResourcesContainer();

            try {
                byte[] sourceBytes = this._memoryHandler.GetByteArray(resource, this._memoryHandler.Structures.JobResources.SourceSize);

                result.JobResourcesContainer.Astrologian = this._jobResourceResolver.ResolveAstrologianFromBytes(sourceBytes);
                result.JobResourcesContainer.Bard = this._jobResourceResolver.ResolveBardFromBytes(sourceBytes);
                result.JobResourcesContainer.BlackMage = this._jobResourceResolver.ResolveBlackMageFromBytes(sourceBytes);
                result.JobResourcesContainer.Dancer = this._jobResourceResolver.ResolveDancerFromBytes(sourceBytes);
                result.JobResourcesContainer.DarkKnight = this._jobResourceResolver.ResolveDarkKnightFromBytes(sourceBytes);
                result.JobResourcesContainer.Dragoon = this._jobResourceResolver.ResolveDragoonFromBytes(sourceBytes);
                result.JobResourcesContainer.GunBreaker = this._jobResourceResolver.ResolveGunBreakerFromBytes(sourceBytes);
                result.JobResourcesContainer.Machinist = this._jobResourceResolver.ResolveMachinistFromBytes(sourceBytes);
                result.JobResourcesContainer.Monk = this._jobResourceResolver.ResolveMonkFromBytes(sourceBytes);
                result.JobResourcesContainer.Ninja = this._jobResourceResolver.ResolveNinjaFromBytes(sourceBytes);
                result.JobResourcesContainer.Paladin = this._jobResourceResolver.ResolvePaladinFromBytes(sourceBytes);
                result.JobResourcesContainer.RedMage = this._jobResourceResolver.ResolveRedMageFromBytes(sourceBytes);
                result.JobResourcesContainer.Samurai = this._jobResourceResolver.ResolveSamuraiFromBytes(sourceBytes);
                result.JobResourcesContainer.Scholar = this._jobResourceResolver.ResolveScholarFromBytes(sourceBytes);
                result.JobResourcesContainer.Summoner = this._jobResourceResolver.ResolveSummonerFromBytes(sourceBytes);
                result.JobResourcesContainer.Warrior = this._jobResourceResolver.ResolveWarriorFromBytes(sourceBytes);
                result.JobResourcesContainer.WhiteMage = this._jobResourceResolver.ResolveWhiteMageFromBytes(sourceBytes);
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }

            return result;
        }
    }
}