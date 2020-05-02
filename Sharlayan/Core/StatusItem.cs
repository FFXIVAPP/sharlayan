﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   StatusItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core {
    using Sharlayan.Core.Interfaces;
    using Sharlayan.Extensions;

    public class StatusItem : IStatusItem {
        private string _targetName;

        public uint CasterID { get; set; }

        public float Duration { get; set; }

        public bool IsCompanyAction { get; set; }

        public ActorItem SourceEntity { get; set; }

        public byte Stacks { get; set; }

        public short StatusID { get; set; }

        public string StatusName { get; set; }

        public ActorItem TargetEntity { get; set; }

        public string TargetName {
            get {
                return this._targetName;
            }

            set {
                this._targetName = value.ToTitleCase();
            }
        }

        public bool IsValid() {
            return this.StatusID > 0 && this.Duration <= 86400 && this.CasterID > 0;
        }
    }
}