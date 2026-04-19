// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FFXIVClientStructsSignatureExtractorTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Unit tests for the signature-extractor's conversion math. The actual FCS reflection
//   (BuildCache) needs the game process and isn't tested here — but BuildSignature's
//   offset arithmetic (rewindBack, isPointer deref hop, multi-hop chains) is pure logic
//   and easy to pin down.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Providers {
    using System;

    using Sharlayan.Resources.Providers;

    using Xunit;

    using Info = Sharlayan.Resources.Providers.FFXIVClientStructsSignatureExtractor.StaticAddressInfo;

    public class FFXIVClientStructsSignatureExtractorTests {
        // Typical 9-byte LEA pattern with rel=3: matches real FCS declarations like
        // GameObjectManager's "48 8D 35 ?? ?? ?? ?? 81 FA".
        private static Info Lea9Bytes(bool isPointer = false) =>
            new Info(pattern: "48 8D 35 ?? ?? ?? ?? 81 FA", relativeFollowOffset: 3, isPointer: isPointer, fullTypeName: "Test.Lea9");

        // isPointer=true pattern (Framework-style 11 bytes, rel=3).
        private static Info PointerSlot11Bytes() =>
            new Info(pattern: "48 8B 1D ?? ?? ?? ?? 8B 7C 24 ??", relativeFollowOffset: 3, isPointer: true, fullTypeName: "Test.Ptr11");

        [Fact]
        public void BuildSignature_CopiesKeyAndSetsAsmFlag() {
            var sig = FFXIVClientStructsSignatureExtractor.BuildSignature("CHARMAP", Lea9Bytes(), innerOffset: 0x20);
            Assert.Equal("CHARMAP", sig.Key);
            Assert.True(sig.ASMSignature);
        }

        [Fact]
        public void BuildSignature_NormalisesPatternToHex() {
            // Spaces stripped and uppercased for consistency with sharlayan-resources JSON.
            var sig = FFXIVClientStructsSignatureExtractor.BuildSignature("K", Lea9Bytes(), innerOffset: 0);
            Assert.Equal("488D35????????81FA", sig.Value);
        }

        [Fact]
        public void BuildSignature_NonPointer_FirstHopRewindsToRipImmediate() {
            // 9-byte pattern, rel=3 → scanner lands at matchStart+9; the 4-byte RIP-relative
            // sits at matchStart+3. Rewind = -(9 - 3) = -6.
            var sig = FFXIVClientStructsSignatureExtractor.BuildSignature("K", Lea9Bytes(), innerOffset: 0x20);
            Assert.Equal(new long[] { -6, 0x20 }, sig.PointerPath);
        }

        [Fact]
        public void BuildSignature_PointerSlot_InsertsExtraDerefHop() {
            // isPointer=true adds a zero-offset hop after the ASM-follow so ResolvePointerPath
            // dereferences the static pointer to get the real struct base.
            // 11-byte pattern, rel=3 → rewind = -8.
            var sig = FFXIVClientStructsSignatureExtractor.BuildSignature("K", PointerSlot11Bytes(), innerOffset: 0x18);
            Assert.Equal(new long[] { -8, 0, 0x18 }, sig.PointerPath);
        }

        [Fact]
        public void BuildSignature_MultiHop_AppendsEachOffsetAsIntermediateDeref() {
            // Each offset after the first is a ReadPointer dereference; the last is a trailing
            // add that ResolvePointerPath returns without dereffing. CHATLOG-style chain:
            //   Framework (isPointer) → +0x2B68 (deref UIModule) → +0x19E0 (trailing add)
            var sig = FFXIVClientStructsSignatureExtractor.BuildSignature("CHATLOG", PointerSlot11Bytes(),
                new long[] { 0x2B68, 0x19E0 });
            Assert.Equal(new long[] { -8, 0, 0x2B68, 0x19E0 }, sig.PointerPath);
        }

        [Fact]
        public void BuildSignature_NonPointerMultiHop_NoExtraDerefHop() {
            // isPointer=false chains skip the extra zero hop — the first ASM follow already
            // lands on the struct base, not a pointer-to-pointer. RECAST-style chain through
            // AtkStage uses a pointer static, but a plain struct like GameMain wouldn't.
            var sig = FFXIVClientStructsSignatureExtractor.BuildSignature("K", Lea9Bytes(isPointer: false),
                new long[] { 0x10, 0x20, 60 });
            Assert.Equal(new long[] { -6, 0x10, 0x20, 60 }, sig.PointerPath);
        }

        [Fact]
        public void BuildSignature_EmptyOffsetChain_Throws() {
            Assert.Throws<ArgumentException>(() =>
                FFXIVClientStructsSignatureExtractor.BuildSignature("K", Lea9Bytes(), Array.Empty<long>()));
        }

        [Fact]
        public void BuildSignature_RelativeFollowOffsetBeyondPattern_Throws() {
            // Guard against malformed [StaticAddress] attributes where relativeFollowOffset
            // exceeds pattern length — would produce a positive rewindBack which silently
            // scans past the match.
            var bad = new Info(pattern: "48 8D 0D ?? ?? ?? ??", relativeFollowOffset: 99, isPointer: false, fullTypeName: "Test.Bad");
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                FFXIVClientStructsSignatureExtractor.BuildSignature("K", bad, innerOffset: 0));
        }

        [Fact]
        public void BuildSignature_PatternAcceptsSpacedAndDashedForms() {
            // Normaliser strips both spaces and dashes. "AA-BB ?? CC" should produce the same
            // hex as "AABB??CC".
            var a = new Info(pattern: "AA BB ?? CC", relativeFollowOffset: 0, isPointer: false, fullTypeName: "A");
            var b = new Info(pattern: "AA-BB ?? CC", relativeFollowOffset: 0, isPointer: false, fullTypeName: "B");
            var sigA = FFXIVClientStructsSignatureExtractor.BuildSignature("K", a, innerOffset: 0);
            var sigB = FFXIVClientStructsSignatureExtractor.BuildSignature("K", b, innerOffset: 0);
            Assert.Equal(sigA.Value, sigB.Value);
            Assert.Equal("AABB??CC", sigA.Value);
        }
    }
}
