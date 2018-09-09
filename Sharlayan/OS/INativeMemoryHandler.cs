using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sharlayan.Models;

namespace Sharlayan.OS {
    public interface INativeMemoryHandler
    {
        bool ReadMemory(IntPtr handle, IntPtr address, byte[] buffer);
        bool ReadMemory(IntPtr handle, IntPtr address, byte[] buffer, IntPtr size);
        bool ReadMemory(IntPtr handle, IntPtr address, IntPtr buffer, IntPtr size);
        IntPtr OpenProcess(int processId);
        int CloseHandle(IntPtr hObject);
        IList<NativeMemoryRegionInfo> LoadRegions(IntPtr handle, ProcessModel process, List<ProcessModule> systemModules, bool scanAllRegions);
    }
}