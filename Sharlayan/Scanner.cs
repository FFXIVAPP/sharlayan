// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scanner.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Scanner.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    using NLog;

    using Sharlayan.Models;

    public sealed class Scanner {
        private const int MemCommit = 0x1000;

        private const int PageExecuteReadwrite = 0x40;

        private const int PageExecuteWritecopy = 0x80;

        private const int PageGuard = 0x100;

        private const int PageNoAccess = 0x01;

        private const int PageReadwrite = 0x04;

        private const int PageWritecopy = 0x08;

        private const int WildCardChar = 63;

        private const int Writable = PageReadwrite | PageWritecopy | PageExecuteReadwrite | PageExecuteWritecopy | PageGuard;

        private const int BufferSize = 0x1200;

        private const int RegionIncrement = 0x1000;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private List<UnsafeNativeMethods.MEMORY_BASIC_INFORMATION> _regions;

        public Scanner(MemoryHandler memoryHandler) {
            this._memoryHandler = memoryHandler;
        }

        public bool IsScanning { get; private set; }

        public ConcurrentDictionary<string, MemoryLocation> Locations { get; } = new ConcurrentDictionary<string, MemoryLocation>();

        private MemoryHandler _memoryHandler { get; }

        public void LoadOffsets(Signature[] signatures, bool scanAllRegions = false) {
            if (this._memoryHandler.Configuration.ProcessModel.Process == null) {
                return;
            }

            this.IsScanning = true;

            Task.Run(
                () => {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    if (scanAllRegions) {
                        this.LoadRegions();
                    }

                    List<Signature> scanable = signatures.ToList();
                    if (scanable.Any()) {
                        foreach (Signature signature in scanable) {
                            if (signature.Value == string.Empty) {
                                // doesn't need a signature scan
                                this.Locations[signature.Key] = new MemoryLocation(signature, this._memoryHandler);
                                continue;
                            }

                            signature.Value = signature.Value.Replace("*", "?"); // allows either ? or * to be used as wildcard
                        }

                        scanable.RemoveAll(a => this.Locations.ContainsKey(a.Key));

                        this.FindExtendedSignatures(scanable, scanAllRegions);
                    }

                    sw.Stop();

                    this._memoryHandler.RaiseMemoryLocationsFound(this.Locations, sw.ElapsedMilliseconds);

                    this.IsScanning = false;
                });
        }

        private static int[] BuildBadShiftTable(byte[] pattern) {
            int i;
            int length = pattern.Length - 1;
            int[] badShiftTable = new int[256];

            for (i = length; i > 0 && pattern[i] != WildCardChar; --i) { }

            int difference = length - i;
            if (difference == 0) {
                difference = 1;
            }

            for (i = 0; i <= 255; ++i) {
                badShiftTable[i] = difference;
            }

            for (i = length - difference; i < length; ++i) {
                badShiftTable[pattern[i]] = length - i;
            }

            return badShiftTable;
        }

        private void FindExtendedSignatures(IEnumerable<Signature> signatures, bool scanAllRegions) {
            List<Signature> notFound = new List<Signature>(signatures);

            if (this._memoryHandler.Configuration.ProcessModel.Process.MainModule == null) {
                return;
            }

            IntPtr baseAddress = this._memoryHandler.Configuration.ProcessModel.Process.MainModule.BaseAddress;
            IntPtr searchEnd = IntPtr.Add(baseAddress, this._memoryHandler.Configuration.ProcessModel.Process.MainModule.ModuleMemorySize);
            IntPtr searchStart = baseAddress;

            this.ResolveLocations(baseAddress, searchStart, searchEnd, ref notFound);

            if (!scanAllRegions) {
                return;
            }

            foreach (UnsafeNativeMethods.MEMORY_BASIC_INFORMATION region in this._regions) {
                baseAddress = region.BaseAddress;
                searchEnd = new IntPtr(baseAddress.ToInt64() + region.RegionSize.ToInt64());
                searchStart = baseAddress;

                this.ResolveLocations(baseAddress, searchStart, searchEnd, ref notFound);
            }
        }

        private int FindSuperSignature(byte[] buffer, byte[] pattern) {
            if (pattern.Length > buffer.Length) {
                return -1;
            }

            int[] badShift = BuildBadShiftTable(pattern);
            int offset = 0;
            int last = pattern.Length - 1;
            int maxoffset = buffer.Length - pattern.Length;
            while (offset <= maxoffset) {
                int position;
                for (position = last; pattern[position] == buffer[position + offset] || pattern[position] == WildCardChar; position--) {
                    if (position == 0) {
                        return offset;
                    }
                }

                offset += badShift[buffer[offset + last]];
            }

            return -1;
        }

        private void LoadRegions() {
            try {
                this._regions = new List<UnsafeNativeMethods.MEMORY_BASIC_INFORMATION>();
                IntPtr address = IntPtr.Zero;
                while (true) {
                    UnsafeNativeMethods.MEMORY_BASIC_INFORMATION info = new UnsafeNativeMethods.MEMORY_BASIC_INFORMATION();
                    int result = UnsafeNativeMethods.VirtualQueryEx(this._memoryHandler.ProcessHandle, address, out info, (uint) Marshal.SizeOf(info));
                    if (result == 0) {
                        break;
                    }

                    if (!this._memoryHandler.IsSystemModule(info.BaseAddress) && (info.State & MemCommit) != 0 && (info.Protect & Writable) != 0 && (info.Protect & PageGuard) == 0) {
                        this._regions.Add(info);
                    }
                    else {
                        this._memoryHandler.RaiseException(Logger, new Exception(info.ToString()));
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
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex);
            }
        }

        private void ResolveLocations(IntPtr baseAddress, IntPtr searchStart, IntPtr searchEnd, ref List<Signature> unresolvedSignatures) {
            byte[] buffer = new byte[BufferSize];
            List<Signature> temp = new List<Signature>();
            int regionCount = 0;

            while (searchStart.ToInt64() < searchEnd.ToInt64()) {
                try {
                    IntPtr regionSize = new IntPtr(BufferSize);
                    if (IntPtr.Add(searchStart, BufferSize).ToInt64() > searchEnd.ToInt64()) {
                        regionSize = (IntPtr) (searchEnd.ToInt64() - searchStart.ToInt64());
                    }

                    if (UnsafeNativeMethods.ReadProcessMemory(this._memoryHandler.ProcessHandle, searchStart, buffer, regionSize, out IntPtr _)) {
                        foreach (Signature unresolvedSignature in unresolvedSignatures) {
                            int index = this.FindSuperSignature(buffer, this.SignatureToByte(unresolvedSignature.Value, WildCardChar));
                            if (index < 0) {
                                temp.Add(unresolvedSignature);
                                continue;
                            }

                            IntPtr baseResult = new IntPtr((long) (baseAddress + regionCount * RegionIncrement));
                            IntPtr searchResult = IntPtr.Add(baseResult, index + unresolvedSignature.Offset);

                            unresolvedSignature.SigScanAddress = new IntPtr(searchResult.ToInt64());

                            if (!this.Locations.ContainsKey(unresolvedSignature.Key)) {
                                this.Locations.TryAdd(unresolvedSignature.Key, new MemoryLocation(unresolvedSignature, this._memoryHandler));
                            }
                        }

                        unresolvedSignatures = new List<Signature>(temp);
                        temp.Clear();
                    }

                    regionCount++;
                    searchStart = IntPtr.Add(searchStart, RegionIncrement);
                }
                catch (Exception ex) {
                    this._memoryHandler.RaiseException(Logger, ex);
                }
            }
        }

        private byte[] SignatureToByte(string signature, byte wildcard) {
            byte[] pattern = new byte[signature.Length / 2];
            int[] hexTable = {
                0x00,
                0x01,
                0x02,
                0x03,
                0x04,
                0x05,
                0x06,
                0x07,
                0x08,
                0x09,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x0A,
                0x0B,
                0x0C,
                0x0D,
                0x0E,
                0x0F,
            };
            try {
                for (int x = 0,
                         i = 0; i < signature.Length; i += 2, x += 1) {
                    if (signature[i] == wildcard) {
                        pattern[x] = wildcard;
                    }
                    else {
                        pattern[x] = (byte) ((hexTable[char.ToUpper(signature[i]) - '0'] << 4) | hexTable[char.ToUpper(signature[i + 1]) - '0']);
                    }
                }

                return pattern;
            }
            catch {
                return null;
            }
        }
    }
}