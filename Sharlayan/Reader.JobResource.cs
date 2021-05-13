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

    public partial class Reader {
        public bool CanGetJobResources() {
            return this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.JobResourceKey);
        }

        public JobResourceResult GetJobResources() {
            byte[] sourceBytes = new byte[this._memoryHandler.Structures.JobResources.SourceSize];
            if (!this.CanGetJobResources() || !this._memoryHandler.IsAttached) {
                return new JobResourceResult {
                    Data = sourceBytes,
                    Offsets = this._memoryHandler.Structures.JobResources,
                };
            }

            IntPtr resourcePtr = this._memoryHandler.Scanner.Locations[Signatures.JobResourceKey];
            if (resourcePtr == IntPtr.Zero) {
                return new JobResourceResult {
                    Data = sourceBytes,
                    Offsets = this._memoryHandler.Structures.JobResources,
                };
            }

            IntPtr resource = new IntPtr(this._memoryHandler.GetInt64(resourcePtr));
            if (resource == IntPtr.Zero) {
                return new JobResourceResult {
                    Data = sourceBytes,
                    Offsets = this._memoryHandler.Structures.JobResources,
                };
            }

            sourceBytes = this._memoryHandler.GetByteArray(resource, this._memoryHandler.Structures.JobResources.SourceSize);
            return new JobResourceResult {
                Data = sourceBytes,
                Offsets = this._memoryHandler.Structures.JobResources,
            };
        }
    }
}