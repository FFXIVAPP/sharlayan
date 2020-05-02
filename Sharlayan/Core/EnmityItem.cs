﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnmityItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   EnmityItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core {
    using Sharlayan.Core.Interfaces;
    using Sharlayan.Extensions;

    public class EnmityItem : IEnmityItem {
        private string _name;

        public uint Enmity { get; set; }

        public uint ID { get; set; }

        public string Name {
            get => this._name ?? string.Empty;
            set => this._name = value.ToTitleCase();
        }
    }
}