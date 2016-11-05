// FFXIVAPP.Memory ~ StatusEntry.cs
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

using FFXIVAPP.Memory.Core.Interfaces;
using FFXIVAPP.Memory.Helpers;

namespace FFXIVAPP.Memory.Core
{
    public class StatusEntry : IStatusEntry
    {
        private string _targetName;
        public bool IsCompanyAction { get; set; }
        public ActorEntity SourceEntity { get; set; }
        public ActorEntity TargetEntity { get; set; }

        public string TargetName
        {
            get { return _targetName; }
            set { _targetName = StringHelper.TitleCase(value); }
        }

        public string StatusName { get; set; }
        public short StatusID { get; set; }
        public byte Stacks { get; set; }
        public float Duration { get; set; }
        public uint CasterID { get; set; }

        public bool IsValid()
        {
            return StatusID > 0 && Duration <= 86400 && CasterID > 0;
        }
    }
}
