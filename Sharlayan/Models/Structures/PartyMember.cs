// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartyMember.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   PartyMember.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public class PartyMember {
        public int DefaultStatusEffectOffset { get; set; }

        public int HPCurrent { get; set; }

        public int HPMax { get; set; }

        public int ID { get; set; }

        public int Job { get; set; }

        public int Level { get; set; }

        public int MPCurrent { get; set; }

        public int Name { get; set; }

        public int SourceSize { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }
    }
}