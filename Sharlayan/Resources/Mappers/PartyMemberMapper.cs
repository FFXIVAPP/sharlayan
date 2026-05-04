// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartyMemberMapper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Builds Sharlayan's Models.Structures.PartyMember from FFXIVClientStructs'
//   Client::Game::Group::PartyMember struct. Note: the FFXIVClientStructs type has the
//   same unqualified name as Sharlayan's, so it is aliased as NativePartyMember here.
//   This is a distinct struct from Character — GroupManager stores a compact
//   FixedSizeArray8<PartyMember> where each entry carries only party-relevant fields.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System.Runtime.InteropServices;

    using FFXIVClientStructs.FFXIV.Common.Math;

    using Sharlayan.Models.Structures;

    using NativePartyMember = FFXIVClientStructs.FFXIV.Client.Game.Group.PartyMember;

    internal static class PartyMemberMapper {
        public static PartyMember Build() {
            int positionOffset = (int)Marshal.OffsetOf<NativePartyMember>(nameof(NativePartyMember.Position));
            int vector3X = (int)Marshal.OffsetOf<Vector3>(nameof(Vector3.X));
            int vector3Y = (int)Marshal.OffsetOf<Vector3>(nameof(Vector3.Y));
            int vector3Z = (int)Marshal.OffsetOf<Vector3>(nameof(Vector3.Z));

            return new PartyMember {
                HPCurrent = (int)Marshal.OffsetOf<NativePartyMember>(nameof(NativePartyMember.CurrentHP)),
                HPMax = (int)Marshal.OffsetOf<NativePartyMember>(nameof(NativePartyMember.MaxHP)),
                MPCurrent = (int)Marshal.OffsetOf<NativePartyMember>(nameof(NativePartyMember.CurrentMP)),
                ID = (int)Marshal.OffsetOf<NativePartyMember>(nameof(NativePartyMember.EntityId)),
                Name = FieldOffsetReader.OffsetOf<NativePartyMember>("_name"),
                Job = (int)Marshal.OffsetOf<NativePartyMember>(nameof(NativePartyMember.ClassJob)),
                Level = (int)Marshal.OffsetOf<NativePartyMember>(nameof(NativePartyMember.Level)),

                // Preserve Sharlayan's historical Y/Z swap — see ActorItemMapper.cs.
                X = positionOffset + vector3X,
                Z = positionOffset + vector3Y,
                Y = positionOffset + vector3Z,

                SourceSize = Marshal.SizeOf<NativePartyMember>(),

                // DefaultStatusEffectOffset → byte offset within NativePartyMember of the
                // FIRST Status entry (StatusManager._status[0]). PartyMemberResolver does a
                // single BlockCopy from this offset to scan all status slots, so it must
                // point at the array's first element, not the StatusManager struct base
                // (which has an `Owner` pointer at +0). _status is internal; resolved via
                // string-name lookup so FCS field-offset bumps continue to track.
                DefaultStatusEffectOffset = (int)Marshal.OffsetOf<NativePartyMember>(nameof(NativePartyMember.StatusManager))
                                            + FieldOffsetReader.OffsetOf<FFXIVClientStructs.FFXIV.Client.Game.StatusManager>("_status"),
            };
        }
    }
}
