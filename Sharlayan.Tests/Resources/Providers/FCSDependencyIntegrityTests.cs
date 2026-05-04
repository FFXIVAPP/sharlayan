// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FCSDependencyIntegrityTests.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Guards Sharlayan's binding surface against the vendored FFXIVClientStructs submodule.
//   Every FCS type Sharlayan references by string (singleton type names in the provider,
//   private field names in FieldOffsetReader) plus every hard-coded chain offset must
//   still resolve against the current submodule snapshot — any upstream rename or
//   offset change that would otherwise silently break a scanner key fails a test here.
//
//   When a test fails after a `git submodule update` of FFXIVClientStructs: fix the
//   binding in Sharlayan AND update DEPENDENCY.md with the new path/offset.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Tests.Resources.Providers {
    using System.Reflection;
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game;
    using FFXIVClientStructs.FFXIV.Client.Game.UI;
    using FFXIVClientStructs.FFXIV.Client.System.Framework;
    using FFXIVClientStructs.FFXIV.Client.System.String;
    using FFXIVClientStructs.FFXIV.Client.UI;
    using FFXIVClientStructs.FFXIV.Client.UI.Arrays;
    using FFXIVClientStructs.FFXIV.Client.UI.Misc;
    using FFXIVClientStructs.FFXIV.Component.GUI;

    using InteropGenerator.Runtime.Attributes;

    using Sharlayan.Resources.Mappers;
    using Sharlayan.Resources.Providers;

    using Xunit;

    using NativePartyMember = FFXIVClientStructs.FFXIV.Client.Game.Group.PartyMember;

    public class FCSDependencyIntegrityTests {
        // ------------------------------------------------------------------------------
        // Singleton type names — passed as strings to FFXIVClientStructsDirectProvider's
        // TryAdd / TryAddChain and resolved at runtime by the reflection cache. Upstream
        // renames escape the C# compiler; catch them here.
        // ------------------------------------------------------------------------------

        public static readonly TheoryData<string> SingletonTypeNames = new() {
            "GameObjectManager",
            "PlayerState",
            "GroupManager",
            "TargetSystem",
            "InventoryManager",
            "JobGaugeManager",
            "GameMain",
            "TerritoryInfo",
            "UIState",
            "Conditions",
            "ContentsFinder",
            "WeatherManager",
            "BGMSystem",
            "SoundManager",
            "Framework",
            "AtkStage",
        };

        [Theory]
        [MemberData(nameof(SingletonTypeNames))]
        public void SingletonTypeNames_Resolve(string fcsTypeName) {
            // Triggers the extractor cache. TryGet returns true iff the type exists AND has a
            // static method carrying [StaticAddress]. Both conditions matter — losing either
            // silently drops the scanner key.
            bool found = FFXIVClientStructsSignatureExtractor.TryGet(fcsTypeName, out var info);
            Assert.True(found, $"{fcsTypeName} not found in FCS extractor cache — upstream renamed or dropped [StaticAddress] from Instance().");
            Assert.False(string.IsNullOrEmpty(info.Pattern), $"{fcsTypeName}'s [StaticAddress] has an empty pattern — patch-day signature likely bumped.");
        }

        // ------------------------------------------------------------------------------
        // Private field names accessed via FieldOffsetReader (reflection — strings, not
        // nameof()). A rename here produces offset 0 silently and corrupts downstream
        // reads.
        // ------------------------------------------------------------------------------

        [Fact]
        public void PrivateFieldOffset_GameObject_name_Resolves() {
            AssertPrivateFieldExists<FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject>("_name");
        }

        [Fact]
        public void PrivateFieldOffset_HaterInfo_name_Resolves() {
            AssertPrivateFieldExists<FFXIVClientStructs.FFXIV.Client.Game.UI.HaterInfo>("_name");
        }

        [Fact]
        public void PrivateFieldOffset_PartyMember_name_Resolves() {
            AssertPrivateFieldExists<NativePartyMember>("_name");
        }

        [Fact]
        public void PrivateFieldOffset_InventoryItem_materia_Resolves() {
            AssertPrivateFieldExists<InventoryItem>("_materia");
        }

        [Fact]
        public void PrivateFieldOffset_InventoryItem_materiaGrades_Resolves() {
            AssertPrivateFieldExists<InventoryItem>("_materiaGrades");
        }

        [Fact]
        public void PrivateFieldOffset_InventoryItem_stains_Resolves() {
            AssertPrivateFieldExists<InventoryItem>("_stains");
        }

        [Fact]
        public void PrivateFieldOffset_DancerGauge_danceSteps_Resolves() {
            AssertPrivateFieldExists<FFXIVClientStructs.FFXIV.Client.Game.Gauge.DancerGauge>("_danceSteps");
        }

        [Fact]
        public void PrivateFieldOffset_HotbarSlot_popUpKeybindHint_Resolves() {
            AssertPrivateFieldExists<RaptureHotbarModule.HotbarSlot>("_popUpKeybindHint");
        }

        [Fact]
        public void PrivateFieldOffset_Utf8String_inlineBuffer_Resolves() {
            AssertPrivateFieldExists<Utf8String>("_inlineBuffer");
        }

        [Fact]
        public void PrivateFieldOffset_SoundManager_FFTBlue1_Resolves() {
            AssertPrivateFieldExists<FFXIVClientStructs.FFXIV.Client.Sound.SoundManager>("_FFTBlue1");
        }



        [Fact]
        public void PrivateFieldOffset_PlayerState_classJobLevels_Resolves() {
            AssertPrivateFieldExists<PlayerState>("_classJobLevels");
        }

        [Fact]
        public void PrivateFieldOffset_PlayerState_classJobExperience_Resolves() {
            AssertPrivateFieldExists<PlayerState>("_classJobExperience");
        }

        [Fact]
        public void PrivateFieldOffset_Hate_hateInfo_Resolves() {
            // Used by FFXIVClientStructsDirectProvider for ENMITYMAP (UIState.Hate._hateInfo).
            AssertPrivateFieldExists<Hate>("_hateInfo");
        }

        [Fact]
        public void PrivateFieldOffset_Hater_haters_Resolves() {
            // Used by FFXIVClientStructsDirectProvider for AGROMAP (UIState.Hater._haters).
            AssertPrivateFieldExists<Hater>("_haters");
        }

        [Fact]
        public void PrivateFieldOffset_RaptureHotbarModule_hotbars_Resolves() {
            // Used by FFXIVClientStructsDirectProvider for HOTBAR (_hotbars[0] inside RaptureHotbarModule).
            AssertPrivateFieldExists<RaptureHotbarModule>("_hotbars");
        }

        [Fact]
        public void PrivateFieldOffset_ActionBarNumberArray_bars_Resolves() {
            // Used by FFXIVClientStructsDirectProvider for RECAST (_bars inside ActionBarNumberArray).
            AssertPrivateFieldExists<ActionBarNumberArray>("_bars");
        }

        [Fact]
        public void PrivateFieldOffset_UIModule_RaptureLogModule_Resolves() {
            // Internal field — referenced via string in the provider.
            AssertPrivateFieldExists<UIModule>("RaptureLogModule");
        }

        [Fact]
        public void PrivateFieldOffset_UIModule_RaptureHotbarModule_Resolves() {
            AssertPrivateFieldExists<UIModule>("RaptureHotbarModule");
        }

        // ------------------------------------------------------------------------------
        // Known-good offsets — documentation pins, not provider guards. The provider now
        // reads these from [FieldOffset] at runtime, so a change upstream silently
        // propagates to correct scanner addresses. These assertions instead:
        //   (1) Pin the current known-good value so DEPENDENCY.md stays accurate;
        //   (2) Fail loudly on the first run after a submodule bump that moved the field,
        //       prompting the author to verify the new offset and update DEPENDENCY.md.
        // The asserted constants are only "hard-coded" in the test, never in production.
        // ------------------------------------------------------------------------------

        // All hard-coded offsets are verified by reading the [FieldOffset] attribute from
        // reflection rather than Marshal.OffsetOf — the latter chokes on FCS's Explicit-
        // layout structs that contain managed function pointers (AtkStage, UIModule, etc.).
        // The attribute is always present since FCS uses LayoutKind.Explicit across the board.

        [Fact]
        public void HardCodedChainOffset_Framework_UIModule_Is_0x2B68() {
            Assert.Equal(0x2B68, FieldOffsetAttributeValue(typeof(Framework), nameof(Framework.UIModule)));
        }

        [Fact]
        public void HardCodedChainOffset_UIModule_RaptureLogModule_Is_0x1AC0() {
            // Bumped from 0x19E0 → 0x1AC0 by FCS commit "Update UIModule" (036e201a2 base).
            // Runtime reads via FieldOffsetReader so CHATLOG_KEY auto-tracks the move; this
            // test merely pins the known-good value for the next FCS bump diff.
            Assert.Equal(0x1AC0, FieldOffsetAttributeValue(typeof(UIModule), "RaptureLogModule"));
        }

        [Fact]
        public void HardCodedChainOffset_UIModule_RaptureHotbarModule_Is_0x57C60() {
            // Bumped from 0x57B80 → 0x57C60 by the same FCS commit ("Update UIModule").
            // Same +0xE0 shift as RaptureLogModule — fields added before both modules.
            Assert.Equal(0x57C60, FieldOffsetAttributeValue(typeof(UIModule), "RaptureHotbarModule"));
        }

        [Fact]
        public void HardCodedChainOffset_StatusManager_status_Is_0x8() {
            // ActorItemMapper / PartyMemberMapper add this to the StatusManager struct
            // base to land on the first Status slot. If FCS shifts _status (rare — Owner
            // pointer at +0 is stable), the resolvers would silently read a wrong region.
            Assert.Equal(0x8, FieldOffsetAttributeValue(typeof(FFXIVClientStructs.FFXIV.Client.Game.StatusManager), "_status"));
        }

        [Fact]
        public void PrivateFieldOffset_StatusManager_status_Resolves() {
            AssertPrivateFieldExists<FFXIVClientStructs.FFXIV.Client.Game.StatusManager>("_status");
        }

        [Fact]
        public void HardCodedChainOffset_RaptureHotbarModule_hotbars_Is_0xA0() {
            // _hotbars is internal FixedSizeArray18<Hotbar> at +0xA0. Provider adds +0xA0
            // on top of the RaptureHotbarModule offset to land on _hotbars[0].
            Assert.Equal(0xA0, FieldOffsetAttributeValue(typeof(RaptureHotbarModule), "_hotbars"));
        }

        [Fact]
        public void HardCodedChainOffset_AtkStage_AtkArrayDataHolder_Is_0x38() {
            Assert.Equal(0x38, FieldOffsetAttributeValue(typeof(AtkStage), nameof(AtkStage.AtkArrayDataHolder)));
        }

        [Fact]
        public void HardCodedChainOffset_AtkArrayDataHolder_NumberArrays_Is_0x18() {
            Assert.Equal(0x18, FieldOffsetAttributeValue(typeof(AtkArrayDataHolder), nameof(AtkArrayDataHolder.NumberArrays)));
        }

        [Fact]
        public void HardCodedChainOffset_NumberArrayData_IntArray_Is_0x28() {
            Assert.Equal(0x28, FieldOffsetAttributeValue(typeof(NumberArrayData), nameof(NumberArrayData.IntArray)));
        }

        [Fact]
        public void HardCodedChainOffset_NumberArrayType_ActionBar_Is_7() {
            // Provider indexes NumberArrays via `7 * 8`. If upstream reorders the enum,
            // RECAST points at the wrong number array (chat log, HUD, party list, …).
            Assert.Equal(7, (int)NumberArrayType.ActionBar);
        }

        [Fact]
        public void HardCodedChainOffset_ActionBarNumberArray_bars_Is_60() {
            // _bars is internal FixedSizeArray20<ActionBarBarNumberArray> at +60 (15 * 4).
            Assert.Equal(60, FieldOffsetAttributeValue(typeof(ActionBarNumberArray), "_bars"));
        }

        // ------------------------------------------------------------------------------
        // Smoke check: every mapper's Build() constructor runs to completion without
        // throwing. If a referenced FCS field disappeared, Marshal.OffsetOf throws and
        // this catches it generically — individual mapper tests pin specific fields.
        // ------------------------------------------------------------------------------

        [Fact]
        public void AllMappers_Build_DoesNotThrow() {
            _ = ActorItemMapper.Build();
            _ = PartyMemberMapper.Build();
            _ = PlayerInfoMapper.Build();
            _ = StatusItemMapper.Build();
            _ = InventoryItemMapper.Build();
            _ = InventoryContainerMapper.Build();
            _ = HotBarItemMapper.Build();
            _ = RecastItemMapper.Build();
            _ = TargetInfoMapper.Build();
            _ = EnmityItemMapper.Build();
            _ = HateItemMapper.Build();
            _ = JobResourcesMapper.Build();
            _ = ChatLogPointersMapper.Build();
        }

        // ------------------------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------------------------

        private static void AssertPrivateFieldExists<T>(string name) {
            // Mirrors FieldOffsetReader.OffsetOf — resolves private/internal fields
            // (interop-generator emits FixedSizeArray backing fields as internal).
            var field = typeof(T).GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.NotNull(field);
        }

        private static int FieldOffsetAttributeValue(System.Type type, string fieldName) {
            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        ?? throw new Xunit.Sdk.XunitException($"{type.Name}.{fieldName} does not exist — upstream rename.");
            var attr = field.GetCustomAttribute<FieldOffsetAttribute>()
                       ?? throw new Xunit.Sdk.XunitException($"{type.Name}.{fieldName} has no [FieldOffset] attribute — upstream removed the explicit layout.");
            return attr.Value;
        }
    }
}
