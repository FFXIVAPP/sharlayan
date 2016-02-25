// FFXIVAPP.Memory ~ Signature.cs
// 
// Copyright © 2007 - 2016 Ryan Wilson - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FFXIVAPP.Memory.Models
{
    public class Signature
    {
        private int _Offset;
        private bool offsetSet;

        public Signature()
        {
            Key = "";
            Value = "";
            RegularExpress = null;
            SigScanAddress = IntPtr.Zero;
            PointerPath = null;
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public Regex RegularExpress { get; set; }
        public IntPtr SigScanAddress { get; set; }
        public bool ASMSignature { get; set; }

        public int Offset
        {
            get
            {
                if (!offsetSet)
                {
                    _Offset = Value.Length / 2;
                }
                return _Offset;
            }
            set
            {
                offsetSet = true;
                _Offset = value;
            }
        }

        public List<long> PointerPath { get; set; }

        public IntPtr GetAddress()
        {
            var baseAddress = IntPtr.Zero;
            var FirstIsOffsetAddress = false;
            if (SigScanAddress != IntPtr.Zero)
            {
                baseAddress = SigScanAddress; // Scanner should have already applied the base offset
                if (MemoryHandler.Instance.ProcessModel.IsWin64 && ASMSignature)
                {
                    FirstIsOffsetAddress = true;
                }
            }
            else
            {
                if (PointerPath == null || PointerPath.Count == 0)
                {
                    return IntPtr.Zero;
                }
                baseAddress = MemoryHandler.Instance.GetStaticAddress(0);
            }
            if (PointerPath == null || PointerPath.Count == 0)
            {
                return baseAddress;
            }
            return MemoryHandler.Instance.ResolvePointerPath(PointerPath, baseAddress, FirstIsOffsetAddress);
        }

        // convenience conversion for less code breakage. 
        // FIXME: convert all calling functions to handle IntPtr properly someday, and stop using long for addresses
        public static implicit operator IntPtr(Signature value)
        {
            return value.GetAddress();
        }
    }
}
