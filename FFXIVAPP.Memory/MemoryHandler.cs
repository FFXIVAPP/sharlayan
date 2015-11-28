// FFXIVAPP.Memory
// MemoryHandler.cs
// 
// Copyright © 2007 - 2015 Ryan Wilson - All Rights Reserved
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions are met: 
// 
//  * Redistributions of source code must retain the above copyright notice, 
//    this list of conditions and the following disclaimer. 
//  * Redistributions in binary form must reproduce the above copyright 
//    notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution. 
//  * Neither the name of SyndicatedLife nor the names of its contributors may 
//    be used to endorse or promote products derived from this software 
//    without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE. 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using FFXIVAPP.Memory.Models;

namespace FFXIVAPP.Memory
{
    public class MemoryHandler : INotifyPropertyChanged
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
        public static int ScanCount { get; set; }

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
            SigScanner.Locations.Clear();
        }

        public static IntPtr ResolvePointerPath(IEnumerable<long> path, IntPtr baseAddress, bool ASMSignature = false)
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

        public static IntPtr GetStaticAddress(long offset)
        {
            return new IntPtr(Instance.ProcessModel.Process.MainModule.BaseAddress.ToInt64() + offset);
        }

        /// <summary>
        /// </summary>
        /// <param name="address"> </param>
        /// <param name="buffer"> </param>
        /// <returns> </returns>
        private static bool Peek(IntPtr address, byte[] buffer)
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

        public struct MemoryBlock
        {
            public long Length;
            public long Start;
        }

        #endregion

        #region Property Bindings

        private static MemoryHandler _instance;
        //private Dictionary<string, List<long>> _pointerPaths;
        private IntPtr _processHandle;
        private ProcessModel _processModel;
        private SigScanner _sigScanner;
        internal static int _scanCount = 0;

        public static int scanCount
        {
            get { return _scanCount; }
        }

        public ProcessModel ProcessModel
        {
            get { return _processModel; }
            set
            {
                _processModel = value;
                RaisePropertyChanged();
            }
        }

        public IntPtr ProcessHandle
        {
            get { return _processHandle; }
            set
            {
                _processHandle = value;
                RaisePropertyChanged();
            }
        }

        public static MemoryHandler Instance
        {
            get { return _instance ?? (_instance = new MemoryHandler(null)); }
            set { _instance = value; }
        }

        public SigScanner SigScanner
        {
            get { return _sigScanner ?? (_sigScanner = new SigScanner()); }
            set
            {
                if (_sigScanner == null)
                {
                    _sigScanner = new SigScanner();
                }
                _sigScanner = value;
            }
        }

        #endregion

        #region Declarations

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}
