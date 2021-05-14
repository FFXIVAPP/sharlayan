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

            byte[] sourceBytes = this._memoryHandler.GetByteArray(resource, this._memoryHandler.Structures.JobResources.SourceSize);

            result.Astrologian = this._jobResourceResolver.ResolveAstrologianFromBytes(sourceBytes);
            result.Bard = this._jobResourceResolver.ResolveBardFromBytes(sourceBytes);
            result.BlackMage = this._jobResourceResolver.ResolveBlackMageFromBytes(sourceBytes);
            result.Dancer = this._jobResourceResolver.ResolveDancerFromBytes(sourceBytes);
            result.DarkKnight = this._jobResourceResolver.ResolveDarkKnightFromBytes(sourceBytes);
            result.Dragoon = this._jobResourceResolver.ResolveDragoonFromBytes(sourceBytes);
            result.GunBreaker = this._jobResourceResolver.ResolveGunBreakerFromBytes(sourceBytes);
            result.Machinist = this._jobResourceResolver.ResolveMachinistFromBytes(sourceBytes);
            result.Monk = this._jobResourceResolver.ResolveMonkFromBytes(sourceBytes);
            result.Ninja = this._jobResourceResolver.ResolveNinjaFromBytes(sourceBytes);
            result.Paladin = this._jobResourceResolver.ResolvePaladinFromBytes(sourceBytes);
            result.RedMage = this._jobResourceResolver.ResolveRedMageFromBytes(sourceBytes);
            result.Samurai = this._jobResourceResolver.ResolveSamuraiFromBytes(sourceBytes);
            result.Scholar = this._jobResourceResolver.ResolveScholarFromBytes(sourceBytes);
            result.Summoner = this._jobResourceResolver.ResolveSummonerFromBytes(sourceBytes);
            result.Warrior = this._jobResourceResolver.ResolveWarriorFromBytes(sourceBytes);
            result.WhiteMage = this._jobResourceResolver.ResolveWhiteMageFromBytes(sourceBytes);

            return result;
        }
    }
}