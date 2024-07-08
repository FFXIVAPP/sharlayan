// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPartyMember.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   IPartyMember.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.Interfaces {
    using System.Collections.Generic;

    using Sharlayan.Core.Enums;

    public interface IPartyMember {
        int HPCurrent { get; set; }

        int HPMax { get; set; }

        uint ID { get; set; }

        Actor.Job Job { get; set; }

        byte JobID { get; set; }

        byte Level { get; set; }

        int MPCurrent { get; set; }

        int MPMax { get; set; }

        string Name { get; set; }

        List<StatusItem> StatusItems { get; }

        string UUID { get; set; }

        double X { get; set; }

        double Y { get; set; }

        double Z { get; set; }

        PartyMember Clone();
    }
}