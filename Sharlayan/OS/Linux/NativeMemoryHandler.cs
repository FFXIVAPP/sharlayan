using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Sharlayan.Models;

namespace Sharlayan.OS.Linux
{
    public class NativeMemoryHandler : INativeMemoryHandler
    {
        private long maxIoVcnt;

        public NativeMemoryHandler()
        {
            maxIoVcnt = UnsafeNativeMethods.sysconf(UnsafeNativeMethods._SC_IOV_MAX);
        }

        public int CloseHandle(IntPtr hObject)
        {
            return 0;
        }

        public IList<NativeMemoryRegionInfo> LoadRegions(IntPtr handle, ProcessModel process, List<ProcessModule> systemModules, bool scanAllRegions)
        {
            int COL_ADDR = 0;
            int COL_PERM = 1;
            int COL_OFFSET = 2;
            int COL_DEVICE = 3;
            int COL_INODE = 4;
            int COL_PATHNAME = 5;

            Func<string, string[]> splitMaps = s =>
            {
                var ret = new string[6];

                if (s.Length < 42)
                {
                    throw new Exception($"Invalid length of mmap line \"{s}\" ({s.Length} should be at least 42)");
                }

                ret[COL_ADDR] = s.Substring(0, 19);
                ret[COL_PERM] = s.Substring(20, 4);
                ret[COL_OFFSET] = s.Substring(25, 8);
                ret[COL_DEVICE] = s.Substring(34, 5);
                if (s.Length == 42)
                {
                    ret[COL_INODE] = s.Substring(40, 1);
                    ret[COL_PATHNAME] = "";
                }
                else
                {
                    ret[COL_INODE] = s.Substring(40, 7);
                    ret[COL_PATHNAME] = s.Substring(48).Trim();
                }

                return ret;
            };

            var regions = new List<NativeMemoryRegionInfo>();
            var found = false;
            foreach(var line in System.IO.File.ReadAllLines($"/proc/{handle}/maps"))
            {
                if (!found && line.EndsWith(process.ProcessName) == false)
                {
                    continue;
                }

                found = true;

                var cols = splitMaps(line);

                // Not an empty pathname and it is not ending with our process name, then we are done
                if (line.EndsWith(process.ProcessName) == false && (cols[COL_PATHNAME]?.Length ?? 0) > 0)
                {
                    break;
                }

                var addr = cols[COL_ADDR].Split('-');
                var start = long.Parse(addr[0], System.Globalization.NumberStyles.HexNumber);
                var end = long.Parse(addr[1], System.Globalization.NumberStyles.HexNumber);

                regions.Add(new NativeMemoryRegionInfo
                {
                    BaseAddress = new IntPtr(start),
                    RegionSize = new IntPtr(end - start)
                });
            }

            return regions;
        }

        public IntPtr OpenProcess(int processId)
        {
            return new IntPtr(processId);
        }

        public bool ReadMemory(IntPtr handle, IntPtr address, byte[] buffer)
        {
            return ReadMemory(handle, address, buffer, new IntPtr(buffer.Length));
        }

        public bool ReadMemory(IntPtr handle, IntPtr address, byte[] buffer, IntPtr size)
        {
            var iov_size = 2048;
            var piov_size = new IntPtr(iov_size);
            var count = Math.Max((int)Math.Ceiling((decimal)size.ToInt64() / iov_size), 1);

            if (size.ToInt64() < iov_size)
            {
                iov_size = size.ToInt32();
                piov_size = size;
            }

            if (count > maxIoVcnt)
            {
                throw new Exception("Trying to read too much!");
            }

            var bytes = new byte[count,iov_size];
            var local = new UnsafeNativeMethods.iovec[count];
            var pointers = new IntPtr[count];
            var remote = new UnsafeNativeMethods.iovec[1] { new UnsafeNativeMethods.iovec { iov_base = address, iov_len = size } };

            for(var i = 0; i < local.Length; i++)
            {
                pointers[i] = Marshal.AllocHGlobal(iov_size);
                local[i].iov_base = pointers[i];
                local[i].iov_len = piov_size;
            }

            local[local.Length- 1].iov_len = new IntPtr(iov_size - ((iov_size * count) - size.ToInt64()));

            var nread = UnsafeNativeMethods.process_vm_readv(handle, local, (ulong)local.Length, remote, (ulong)remote.Length, 0);

            for(var i = 0; i < local.Length; i++)
            {
                Marshal.Copy(pointers[i], buffer, (i*iov_size), local[i].iov_len.ToInt32());
                Marshal.FreeHGlobal(pointers[i]);
            }

            return nread.ToInt64() == remote[0].iov_len.ToInt64();
        }

        public bool ReadMemory(IntPtr handle, IntPtr address, IntPtr buffer, IntPtr size)
        {
            var iov_size = 2048;
            var piov_size = new IntPtr(iov_size);
            var count = Math.Max((int)Math.Ceiling((decimal)size.ToInt64() / iov_size), 1);

            if (size.ToInt64() < iov_size)
            {
                iov_size = size.ToInt32();
                piov_size = size;
            }

            if (count > maxIoVcnt)
            {
                throw new Exception("Trying to read too much!");
            }

            var local = new UnsafeNativeMethods.iovec[count];
            var remote = new UnsafeNativeMethods.iovec[1] { new UnsafeNativeMethods.iovec { iov_base = address, iov_len = size } };

            for(var i = 0; i < local.Length; i++)
            {
                local[i].iov_base = IntPtr.Add(buffer, i * iov_size);
                local[i].iov_len = piov_size;
            }

            local[local.Length- 1].iov_len = new IntPtr(iov_size - ((iov_size * count) - size.ToInt64()));

            var nread = UnsafeNativeMethods.process_vm_readv(handle, local, (ulong)local.Length, remote, (ulong)remote.Length, 0);

            return nread.ToInt64() == remote[0].iov_len.ToInt64();
        }
    }
}