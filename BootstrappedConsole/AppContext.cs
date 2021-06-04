namespace BootstrappedConsole {
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;

    using Sharlayan;
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
                    },
                };
                MemoryHandler handler = SharlayanMemoryManager.Instance.AddHandler(sharlayanConfiguration);
                handler.OnException += delegate { };
                handler.OnMemoryLocationsFound += delegate(object sender, ConcurrentDictionary<string, MemoryLocation> memoryLocations, long processingTime) {
                    foreach ((string key, MemoryLocation memoryLocation) in memoryLocations) {
                        Console.WriteLine($"Process[{handler.Configuration.ProcessModel.ProcessID}] -> MemoryLocation Found -> {key} => {memoryLocation.GetAddress():X}");
                    }
                };
            }
        }
    }
}