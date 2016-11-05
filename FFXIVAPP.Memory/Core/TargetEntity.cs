// FFXIVAPP.Memory ~ TargetEntity.cs
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

using System.Collections.Generic;
using FFXIVAPP.Memory.Core.Interfaces;

namespace FFXIVAPP.Memory.Core
{
    public class TargetEntity : ITargetEntity
    {
        private List<EnmityEntry> _enmityEntries;
        public ActorEntity CurrentTarget { get; set; }
        public ActorEntity MouseOverTarget { get; set; }
        public ActorEntity FocusTarget { get; set; }
        public ActorEntity PreviousTarget { get; set; }
        public uint CurrentTargetID { get; set; }

        public List<EnmityEntry> EnmityEntries
        {
            get { return _enmityEntries ?? (_enmityEntries = new List<EnmityEntry>()); }
            set
            {
                if (_enmityEntries == null)
                {
                    _enmityEntries = new List<EnmityEntry>();
                }
                _enmityEntries = value;
            }
        }
    }
}
