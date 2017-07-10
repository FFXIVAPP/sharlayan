// Sharlayan ~ PartyEntity.cs
// 
// Copyright © 2007 - 2017 Ryan Wilson - All Rights Reserved
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
using Sharlayan.Core.Enums;
using Sharlayan.Core.Interfaces;
using Sharlayan.Helpers;

namespace Sharlayan.Core
{
    public class PartyEntity : IPartyEntity
    {
        private string _name;
        private List<StatusEntry> _statusEntries;

        public double HPPercent
        {
            get { return (double) (HPMax == 0 ? 0 : decimal.Divide(HPCurrent, HPMax)); }
        }

        public string HPString
        {
            get { return $"{HPCurrent}/{HPMax} [{HPPercent:P2}]"; }
        }

        public double MPPercent
        {
            get { return (double) (MPMax == 0 ? 0 : decimal.Divide(MPCurrent, MPMax)); }
        }

        public string MPString
        {
            get { return $"{MPCurrent}/{MPMax} [{MPPercent:P2}]"; }
        }

        public bool IsValid
        {
            get { return ID > 0 && !string.IsNullOrWhiteSpace(Name); }
        }

        public Coordinate Coordinate { get; set; }

        public string Name
        {
            get { return _name ?? string.Empty; }
            set { _name = StringHelper.TitleCase(value); }
        }

        public uint ID { get; set; }
        public string UUID { get; set; }
        public double X { get; set; }
        public double Z { get; set; }
        public double Y { get; set; }
        public byte JobID { get; set; }
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
            return (float) Math.Sqrt(distanceX * distanceX + distanceY * distanceY + distanceZ * distanceZ);
        }
    }
}
