// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Program.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Bootstrapped {
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Xml;
    using System.Xml.Linq;

    using NLog;
    using NLog.Config;

    using Sharlayan;
    using Sharlayan.Events;
    using Sharlayan.Models;
    using Sharlayan.Models.XIVDatabase;
    using Sharlayan.Utilities;

    class Program {
        static void Main(string[] args) {
            var stringReader = new StringReader(XElement.Load("./Bootstrapped.exe.nlog").ToString());

            using (XmlReader xmlReader = XmlReader.Create(stringReader)) {
                LogManager.Configuration = new XmlLoggingConfiguration(xmlReader, null);
            }

            ActionLookup.GetActionInfo(2);
            StatusEffectLookup.GetStatusInfo(2);
            ZoneLookup.GetZoneInfo(138);

            ActionItem action = ActionLookup.GetActionInfo(2);
            StatusItem status = StatusEffectLookup.GetStatusInfo(2);
            MapItem zone = ZoneLookup.GetZoneInfo(138);

            Process process = Process.GetProcessesByName("ffxiv_dx11").FirstOrDefault();

            if (process != null) {
                MemoryHandler.Instance.SetProcess(
                    new ProcessModel {
                        Process = process,
                    });

                while (Scanner.Instance.IsScanning) {
                    Thread.Sleep(1000);
                    Console.WriteLine("Scanning...");
                }

                MemoryHandler.Instance.SignaturesFoundEvent += delegate(object sender, SignaturesFoundEvent e) {
                    foreach (KeyValuePair<string, Signature> kvp in e.Signatures) {
                        Console.WriteLine($"{kvp.Key} => {kvp.Value.GetAddress():X}");
                    }
                };
            }

            Console.WriteLine("To exit this application press \"Enter\".");
            Console.ReadLine();
        }
    }
}