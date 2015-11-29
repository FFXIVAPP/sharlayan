// FFXIVAPP.Memory
// PartyEntity.cs
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
using FFXIVAPP.Memory.Core.Enums;
using FFXIVAPP.Memory.Core.Interfaces;
using FFXIVAPP.Memory.Helpers;

namespace FFXIVAPP.Memory.Core
{
    public class PartyEntity : IPartyEntity
    {
        private string _name;
        private List<StatusEntry> _statusEntries;

        public double HPPercent
        {
            get { return (double) (HPMax == 0 ? 0 : Decimal.Divide(HPCurrent, HPMax)); }
        }

        public string HPString
        {
            get { return String.Format("{0}/{1} [{2:P2}]", HPCurrent, HPMax, HPPercent); }
        }

        public double MPPercent
        {
            get { return (double) (MPMax == 0 ? 0 : Decimal.Divide(MPCurrent, MPMax)); }
        }

        public string MPString
        {
            get { return String.Format("{0}/{1} [{2:P2}]", MPCurrent, MPMax, MPPercent); }
        }

        public bool IsValid
        {
            get { return ID > 0 && !String.IsNullOrWhiteSpace(Name); }
        }

        public Coordinate Coordinate { get; set; }

        public string Name
        {
            get { return _name ?? ""; }
            set { _name = StringHelper.TitleCase(value); }
        }

        public uint ID { get; set; }
        public double X { get; set; }
        public double Z { get; set; }
        public double Y { get; set; }
        public Actor.Job Job { get; set; }
        public byte Level { get; set; }
        public int HPCurrent { get; set; }
        public int HPMax { get; set; }
        public int MPCurrent { get; set; }
        public int MPMax { get; set; }

        public List<StatusEntry> StatusEntries
        {
            get { return _statusEntries ?? (_statusEntries = new List<StatusEntry>()); }
            set
            {
                if (_statusEntries == null)
                {
                    _statusEntries = new List<StatusEntry>();
                }
                _statusEntries = value;
            }
        }

        public float GetDistanceTo(ActorEntity compare)
        {
            var distanceX = (float) Math.Abs(X - compare.X);
            var distanceY = (float) Math.Abs(Y - compare.Y);
            var distanceZ = (float) Math.Abs(Z - compare.Z);
            return (float) Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY) + (distanceZ * distanceZ));
        }
    }
}
