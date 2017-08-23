// Sharlayan ~ UnsafeNativeMethods.cs
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
using System.Runtime.InteropServices;
using System.Text;

namespace Sharlayan
{
    internal static class UnsafeNativeMethods
    {
        public enum ProcessAccessFlags
        {
            PROCESS_VM_ALL = 0x001F0FFF
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr lpBaseAddress, [In] [Out] byte[] lpBuffer, IntPtr regionSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr lpBaseAddress, [In] [Out] IntPtr lpBuffer, IntPtr regionSize, out IntPtr lpNumberOfBytesRead);

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

            public override string ToString()
            {
                var sb = new StringBuilder();

                sb.AppendFormat($"BaseAddress:{BaseAddress}{Environment.NewLine}");
                sb.AppendFormat($"AllocationBase:{AllocationBase}{Environment.NewLine}");
                sb.AppendFormat($"AllocationProtect:{AllocationProtect}{Environment.NewLine}");
                sb.AppendFormat($"RegionSize:{RegionSize}{Environment.NewLine}");
                sb.AppendFormat($"State:{State}{Environment.NewLine}");
                sb.AppendFormat($"Protect:{Protect}{Environment.NewLine}");
                sb.AppendFormat($"Type:{Type}{Environment.NewLine}");

                return sb.ToString();
            }
        }
    }
}
