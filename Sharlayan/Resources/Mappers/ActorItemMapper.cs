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

                // --- Bookkeeping ----------------------------------------------------
                // SourceSize tells ActorItemResolver how many bytes to read per actor.
                // Must match the full Character struct size.
                SourceSize = Marshal.SizeOf<Character>(),

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
