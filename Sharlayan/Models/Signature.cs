// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Signature.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Signature.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models {
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Newtonsoft.Json;

    public class Signature {
        private Regex _regularExpress;

        public Signature() {
            this.Key = string.Empty;
            this.Value = string.Empty;
            this.RegularExpress = null;
            this.SigScanAddress = IntPtr.Zero;
            this.PointerPath = null;
        }

        public bool ASMSignature { get; set; }

        public string Key { get; set; }

        [JsonIgnore,]
        public int Offset => this.Value.Length / 2;

        public List<long> PointerPath { get; set; }

        public Regex RegularExpress {
            get => this._regularExpress;

            set {
                if (value != null) {
                    this._regularExpress = value;
                }
            }
        }

        [JsonIgnore,]
        public IntPtr SigScanAddress { get; set; }

        public string Value { get; set; }
    }
}