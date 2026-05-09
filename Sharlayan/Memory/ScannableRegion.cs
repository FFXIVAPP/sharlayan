// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScannableRegion.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Platform-neutral memory region descriptor used by Scanner. Replaces direct use of
//   the Windows MEMORY_BASIC_INFORMATION struct outside the Windows-specific accessor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Memory {
    using System;

    /// <summary>
    /// A range of process memory the Scanner may walk while searching for byte signatures.
    /// Returned by <see cref="IProcessMemoryAccessor.EnumerateScannableRegions"/>.
    /// </summary>
    public readonly struct ScannableRegion {
        public ScannableRegion(IntPtr baseAddress, long regionSize) {
            this.BaseAddress = baseAddress;
            this.RegionSize = regionSize;
        }

        /// <summary>Inclusive lower bound of the region in process address space.</summary>
        public IntPtr BaseAddress { get; }

        /// <summary>Total bytes in the region. Region spans <c>[BaseAddress, BaseAddress + RegionSize)</c>.</summary>
        public long RegionSize { get; }
    }
}
