using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Sharlayan.Models;

namespace Sharlayan.OS.OSX
{
    public class NativeMemoryHandler: INativeMemoryHandler {
        public IList<NativeMemoryRegionInfo> LoadRegions(IntPtr handle, ProcessModel process,
                                                         List<ProcessModule> systemModules, bool scanAllRegions)
        {
            IntPtr address = IntPtr.Zero;
            IntPtr size = IntPtr.Zero;
            List<NativeMemoryRegionInfo> regions = new List<NativeMemoryRegionInfo>();
            var depth = 1;
            int result;
            var safety = 0;

            do {
                var info = new UnsafeNativeMethods.vm_region_info_64_t();
                var infoCnt = UnsafeNativeMethods.VM_REGION_SUBMAP_INFO_COUNT_64;
                result = UnsafeNativeMethods.VmRegionRecurse64(handle, ref address, ref size, ref depth, ref info, ref infoCnt);
                if (result == UnsafeNativeMethods.KERN_INVALID_ADDRESS)
                    break;

                {
                    if ((info.Protection & UnsafeNativeMethods.VM_PROT_READ) == UnsafeNativeMethods.VM_PROT_READ &&
                        (info.Protection & UnsafeNativeMethods.VM_PROT_EXECUTE) == UnsafeNativeMethods.VM_PROT_EXECUTE &&
                        info.Offset == IntPtr.Zero &&
                        depth == 0) {
                        regions.Add(
                            new NativeMemoryRegionInfo {
                                BaseAddress = address,
                                RegionSize = size,
                            });
                    }

                    //Console.WriteLine($"Found region: {(ulong) address:x} to {(ulong) (address.ToInt64() + size.ToInt64()):x} ({depth})  (result: {result}, [{info.Protection}, {info.MaxProtection}, {info.Inheritance}, {info.Offset}, {info.UserTag}, {info.PagesResident}])");

                    address = new IntPtr(address.ToInt64() + size.ToInt64());
                }

                safety++;
                if (safety > 100000)
                {
                    break;
                }
            } while (result == 0);

            return regions;
        }

        private void RequestRights() {

            var flags = UnsafeNativeMethods.AuthorizationFlagExtendRights | UnsafeNativeMethods.AuthorizationFlagPreAuthorize | UnsafeNativeMethods.AuthorizationFlagInteractionAllowed | ( 1 << 5);
            var stat = UnsafeNativeMethods.AuthorizationCreate(IntPtr.Zero, IntPtr.Zero, flags, out var author);
            var rights = new UnsafeNativeMethods.AuthorizationItemSet {
                Count = 1,
                Items = new[] {
                    new UnsafeNativeMethods.AuthorizationItem {
                        Name = "system.privilege.taskport:",
                    },
                },
            };
            
            if (stat != UnsafeNativeMethods.ErrAuthorizationSuccess) {
                throw new Exception($"Could not AuthorizationCreate (stat: {stat})");
            }

            stat = UnsafeNativeMethods.AuthorizationCopyRights(author, rights, IntPtr.Zero, flags,out var givenRights);
            if (stat != UnsafeNativeMethods.ErrAuthorizationSuccess) {
                throw new Exception("Could not AuthorizationCopyRights (stat: {stats})");
            }
        }

        public IntPtr OpenProcess(int processId)
        {
            //RequestRights();
            
            IntPtr port = IntPtr.Zero;
            var taskResult = UnsafeNativeMethods.TaskForPid(UnsafeNativeMethods.machTaskSelf(), processId, ref port);
            if (taskResult != 0)
            {
                throw new Exception(
                    $"No success from task_for_pid, cannot read memory ({UnsafeNativeMethods.machErrorString(taskResult)}");
            }
            
            return port;
        }
        
        public int CloseHandle(IntPtr handler)
        {
            return 0;
        }
        
        public bool ReadMemory(IntPtr handle, IntPtr address, byte[] buffer)
        {
            return this.ReadMemory(handle, address, buffer, new IntPtr(buffer.Length));
        }

        public bool ReadMemory(IntPtr handle, IntPtr address, byte[] buffer, IntPtr size)
        {
            // read memory - vm_read_overwrite because we supply the buffer
            var read = 0;
            IntPtr pbuffer = Marshal.AllocHGlobal(size);

            var result = UnsafeNativeMethods.MachVmReadOverwrite(handle, address, size, pbuffer, ref read);
            if (result != 0)
            {
                Marshal.FreeHGlobal(pbuffer);
                Console.WriteLine($"vm_read failed! result: {result}, requested size: {size} read: {read}");
                return true; // Silently fail so we got forward
            }
            else if (read != size.ToInt32())
            {
                Console.WriteLine($"vm_read failed! requested size: {size} read: {read}");
            }
            
            Marshal.Copy(pbuffer, buffer, 0, read);
            Marshal.FreeHGlobal(pbuffer);

            return true; // size.ToInt32() == read;
        }

        public bool ReadMemory(IntPtr handle, IntPtr address, IntPtr buffer, IntPtr size)
        {
            // read memory - vm_read_overwrite because we supply the buffer
            var read = 0;

            var result = UnsafeNativeMethods.MachVmReadOverwrite(handle, address, size, buffer, ref read);
            if (result != 0)
            {
                //throw new Exception($"vm_read failed! {result} ({UnsafeNativeMethods.machErrorString(result)})");
                Console.WriteLine($"vm_read failed! result: {result}, requested size: {size} read: {read}");
            }
            else if (read != size.ToInt32())
            {
                Console.WriteLine($"vm_read failed! requested size: {size} read: {read}");
            }

            return true; //size.ToInt32() == read;
        }
    }
}