// FFXIVAPP.Memory ~ ChatLogPointers.cs
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

namespace FFXIVAPP.Memory.Models
{
    internal class ChatLogPointers
    {
        public uint LineCount { get; set; }
        public uint OffsetArrayStart { get; set; }
        public uint OffsetArrayPos { get; set; }
        public uint OffsetArrayEnd { get; set; }
        public uint LogStart { get; set; }
        public uint LogNext { get; set; }
        public uint LogEnd { get; set; }
    }
}
