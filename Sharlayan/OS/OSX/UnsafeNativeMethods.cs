using System;
using System.Runtime.InteropServices;

namespace Sharlayan.OS.OSX
{
    using System.Linq;

    internal static class UnsafeNativeMethods
    {
        public const int VM_REGION_SUBMAP_INFO_COUNT_64 = 19;

        public const uint VM_PROT_READ = 1;
        public const uint VM_PROT_WRITE = 2;
        public const uint VM_PROT_EXECUTE = 4;
        public const int KERN_SUCCESS = 0;
        public const int KERN_INVALID_ADDRESS = 1;
        
        private const string MachFramework = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
        private const string SecurityFramework = "/System/Library/Frameworks/Security.framework/Security";

        [DllImport(MachFramework, EntryPoint = "mach_error_string")]
        public static extern IntPtr machErrorString(int error);

        [DllImport(MachFramework, EntryPoint = "mach_task_self")]
        public static extern IntPtr machTaskSelf();

        [DllImport(MachFramework, EntryPoint = "task_for_pid")]
        public static extern int TaskForPid(IntPtr targetPort, int pid, ref IntPtr t);

        public const int AuthorizationFlagDefaults = 0;
        public const int AuthorizationFlagInteractionAllowed = 1;
        public const int AuthorizationFlagExtendRights = 2;
        public const int AuthorizationFlagPartialRights = 4;
        public const int AuthorizationFlagDestroyRights = 8;
        public const int AuthorizationFlagPreAuthorize = 16;

        public const int ErrAuthorizationSuccess = 0;
        
        public struct AuthorizationItem
        {
            public string Name;
            public ulong ValueLength;
            public IntPtr Value;
            public int Flags;
        }
        
        public struct AuthorizationItemSet
        {
            public uint Count;
            public AuthorizationItem[] Items;
        }
        
        private struct InternalAuthorizationItem
        {
            public IntPtr Name;
            public ulong ValueLength;
            public IntPtr Value;
            public int Flags;
        }
        
        private struct InternalAuthorizationItemSet
        {
            public uint Count;
            public IntPtr Items;
        }
        
        [DllImport(SecurityFramework)]
        public static extern int AuthorizationCreate(IntPtr rights, IntPtr environment, int flags, out IntPtr authorization);

        [DllImport(SecurityFramework, EntryPoint = "AuthorizationCopyRights")]
        private static extern int InternalAuthorizationCopyRights(IntPtr authorization, InternalAuthorizationItemSet rights, IntPtr environment, int flags, out InternalAuthorizationItemSet authorizedRights);

        public static int AuthorizationCopyRights(IntPtr authorization, AuthorizationItemSet rights, IntPtr environment, int flags, out AuthorizationItemSet authorizedRights) {
            var itemSize = Marshal.SizeOf<InternalAuthorizationItem>();
            IntPtr itemsPointer = IntPtr.Zero;
            int result;
            
            // Convert the inner struct to a more "C native"-friendly struct
            var items = rights.Items.Select(
                x => new InternalAuthorizationItem
                {
                    Name = Marshal.StringToHGlobalAuto(x.Name),
                    Flags = x.Flags,
                    Value = x.Value,
                    ValueLength = x.ValueLength,
                }).ToList();
            try {
                // Allocate space so we can copy the items-array into it
                itemsPointer = Marshal.AllocHGlobal(items.Count * itemSize);
                var iRights = new InternalAuthorizationItemSet {
                    Count = rights.Count,
                    Items = itemsPointer,
                };

                var ptr = new IntPtr(itemsPointer.ToInt64());
                
                // Copy all structures into our allocated area
                for (var i = 0; i < items.Count; i++) {
                    Marshal.StructureToPtr(items[i], ptr, false);
                    ptr = IntPtr.Add(ptr, itemSize);
                }

                result = InternalAuthorizationCopyRights(authorization, iRights, environment, flags, out var outRights);

                authorizedRights = new AuthorizationItemSet {
                    Count = 0,
                    Items = new AuthorizationItem[0],
                };
            }
            finally {
                //if (itemsPointer.IsAllocated)
                //    itemsPointer.Free();

                for (var i = 0; i < items.Count; i++) {
                    Marshal.FreeHGlobal(items[i].Name);
                }

                if (itemsPointer != IntPtr.Zero)
                    Marshal.FreeHGlobal(itemsPointer);
            }

            return result;
        }
        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct vm_region_info_64_t
        {
            //            [MarshalAs(UnmanagedType.LPArray, SizeConst=100)]
            public uint Protection;                  // present access protection
            public uint MaxProtection;               // max avail through vm_prot
            public uint Inheritance;                 // behavior of map/obj on fork
            public IntPtr Offset;                      // offset into object/map
            public uint UserTag;                    // user tag on map entry
            public uint PagesResident;              // only valid for objects
            public uint PagesSharedNowPrivate;      // only for objects
            public uint PagesSwappedOut;            // only for objects
            public uint PagesDirtied;               // only for objects
            public uint RefCount;                   // obj/map mappers, etc
            public ushort ShadowDepth;              // only for obj
            public byte ExternalPager;              // only for obj
            public byte ShareMode;                  // see enumeration
            public bool IsSubmap;                   // submap vs obj
            public uint Behavior;                    // access behavior hint
            public uint ObjectId;                    // obj/map name, not a handle
            public ushort UserWiredCount;
            public ushort NotUsedTwoBytes;
            public uint PagesReusable;
            public ulong ObjectIdFull;
        }

        public const int VM_REGION_BASIC_INFO_64 = 9;

        public struct VmRegionBasicInfo64
        {
            public int Protection;
            public int MaxProtection;
            public uint Inheritance;
            public bool Shared;
            public bool Reserved;
            public ulong Offset;
            public uint Behavior;
            public ushort UserWiredCount;
        }
        
        [DllImport(MachFramework, EntryPoint = "vm_region_recurse_64")]
        public static extern int VmRegionRecurse64(IntPtr port, ref IntPtr address, ref IntPtr size, ref int depth, ref vm_region_info_64_t info, ref int count);
        //public static extern int VmRegionRecurse64(IntPtr port, ref IntPtr address, ref IntPtr size, ref int depth, out vm_region_info_64_t info, ref int count);
        
        [DllImport(MachFramework, EntryPoint = "mach_vm_region")]
        public static extern int MachVmRegion(IntPtr port, ref IntPtr address, ref IntPtr size, int flavor, out VmRegionBasicInfo64 info, ref int infoCnt, ref IntPtr object_name);        
        
        [DllImport(MachFramework, EntryPoint = "mach_vm_read_overwrite")]
        public static extern int MachVmReadOverwrite(IntPtr port, IntPtr address, IntPtr size, IntPtr buffer, ref int read);
    }
}