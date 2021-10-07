// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Program.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Bootstrapper {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
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
            var stringReader = new StringReader(XElement.Load("./Bootstrapper.exe.nlog").ToString());

            using (XmlReader xmlReader = XmlReader.Create(stringReader)) {
                LogManager.Configuration = new XmlLoggingConfiguration(xmlReader, null);
            }

            ActionLookup.GetActionInfo(2);
            StatusEffectLookup.GetStatusInfo(2);
            ZoneLookup.GetZoneInfo(138);

            ActionItem action = ActionLookup.GetActionInfo(2);
            StatusItem status = StatusEffectLookup.GetStatusInfo(2);
            MapItem zone = ZoneLookup.GetZoneInfo(138);

            var processName = "ffxiv_dx11";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                processName = "ffxiv_dx11.exe";
            }

            Process process = Process.GetProcessesByName(processName).FirstOrDefault();

            if (process == null && RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                var ps = new Process();
                ps.StartInfo = new ProcessStartInfo("ps", "-Ao pid,command");
                ps.StartInfo.RedirectStandardOutput = true;
                ps.Start();
                var content = ps.StandardOutput.ReadToEnd();
                ps.WaitForExit(2000);

                var pid = content.Split(Environment.NewLine).Where(x => x.Contains(processName)).Select(x => Enumerable.FirstOrDefault(x.Split(' '))).FirstOrDefault(x => x != null);
                if (!string.IsNullOrEmpty(pid))
                    process = Process.GetProcessById(int.Parse(pid));
            }

            if (process != null) {
                MemoryHandler.Instance.SetProcess(
                    new ProcessModel {
                        IsWin64 = true,
                        Process = process,
                    });

                MemoryHandler.Instance.SignaturesFoundEvent += delegate(object sender, SignaturesFoundEvent e) {
                    foreach (KeyValuePair<string, Signature> kvp in e.Signatures) {
                        Console.WriteLine($"{kvp.Key} => {kvp.Value.GetAddress():X}");
                    }
                };

                while (Scanner.Instance.IsScanning) {
                    Thread.Sleep(1000);
                    Console.WriteLine("Scanning...");
                }
            }

            Console.WriteLine("To exit this application press \"Enter\".");
            Console.ReadLine();
        }
    }
}