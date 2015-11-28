// FFXIVAPP.Memory
// UnsafeNativeMethods.cs
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
using System.Runtime.InteropServices;

namespace FFXIVAPP.Memory
{
    internal static class UnsafeNativeMethods
    {
        public enum ProcessAccessFlags
        {
            PROCESS_VM_ALL = 0x001F0FFF
        }

        /// <summary>
        /// </summary>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="bInheritHandle"></param>
        /// <param name="dwProcessId"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, UInt32 dwProcessId);

        /// <summary>
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Int32 CloseHandle(IntPtr hObject);

        /// <summary>
        /// </summary>
        /// <param name="processHandle"> </param>
        /// <param name="lpBaseAddress"> </param>
        /// <param name="lpBuffer"> </param>
        /// <param name="regionSize"> </param>
        /// <param name="lpNumberOfBytesRead"> </param>
        /// <returns> </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr lpBaseAddress, [In] [Out] Byte[] lpBuffer, IntPtr regionSize, out IntPtr lpNumberOfBytesRead);

        /// <summary>
        /// </summary>
        /// <param name="processHandle"> </param>
        /// <param name="lpBaseAddress"> </param>
        /// <param name="lpBuffer"> </param>
        /// <param name="regionSize"> </param>
        /// <param name="lpNumberOfBytesRead"> </param>
        /// <returns> </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr lpBaseAddress, [In] [Out] IntPtr lpBuffer, IntPtr regionSize, out IntPtr lpNumberOfBytesRead);

        /// <summary>
        /// </summary>
        /// <param name="processHandle"> </param>
        /// <param name="lpBaseAddress"> </param>
        /// <param name="lpBuffer"> </param>
        /// <param name="regionSize"> </param>
        /// <param name="lpNumberOfBytesRead"> </param>
        /// <returns> </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr lpBaseAddress, byte[] lpBuffer, uint regionSize, out IntPtr lpNumberOfBytesRead);

        /// <summary>
        /// </summary>
        /// <param name="processHandle"> </param>
        /// <param name="lpBaseAddress"> </param>
        /// <param name="lpBuffer"> </param>
        /// <param name="dwLength"> </param>
        /// <returns> </returns>
        [DllImport("kernel32.dll")]
        public static extern int VirtualQueryEx(IntPtr processHandle, IntPtr lpBaseAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }
    }
}
