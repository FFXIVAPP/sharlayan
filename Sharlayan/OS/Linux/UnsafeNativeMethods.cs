using System;
using System.Runtime.InteropServices;

namespace Sharlayan.OS.Linux
{
    internal static class UnsafeNativeMethods {
        [StructLayout(LayoutKind.Sequential)]
        public struct iovec
        {
            public IntPtr iov_base;
            public IntPtr iov_len;
        }

        public const int _SC_IOV_MAX = 60;

        [DllImport("libc.so.6")]
        public static extern long sysconf(int name);

        [DllImport("libc.so.6")]
        public static extern IntPtr process_vm_readv(IntPtr pid, iovec[] local_iov, ulong liovcnt, iovec[] remote_iov, ulong riovcnt, ulong flags);
    }
}