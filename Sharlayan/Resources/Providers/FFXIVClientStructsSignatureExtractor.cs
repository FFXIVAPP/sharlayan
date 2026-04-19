// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FFXIVClientStructsSignatureExtractor.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reflects over the merged FFXIVClientStructs types to extract every [StaticAddress] attribute
//   on a static Instance() method. Converts each attribute's (pattern, relativeFollowOffset,
//   isPointer) triple into a Sharlayan.Models.Signature suitable for the out-of-process Scanner.
//
//   Conversion model:
//     FFXIVClientStructs [StaticAddress("AA BB CC ?? ?? ?? ?? DD EE", rel=3)]
//     — pattern is 9 bytes, the 32-bit RIP-relative immediate lives at bytes 3..6 inclusive.
//     — Sharlayan's Scanner lands at matchStart + patternLength (byte AFTER the last match byte);
//       we need to rewind to matchStart + rel to reach the immediate, so the first PointerPath
//       hop is a negative byte offset: -(patternLength - rel). With ASMSignature=true the next
//       step does RIP-relative follow.
//     — For isPointer=true (e.g. Framework), the static slot itself holds a pointer rather than
//       being the struct base, so an extra deref (+0) step is inserted.
//     — An optional innerOffset is added as a final hop to reach a specific field inside the
//       resolved struct (e.g. CHARMAP = GameObjectManager + 0x20 for the _indexSorted array).
//
//   This lets us reuse the existing Sharlayan Scanner + Reader with no changes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Providers {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using InteropGenerator.Runtime.Attributes;

    using Sharlayan.Models;

    internal static class FFXIVClientStructsSignatureExtractor {
        private static readonly Lazy<Dictionary<string, StaticAddressInfo>> _cache =
            new(BuildCache, isThreadSafe: true);

        public readonly struct StaticAddressInfo {
            public StaticAddressInfo(string pattern, ushort relativeFollowOffset, bool isPointer, string fullTypeName) {
                this.Pattern = pattern;
                this.RelativeFollowOffset = relativeFollowOffset;
                this.IsPointer = isPointer;
                this.FullTypeName = fullTypeName;
            }

            public string Pattern { get; }
            public ushort RelativeFollowOffset { get; }
            public bool IsPointer { get; }
            public string FullTypeName { get; }
        }

        public static bool TryGet(string fcsTypeName, out StaticAddressInfo info) {
            return _cache.Value.TryGetValue(fcsTypeName, out info);
        }

        /// <summary>
        /// Convert an extracted FFXIVClientStructs StaticAddress entry into a Sharlayan Signature.
        /// </summary>
        /// <param name="sharlayanKey">Scanner key expected by Sharlayan's Reader (e.g. "CHARMAP").</param>
        /// <param name="info">The FFXIVClientStructs static-address metadata.</param>
        /// <param name="innerOffset">
        /// Byte offset inside the resolved struct to reach the field Sharlayan expects. For example
        /// CHARMAP is GameObjectManager + 0x20 for the _indexSorted pointer array.
        /// </param>
        public static Signature BuildSignature(string sharlayanKey, StaticAddressInfo info, long innerOffset) {
            string patternHex = NormalisePatternHex(info.Pattern);
            int patternBytes = patternHex.Length / 2;
            if (info.RelativeFollowOffset > patternBytes) {
                throw new ArgumentOutOfRangeException(nameof(info), $"relativeFollowOffset {info.RelativeFollowOffset} > pattern length {patternBytes} bytes for {sharlayanKey}");
            }

            // rewindBack is always negative: scanner lands at matchStart + patternBytes and we need
            // to hop backward to the RIP-relative byte (matchStart + relativeFollowOffset).
            long rewindBack = -(long)(patternBytes - info.RelativeFollowOffset);
            List<long> path = new List<long> { rewindBack };
            if (info.IsPointer) {
                // The static slot holds a pointer to the struct, not the struct itself. After the
                // ASM follow resolves to the slot address, we need to dereference it once.
                path.Add(0);
            }
            // Final hop reaches the Sharlayan-specific field inside the resolved struct. Sharlayan's
            // ResolvePointerPath always does a trailing dereference on the last entry (value ignored),
            // so baseAddress returned = struct + innerOffset.
            path.Add(innerOffset);

            return new Signature {
                Key = sharlayanKey,
                Value = patternHex,
                ASMSignature = true,
                PointerPath = path,
            };
        }

        private static Dictionary<string, StaticAddressInfo> BuildCache() {
            Dictionary<string, StaticAddressInfo> cache = new Dictionary<string, StaticAddressInfo>(StringComparer.Ordinal);
            Assembly fcsAssembly = typeof(InteropGenerator.Runtime.Attributes.StaticAddressAttribute).Assembly;
            // After ILRepack, FFXIVClientStructs types end up inside Sharlayan.dll; but if that
            // assembly doesn't carry them (e.g. harness referencing unmerged builds), also scan
            // any loaded assembly named FFXIVClientStructs.
            List<Assembly> probe = new List<Assembly> { fcsAssembly };
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
                if (a != fcsAssembly && a.GetName().Name is "FFXIVClientStructs" or "Sharlayan") {
                    probe.Add(a);
                }
            }

            foreach (Assembly asm in probe.Distinct()) {
                Type[] types;
                try { types = asm.GetTypes(); }
                catch (ReflectionTypeLoadException ex) { types = Array.FindAll(ex.Types, t => t != null); }

                foreach (Type t in types) {
                    if (t == null) continue;
                    // Look at all static methods, not only those named "Instance" — partial Instance
                    // methods generated from [StaticAddress] can appear under generated backing names.
                    foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)) {
                        StaticAddressAttribute attr = m.GetCustomAttribute<StaticAddressAttribute>();
                        if (attr == null) continue;
                        // Only the first relative-follow is modelled; multi-step follows (rare) aren't
                        // expressible via Sharlayan's PointerPath today and are skipped.
                        ushort rel = attr.RelativeFollowOffsets.Length > 0 ? attr.RelativeFollowOffsets[0] : (ushort)0;
                        StaticAddressInfo info = new StaticAddressInfo(attr.Signature, rel, attr.IsPointer, t.FullName ?? t.Name);
                        cache[t.Name] = info;
                        if (t.FullName != null) {
                            cache[t.FullName] = info;
                        }
                    }
                }
            }
            return cache;
        }

        private static string NormalisePatternHex(string pattern) {
            // Accepts "AA BB ?? CC" or "AABB??CC"; emits "AABB??CC" (no spaces, lowercase letters
            // preserved as uppercase for consistency with sharlayan-resources JSON).
            string stripped = pattern.Replace(" ", string.Empty).Replace("-", string.Empty);
            return stripped.ToUpperInvariant();
        }
    }
}
