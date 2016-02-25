// FFXIVAPP.Memory ~ MemoryHandler.cs
// 
// Copyright © 2007 - 2016 Ryan Wilson - All Rights Reserved
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
using System.Runtime.InteropServices;
using System.Text;
using FFXIVAPP.Memory.Models;

namespace FFXIVAPP.Memory
{
    public class MemoryHandler
    {
        /// <summary>
        /// </summary>
        /// <param name="processModel"> </param>
        /// <param name="gameLanguage"></param>
        public MemoryHandler(ProcessModel processModel, string gameLanguage = "English")
        {
            GameLanguage = gameLanguage;
            if (processModel != null)
            {
                SetProcess(processModel, gameLanguage);
            }
        }

        public string GameLanguage { get; set; }
        public int ScanCount { get; set; }

        ~MemoryHandler()
        {
            try
            {
                UnsafeNativeMethods.CloseHandle(Instance.ProcessHandle);
            }
            catch (Exception ex)
            {
            }
        }

        public void SetProcess(ProcessModel processModel, string gameLanguage = "English")
        {
            ProcessModel = processModel;
            GameLanguage = gameLanguage;
            try
            {
                ProcessHandle = UnsafeNativeMethods.OpenProcess(UnsafeNativeMethods.ProcessAccessFlags.PROCESS_VM_ALL, false, (uint) ProcessModel.ProcessID);
            }
            catch (Exception ex)
            {
                ProcessHandle = processModel.Process.Handle;
            }
            Constants.ProcessHandle = ProcessHandle;
            Scanner.Instance.Locations.Clear();
            Scanner.Instance.LoadOffsets(Signatures.Resolve(ProcessModel.IsWin64, GameLanguage));
        }

        public IntPtr ResolvePointerPath(IEnumerable<long> path, IntPtr baseAddress, bool ASMSignature = false)
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

                    if (ASMSignature)
                    {
                        nextAddress = baseAddress + Instance.GetInt32(new IntPtr(baseAddress.ToInt64())) + 4;
                        ASMSignature = false;
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
        private bool Peek(IntPtr address, byte[] buffer)
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
            return BitConverter.ToInt16(value, 0);
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
            return BitConverter.ToInt32(value, 0);
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
            return BitConverter.ToInt64(value, 0);
        }

        public long GetPlatformInt(IntPtr address, long offset = 0)
        {
            if (ProcessModel.IsWin64)
            {
                var win64 = new byte[8];
                Peek(new IntPtr(address.ToInt64() + offset), win64);
                return BitConverter.ToInt64(win64, 0);
            }
            var win32 = new byte[4];
            Peek(new IntPtr(address.ToInt64() + offset), win32);
            return BitConverter.ToInt32(win32, 0);
        }

        public IntPtr ReadPointer(IntPtr address, long offset = 0)
        {
            if (ProcessModel.IsWin64)
            {
                var win64 = new byte[8];
                Peek(new IntPtr(address.ToInt64() + offset), win64);
                return IntPtr.Add(IntPtr.Zero, (int) BitConverter.ToInt64(win64, 0));
            }
            var win32 = new byte[4];
            Peek(new IntPtr(address.ToInt64() + offset), win32);
            return IntPtr.Add(IntPtr.Zero, BitConverter.ToInt32(win32, 0));
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
            return BitConverter.ToUInt16(value, 0);
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
            return BitConverter.ToUInt32(value, 0);
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
            return BitConverter.ToUInt32(value, 0);
        }

        public long GetPlatformUInt(IntPtr address, long offset = 0)
        {
            if (ProcessModel.IsWin64)
            {
                var win64 = new byte[8];
                Peek(new IntPtr(address.ToInt64() + offset), win64);
                return (long) BitConverter.ToUInt64(win64, 0);
            }
            var win32 = new byte[4];
            Peek(new IntPtr(address.ToInt64() + offset), win32);
            return BitConverter.ToUInt32(win32, 0);
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
            var buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof (T)));
            UnsafeNativeMethods.ReadProcessMemory(ProcessModel.Process.Handle, address + offset, buffer, new IntPtr(Marshal.SizeOf(typeof (T))), out lpNumberOfBytesRead);
            var retValue = (T) Marshal.PtrToStructure(buffer, typeof (T));
            Marshal.FreeCoTaskMem(buffer);
            return retValue;
        }

        #region Private Structs

        internal struct MemoryBlock
        {
            public long Length;
            public long Start;
        }

        #endregion

        #region Property Bindings

        private static MemoryHandler _instance;

        public ProcessModel ProcessModel { get; set; }

        public IntPtr ProcessHandle { get; set; }

        public static MemoryHandler Instance
        {
            get { return _instance ?? (_instance = new MemoryHandler(null)); }
            set { _instance = value; }
        }

        #endregion
    }
}
