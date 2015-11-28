// FFXIVAPP.Memory
// Scanner.cs
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
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using FFXIVAPP.Memory.Models;

namespace FFXIVAPP.Memory
{
    internal class Scanner
    {
        /// <summary>
        /// </summary>
        /// <param name="pSignatures"> </param>
        public void LoadOffsets(IEnumerable<Signature> pSignatures)
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
                    FindExtendedSignatures(signatures);
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
        private int FindSuperSig(byte[] buffer, byte[] pattern)
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
        private byte[] SigToByte(string signature, byte wildcard)
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

        private byte[] _memDump;
        private List<UnsafeNativeMethods.MEMORY_BASIC_INFORMATION> _regions;

        #endregion
    }
}
