// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryHandler.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   MemoryHandler.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;

    using NLog;

    using Sharlayan.Events;
    using Sharlayan.Models;
    using Sharlayan.Models.Structures;
    using Sharlayan.Utilities;

    using BitConverter = Sharlayan.Utilities.BitConverter;

    public class MemoryHandler : IDisposable {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private AttachmentWorker _attachmentWorker;

        private bool _isNewInstance = true;

        public MemoryHandler(MemoryHandlerConfiguration configuration) {
            this.Configuration = configuration;

            try {
                this.ProcessHandle = UnsafeNativeMethods.OpenProcess(UnsafeNativeMethods.ProcessAccessFlags.PROCESS_VM_ALL, false, (uint) this.Configuration.ProcessModel.ProcessID);
            }
            catch (Exception) {
                this.ProcessHandle = this.Configuration.ProcessModel.Process.Handle;
            }
            finally {
                Constants.ProcessHandle = this.ProcessHandle;
                this.IsAttached = true;
            }

            if (this._isNewInstance) {
                this._isNewInstance = false;

                ActionLookup.Resolve();
                StatusEffectLookup.Resolve();
                ZoneLookup.Resolve();

                this.ResolveMemoryStructures(this.Configuration.ProcessModel, this.Configuration.PatchVersion);
            }

            this._attachmentWorker = new AttachmentWorker(this);
            this._attachmentWorker.StartScanning(this.Configuration.ProcessModel);

            this._systemModules.Clear();

            this.GetProcessModules();

            this.Scanner = new Scanner(this);
            this.Reader = new Reader(this);

            this.Scanner.Locations.Clear();
            this.Scanner.LoadOffsets(Signatures.Resolve(this.Configuration), this.Configuration.ScanAlMemoryRegions);
        }

        ~MemoryHandler() {
            this.Dispose();
        }

        public event EventHandler<ExceptionEvent> ExceptionEvent = (sender, args) => { };

        public event EventHandler<MemoryLocationsFoundEvent> MemoryLocationsFoundEvent = (sender, args) => { };

        public MemoryHandlerConfiguration Configuration { get; set; }

        public bool IsAttached { get; set; }

        public Reader Reader { get; set; }

        public long ScanCount { get; set; }

        public Scanner Scanner { get; }

        internal IntPtr ProcessHandle { get; set; }

        internal StructuresContainer Structures { get; set; }

        private List<ProcessModule> _systemModules { get; } = new List<ProcessModule>();

        public void Dispose() {
            if (this._attachmentWorker != null) {
                this._attachmentWorker.StopScanning();
                this._attachmentWorker.Dispose();
            }

            try {
                if (this.IsAttached) {
                    UnsafeNativeMethods.CloseHandle(this.ProcessHandle);
                }
            }
            catch (Exception ex) {
                // IGNORED
            }
            finally {
                Constants.ProcessHandle = this.ProcessHandle = IntPtr.Zero;
                this.IsAttached = false;
            }
        }

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

        public short GetInt16(IntPtr address, long offset = 0) {
            byte[] value = new byte[2];
            this.Peek(new IntPtr(address.ToInt64() + offset), value);
            return BitConverter.TryToInt16(value, 0);
        }

        public int GetInt32(IntPtr address, long offset = 0) {
            byte[] value = new byte[4];
            this.Peek(new IntPtr(address.ToInt64() + offset), value);
            return BitConverter.TryToInt32(value, 0);
        }

        public long GetInt64(IntPtr address, long offset = 0) {
            byte[] value = new byte[8];
            this.Peek(new IntPtr(address.ToInt64() + offset), value);
            return BitConverter.TryToInt64(value, 0);
        }

        public long GetInt64FromBytes(byte[] source, int index = 0) {
            return BitConverter.TryToInt64(source, index);
        }

        public IntPtr GetStaticAddress(long offset) {
            ProcessModule processMainModule = this.Configuration.ProcessModel.Process.MainModule;
            if (processMainModule != null) {
                return new IntPtr(processMainModule.BaseAddress.ToInt64() + offset);
            }

            return IntPtr.Zero;
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
            if (source.Length == 0) {
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
            IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(T)));
            UnsafeNativeMethods.ReadProcessMemory(this.Configuration.ProcessModel.Process.Handle, address + offset, buffer, new IntPtr(Marshal.SizeOf(typeof(T))), out IntPtr bytesRead);
            T retValue = (T) Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeCoTaskMem(buffer);
            return retValue;
        }

        public ushort GetUInt16(IntPtr address, long offset = 0) {
            byte[] value = new byte[4];
            this.Peek(new IntPtr(address.ToInt64() + offset), value);
            return BitConverter.TryToUInt16(value, 0);
        }

        public uint GetUInt32(IntPtr address, long offset = 0) {
            byte[] value = new byte[4];
            this.Peek(new IntPtr(address.ToInt64() + offset), value);
            return BitConverter.TryToUInt32(value, 0);
        }

        public ulong GetUInt64(IntPtr address, long offset = 0) {
            byte[] value = new byte[8];
            this.Peek(new IntPtr(address.ToInt64() + offset), value);
            return BitConverter.TryToUInt32(value, 0);
        }

        public ulong GetUInt64FromBytes(byte[] source, int index = 0) {
            return BitConverter.TryToUInt64(source, index);
        }

        public bool Peek(IntPtr address, byte[] buffer) {
            return UnsafeNativeMethods.ReadProcessMemory(this.ProcessHandle, address, buffer, new IntPtr(buffer.Length), out IntPtr bytesRead);
        }

        public IntPtr ReadPointer(IntPtr address, long offset = 0) {
            byte[] win64 = new byte[8];
            this.Peek(new IntPtr(address.ToInt64() + offset), win64);
            return new IntPtr(BitConverter.TryToInt64(win64, 0));
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
                for (int i = 0; i < this._systemModules.Count; i++) {
                    ProcessModule module = this._systemModules[i];
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
            if (moduleByAddress != null) {
                foreach (ProcessModule module in this._systemModules) {
                    if (module.ModuleName == moduleByAddress.ModuleName) {
                        return true;
                    }
                }
            }

            return false;
        }

        internal void ResolveMemoryStructures(ProcessModel processModel, string patchVersion = "latest") {
            this.Structures = APIHelper.GetStructures(processModel, patchVersion);
        }

        protected internal virtual void RaiseException(Logger logger, Exception e, bool levelIsError = false) {
            this.ExceptionEvent?.Invoke(this, new ExceptionEvent(this, logger, e, levelIsError));
        }

        protected internal virtual void RaiseMemoryLocationsFound(Logger logger, Dictionary<string, MemoryLocation> memoryLocations, long processingTime) {
            this.MemoryLocationsFoundEvent?.Invoke(this, new MemoryLocationsFoundEvent(this, logger, memoryLocations, processingTime));
        }

        private void GetProcessModules() {
            ProcessModuleCollection modules = this.Configuration.ProcessModel.Process.Modules;

            for (int i = 0; i < modules.Count; i++) {
                ProcessModule module = modules[i];
                this._systemModules.Add(module);
            }
        }
    }
}