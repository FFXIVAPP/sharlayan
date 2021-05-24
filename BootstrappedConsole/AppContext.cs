// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContext.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   AppContext.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedConsole {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Sharlayan;
    using Sharlayan.Events;
    using Sharlayan.Models;

    public class AppContext {
        private static Lazy<AppContext> _instance = new Lazy<AppContext>(() => new AppContext());

        private Process[] _gameInstances;

        public static AppContext Instance => _instance.Value;

        public void Initialize() {
            this.FindGameInstances();
            this.SetupSharlayanManager();
        }

        private void FindGameInstances() {
            this._gameInstances = Process.GetProcessesByName("ffxiv_dx11");
        }

        private void SetupSharlayanManager() {
            foreach (Process process in this._gameInstances) {
                SharlayanConfiguration sharlayanConfiguration = new SharlayanConfiguration {
                    ProcessModel = new ProcessModel {
                        Process = process,
                    }
                };
                MemoryHandler handler = SharlayanMemoryManager.Instance.AddHandler(sharlayanConfiguration);
                handler.ExceptionEvent += delegate { };
                handler.MemoryLocationsFoundEvent += delegate(object? sender, MemoryLocationsFoundEvent e) {
                    foreach (KeyValuePair<string, MemoryLocation> kvp in e.MemoryLocations) {
                        Console.WriteLine($"Process[{handler.Configuration.ProcessModel.ProcessID}] -> {kvp.Key} => {kvp.Value.GetAddress():X}");
                    }
                };
            }
        }
    }
}