// Bootstrapper
// FFXIVAPP & Related Plugins/Modules
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using FFXIVAPP.Memory;
using FFXIVAPP.Memory.Events;
using FFXIVAPP.Memory.Helpers;
using FFXIVAPP.Memory.Models;
using NLog;
using NLog.Config;

namespace Bootstrapper
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var stringReader = new StringReader(XElement.Load("./Bootstrapper.exe.nlog")
                                                        .ToString());
        
            using (var xmlReader = XmlReader.Create(stringReader))
            {
                LogManager.Configuration = new XmlLoggingConfiguration(xmlReader, null);
            }

            ActionHelper.ActionInfo(2);
            StatusEffectHelper.StatusInfo(2);
            ZoneHelper.MapInfo(138);

            var process = Process.GetProcessesByName("ffxiv_dx11")
                                 .FirstOrDefault();

            MemoryHandler.Instance.SetProcess(new ProcessModel
            {
                IsWin64 = true,
                Process = process
            });

            while (Scanner.Instance.IsScanning)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Scanning...");
            }

            Scanner.Instance.SignaturesFoundEvent += delegate(object sender, SignaturesFoundEvent e)
            {
                foreach (var kvp in e.Signatures)
                {
                    Console.WriteLine($"{kvp.Key} => {kvp.Value.GetAddress():X}");
                }

                Console.WriteLine("To exit this application press \"Enter\".");
                Console.ReadLine();
            };
        }
    }
}
