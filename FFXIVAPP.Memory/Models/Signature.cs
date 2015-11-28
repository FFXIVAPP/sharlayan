// FFXIVAPP.Memory
// Signature.cs
// 
// Copyright © 2007 - 2015 Ryan Wilson - All Rights Reserved
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions are met: 
// 
//  * Redistributions of source code must retain the above copyright notice, 
//    this list of conditions and the following disclaimer. 
//  * Redistributions in binary form must reproduce the above copyright 
//    notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution. 
//  * Neither the name of SyndicatedLife nor the names of its contributors may 
//    be used to endorse or promote products derived from this software 
//    without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE. 

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
                baseAddress = SigScanAddress; // SigScanner should have already applied the base offset
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
                baseAddress = MemoryHandler.GetStaticAddress(0);
            }
            if (PointerPath == null || PointerPath.Count == 0)
            {
                return baseAddress;
            }
            return MemoryHandler.ResolvePointerPath(PointerPath, baseAddress, FirstIsOffsetAddress);
        }

        // convenience conversion for less code breakage. 
        // FIXME: convert all calling functions to handle IntPtr properly someday, and stop using long for addresses
        public static implicit operator IntPtr(Signature value)
        {
            return value.GetAddress();
        }
    }
}
