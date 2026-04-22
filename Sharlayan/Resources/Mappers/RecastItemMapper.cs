// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecastItemMapper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Maps Sharlayan's RecastItem against FFXIVClientStructs' ActionBarSlotNumberArray
//   (Client::UI::Arrays::Common). This is the UI-side cache of per-slot state that the
//   action bar addons render from — IsAvailable / InRange / CoolDown% / charge display /
//   proc highlighting all live here, computed by the game each frame.
//
//   The earlier mapping pointed at ActionManager.RecastDetail which has ONLY raw cooldown
//   timers — Chromatics' Keybinds layer (and any consumer of Reader.GetActions) requires
//   the full UI slot struct. RECAST_KEY now resolves via
//   AtkStage → AtkArrayDataHolder.NumberArrays[ActionBar=7] → IntArray + 60 bytes
//   (see FFXIVClientStructsDirectProvider). Container stride = 272 × 4 bytes per bar,
//   item stride = 17 × 4 bytes per slot.
//
//   Intentionally unmapped (no clean equivalent on this struct):
//     - Category: the old JSON encoded action-category id here; the UI array only carries
//       ActionType. Consumers that need the ActionCategory row should look up the action
//       ID via Lumina.
//     - Amount / ChargesRemaining == same slot as CurrentCharges (ChargeReady reads it as
//       bool, ChargesRemaining reads it as int — both via CurrentCharges offset).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.UI.Arrays.Common;

    using Sharlayan.Models.Structures;

    internal static class RecastItemMapper {
        public static RecastItem Build() {
            int slotSize = Marshal.SizeOf<ActionBarSlotNumberArray>();

            return new RecastItem {
                // Each hotbar's bar-level struct is 272 ints = 1088 bytes; its _slots FixedSizeArray12
                // is at bar+0x00, so stride between bars == ContainerSize. Reader.Actions reads
                // ContainerSize bytes per type and iterates ItemSize-sized slots inside.
                ContainerSize = 272 * sizeof(int),
                ItemSize = slotSize,

                ID = (int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.ActionId)),
                // FCS labels this field ActionType, but the game actually stores the
                // ActionCategory row id there (e.g. BRD weaponskills = 47, Limit Break = 51).
                // Sharlayan's Type AND Category both map to that same int — the Type value
                // matches the Category semantic that legacy consumers (Chromatics) expect.
                Type = (int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.ActionType)),
                Category = (int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.ActionType)),
                Icon = (int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.IconId)),

                IsAvailable = (int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.Executable)),
                InRange = (int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.InRange)),

                // Reader.Actions reads a single byte at CoolDownPercent; GlobalCoolDownPercentage
                // is an int in [0,100], so the low byte equals the percent.
                CoolDownPercent = (int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.GlobalCoolDownPercentage)),

                // Reader reads ChargeReady via TryToBoolean (byte != 0) and ChargesRemaining via
                // TryToInt32 — both from the same CurrentCharges int field.
                ChargeReady = (int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.CurrentCharges)),
                ChargesRemaining = (int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.CurrentCharges)),

                // Glows is the steady proc/combo indicator — True while the action is highlighted
                // (e.g. Refulgent Arrow after Straight Shot crit). Pulses is a short animation
                // trigger fired on keypress and flickers off again, so it's unreliable for
                // polling. Reader.Actions reads the byte and sets IsProcOrCombo = byte > 0.
                ActionProc = (int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.Glows)),

                // ManaCost / RechargeTime share the same int slot; ManaCost is what the addon
                // displays for action cost. TryToInt32 pulls the int value; the reader treats -1
                // as "no cost" and zeroes it.
                RemainingCost = (int)Marshal.OffsetOf<ActionBarSlotNumberArray>(nameof(ActionBarSlotNumberArray.ManaCost)),

                // Amount is read as int32 — the UI array doesn't carry a per-slot quantity
                // separate from CurrentCharges. Leave unmapped (= 0).
            };
        }
    }
}
