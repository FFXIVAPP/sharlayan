using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Sharlayan.Models;

namespace Sharlayan.OS.Windows
{
    public class NativeMemoryHandler: INativeMemoryHandler
    {
        private const int MemCommit = 0x1000;

        private const int PageExecuteReadwrite = 0x40;

        private const int PageExecuteWritecopy = 0x80;

        private const int PageGuard = 0x100;

        private const int PageNoAccess = 0x01;

        private const int PageReadwrite = 0x04;

        private const int PageWritecopy = 0x08;

        private const int Writable = PageReadwrite | PageWritecopy | PageExecuteReadwrite | PageExecuteWritecopy | PageGuard;

        public bool ReadMemory(IntPtr handle, IntPtr address, byte[] buffer)
        {
            return ReadMemory(handle, address, buffer, new IntPtr(buffer.Length));
        }

        public bool ReadMemory(IntPtr handle, IntPtr address, byte[] buffer, IntPtr size)
        {
            return UnsafeNativeMethods.ReadProcessMemory(handle, address, buffer, size, out _);
        }

        public bool ReadMemory(IntPtr handle, IntPtr address, IntPtr buffer, IntPtr size)
        {
            return UnsafeNativeMethods.ReadProcessMemory(handle, address, buffer, size, out _);
        }

        public IntPtr OpenProcess(int processId)
        {
            return UnsafeNativeMethods.OpenProcess(UnsafeNativeMethods.ProcessAccessFlags.PROCESS_VM_ALL, false, (uint) processId);
        }

        public int CloseHandle(IntPtr hObject)
        {
            return UnsafeNativeMethods.CloseHandle(hObject);
        }

        public IList<NativeMemoryRegionInfo> LoadRegions(IntPtr handle, ProcessModel process, List<ProcessModule> systemModules, bool scanAllRegions)
        {
            var regions = new List<NativeMemoryRegionInfo>();
            var moduleRegion = new NativeMemoryRegionInfo
            {
                BaseAddress = process.Process.MainModule.BaseAddress,
                RegionSize = new IntPtr(process.Process.MainModule.ModuleMemorySize)
            };

            regions.Add(moduleRegion);

            IntPtr address = IntPtr.Zero;
            while (true) {
                var info = new UnsafeNativeMethods.MEMORY_BASIC_INFORMATION();
                var result = UnsafeNativeMethods.VirtualQueryEx(handle, address, out info, (uint)Marshal.SizeOf(info));
                if (result == 0) {
                    break;
                }

                if (!IsSystemModule(info.BaseAddress, process.IsWin64, systemModules) && (info.State & MemCommit) != 0 && (info.Protect & Writable) != 0 && (info.Protect & PageGuard) == 0) {
                    if (moduleRegion.BaseAddress != info.BaseAddress)
                    {
                        regions.Add(new NativeMemoryRegionInfo
                        {
                            BaseAddress = info.BaseAddress,
                            RegionSize = info.RegionSize
                        });
                    }
                }
                else {
                    new Exception(info.ToString());
                    // TODO:
                    //MemoryHandler.Instance.RaiseException(Logger, new Exception(info.ToString()));
                }

                unchecked {
                    switch (IntPtr.Size) {
                        case sizeof(int):
                            address = new IntPtr(info.BaseAddress.ToInt32() + info.RegionSize.ToInt32());
                            break;
                        default:
                            address = new IntPtr(info.BaseAddress.ToInt64() + info.RegionSize.ToInt64());
                            break;
                    }
                }
            }

            return regions;
        }

        private bool IsSystemModule(IntPtr address, bool isWin64, List<ProcessModule> systemModules) {
            ProcessModule moduleByAddress = this.GetModuleByAddress(address, isWin64, systemModules);
            if (moduleByAddress != null) {
                foreach (ProcessModule module in systemModules) {
                    if (module.ModuleName == moduleByAddress.ModuleName) {
                        return true;
                    }
                }
            }

            return false;
        }

        private ProcessModule GetModuleByAddress(IntPtr address, bool isWin64, List<ProcessModule> systemModules) {
            try {
                for (var i = 0; i < systemModules.Count; i++) {
                    ProcessModule module = systemModules[i];
                    var baseAddress = isWin64
                                          ? module.BaseAddress.ToInt64()
                                          : module.BaseAddress.ToInt32();
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
    }
}
