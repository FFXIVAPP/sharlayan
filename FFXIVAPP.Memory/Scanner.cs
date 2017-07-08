// FFXIVAPP.Memory ~ Scanner.cs
// 
// Copyright © 2007 - 2017 Ryan Wilson - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using FFXIVAPP.Memory.Models;
using NLog;

namespace FFXIVAPP.Memory
{
    public sealed class Scanner
    {
        #region Logger

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="pSignatures"> </param>
        /// <param name="scanAllMemoryRegions"></param>
        public void LoadOffsets(IEnumerable<Signature> pSignatures, bool scanAllMemoryRegions = false)
        {
            IsScanning = true;

            Func<bool> d = delegate
            {
                var sw = new Stopwatch();
                sw.Start();
                if (MemoryHandler.Instance.ProcessModel?.Process == null)
                {
                    return false;
                }
                var signatures = new List<Signature>(pSignatures);

                if (scanAllMemoryRegions)
                {
                    LoadRegions();
                }
                
                if (signatures.Any())
                {
                    foreach (var signature in signatures)
                    {
                        if (signature.Value == string.Empty)
                        {
                            // doesn't need a signature scan
                            Locations[signature.Key] = signature;
                            continue;
                        }
                        signature.Value = signature.Value.Replace("*", "?"); // allows either ? or * to be used as wildcard
                    }
                    signatures.RemoveAll(a => Locations.ContainsKey(a.Key));

                    FindExtendedSignatures(signatures, scanAllMemoryRegions);
                }

                sw.Stop();

                MemoryHandler.Instance.RaiseSignaturesFound(Logger, Locations, sw.ElapsedMilliseconds);

                IsScanning = false;

                return true;
            };
            d.BeginInvoke(null, null);
        }

        private void LoadRegions()
        {
            try
            {
                _regions = new List<UnsafeNativeMethods.MEMORY_BASIC_INFORMATION>();
                var address = new IntPtr();
                while (true)
                {
                    var info = new UnsafeNativeMethods.MEMORY_BASIC_INFORMATION();
                    var result = UnsafeNativeMethods.VirtualQueryEx(MemoryHandler.Instance.ProcessHandle, address, out info, (uint) Marshal.SizeOf(info));
                    if (0 == result)
                    {
                        break;
                    }
                    if (!MemoryHandler.Instance.IsSystemModule(info.BaseAddress) && 0 != (info.State & MemCommit) && 0 != (info.Protect & Writable) && 0 == (info.Protect & PageGuard))
                    {
                        _regions.Add(info);
                    }
                    else
                    {
                        MemoryHandler.Instance.RaiseException(Logger, new Exception(info.ToString()));
                    }
                    unchecked
                    {
                        switch (IntPtr.Size)
                        {
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
            catch (Exception ex)
            {
                MemoryHandler.Instance.RaiseException(Logger, ex, true);
            }
        }

        private void FindExtendedSignatures(IEnumerable<Signature> signatures, bool scanAllMemoryRegions = false)
        {
            var notFound = new List<Signature>(signatures);

            var baseAddress = MemoryHandler.Instance.ProcessModel.Process.MainModule.BaseAddress;
            var searchEnd = IntPtr.Add(baseAddress, MemoryHandler.Instance.ProcessModel.Process.MainModule.ModuleMemorySize);
            var searchStart = baseAddress;

            ResolveLocations(baseAddress, searchStart, searchEnd, ref notFound);

            if (scanAllMemoryRegions)
            {
                foreach (var region in _regions)
                {
                    baseAddress = region.BaseAddress;
                    searchEnd = new IntPtr(baseAddress.ToInt64() + region.RegionSize.ToInt64());
                    searchStart = baseAddress;
                    
                    ResolveLocations(baseAddress, searchStart, searchEnd, ref notFound);
                }
            }
        }

        private void ResolveLocations(IntPtr baseAddress, IntPtr searchStart, IntPtr searchEnd, ref List<Signature> notFound)
        {
            const int bufferSize = 0x1200;
            const int regionIncrement = 0x1000;
            
            var buffer = new byte[bufferSize];
            var temp = new List<Signature>();
            var regionCount = 0;

            while (searchStart.ToInt64() < searchEnd.ToInt64())
            {
                try
                {
                    IntPtr lpNumberOfBytesRead;
                    var regionSize = new IntPtr(bufferSize);
                    if (IntPtr.Add(searchStart, bufferSize).ToInt64() > searchEnd.ToInt64())
                    {
                        regionSize = (IntPtr)(searchEnd.ToInt64() - searchStart.ToInt64());
                    }
                    if (UnsafeNativeMethods.ReadProcessMemory(MemoryHandler.Instance.ProcessHandle, searchStart, buffer, regionSize, out lpNumberOfBytesRead))
                    {
                        foreach (var signature in notFound)
                        {
                            var index = FindSuperSignature(buffer, SignatureToByte(signature.Value, WildCardChar));
                            if (index < 0)
                            {
                                temp.Add(signature);
                                continue;
                            }

                            var baseResult = new IntPtr((long)(baseAddress + regionCount * regionIncrement));
                            var searchResult = IntPtr.Add(baseResult, index + signature.Offset);

                            signature.SigScanAddress = new IntPtr(searchResult.ToInt64());

                            if (!Locations.ContainsKey(signature.Key))
                            {
                                Locations.Add(signature.Key, signature);
                            }
                        }
                        notFound = new List<Signature>(temp);
                        temp.Clear();
                    }
                    regionCount++;
                    searchStart = IntPtr.Add(searchStart, regionIncrement);
                }
                catch (Exception ex)
                {
                    MemoryHandler.Instance.RaiseException(Logger, ex, true);
                }
            }
        }

        private int FindSuperSignature(byte[] buffer, byte[] pattern)
        {
            var result = -1;
            if (buffer.Length <= 0 || pattern.Length <= 0 || buffer.Length < pattern.Length)
            {
                return result;
            }
            for (var i = 0; i <= buffer.Length - pattern.Length; i++)
            {
                if (buffer[i] != pattern[0])
                {
                    continue;
                }
                if (buffer.Length > 1)
                {
                    var matched = true;
                    for (var y = 1; y <= pattern.Length - 1; y++)
                    {
                        if (buffer[i + y] == pattern[y] || pattern[y] == WildCardChar)
                        {
                            continue;
                        }
                        matched = false;
                        break;
                    }
                    if (!matched)
                    {
                        continue;
                    }
                    result = i;
                    break;
                }
                result = i;
                break;
            }
            return result;
        }

        /// <summary>
        ///     Convert a hex string to a binary array while preserving any wildcard characters.
        /// </summary>
        /// <param name="signature">A hex string "signature"</param>
        /// <param name="wildcard">The byte to treat as the wildcard</param>
        /// <returns>The converted binary array. Null if the conversion failed.</returns>
        private byte[] SignatureToByte(string signature, byte wildcard)
        {
            var pattern = new byte[signature.Length / 2];
            var hexTable = new[]
            {
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
            };
            try
            {
                for (int x = 0, i = 0; i < signature.Length; i += 2, x += 1)
                {
                    if (signature[i] == wildcard)
                    {
                        pattern[x] = wildcard;
                    }
                    else
                    {
                        pattern[x] = (byte) (hexTable[char.ToUpper(signature[i]) - '0'] << 4 | hexTable[char.ToUpper(signature[i + 1]) - '0']);
                    }
                }
                return pattern;
            }
            catch
            {
                return null;
            }
        }
        
        #region Property Bindings

        private static Scanner _instance;
        private Dictionary<string, Signature> _locations;

        public static Scanner Instance
        {
            get { return _instance ?? (_instance = new Scanner()); }
            set { _instance = value; }
        }

        public Dictionary<string, Signature> Locations
        {
            get { return _locations ?? (_locations = new Dictionary<string, Signature>()); }
            private set
            {
                if (_locations == null)
                {
                    _locations = new Dictionary<string, Signature>();
                }
                _locations = value;
            }
        }

        #endregion

        #region Constants

        private const int WildCardChar = 63;
        private const int MemCommit = 0x1000;
        private const int PageNoAccess = 0x01;
        private const int PageReadwrite = 0x04;
        private const int PageWritecopy = 0x08;
        private const int PageExecuteReadwrite = 0x40;
        private const int PageExecuteWritecopy = 0x80;
        private const int PageGuard = 0x100;

        private const int Writable = PageReadwrite | PageWritecopy | PageExecuteReadwrite | PageExecuteWritecopy | PageGuard;

        #endregion

        #region Declarations

        private List<UnsafeNativeMethods.MEMORY_BASIC_INFORMATION> _regions;

        public bool IsScanning { get; set; }

        #endregion
    }
}
