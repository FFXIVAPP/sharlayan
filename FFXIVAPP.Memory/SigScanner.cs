// FFXIVAPP.Memory
// SigScanner.cs
// 
// Copyright © 2007 - 2015 Ryan Wilson - All Rights Reserved
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions are met: 
// 
//  * Redistributions of source code must retain the above copyright notice, 
//    this list of conditions and the following disclaimer. 
//  * Redistributions in binary form must reproduce the above copyright 
//    notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution. 
//  * Neither the name of SyndicatedLife nor the names of its contributors may 
//    be used to endorse or promote products derived from this software 
//    without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE. 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FFXIVAPP.Memory.Models;

namespace FFXIVAPP.Memory
{
    public class SigScanner : INotifyPropertyChanged
    {
        #region ResultTypes

        /// <summary>
        ///     Where to return the pointer from
        /// </summary>
        public enum ScanResultType
        {
            /// <summary>
            ///     Read in the pointer before the signature
            /// </summary>
            ValueBeforeSig,

            /// <summary>
            ///     Read in the pointer after the signature
            /// </summary>
            ValueAfterSig,

            /// <summary>
            ///     Read the address at the start of where it found the signature
            /// </summary>
            AddressStartOfSig,

            /// <summary>
            ///     Read the value at the start of the wildcard
            /// </summary>
            ValueAtWildCard
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="signatures"> </param>
        public SigScanner(List<Signature> signatures = null)
        {
            if (signatures != null && signatures.Any())
            {
                LoadOffsets(signatures);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="signatures"> </param>
        public void LoadOffsets(List<Signature> pSignatures)
        {
            Func<bool> d = delegate
            {
                var sw = new Stopwatch();
                sw.Start();
                if (MemoryHandler.Instance.ProcessModel.Process == null)
                {
                    return false;
                }
                var signatures = new List<Signature>(pSignatures);
                LoadRegions();
                //Locations = new Dictionary<string, Signature>();
                if (signatures.Any())
                {
                    foreach (var sig in signatures)
                    {
                        if (sig.Value == "")
                        {
                            // doesn't need a signature scan
                            Locations[sig.Key] = sig;
                            continue;
                        }
                        sig.Value = sig.Value.Replace("*", "?"); // allows either ? or * to be used as wildcard
                    }

                    signatures.RemoveAll(a => Locations.ContainsKey(a.Key));

                    // this will scan 32 bit regions for some reason
                    //FindSignatures(signatures, ScanResultType.AddressStartOfSig);

                    //var signaturesNotFound = signatures.Where(s => !Locations.ContainsKey(s.Key)).ToList();

                    //signatures.RemoveAll(a => Locations.ContainsKey(a.Key));

                    //if (signatures.Any())
                    {
                        // have to extend this to scan from game base address up
                        FindExtendedSignatures(signatures);
                    }
                }
                _memDump = null;
                sw.Stop();
                return true;
            };
            d.BeginInvoke(null, null);
        }

        /// <summary>
        /// </summary>
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
                    if (0 != (info.State & MemCommit) && 0 != (info.Protect & Writable) && 0 == (info.Protect & PageGuard))
                    {
                        _regions.Add(info);
                    }
                    address = IntPtr.Add(info.BaseAddress, info.RegionSize.ToInt32());
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        ///     Search the loaded regions for all signatures at once and remove from list once found
        /// </summary>
        /// <param name="signatures"></param>
        /// <param name="searchType"></param>
        private void FindSignatures(IEnumerable<Signature> signatures, ScanResultType searchType)
        {
            try
            {
                var notFound = new List<Signature>(signatures);
                var temp = new List<Signature>();
                foreach (var region in _regions)
                {
                    try
                    {
                        var buffer = new byte[region.RegionSize.ToInt32()];
                        IntPtr lpNumberOfByteRead;
                        if (!UnsafeNativeMethods.ReadProcessMemory(MemoryHandler.Instance.ProcessHandle, region.BaseAddress, buffer, region.RegionSize, out lpNumberOfByteRead))
                        {
                            var errorCode = Marshal.GetLastWin32Error();
                            throw new Exception("FindSignature(): Unable to read memory. Error Code [" + errorCode + "]");
                        }
                        foreach (var signature in notFound)
                        {
                            var searchResult = FindSignature(buffer, signature.Value, signature.Offset, searchType);
                            if (IntPtr.Zero == searchResult)
                            {
                                temp.Add(signature);
                                continue;
                            }
                            if (ScanResultType.AddressStartOfSig == searchType)
                            {
                                searchResult = IntPtr.Add(searchResult, (int) region.BaseAddress);
                            }
                            signature.SigScanAddress = searchResult;
                            Locations.Add(signature.Key, signature);
                        }
                        notFound = new List<Signature>(temp);
                        temp.Clear();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        ///     Searches the buffer for the given hex string and returns the pointer matching the first wildcard location, or the
        ///     pointer following the pattern if not using wildcards.
        ///     Prefix with &lt;&lt; to always return the pointer preceding the match or &gt;&gt; to always return the pointer
        ///     following (regardless of wildcards)
        /// </summary>
        /// <param name="buffer">The source binary buffer to search within</param>
        /// <param name="signature">A hex string representation of a sequence of bytes to search for</param>
        /// <param name="offset">An offset to add to the found pointer VALUE.</param>
        /// <param name="searchType"></param>
        /// <returns>A pointer at the matching location</returns>
        private static IntPtr FindSignature(byte[] buffer, string signature, int offset, ScanResultType searchType)
        {
            try
            {
                if (signature.Length == 0 || signature.Length % 2 != 0)
                {
                    return IntPtr.Zero;
                }
                var pattern = SigToByte(signature, WildCardChar);
                if (pattern != null)
                {
                    var pos = 0;
                    for (pos = 0; pos < pattern.Length; pos++)
                    {
                        if (pattern[pos] == WildCardChar)
                        {
                            break;
                        }
                    }
                    var idx = -1;
                    if (pattern.Length > 32)
                    {
                        idx = FindSuperSig(buffer, pattern);
                    }
                    else
                    {
                        idx = pos == pattern.Length ? Horspool(buffer, pattern) : BNDM(buffer, pattern, WildCardChar);
                    }
                    if (idx < 0)
                    {
                        return IntPtr.Zero;
                    }
                    switch (searchType)
                    {
                        case ScanResultType.ValueBeforeSig:
                            if (MemoryHandler.Instance.ProcessModel.IsWin64)
                            {
                                return (IntPtr) (BitConverter.ToInt64(buffer, idx - 8) + offset);
                            }
                            return (IntPtr) (BitConverter.ToInt32(buffer, idx - 4) + offset);
                        case ScanResultType.ValueAfterSig:
                            if (MemoryHandler.Instance.ProcessModel.IsWin64)
                            {
                                return (IntPtr) (BitConverter.ToInt64(buffer, idx + pattern.Length) + offset);
                            }
                            return (IntPtr) (BitConverter.ToInt32(buffer, idx + pattern.Length) + offset);
                        case ScanResultType.AddressStartOfSig:
                            return (IntPtr) (idx + offset);
                        case ScanResultType.ValueAtWildCard:
                        default:
                            return (IntPtr) (BitConverter.ToInt32(buffer, idx + pos) + offset);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return IntPtr.Zero;
        }

        private void FindExtendedSignatures(IEnumerable<Signature> signatures)
        {
            const int bufferSize = 0x1000;
            var moduleMemorySize = MemoryHandler.Instance.ProcessModel.Process.MainModule.ModuleMemorySize;
            var baseAddress = MemoryHandler.Instance.ProcessModel.Process.MainModule.BaseAddress;
            var searchEnd = IntPtr.Add(baseAddress, moduleMemorySize);
            var searchStart = baseAddress;
            var lpBuffer = new byte[bufferSize];
            var notFound = new List<Signature>(signatures);
            var temp = new List<Signature>();
            var regionCount = 0;
            while (searchStart.ToInt64() < searchEnd.ToInt64())
            {
                try
                {
                    IntPtr lpNumberOfBytesRead;
                    var regionSize = new IntPtr(bufferSize);
                    if (IntPtr.Add(searchStart, bufferSize)
                              .ToInt64() > searchEnd.ToInt64())
                    {
                        regionSize = (IntPtr) (searchEnd.ToInt64() - searchStart.ToInt64());
                    }
                    if (UnsafeNativeMethods.ReadProcessMemory(MemoryHandler.Instance.ProcessHandle, searchStart, lpBuffer, regionSize, out lpNumberOfBytesRead))
                    {
                        foreach (var signature in notFound)
                        {
                            var idx = FindSuperSig(lpBuffer, SigToByte(signature.Value, WildCardChar));
                            if (idx < 0)
                            {
                                temp.Add(signature);
                                continue;
                            }
                            var baseResult = new IntPtr((long) (baseAddress + (regionCount * bufferSize)));
                            var searchResult = IntPtr.Add(baseResult, idx + signature.Offset);
                            signature.SigScanAddress = new IntPtr(searchResult.ToInt64());
                            Locations.Add(signature.Key, signature);
                        }
                        notFound = new List<Signature>(temp);
                        temp.Clear();
                    }
                    regionCount++;
                    searchStart = IntPtr.Add(searchStart, bufferSize);
                }
                catch (Exception ex)
                {
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private static int FindSuperSig(byte[] buffer, byte[] pattern)
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

        /// <summary>Backward Nondeterministic Dawg Matching search algorithm</summary>
        /// <param name="buffer">The haystack to search within</param>
        /// <param name="pattern">The needle to locate</param>
        /// <param name="wildcard">
        ///     The byte to treat as a wildcard character. Note that this only matches one char for one char and
        ///     does not expand.
        /// </param>
        /// <returns>The index the pattern was found at, or -1 if not found</returns>
        private static int BNDM(byte[] buffer, byte[] pattern, byte wildcard)
        {
            var end = pattern.Length < 32 ? pattern.Length : 32;
            var b = new int[256];
            var j = 0;
            for (var i = 0; i < end; ++i)
            {
                if (pattern[i] == wildcard)
                {
                    j |= (1 << end - i - 1);
                }
            }
            if (j != 0)
            {
                for (var i = 0; i < b.Length; i++)
                {
                    b[i] = j;
                }
            }
            j = 1;
            for (var i = end - 1; i >= 0; --i, j <<= 1)
            {
                b[pattern[i]] |= j;
            }
            var pos = 0;
            while (pos <= buffer.Length - pattern.Length)
            {
                j = pattern.Length - 1;
                var last = pattern.Length;
                var d = -1;
                while (d != 0)
                {
                    d &= b[buffer[pos + j]];
                    if (d != 0)
                    {
                        if (j == 0)
                        {
                            return pos;
                        }
                        last = j;
                    }
                    --j;
                    d <<= 1;
                }
                pos += last;
            }
            return -1;
        }

        /// <summary>Boyer-Moore-Horspool search algorithm</summary>
        /// <param name="buffer">The haystack to search within</param>
        /// <param name="pattern">The needle to locate</param>
        /// <returns>The index the pattern was found at, or -1 if not found</returns>
        private static int Horspool(byte[] buffer, byte[] pattern)
        {
            var bcs = new int[256];
            var scan = 0;
            for (scan = 0; scan < 256; scan = scan + 1)
            {
                bcs[scan] = pattern.Length;
            }
            var last = pattern.Length - 1;
            for (scan = 0; scan < last; scan = scan + 1)
            {
                bcs[pattern[scan]] = last - scan;
            }
            var hidx = 0;
            var hlen = buffer.Length;
            var nlen = pattern.Length;
            while (hidx <= hlen - nlen)
            {
                for (scan = last; buffer[hidx + scan] == pattern[scan]; scan = scan - 1)
                {
                    if (scan == 0)
                    {
                        return hidx;
                    }
                }
                hidx += bcs[buffer[hidx + last]];
            }
            return -1;
        }

        /// <summary>
        ///     Convert a hex string to a binary array while preserving any wildcard characters.
        /// </summary>
        /// <param name="signature">A hex string "signature"</param>
        /// <param name="wildcard">The byte to treat as the wildcard</param>
        /// <returns>The converted binary array. Null if the conversion failed.</returns>
        private static byte[] SigToByte(string signature, byte wildcard)
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
                        pattern[x] = (byte) (hexTable[Char.ToUpper(signature[i]) - '0'] << 4 | hexTable[Char.ToUpper(signature[i + 1]) - '0']);
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

        private static SigScanner _instance;
        private Dictionary<string, Signature> _locations;

        public static SigScanner Instance
        {
            get { return _instance ?? (_instance = new SigScanner()); }
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
                RaisePropertyChanged();
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

        private byte[] _memDump;
        private List<UnsafeNativeMethods.MEMORY_BASIC_INFORMATION> _regions;

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}
