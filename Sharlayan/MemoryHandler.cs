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

    using Sharlayan.Memory;
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
            // Phase 9.1.0: all platform-specific OpenProcess / CloseHandle / RPM /
            // VirtualQueryEx calls now live behind IProcessMemoryAccessor. Selection
            // is OS-driven via the factory; the Windows implementation preserves the
            // exact "read-only first, fall back to all-access, finally Process.Handle"
            // attach sequence used pre-refactor.
            this.Accessor = ProcessMemoryAccessorFactory.Create();
            this.Accessor.Attach(configuration);
            this.IsAttached = true;

            this.Configuration.ProcessModel.Process.EnableRaisingEvents = true;
            this.Configuration.ProcessModel.Process.Exited += this.Process_OnExited;

            this.GetProcessModules();

            this.Scanner = new Scanner(this);
            this.Reader = new Reader(this);

            if (this._isNewInstance) {
                this._isNewInstance = false;

                Task.Run(
                    async () => {
                        await this.ResolveMemoryStructures();

                        await ActionLookup.Resolve(this.Configuration);
                        await StatusEffectLookup.Resolve(this.Configuration);
                        await ZoneLookup.Resolve(this.Configuration);
                    });
            }

            Task.Run(
                async () => {
                    Signature[] signatures = await Signatures.Resolve(this.Configuration);
                    this.Scanner.LoadOffsets(signatures, this.Configuration.ScanAllRegions);
                });
        }

        public SharlayanConfiguration Configuration { get; set; }

        internal bool IsAttached { get; set; }

        public Reader Reader { get; set; }

        public long ScanCount { get; set; }

        public Scanner Scanner { get; }

        /// <summary>
        /// Platform handle passed through from the active <see cref="IProcessMemoryAccessor"/>.
        /// On Windows this is the kernel32 process handle; on Linux this will be
        /// <see cref="IntPtr.Zero"/> (Linux uses the PID directly via process_vm_readv).
        /// Existing internal callers continue to work via the property setter on the
        /// accessor side.
        /// </summary>
        internal IntPtr ProcessHandle => this.Accessor?.ProcessHandle ?? IntPtr.Zero;

        /// <summary>
        /// Platform-neutral memory access primitives (open / read / region enumeration).
        /// Constructed via <see cref="ProcessMemoryAccessorFactory.Create"/> so the
        /// concrete type is selected by OS at runtime.
        /// </summary>
        internal IProcessMemoryAccessor Accessor { get; private set; }

        internal StructuresContainer Structures { get; set; } = new StructuresContainer();

        private List<ProcessModule> _systemModules { get; } = new List<ProcessModule>();

        public void Dispose() {
            try {
                this.Accessor?.Dispose();
            }
            catch (Exception) {
                // IGNORED — historical swallow behaviour preserved.
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

        public byte GetByte(IntPtr address, long offset = 0) {
            byte[] data = new byte[1];
            this.Peek(new IntPtr(address.ToInt64() + offset), data);
            return data[0];
        }

        public byte[] GetByteArray(IntPtr address, int length) {
            byte[] data = new byte[length];
            this.Peek(address, data);
            return data;
        }

        public void GetByteArray(IntPtr address, byte[] destination) {
            this.Peek(address, destination);
        }

        public short GetInt16(IntPtr address, long offset = 0) {
            byte[] value = new byte[2];
            this.Peek(new IntPtr(address.ToInt64() + offset), value);
            return SharlayanBitConverter.TryToInt16(value, 0);
        }

        public int GetInt32(IntPtr address, long offset = 0) {
            byte[] value = new byte[4];
            this.Peek(new IntPtr(address.ToInt64() + offset), value);
            return SharlayanBitConverter.TryToInt32(value, 0);
        }

        public long GetInt64(IntPtr address, long offset = 0) {
            byte[] value = new byte[8];
            this.Peek(new IntPtr(address.ToInt64() + offset), value);
            return SharlayanBitConverter.TryToInt64(value, 0);
        }

        public long GetInt64FromBytes(byte[] source, int index = 0) {
            return SharlayanBitConverter.TryToInt64(source, index);
        }

        public IntPtr GetStaticAddress(long offset) {
            // Routed through the accessor so the Linux backend can resolve
            // ffxiv_dx11.exe via /proc/<pid>/maps with min(start - file_offset)
            // instead of relying on Process.MainModule (which would point at wine64).
            IntPtr baseAddress = this.Accessor?.GetMainModuleBaseAddress() ?? IntPtr.Zero;
            return baseAddress == IntPtr.Zero ? IntPtr.Zero : new IntPtr(baseAddress.ToInt64() + offset);
        }

        public string GetString(IntPtr address, long offset = 0, int size = 256) {
            byte[] bytes = new byte[size];
            this.Peek(new IntPtr(address.ToInt64() + offset), bytes);
            int realSize = 0;
            for (int i = 0; i < size; i++) {
                if (bytes[i] != 0) {
                    continue;
                }

                realSize = i;
                break;
            }

            Array.Resize(ref bytes, realSize);
            return Encoding.UTF8.GetString(bytes);
        }

        public string GetStringFromBytes(byte[] source, int offset = 0, int size = 256) {
            if (!source.Any()) {
                return string.Empty;
            }

            int safeSize = source.Length - offset;
            if (safeSize < size) {
                size = safeSize;
            }

            byte[] bytes = new byte[size];
            Array.Copy(source, offset, bytes, 0, size);
            int realSize = 0;
            for (int i = 0; i < size; i++) {
                if (bytes[i] != 0) {
                    continue;
                }

                realSize = i;
                break;
            }

            Array.Resize(ref bytes, realSize);
            return Encoding.UTF8.GetString(bytes);
        }

        public T GetStructure<T>(IntPtr address, int offset = 0) {
            int size = Marshal.SizeOf(typeof(T));
            IntPtr buffer = Marshal.AllocCoTaskMem(size);
            try {
                this.Accessor?.ReadBytes(address + offset, buffer, size);
                return (T) Marshal.PtrToStructure(buffer, typeof(T));
            }
            finally {
                Marshal.FreeCoTaskMem(buffer);
            }
        }

        public ushort GetUInt16(IntPtr address, long offset = 0) {
            byte[] value = new byte[4];
            this.Peek(new IntPtr(address.ToInt64() + offset), value);
            return SharlayanBitConverter.TryToUInt16(value, 0);
        }

        public uint GetUInt32(IntPtr address, long offset = 0) {
            byte[] value = new byte[4];
            this.Peek(new IntPtr(address.ToInt64() + offset), value);
            return SharlayanBitConverter.TryToUInt32(value, 0);
        }

        public ulong GetUInt64(IntPtr address, long offset = 0) {
            byte[] value = new byte[8];
            this.Peek(new IntPtr(address.ToInt64() + offset), value);
            return SharlayanBitConverter.TryToUInt32(value, 0);
        }

        public ulong GetUInt64FromBytes(byte[] source, int index = 0) {
            return SharlayanBitConverter.TryToUInt64(source, index);
        }

        public bool Peek(IntPtr address, byte[] buffer) {
            return this.Accessor != null && this.Accessor.ReadBytes(address, buffer);
        }

        public IntPtr ReadPointer(IntPtr address, long offset = 0) {
            byte[] win64 = new byte[8];
            this.Peek(new IntPtr(address.ToInt64() + offset), win64);
            return new IntPtr(SharlayanBitConverter.TryToInt64(win64, 0));
        }

        public IntPtr ResolvePointerPath(IEnumerable<long> path, IntPtr baseAddress, bool IsASMSignature = false) {
            IntPtr nextAddress = baseAddress;
            foreach (long offset in path) {
                try {
                    baseAddress = new IntPtr(nextAddress.ToInt64() + offset);
                    if (baseAddress == IntPtr.Zero) {
                        return IntPtr.Zero;
                    }

                    if (IsASMSignature) {
                        nextAddress = baseAddress + this.GetInt32(new IntPtr(baseAddress.ToInt64())) + 4;
                        IsASMSignature = false;
                    }
                    else {
                        nextAddress = this.ReadPointer(baseAddress);
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