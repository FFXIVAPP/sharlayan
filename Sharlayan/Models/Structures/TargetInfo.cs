// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetInfo.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   TargetInfo.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public class TargetInfo {
        public int Current { get; set; }

        public int CurrentID { get; set; }

        public int Focus { get; set; }

        public int MouseOver { get; set; }

        public int Previous { get; set; }

        public int Size { get; set; }

        public int SourceSize { get; set; }
    }
}