// Sharlayan ~ MemoryHandler.cs
// 
// Copyright © 2007 - 2017 Ryan Wilson - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Sharlayan.Events;
using Sharlayan.Helpers;
using Sharlayan.Models;
using NLog;
using BitConverter = Sharlayan.Helpers.BitConverter;

namespace Sharlayan
{
    public class MemoryHandler
    {
        #region Logger

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        private List<ProcessModule> _systemModules;

        public MemoryHandler(ProcessModel processModel, string gameLanguage = "English", string patchVersion = "latest", bool ignoreJSONCache = false, bool scanAllMemoryRegions = false)
        {
            GameLanguage = gameLanguage;
            IgnoreJSONCache = ignoreJSONCache;

            if (processModel == null)
            {
                return;
            }

            SetProcess(processModel, gameLanguage, patchVersion, ignoreJSONCache, scanAllMemoryRegions);
        }

        public string GameLanguage { get; set; }
        internal bool IgnoreJSONCache { get; set; }
        public long ScanCount { get; set; }
        public Structures Structures { get; set; }

        ~MemoryHandler()
        {
            try
            {
                UnsafeNativeMethods.CloseHandle(Instance.ProcessHandle);
            }
            catch (Exception ex)
            {
                RaiseException(Logger, ex);
            }
        }

        public void SetProcess(ProcessModel processModel, string gameLanguage = "English", string patchVersion = "latest", bool ignoreJSONCache = false, bool scanAllMemoryRegions = false)
        {
            ProcessModel = processModel;
            GameLanguage = gameLanguage;
            IgnoreJSONCache = ignoreJSONCache;
            try
            {
                ProcessHandle = UnsafeNativeMethods.OpenProcess(UnsafeNativeMethods.ProcessAccessFlags.PROCESS_VM_ALL, false, (uint) ProcessModel.ProcessID);
            }
            catch (Exception)
            {
                ProcessHandle = processModel.Process.Handle;
            }
            Constants.ProcessHandle = ProcessHandle;

            _systemModules = GetProcessModules();

            SetStructures(processModel, patchVersion);

            Scanner.Instance.Locations.Clear();
            Scanner.Instance.LoadOffsets(Signatures.Resolve(processModel, patchVersion), scanAllMemoryRegions);

            ActionHelper.Resolve();
            StatusEffectHelper.Resolve();
            ZoneHelper.Resolve();
        }

        public void SetStructures(ProcessModel processModel, string patchVersion = "latest")
        {
            Structures = APIHelper.GetStructures(processModel, patchVersion);
        }

        public IntPtr ResolvePointerPath(IEnumerable<long> path, IntPtr baseAddress, bool IsASMSignature = false)
        {
            var nextAddress = baseAddress;
            foreach (var offset in path)
            {
                try
                {
                    baseAddress = new IntPtr(nextAddress.ToInt64() + offset);
                    if (baseAddress == IntPtr.Zero)
                    {
                        return IntPtr.Zero;
                    }

                    if (IsASMSignature)
                    {
                        nextAddress = baseAddress + Instance.GetInt32(new IntPtr(baseAddress.ToInt64())) + 4;
                        IsASMSignature = false;
                    }
                    else
                    {
                        nextAddress = Instance.ReadPointer(baseAddress);
                    }
                }
                catch
                {
                    return IntPtr.Zero;
                }
            }
            return baseAddress;
        }

        public IntPtr GetStaticAddress(long offset)
        {
            return new IntPtr(Instance.ProcessModel.Process.MainModule.BaseAddress.ToInt64() + offset);
        }

        /// <summary>
        /// </summary>
        /// <param name="address"> </param>
        /// <param name="buffer"> </param>
        /// <returns> </returns>
        public bool Peek(IntPtr address, byte[] buffer)
        {
            IntPtr lpNumberOfBytesRead;
            return UnsafeNativeMethods.ReadProcessMemory(Instance.ProcessHandle, address, buffer, new IntPtr(buffer.Length), out lpNumberOfBytesRead);
        }

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public byte GetByte(IntPtr address, long offset = 0)
        {
            var data = new byte[1];
            Peek(new IntPtr(address.ToInt64() + offset), data);
            return data[0];
        }

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] GetByteArray(IntPtr address, int length)
        {
            var data = new byte[length];
            Peek(address, data);
            return data;
        }

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public short GetInt16(IntPtr address, long offset = 0)
        {
            var value = new byte[2];
            Peek(new IntPtr(address.ToInt64() + offset), value);
            return BitConverter.TryToInt16(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public int GetInt32(IntPtr address, long offset = 0)
        {
            var value = new byte[4];
            Peek(new IntPtr(address.ToInt64() + offset), value);
            return BitConverter.TryToInt32(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public long GetInt64(IntPtr address, long offset = 0)
        {
            var value = new byte[8];
            Peek(new IntPtr(address.ToInt64() + offset), value);
            return BitConverter.TryToInt64(value, 0);
        }

        public long GetPlatformInt(IntPtr address, long offset = 0)
        {
            var bytes = new byte[ProcessModel.IsWin64 ? 8 : 4];
            Peek(new IntPtr(address.ToInt64() + offset), bytes);
            return GetPlatformIntFromBytes(bytes);
        }

        public long GetPlatformIntFromBytes(byte[] source, int index = 0)
        {
            if (ProcessModel.IsWin64)
            {
                return BitConverter.TryToInt64(source, index);
            }
            return BitConverter.TryToInt32(source, index);
        }

        public IntPtr ReadPointer(IntPtr address, long offset = 0)
        {
            if (ProcessModel.IsWin64)
            {
                var win64 = new byte[8];
                Peek(new IntPtr(address.ToInt64() + offset), win64);
                return new IntPtr(BitConverter.TryToInt64(win64, 0));
            }
            var win32 = new byte[4];
            Peek(new IntPtr(address.ToInt64() + offset), win32);
            return IntPtr.Add(IntPtr.Zero, BitConverter.TryToInt32(win32, 0));
        }

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public string GetString(IntPtr address, long offset = 0, int size = 256)
        {
            var bytes = new byte[size];
            Peek(new IntPtr(address.ToInt64() + offset), bytes);
            var realSize = 0;
            for (var i = 0; i < size; i++)
            {
                if (bytes[i] != 0)
                {
                    continue;
                }
                realSize = i;
                break;
            }
            Array.Resize(ref bytes, realSize);
            return Encoding.UTF8.GetString(bytes);
        }

        public string GetStringFromBytes(byte[] source, int offset = 0, int size = 256)
        {
            var safeSize = source.Length - offset;
            if (safeSize < size)
            {
                size = safeSize;
            }
            var bytes = new byte[size];
            Array.Copy(source, offset, bytes, 0, size);
            var realSize = 0;
            for (var i = 0; i < size; i++)
            {
                if (bytes[i] != 0)
                {
                    continue;
                }
                realSize = i;
                break;
            }
            Array.Resize(ref bytes, realSize);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public ushort GetUInt16(IntPtr address, long offset = 0)
        {
            var value = new byte[4];
            Peek(new IntPtr(address.ToInt64() + offset), value);
            return BitConverter.TryToUInt16(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public uint GetUInt32(IntPtr address, long offset = 0)
        {
            var value = new byte[4];
            Peek(new IntPtr(address.ToInt64() + offset), value);
            return BitConverter.TryToUInt32(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public ulong GetUInt64(IntPtr address, long offset = 0)
        {
            var value = new byte[8];
            Peek(new IntPtr(address.ToInt64() + offset), value);
            return BitConverter.TryToUInt32(value, 0);
        }

        public long GetPlatformUInt(IntPtr address, long offset = 0)
        {
            var bytes = new byte[ProcessModel.IsWin64 ? 8 : 4];
            Peek(new IntPtr(address.ToInt64() + offset), bytes);
            return GetPlatformUIntFromBytes(bytes);
        }

        public long GetPlatformUIntFromBytes(byte[] source, int index = 0)
        {
            if (ProcessModel.IsWin64)
            {
                return (long) BitConverter.TryToUInt64(source, index);
            }
            return BitConverter.TryToUInt32(source, index);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public T GetStructure<T>(IntPtr address, int offset = 0)
        {
            IntPtr lpNumberOfBytesRead;
            var buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(T)));
            UnsafeNativeMethods.ReadProcessMemory(ProcessModel.Process.Handle, address + offset, buffer, new IntPtr(Marshal.SizeOf(typeof(T))), out lpNumberOfBytesRead);
            var retValue = (T) Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeCoTaskMem(buffer);
            return retValue;
        }

        private List<ProcessModule> GetProcessModules()
        {
            var modules = ProcessModel.Process.Modules;
            var result = new List<ProcessModule>(modules.Count);

            for (var i = 0; i < modules.Count; i++)
            {
                var module = modules[i];
                result.Add(module);
            }

            return result;
        }

        /// <summary>
        ///     Allows the user to find out what module an address is in.
        /// </summary>
        /// <param name="address">Int32 Address</param>
        /// <returns>ProcessModule</returns>
        internal ProcessModule GetModuleByAddress(IntPtr address)
        {
            try
            {
                for (var i = 0; i < _systemModules.Count; i++)
                {
                    var module = _systemModules[i];
                    var baseAddress = ProcessModel.IsWin64 ? module.BaseAddress.ToInt64() : module.BaseAddress.ToInt32();
                    if (baseAddress <= (long) address && baseAddress + module.ModuleMemorySize >= (long) address)
                    {
                        return module;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal bool IsSystemModule(IntPtr address)
        {
            var moduleByAddress = GetModuleByAddress(address);
            if (moduleByAddress != null)
            {
                foreach (var module in _systemModules)
                {
                    if (module.ModuleName == moduleByAddress.ModuleName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #region Private Structs

        internal struct MemoryBlock
        {
            public long Length;
            public long Start;
        }

        #endregion

        #region Event Raising

        public event EventHandler<ExceptionEvent> ExceptionEvent = delegate { };

        protected internal virtual void RaiseException(Logger logger, Exception e, bool levelIsError = false)
        {
            ExceptionEvent?.Invoke(this, new ExceptionEvent(this, logger, e, levelIsError));
        }

        public event EventHandler<SignaturesFoundEvent> SignaturesFoundEvent = delegate { };

        protected internal virtual void RaiseSignaturesFound(Logger logger, Dictionary<string, Signature> signatures, long processingTime)
        {
            SignaturesFoundEvent?.Invoke(this, new SignaturesFoundEvent(this, logger, signatures, processingTime));
        }

        #endregion

        #region Property Bindings

        private static Lazy<MemoryHandler> _instance = new Lazy<MemoryHandler>(() => new MemoryHandler(null));

        public ProcessModel ProcessModel { get; set; }

        public IntPtr ProcessHandle { get; set; }

        public static MemoryHandler Instance
        {
            get { return _instance.Value; }
        }

        #endregion
    }
}
