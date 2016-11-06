// Bootstrapper
// FFXIVAPP & Related Plugins/Modules
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
using FFXIVAPP.Memory;
using FFXIVAPP.Memory.Helpers;
using FFXIVAPP.Memory.Models;

namespace Bootstrapper
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ZoneHelper.MapInfo(1);
            StatusEffectHelper.StatusInfo(1);
            MemoryHandler.Instance.SetStructures(new ProcessModel());
            MemoryHandler.Instance.SetEnumerations(new ProcessModel());
            Scanner.Instance.LoadOffsets(Signatures.Resolve(false));
            Console.WriteLine("To exit this application press \"Enter\".");
            Console.ReadLine();
        }
    }
}
