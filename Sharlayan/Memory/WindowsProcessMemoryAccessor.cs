// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowsProcessMemoryAccessor.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Windows IProcessMemoryAccessor implementation. Wraps the existing kernel32
//   P/Invokes (OpenProcess / CloseHandle / ReadProcessMemory / VirtualQueryEx) that
//   used to live inline inside MemoryHandler and Scanner. Behaviour is identical to
//   the pre-Phase-9.1.0 direct calls — refactor only.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Memory {
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal sealed class WindowsProcessMemoryAccessor : IProcessMemoryAccessor {
        // Mirror of Scanner's region-filter constants. Kept here so the platform
        // specific bit-flag arithmetic stays inside the platform-specific file.
        private const int MemCommit = 0x1000;
        private const int PageGuard = 0x100;
        private const int PageReadwrite = 0x04;
        private const int PageWritecopy = 0x08;
        private const int PageExecuteReadwrite = 0x40;
        private const int PageExecuteWritecopy = 0x80;
        private const int Writable = PageReadwrite | PageWritecopy | PageExecuteReadwrite | PageExecuteWritecopy | PageGuard;

        private SharlayanConfiguration _configuration;

        public bool IsAttached { get; private set; }

        public IntPtr ProcessHandle { get; private set; }

        public void Attach(SharlayanConfiguration configuration) {
            this._configuration = configuration;

            // Read-only first to avoid elevation prompts when the game runs at the same
            // integrity level. PROCESS_VM_ALL is the legacy fallback; finally fall back
            // to Process.Handle for the historic .NET-managed handle.
            this.ProcessHandle = UnsafeNativeMethods.OpenProcess(
                UnsafeNativeMethods.ProcessAccessFlags.PROCESS_VM_READ_QUERY,
                false,
                (uint) configuration.ProcessModel.ProcessID);

            if (this.ProcessHandle == IntPtr.Zero) {
                try {
                    this.ProcessHandle = UnsafeNativeMethods.OpenProcess(
                        UnsafeNativeMethods.ProcessAccessFlags.PROCESS_VM_ALL,
                        false,
                        (uint) configuration.ProcessModel.ProcessID);
                }
                catch (Exception) {
                    this.ProcessHandle = configuration.ProcessModel.Process.Handle;
                }
            }

            this.IsAttached = true;
        }

        public bool ReadBytes(IntPtr address, byte[] buffer, int length) {
            return UnsafeNativeMethods.ReadProcessMemory(this.ProcessHandle, address, buffer, new IntPtr(length), out IntPtr _);
        }

        public bool ReadBytes(IntPtr address, byte[] buffer) {
            return UnsafeNativeMethods.ReadProcessMemory(this.ProcessHandle, address, buffer, new IntPtr(buffer.Length), out IntPtr _);
        }

        public bool ReadBytes(IntPtr address, IntPtr buffer, int length) {
            return UnsafeNativeMethods.ReadProcessMemory(this.ProcessHandle, address, buffer, new IntPtr(length), out IntPtr _);
        }

        public IEnumerable<ScannableRegion> EnumerateScannableRegions() {
            if (this.ProcessHandle == IntPtr.Zero) {
                yield break;
            }

            IntPtr address = IntPtr.Zero;
            while (true) {
                UnsafeNativeMethods.MEMORY_BASIC_INFORMATION info = default;
                int result = UnsafeNativeMethods.VirtualQueryEx(this.ProcessHandle, address, out info, (uint) Marshal.SizeOf(info));
                if (result == 0) {
                    yield break;
                }

                // The "is this a system module?" filter lives one layer up in
                // MemoryHandler.IsSystemModule because it's cross-platform (driven by
                // Process.Modules). Here we apply the platform-specific "is this a
                // valid scan target?" filter: committed + writable + not page-guarded.
                if ((info.State & MemCommit) != 0 && (info.Protect & Writable) != 0 && (info.Protect & PageGuard) == 0) {
                    yield return new ScannableRegion(info.BaseAddress, info.RegionSize.ToInt64());
                }

                unchecked {
                    long next = IntPtr.Size == sizeof(int)
                        ? info.BaseAddress.ToInt32() + info.RegionSize.ToInt32()
                        : info.BaseAddress.ToInt64() + info.RegionSize.ToInt64();
                    address = new IntPtr(next);
                }
            }
        }

        public IntPtr GetMainModuleBaseAddress() {
            System.Diagnostics.ProcessModule mainModule = this._configuration?.ProcessModel?.Process?.MainModule;
            return mainModule != null ? mainModule.BaseAddress : IntPtr.Zero;
        }

        public long GetMainModuleSize() {
            System.Diagnostics.ProcessModule mainModule = this._configuration?.ProcessModel?.Process?.MainModule;
            return mainModule != null ? mainModule.ModuleMemorySize : 0;
        }

        public void Dispose() {
            try {
                if (this.IsAttached) {
                    UnsafeNativeMethods.CloseHandle(this.ProcessHandle);
                }
            }
            catch (Exception) {
                // IGNORED — same swallow behaviour as the pre-9.1.0 inline Dispose.
            }
            finally {
                this.IsAttached = false;
                this.ProcessHandle = IntPtr.Zero;
            }
        }
    }
}
