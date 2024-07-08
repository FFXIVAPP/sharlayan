// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryLocation.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   MemoryLocation.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Linq;

    using Sharlayan.Models;

    public class MemoryLocation {
        private MemoryHandler _memoryHandler;

        private Signature _signature;

        public MemoryLocation(Signature signature, MemoryHandler memoryHandler) {
            this._signature = signature;
            this._memoryHandler = memoryHandler;
        }

        public static implicit operator IntPtr(MemoryLocation location) {
            return location.GetAddress();
        }

        public IntPtr GetAddress() {
            IntPtr baseAddress;
            bool IsASMSignature = false;
            if (this._signature.SigScanAddress != IntPtr.Zero) {
                baseAddress = this._signature.SigScanAddress; // Scanner should have already applied the base offset
                if (this._signature.ASMSignature) {
                    IsASMSignature = true;
                }
            }
            else {
                if (this._signature.PointerPath == null || !this._signature.PointerPath.Any()) {
                    return IntPtr.Zero;
                }

                baseAddress = this._memoryHandler.GetStaticAddress(0);
            }

            if (this._signature.PointerPath == null || !this._signature.PointerPath.Any()) {
                return baseAddress;
            }

            return this._memoryHandler.ResolvePointerPath(this._signature.PointerPath, baseAddress, IsASMSignature);
        }
    }
}