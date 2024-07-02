// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   StatusItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core {
    using Sharlayan.Core.Interfaces;

    public class StatusItem : IStatusItem {
        public uint CasterID { get; set; }

        public float Duration { get; set; }

        public bool IsCompanyAction { get; set; }

        public ActorItem SourceEntity { get; set; }

        public byte Stacks { get; set; }

        public short StatusID { get; set; }

        public string StatusName { get; set; }

        public ActorItem TargetEntity { get; set; }

        public string TargetName { get; set; }

        public bool IsValid() {
            return this.StatusID > 0 && this.Duration <= 86400 && this.CasterID > 0;
        }
    }
}