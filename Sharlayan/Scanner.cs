// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scanner.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Scanner.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using NLog;

    using Sharlayan.Models;
    using Sharlayan.OS;

    public sealed class Scanner {

        private const int WildCardChar = 63;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static Lazy<Scanner> _instance = new Lazy<Scanner>(() => new Scanner());

        private IList<NativeMemoryRegionInfo> _regions;

        public static Scanner Instance => _instance.Value;

        public bool IsScanning { get; private set; }

        public Dictionary<string, Signature> Locations { get; } = new Dictionary<string, Signature>();

        public void LoadOffsets(IEnumerable<Signature> signatures, bool scanAllMemoryRegions = false) {
            if (MemoryHandler.Instance.ProcessModel?.Process == null) {
                return;
            }

            this.IsScanning = true;

            Func<bool> scanningFunc = delegate {
                var sw = new Stopwatch();
                sw.Start();

                try {
                    this._regions = NativeMemoryHandler.Instance.LoadRegions(MemoryHandler.Instance.ProcessHandle, MemoryHandler.Instance.ProcessModel, MemoryHandler.Instance.SystemModules, scanAllMemoryRegions);
                }
                catch (Exception ex) {
                    MemoryHandler.Instance.RaiseException(Logger, ex, true);
                }

                List<Signature> scanable = signatures as List<Signature> ?? signatures.ToList();
                if (scanable.Any()) {
                    foreach (Signature signature in scanable) {
                        if (signature.Value == string.Empty) {
                            // doesn't need a signature scan
                            this.Locations[signature.Key] = signature;
                            continue;
                        }

                        signature.Value = signature.Value.Replace("*", "?"); // allows either ? or * to be used as wildcard
                    }

                    scanable.RemoveAll(a => this.Locations.ContainsKey(a.Key));

                    this.FindExtendedSignatures(scanable);
                }

                sw.Stop();

                MemoryHandler.Instance.RaiseSignaturesFound(Logger, this.Locations, sw.ElapsedMilliseconds);

                this.IsScanning = false;

                return true;
            };

            Task.Run(() => scanningFunc.Invoke());
        }

        private void FindExtendedSignatures(IEnumerable<Signature> signatures) {
            List<Signature> notFound = new List<Signature>(signatures);

            foreach (var region in (this._regions ?? new List<NativeMemoryRegionInfo>())) {
                var baseAddress = region.BaseAddress;
                var searchEnd = new IntPtr(baseAddress.ToInt64() + region.RegionSize.ToInt64());
                var searchStart = baseAddress;

                this.ResolveLocations(baseAddress, searchStart, searchEnd, ref notFound);
            }
        }

        private int FindSuperSignature(byte[] buffer, byte[] pattern) {
            var result = -1;
            if (buffer.Length <= 0 || pattern.Length <= 0 || buffer.Length < pattern.Length) {
                return result;
            }

            for (var i = 0; i <= buffer.Length - pattern.Length; i++) {
                if (buffer[i] != pattern[0]) {
                    continue;
                }

                if (buffer.Length > 1) {
                    var matched = true;
                    for (var y = 1; y <= pattern.Length - 1; y++) {
                        if (buffer[i + y] == pattern[y] || pattern[y] == WildCardChar) {
                            continue;
                        }

                        matched = false;
                        break;
                    }

                    if (!matched) {
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

        private void ResolveLocations(IntPtr baseAddress, IntPtr searchStart, IntPtr searchEnd, ref List<Signature> notFound) {
            const int bufferSize = 0x1200;
            const int regionIncrement = 0x1000;

            byte[] buffer = new byte[bufferSize];
            List<Signature> temp = new List<Signature>();
            var regionCount = 0;

            while (searchStart.ToInt64() < searchEnd.ToInt64()) {
                try {
                    var regionSize = new IntPtr(bufferSize);
                    if (IntPtr.Add(searchStart, bufferSize).ToInt64() > searchEnd.ToInt64()) {
                        regionSize = (IntPtr) (searchEnd.ToInt64() - searchStart.ToInt64());
                    }

                    if (NativeMemoryHandler.Instance.ReadMemory(MemoryHandler.Instance.ProcessHandle, searchStart, buffer, regionSize)) {
                        foreach (Signature signature in notFound) {
                            var index = this.FindSuperSignature(buffer, this.SignatureToByte(signature.Value, WildCardChar));
                            if (index < 0) {
                                temp.Add(signature);
                                continue;
                            }

                            var baseResult = new IntPtr((long) (baseAddress + regionCount * regionIncrement));
                            IntPtr searchResult = IntPtr.Add(baseResult, index + signature.Offset);

                            signature.SigScanAddress = new IntPtr(searchResult.ToInt64());

                            if (!this.Locations.ContainsKey(signature.Key)) {
                                this.Locations.Add(signature.Key, signature);
                            }
                        }

                        notFound = new List<Signature>(temp);
                        temp.Clear();
                    }

                    regionCount++;
                    searchStart = IntPtr.Add(searchStart, regionIncrement);
                }
                catch (Exception ex) {
                    MemoryHandler.Instance.RaiseException(Logger, ex, true);
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