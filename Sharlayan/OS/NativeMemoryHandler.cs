using WinHandler = Sharlayan.OS.Windows.NativeMemoryHandler;
using LinuxHandler = Sharlayan.OS.Linux.NativeMemoryHandler;
using OSXHandler = Sharlayan.OS.OSX.NativeMemoryHandler;
using System.Runtime.InteropServices;

namespace Sharlayan.OS
{
    /// <summary>
    /// Abstracts memory access for different os
    /// </summary>
    public static class NativeMemoryHandler
    {
        private static INativeMemoryHandler _instance = null;

        public static INativeMemoryHandler Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return (_instance = new LinuxHandler());
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                    return (_instance = new OSXHandler());
                }

                return (_instance = new WinHandler());
            }
        }
   }
}