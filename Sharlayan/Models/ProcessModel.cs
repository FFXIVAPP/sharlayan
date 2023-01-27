// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessModel.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ProcessModel.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models {
    using System.Diagnostics;

    public class ProcessModel {
        public Process Process { get; set; }
        public int ProcessID => this.Process?.Id ?? -1;
        public string ProcessName => this.Process?.ProcessName ?? string.Empty;
    }
}