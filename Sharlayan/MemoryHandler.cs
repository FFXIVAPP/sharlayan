// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryHandler.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   MemoryHandler.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    using NLog;

    using Sharlayan.Models;
    using Sharlayan.Models.Structures;
    using Sharlayan.Resources;
    using Sharlayan.Utilities;

    public class MemoryHandler : IDisposable {
        public delegate void ExceptionEvent(object sender, Logger logger, Exception ex);

        public delegate void MemoryHandlerDisposedEvent(object sender);

        public delegate void MemoryLocationsFoundEvent(object sender, ConcurrentDictionary<string, MemoryLocation> memoryLocations, long processingTime);

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool _isNewInstance = true;

        internal ClearingArrayPool<byte> BufferPool = new ClearingArrayPool<byte>();

        public MemoryHandler(SharlayanConfiguration configuration) {
            this.Configuration = configuration;
            // Prefer read-only access so Sharlayan can attach without admin when the
            // game runs at the same integrity level; fall back to PROCESS_VM_ALL if
            // the reduced open fails. (A former third fallback to Process.Handle was
            // unreachable — the OpenProcess P/Invoke returns Zero rather than throwing —
            // and would have aliased a handle the Process object owns, so Dispose()'s
            // CloseHandle could double-close it. F77: removed.)
            this.ProcessHandle = UnsafeNativeMethods.OpenProcess(UnsafeNativeMethods.ProcessAccessFlags.PROCESS_VM_READ_QUERY, false, (uint) this.Configuration.ProcessModel.ProcessID);
            if (this.ProcessHandle == IntPtr.Zero) {
                this.ProcessHandle = UnsafeNativeMethods.OpenProcess(UnsafeNativeMethods.ProcessAccessFlags.PROCESS_VM_ALL, false, (uint) this.Configuration.ProcessModel.ProcessID);
            }
            this.IsAttached = true;

            this.Configuration.ProcessModel.Process.EnableRaisingEvents = true;
            this.Configuration.ProcessModel.Process.Exited += this.Process_OnExited;

            this.GetProcessModules();

            this.Scanner = new Scanner(this);
            this.Reader = new Reader(this);

            if (this._isNewInstance) {
                this._isNewInstance = false;

                // Observe the fire-and-forget task. Without this, any exception raised by
                // ResolveMemoryStructures / ActionLookup / StatusEffectLookup / ZoneLookup
                // becomes an UnobservedTaskException at GC time and crashes the host AppDomain
                // (default TaskScheduler behaviour). Faults get surfaced via OnException so
                // consumers can log / display them instead.
                Task.Run(
                    async () => {
                        await this.ResolveMemoryStructures();

                        await ActionLookup.Resolve(this.Configuration);
                        await StatusEffectLookup.Resolve(this.Configuration);
                        await ZoneLookup.Resolve(this.Configuration);
                    })
                    .ContinueWith(
                        t => {
                            Logger.Error(t.Exception, "Background resource resolution faulted.");
                            this.RaiseException(Logger, t.Exception);
                        },
                        TaskContinuationOptions.OnlyOnFaulted);
            }

            // Same fire-and-forget shape as above; same continuation guard so a faulting
            // signature scan doesn't escape as an UnobservedTaskException either.
            Task.Run(
                async () => {
                    Signature[] signatures = await Signatures.Resolve(this.Configuration);
                    this.Scanner.LoadOffsets(signatures, this.Configuration.ScanAllRegions);
                })
                .ContinueWith(
                    t => {
                        Logger.Error(t.Exception, "Background signature resolution faulted.");
                        this.RaiseException(Logger, t.Exception);
                    },
                    TaskContinuationOptions.OnlyOnFaulted);
        }

        public SharlayanConfiguration Configuration { get; set; }

        private volatile bool _isAttached;

        internal bool IsAttached {
            get => this._isAttached;
            set => this._isAttached = value;
        }

        public Reader Reader { get; set; }

        public long ScanCount { get; set; }

        public Scanner Scanner { get; }

        internal IntPtr ProcessHandle { get; set; }

        internal StructuresContainer Structures { get; set; } = new StructuresContainer();

        private List<ProcessModule> _systemModules { get; } = new List<ProcessModule>();

        public void Dispose() {
            try {
                if (this.IsAttached) {
                    UnsafeNativeMethods.CloseHandle(this.ProcessHandle);
                }
            }
            catch (Exception) {
                // IGNORED
            }
            finally {
                this.IsAttached = false;
                this.RaiseMemoryHandlerDisposed();
            }
        }

        ~MemoryHandler() {
            this.Dispose();
        }

        public event ExceptionEvent OnException = delegate { };

        public event MemoryHandlerDisposedEvent OnMemoryHandlerDisposed = delegate { };

        public event MemoryLocationsFoundEvent OnMemoryLocationsFound = delegate { };

        // F78: the [ThreadStatic] scalar buffers below are shared by every same-width
        // read on a thread and are never cleared. Each accessor must check Peek's
        // result and return 0 on failure — otherwise a transient ReadProcessMemory
        // failure silently yields the bytes of the previous, unrelated read.
        [ThreadStatic]
        private static byte[] _singleByteBuffer;

        public byte GetByte(IntPtr address, long offset = 0) {
            if (_singleByteBuffer == null) {
                _singleByteBuffer = new byte[1];
            }

            if (!this.Peek(new IntPtr(address.ToInt64() + offset), _singleByteBuffer)) {
                return 0;
            }

            return _singleByteBuffer[0];
        }

        public byte[] GetByteArray(IntPtr address, int length) {
            byte[] data = new byte[length];
            this.Peek(address, data);
            return data;
        }

        // F78: destination buffers are pooled and reused across actors/polls — zero
        // them on a failed read so the caller parses an empty record instead of the
        // PREVIOUS occupant's bytes (e.g. a failed per-actor read silently duplicating
        // the prior actor).
        public void GetByteArray(IntPtr address, byte[] destination) {
            if (!this.Peek(address, destination)) {
                Array.Clear(destination, 0, destination.Length);
            }
        }

        public void GetByteArray(IntPtr address, byte[] destination, int count) {
            if (!this.Peek(address, destination, count)) {
                Array.Clear(destination, 0, count);
            }
        }

        [ThreadStatic]
        private static byte[] _twoByteBuffer;

        [ThreadStatic]
        private static byte[] _fourByteBuffer;

        [ThreadStatic]
        private static byte[] _eightByteBuffer;

        public short GetInt16(IntPtr address, long offset = 0) {
            if (_twoByteBuffer == null) {
                _twoByteBuffer = new byte[2];
            }

            if (!this.Peek(new IntPtr(address.ToInt64() + offset), _twoByteBuffer)) {
                return 0;
            }

            return SharlayanBitConverter.TryToInt16(_twoByteBuffer, 0);
        }

        public int GetInt32(IntPtr address, long offset = 0) {
            this.TryGetInt32(new IntPtr(address.ToInt64() + offset), out int value);
            return value;
        }

        // Failure-distinguishing variant for callers where a legitimate 0 and a failed
        // read must not be conflated (e.g. the ASM-signature displacement hop).
        internal bool TryGetInt32(IntPtr address, out int value) {
            if (_fourByteBuffer == null) {
                _fourByteBuffer = new byte[4];
            }

            if (!this.Peek(address, _fourByteBuffer)) {
                value = 0;
                return false;
            }

            value = SharlayanBitConverter.TryToInt32(_fourByteBuffer, 0);
            return true;
        }

        public long GetInt64(IntPtr address, long offset = 0) {
            if (_eightByteBuffer == null) {
                _eightByteBuffer = new byte[8];
            }

            if (!this.Peek(new IntPtr(address.ToInt64() + offset), _eightByteBuffer)) {
                return 0;
            }

            return SharlayanBitConverter.TryToInt64(_eightByteBuffer, 0);
        }

        public long GetInt64FromBytes(byte[] source, int index = 0) {
            return SharlayanBitConverter.TryToInt64(source, index);
        }

        public IntPtr GetStaticAddress(long offset) {
            ProcessModule processMainModule = this.Configuration.ProcessModel.Process.MainModule;
            if (processMainModule != null) {
                return new IntPtr(processMainModule.BaseAddress.ToInt64() + offset);
            }

            return IntPtr.Zero;
        }

        public string GetString(IntPtr address, long offset = 0, int size = 256) {
            byte[] bytes = this.BufferPool.Rent(size);
            try {
                // Rented buffers can exceed `size`; read exactly `size` bytes as before.
                this.Peek(new IntPtr(address.ToInt64() + offset), bytes, size);
                int realSize = 0;
                for (int i = 0; i < size; i++) {
                    if (bytes[i] != 0) {
                        continue;
                    }

                    realSize = i;
                    break;
                }

                return Encoding.UTF8.GetString(bytes, 0, realSize);
            }
            finally {
                this.BufferPool.Return(bytes);
            }
        }

        public string GetStringFromBytes(byte[] source, int offset = 0, int size = 256) {
            if (source.Length == 0) {
                return string.Empty;
            }

            int safeSize = source.Length - offset;
            if (safeSize < size) {
                size = safeSize;
            }

            int realSize = 0;
            for (int i = 0; i < size; i++) {
                if (source[offset + i] != 0) {
                    continue;
                }

                realSize = i;
                break;
            }

            return Encoding.UTF8.GetString(source, offset, realSize);
        }

        public T GetStructure<T>(IntPtr address, int offset = 0) {
            int size = Marshal.SizeOf(typeof(T));
            IntPtr buffer = Marshal.AllocCoTaskMem(size);
            try {
                // F78: AllocCoTaskMem does not zero-initialize — on a failed read,
                // marshalling would return a struct built from uninitialized heap bytes.
                if (!UnsafeNativeMethods.ReadProcessMemory(this.ProcessHandle, address + offset, buffer, new IntPtr(size), out IntPtr _)) {
                    return default;
                }

                return (T) Marshal.PtrToStructure(buffer, typeof(T));
            }
            finally {
                Marshal.FreeCoTaskMem(buffer);
            }
        }

        public ushort GetUInt16(IntPtr address, long offset = 0) {
            if (_twoByteBuffer == null) {
                _twoByteBuffer = new byte[2];
            }

            if (!this.Peek(new IntPtr(address.ToInt64() + offset), _twoByteBuffer)) {
                return 0;
            }

            return SharlayanBitConverter.TryToUInt16(_twoByteBuffer, 0);
        }

        public uint GetUInt32(IntPtr address, long offset = 0) {
            if (_fourByteBuffer == null) {
                _fourByteBuffer = new byte[4];
            }

            if (!this.Peek(new IntPtr(address.ToInt64() + offset), _fourByteBuffer)) {
                return 0;
            }

            return SharlayanBitConverter.TryToUInt32(_fourByteBuffer, 0);
        }

        public ulong GetUInt64(IntPtr address, long offset = 0) {
            if (_eightByteBuffer == null) {
                _eightByteBuffer = new byte[8];
            }

            if (!this.Peek(new IntPtr(address.ToInt64() + offset), _eightByteBuffer)) {
                return 0;
            }

            return SharlayanBitConverter.TryToUInt64(_eightByteBuffer, 0);
        }

        public ulong GetUInt64FromBytes(byte[] source, int index = 0) {
            return SharlayanBitConverter.TryToUInt64(source, index);
        }

        public bool Peek(IntPtr address, byte[] buffer) {
            return UnsafeNativeMethods.ReadProcessMemory(this.ProcessHandle, address, buffer, new IntPtr(buffer.Length), out IntPtr bytesRead);
        }

        // Count-bounded overload for pooled buffers, which may be larger than the
        // amount of process memory the caller actually wants to read.
        public bool Peek(IntPtr address, byte[] buffer, int count) {
            return UnsafeNativeMethods.ReadProcessMemory(this.ProcessHandle, address, buffer, new IntPtr(count), out IntPtr bytesRead);
        }

        public IntPtr ReadPointer(IntPtr address, long offset = 0) {
            if (_eightByteBuffer == null) {
                _eightByteBuffer = new byte[8];
            }

            if (!this.Peek(new IntPtr(address.ToInt64() + offset), _eightByteBuffer)) {
                return IntPtr.Zero;
            }

            return new IntPtr(SharlayanBitConverter.TryToInt64(_eightByteBuffer, 0));
        }

        public IntPtr ResolvePointerPath(IEnumerable<long> path, IntPtr baseAddress, bool IsASMSignature = false) {
            // F79: only baseAddress is ever returned, so dereferencing on the final
            // hop was a wasted ReadProcessMemory syscall on every resolution — skip it.
            // F78: a null pointer mid-chain now aborts the walk instead of continuing
            // from a garbage address.
            IntPtr nextAddress = baseAddress;
            using IEnumerator<long> hops = path.GetEnumerator();
            bool hasHop = hops.MoveNext();
            while (hasHop) {
                long offset = hops.Current;
                hasHop = hops.MoveNext();
                try {
                    baseAddress = new IntPtr(nextAddress.ToInt64() + offset);
                    if (baseAddress == IntPtr.Zero) {
                        return IntPtr.Zero;
                    }

                    if (!hasHop) {
                        break;
                    }

                    if (IsASMSignature) {
                        // Same failure handling as the pointer branch: a failed
                        // displacement read aborts instead of continuing from base+0+4.
                        if (!this.TryGetInt32(new IntPtr(baseAddress.ToInt64()), out int displacement)) {
                            return IntPtr.Zero;
                        }

                        nextAddress = baseAddress + displacement + 4;
                        IsASMSignature = false;
                    }
                    else {
                        nextAddress = this.ReadPointer(baseAddress);
                        if (nextAddress == IntPtr.Zero) {
                            return IntPtr.Zero;
                        }
                    }
                }
                catch {
                    return IntPtr.Zero;
                }
            }

            return baseAddress;
        }

        internal ProcessModule GetModuleByAddress(IntPtr address) {
            try {
                foreach (ProcessModule module in this._systemModules) {
                    long baseAddress = module.BaseAddress.ToInt64();
                    if (baseAddress <= (long) address && baseAddress + module.ModuleMemorySize >= (long) address) {
                        return module;
                    }
                }

                return null;
            }
            catch (Exception) {
                return null;
            }
        }

        internal bool IsSystemModule(IntPtr address) {
            ProcessModule moduleByAddress = this.GetModuleByAddress(address);
            if (moduleByAddress == null) {
                return false;
            }

            foreach (ProcessModule module in this._systemModules) {
                if (module.ModuleName == moduleByAddress.ModuleName) {
                    return true;
                }
            }

            return false;
        }

        internal async Task ResolveMemoryStructures() {
            IResourceProvider provider = ResourceProviderFactory.Create(this.Configuration);
            this.Structures = await provider.GetStructuresAsync(this.Configuration);
        }

        protected internal virtual void RaiseException(Logger logger, Exception ex) {
            this.OnException?.Invoke(this, logger, ex);
        }

        protected internal virtual void RaiseMemoryHandlerDisposed() {
            this.OnMemoryHandlerDisposed?.Invoke(this);
        }

        protected internal virtual void RaiseMemoryLocationsFound(ConcurrentDictionary<string, MemoryLocation> memoryLocations, long processingTime) {
            this.OnMemoryLocationsFound?.Invoke(this, memoryLocations, processingTime);
        }

        private void GetProcessModules() {
            ProcessModuleCollection modules = this.Configuration.ProcessModel.Process.Modules;

            foreach (ProcessModule module in modules) {
                this._systemModules.Add(module);
            }
        }

        private void Process_OnExited(object sender, EventArgs e) {
            if (!SharlayanMemoryManager.Instance.RemoveHandler(this.Configuration.ProcessModel.ProcessID)) {
                this.Dispose();
            }

            this.Configuration.ProcessModel.Process.Exited -= this.Process_OnExited;
        }
    }
}