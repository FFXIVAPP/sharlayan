// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProcessMemoryAccessor.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Platform-neutral abstraction over the cross-process memory primitives Sharlayan
//   needs (open / close, byte-buffer read, region enumeration, main-module base).
//
//   Phase 9.1.0 introduces the abstraction with a Windows-only implementation
//   (WindowsProcessMemoryAccessor) — behaviour is identical to the previous direct
//   kernel32 P/Invokes. Phase 9.1.1 will add a Linux implementation backed by
//   process_vm_readv + /proc/&lt;pid&gt;/maps.
//
//   Implementations must NOT take live pointers into FFXIVClientStructs structs or
//   invoke FCS member-function / vfunc wrappers. FCS is consumed only as a metadata
//   source (FieldOffset attributes via FieldOffsetReader, StaticAddress patterns via
//   FFXIVClientStructsSignatureExtractor); all live data flows through copy-out reads
//   exposed by this interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Memory {
    using System;
    using System.Collections.Generic;

    public interface IProcessMemoryAccessor : IDisposable {
        /// <summary>
        /// True once <see cref="Attach"/> has run and a usable read primitive is in
        /// place; false once <see cref="IDisposable.Dispose"/> has been invoked.
        /// </summary>
        bool IsAttached { get; }

        /// <summary>
        /// Opaque platform handle — kept for legacy callers that read
        /// <see cref="MemoryHandler.ProcessHandle"/> directly. Returns the Win32
        /// process handle on Windows; expected to be <see cref="IntPtr.Zero"/> on
        /// platforms where no equivalent handle exists (Linux uses the PID directly).
        /// </summary>
        IntPtr ProcessHandle { get; }

        /// <summary>
        /// Open the target process for memory reads. Implementations should request
        /// read-only access first and only escalate to all-access if the read-only
        /// open fails — staying read-only avoids elevation prompts on Windows and
        /// minimises permission requirements on Linux.
        /// </summary>
        void Attach(SharlayanConfiguration configuration);

        /// <summary>
        /// Read <paramref name="length"/> bytes starting at <paramref name="address"/>
        /// in the target process into <paramref name="buffer"/>. Returns true on
        /// success. A short read (fewer bytes returned than requested) MUST be
        /// reported as failure — callers that need page-bounded partial reads will
        /// be served by a future <c>ReadBytesPaged</c> overload introduced alongside
        /// the Linux backend in 9.1.1.
        /// </summary>
        bool ReadBytes(IntPtr address, byte[] buffer, int length);

        /// <summary>
        /// Convenience overload that reads <c>buffer.Length</c> bytes.
        /// </summary>
        bool ReadBytes(IntPtr address, byte[] buffer);

        /// <summary>
        /// Read into an unmanaged buffer. Provided for the legacy
        /// <see cref="MemoryHandler.GetStructure{T}"/> path which Marshal.PtrToStructure
        /// over a pinned region. New code should prefer the byte-buffer overload.
        /// </summary>
        bool ReadBytes(IntPtr address, IntPtr buffer, int length);

        /// <summary>
        /// Walk the process address space and yield every region the Scanner may scan
        /// for byte signatures — committed, writable, not page-guarded, not backed by
        /// a system module the host knows about.
        /// </summary>
        IEnumerable<ScannableRegion> EnumerateScannableRegions();

        /// <summary>
        /// Base virtual address of the target process's main module (the FFXIV
        /// executable). On Windows this is <c>Process.MainModule.BaseAddress</c>;
        /// the Linux implementation will resolve <c>ffxiv_dx11.exe</c> from
        /// <c>/proc/&lt;pid&gt;/maps</c> using <c>min(start - file_offset)</c>
        /// across all mappings of that file.
        /// </summary>
        IntPtr GetMainModuleBaseAddress();

        /// <summary>
        /// Total mapped size of the main module. Used by Scanner.FindExtendedSignatures
        /// as the search end. On Linux this is <c>max(end) - base</c> across the file's
        /// mappings.
        /// </summary>
        long GetMainModuleSize();
    }
}
