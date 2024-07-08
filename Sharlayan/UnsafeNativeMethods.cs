// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnsafeNativeMethods.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   UnsafeNativeMethods.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    public static class UnsafeNativeMethods {
        public enum ProcessAccessFlags {
            PROCESS_VM_ALL = 0x001F0FFF,
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true),]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool),] bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true),]
        public static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true),]
        public static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr lpBaseAddress, [In,] [Out,] byte[] lpBuffer, IntPtr regionSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true),]
        public static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr lpBaseAddress, [In,] [Out,] IntPtr lpBuffer, IntPtr regionSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll"),]
        public static extern int VirtualQueryEx(IntPtr processHandle, IntPtr lpBaseAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [StructLayout(LayoutKind.Sequential),]
        public struct MEMORY_BASIC_INFORMATION {
            public IntPtr BaseAddress;

            public IntPtr AllocationBase;

            public uint AllocationProtect;

            public IntPtr RegionSize;

            public uint State;

            public uint Protect;

            public uint Type;

            public override string ToString() {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat($"BaseAddress:{this.BaseAddress}{Environment.NewLine}");
                sb.AppendFormat($"AllocationBase:{this.AllocationBase}{Environment.NewLine}");
                sb.AppendFormat($"AllocationProtect:{this.AllocationProtect}{Environment.NewLine}");
                sb.AppendFormat($"RegionSize:{this.RegionSize}{Environment.NewLine}");
                sb.AppendFormat($"State:{this.State}{Environment.NewLine}");
                sb.AppendFormat($"Protect:{this.Protect}{Environment.NewLine}");
                sb.AppendFormat($"Type:{this.Type}{Environment.NewLine}");

                return sb.ToString();
            }
        }
    }
}