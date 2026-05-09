// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessMemoryAccessorFactory.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Picks the right IProcessMemoryAccessor for the running OS. Phase 9.1.0 only
//   ships a Windows implementation; non-Windows hosts throw a clear PlatformNotSupportedException
//   so the eventual Linux backend slot is obvious.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Memory {
    using System;
    using System.Runtime.InteropServices;

    internal static class ProcessMemoryAccessorFactory {
        public static IProcessMemoryAccessor Create() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return new WindowsProcessMemoryAccessor();
            }

            // Phase 9.1.1 will land LinuxProcessMemoryAccessor here. Until then we
            // surface a clear error rather than silently building a half-working
            // handler — the consumer can catch this and degrade.
            throw new PlatformNotSupportedException(
                "Sharlayan currently supports Windows only. Linux support is planned for 9.1.1 (initial_linuxsupport branch).");
        }
    }
}
