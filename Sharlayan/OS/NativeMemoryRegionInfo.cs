using System;

namespace Sharlayan.OS {
    public struct NativeMemoryRegionInfo
    {
        public IntPtr BaseAddress { get; set; }
        public IntPtr RegionSize { get; set; }
    }
}
