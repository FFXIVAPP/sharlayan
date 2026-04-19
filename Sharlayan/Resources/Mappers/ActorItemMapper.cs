// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActorItemMapper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Builds a Sharlayan Models.Structures.ActorItem whose int-offset properties describe
//   where each actor field lives inside FFXIVClientStructs' Character struct. Sharlayan's
//   ActorItemResolver reads game memory at these offsets to populate the public
//   Core.ActorItem returned by Reader.GetActors().
//
//   Coordinate convention: Sharlayan's ActorItem.X/Y/Z historically swapped the game's
//   native Y and Z — entry.Y is read from the third float of Position, entry.Z from the
//   second float. See ActorItemResolver.cs for the consumer reads. We preserve this swap
//   so downstream clients see the same X/Y/Z values they always did.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Client.Game.Character;
    using FFXIVClientStructs.FFXIV.Client.Game.Object;
    using FFXIVClientStructs.FFXIV.Common.Math;

    using Sharlayan.Models.Structures;

    internal static class ActorItemMapper {
        public static ActorItem Build() {
            int positionOffset = (int)Marshal.OffsetOf<Character>(nameof(Character.Position));
            int vector3X = (int)Marshal.OffsetOf<Vector3>(nameof(Vector3.X));
            int vector3Y = (int)Marshal.OffsetOf<Vector3>(nameof(Vector3.Y));
            int vector3Z = (int)Marshal.OffsetOf<Vector3>(nameof(Vector3.Z));

            // Casting & status offsets are BattleChara-relative (0x2790 + field within CastInfo,
            // 0x23B0 for StatusManager). Non-BattleChara actors will read garbage at these
            // offsets, same as the legacy mapping — callers guard on IsCasting1 being truthy.
            int castInfoBase     = (int)Marshal.OffsetOf<BattleChara>(nameof(BattleChara.CastInfo));
            int statusManagerOff = (int)Marshal.OffsetOf<BattleChara>(nameof(BattleChara.StatusManager));
            int ciIsCasting      = (int)Marshal.OffsetOf<CastInfo>(nameof(CastInfo.IsCasting));
            int ciInterruptible  = (int)Marshal.OffsetOf<CastInfo>(nameof(CastInfo.Interruptible));
            int ciActionId       = (int)Marshal.OffsetOf<CastInfo>(nameof(CastInfo.ActionId));
            int ciTargetId       = (int)Marshal.OffsetOf<CastInfo>(nameof(CastInfo.TargetId));
            int ciCurrentCast    = (int)Marshal.OffsetOf<CastInfo>(nameof(CastInfo.CurrentCastTime));
            int ciTotalCast      = (int)Marshal.OffsetOf<CastInfo>(nameof(CastInfo.TotalCastTime));

            return new ActorItem {
                // --- GameObject base (offset 0 within Character) ----------------------
                Name = FieldOffsetReader.OffsetOf<GameObject>("_name"),
                ID = (int)Marshal.OffsetOf<Character>(nameof(Character.EntityId)),
                OwnerID = (int)Marshal.OffsetOf<Character>(nameof(Character.OwnerId)),
                Type = (int)Marshal.OffsetOf<Character>(nameof(Character.ObjectKind)),
                Heading = (int)Marshal.OffsetOf<Character>(nameof(Character.Rotation)),
                HitBoxRadius = (int)Marshal.OffsetOf<Character>(nameof(Character.HitboxRadius)),
                Fate = (int)Marshal.OffsetOf<Character>(nameof(Character.FateId)),
                NPCID1 = (int)Marshal.OffsetOf<Character>(nameof(Character.BaseId)),
                Distance = (int)Marshal.OffsetOf<Character>(nameof(Character.YalmDistanceFromPlayerX)),

                // Position vector — Y and Z intentionally swapped vs. the C++ struct layout.
                // See class header for rationale.
                X = positionOffset + vector3X,
                Z = positionOffset + vector3Y,
                Y = positionOffset + vector3Z,

                // --- CharacterData base (offset 0x1A0 within Character, flattened by source gen) --
                HPCurrent = (int)Marshal.OffsetOf<Character>(nameof(Character.Health)),
                HPMax = (int)Marshal.OffsetOf<Character>(nameof(Character.MaxHealth)),
                MPCurrent = (int)Marshal.OffsetOf<Character>(nameof(Character.Mana)),
                GPCurrent = (int)Marshal.OffsetOf<Character>(nameof(Character.GatheringPoints)),
                GPMax = (int)Marshal.OffsetOf<Character>(nameof(Character.MaxGatheringPoints)),
                CPCurrent = (int)Marshal.OffsetOf<Character>(nameof(Character.CraftingPoints)),
                CPMax = (int)Marshal.OffsetOf<Character>(nameof(Character.MaxCraftingPoints)),
                Title = (int)Marshal.OffsetOf<Character>(nameof(Character.TitleId)),
                Job = (int)Marshal.OffsetOf<Character>(nameof(Character.ClassJob)),
                Level = (int)Marshal.OffsetOf<Character>(nameof(Character.Level)),
                Icon = (int)Marshal.OffsetOf<Character>(nameof(Character.Icon)),

                // --- Character's own fields -----------------------------------------
                IsGM = (int)Marshal.OffsetOf<Character>(nameof(Character.GMRank)),
                TargetID = (int)Marshal.OffsetOf<Character>(nameof(Character.TargetId)),
                NPCID2 = (int)Marshal.OffsetOf<Character>(nameof(Character.NameId)),

                // --- CharacterData fields flattened into Character ------------------
                ClaimedByID = (int)Marshal.OffsetOf<Character>(nameof(Character.CombatTaggerId)),
                // AgroFlags is a single byte bitfield; Sharlayan's IsAgroed = (AgroFlags & 1) > 0.
                // CharacterData.Flags.IsHostile is bit 0 of that byte — semantic match.
                AgroFlags = (int)Marshal.OffsetOf<Character>(nameof(Character.Flags)),
                // CombatFlags — same byte, different bit (InCombat = bit 1); reuse the offset.
                CombatFlags = (int)Marshal.OffsetOf<Character>(nameof(Character.Flags)),
                // Status byte for nameplate/targetability — GameObject.TargetStatus (0x95). Different
                // from legacy's "Status" which was a 16-bit status-effect id slot; no longer exists.
                // TargetFlags maps to the targetability-related flags on GameObject.
                TargetFlags = (int)Marshal.OffsetOf<Character>(nameof(Character.TargetableStatus)),
                // EventObjectType → EventId on GameObject (32-bit event id; legacy read a ushort).
                EventObjectType = (int)Marshal.OffsetOf<Character>(nameof(Character.EventId)),

                // --- BattleChara casting + status (offsets only valid for battle actors) ---
                IsCasting1      = castInfoBase + ciIsCasting,
                IsCasting2      = castInfoBase + ciInterruptible,
                CastingID       = castInfoBase + ciActionId,
                CastingTargetID = castInfoBase + ciTargetId,
                CastingProgress = castInfoBase + ciCurrentCast,
                CastingTime     = castInfoBase + ciTotalCast,
                // Status offset = base of BattleChara.StatusManager; consumers iterate
                // StatusManager._status[60] from there.
                Status          = statusManagerOff,

                // --- Bookkeeping ----------------------------------------------------
                // SourceSize tells ActorItemResolver how many bytes to read per actor.
                // Must match the full Character struct size.
                SourceSize = Marshal.SizeOf<Character>(),

                // EntityCount is the loop limit used by Reader.GetActors — number of
                // 8-byte pointer slots to read from the CHARMAP pointer array. Matches
                // FCS' GameObjectManager.ObjectArrays._indexSorted: FixedSizeArray819.
                EntityCount = 819,

                // The following Sharlayan fields have no clean direct equivalent in
                // FFXIVClientStructs' Character type at this time. Leaving them at
                // default(int)=0 preserves the legacy "field absent from JSON" behaviour,
                // and the existing resolvers tolerate the zero case. Revisit when
                // consumers need them:
                //   ActionStatus, AgroFlags, CastingID, CastingProgress, CastingTargetID,
                //   CastingTime, ClaimedByID, CombatFlags, DefaultBaseOffset,
                //   DefaultStatOffset, DefaultStatusEffectOffset, DifficultyRank,
                //   EntityCount, EventObjectType, GatheringInvisible, GatheringStatus,
                //   GrandCompany, GrandCompanyRank, InCutscene, IsCasting1, IsCasting2,
                //   ModelID, Status, TargetFlags, TargetType.
            };
        }
    }
}
