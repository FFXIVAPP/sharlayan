// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HateItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Offset layout for FFXIVClientStructs' HateInfo — used by Reader.Target to iterate
//   UIState.Hate._hateInfo when building the enmity list on the current target.
//   Distinct from EnmityItem (which uses HaterInfo for the AGROMAP / player aggro list).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public class HateItem {
        public int ID { get; set; }

        public int Enmity { get; set; }

        public int SourceSize { get; set; }
    }
}
