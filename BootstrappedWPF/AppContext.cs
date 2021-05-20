// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContext.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   AppContext.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using BootstrappedWPF.Controls;
    using BootstrappedWPF.Helpers;
    using BootstrappedWPF.Models;
    using BootstrappedWPF.Properties;
    using BootstrappedWPF.SharlayanWrappers;
    using BootstrappedWPF.Utilities;
    using BootstrappedWPF.ViewModels;

    using MaterialDesignColors;

    using MaterialDesignThemes.Wpf;

    using Sharlayan;
    using Sharlayan.Events;
    using Sharlayan.Models;

    public class AppContext {
        private static Lazy<AppContext> _instance = new Lazy<AppContext>(() => new AppContext());

        public readonly ConcurrentDictionary<int, ResultSet> ResultSets = new ConcurrentDictionary<int, ResultSet>();

        private readonly ConcurrentDictionary<int, WorkerSet> _workerSets = new ConcurrentDictionary<int, WorkerSet>();

        private Process[] _gameInstances;

        public static AppContext Instance => _instance.Value;

        public void Initialize() {
            this.SetupDirectories();
            this.ApplyTheme();
            this.LoadChatCodes();
            this.LoadChatColors();
            this.FindGameInstances();
            this.SetupSharlayanManager();
            this.SetupWorkerSets();
            this.SetupResultSets();
            this.StartAllSharlayanWorkers();
        }

        private void ApplyTheme() {
            ThemeUtilities.ModifyTheme(
                theme => theme.SetBaseTheme(
                    Settings.Default.DarkMode
                        ? Theme.Dark
                        : Theme.Light));
            SwatchesProvider swatchesProvider = new SwatchesProvider();
            Swatch primaryColor = swatchesProvider.Swatches.FirstOrDefault(a => string.Equals(a.Name, Settings.Default.UserThemePrimary, StringComparison.OrdinalIgnoreCase));
            if (primaryColor is not null) {
                ThemeUtilities.ModifyTheme(theme => theme.SetPrimaryColor(primaryColor.ExemplarHue.Color));
            }

            Swatch accentColor = swatchesProvider.Swatches.FirstOrDefault(a => string.Equals(a.Name, Settings.Default.UserThemeAccent, StringComparison.OrdinalIgnoreCase));
            if (accentColor is { AccentExemplarHue: not null, }) {
                ThemeUtilities.ModifyTheme(theme => theme.SetSecondaryColor(accentColor.AccentExemplarHue.Color));
            }
        }

        private void FindGameInstances() {
            this._gameInstances = Process.GetProcessesByName("ffxiv_dx11");
        }

        private void LoadChatCodes() {
            foreach (XElement xElement in Constants.Instance.XChatCodes.Descendants().Elements("Code")) {
                string xKey = xElement.Attribute("Key")?.Value;
                string xDescription = xElement.Element("Description")?.Value;
                if (string.IsNullOrWhiteSpace(xKey) || string.IsNullOrWhiteSpace(xDescription)) {
                    continue;
                }

                Constants.Instance.ChatCodes.Add(new ChatCode(xKey, xDescription));
            }
        }

        private void LoadChatColors() {
            foreach (XElement xElement in Constants.Instance.XChatColors.Descendants().Elements("Color")) {
                string xKey = xElement.Attribute("Key")?.Value;
                string xValue = xElement.Element("Value")?.Value;
                string xDescription = xElement.Element("Description")?.Value;
                if (string.IsNullOrWhiteSpace(xKey) || string.IsNullOrWhiteSpace(xValue)) {
                    continue;
                }

                ChatCode existingCode = Constants.Instance.ChatCodes.FirstOrDefault(chat => chat.Code == xKey);
                if (existingCode is not null) {
                    if (xDescription != null && (xDescription.ToLower().Contains("unknown") || string.IsNullOrWhiteSpace(xDescription))) {
                        xDescription = existingCode.Description;
                    }
                }

                Constants.Instance.ChatColors.Add(new ChatColor(xKey, xValue, xDescription));
            }

            foreach (ChatCode chatCode in Constants.Instance.ChatCodes.Where(chat => Constants.Instance.ChatColors.All(color => color.Code != chat.Code))) {
                Constants.Instance.ChatColors.Add(new ChatColor(chatCode.Code, "FFFFFF", chatCode.Description));
            }
        }

        private void MemoryHandler_OnExceptionEvent(object? sender, ExceptionEvent e) {
            if (sender is not MemoryHandler memoryHandler) {
                return;
            }

            // TODO: this should be handled in sharlayan; when we can detect character changes this will be updated/removed and placed in sharlayan
            if (e.Exception.GetType() != typeof(OverflowException)) {
                return;
            }

            if (e.Exception.StackTrace is null || !e.Exception.StackTrace.Contains("ChatLogReader")) {
                return;
            }

            SharlayanConfiguration configuration = memoryHandler.Configuration;

            if (!this._workerSets.TryGetValue(configuration.ProcessModel.ProcessID, out WorkerSet workerSet)) {
                return;
            }

            workerSet.ChatLogWorker.StopScanning();

            Task.Run(
                async () => {
                    await Task.Delay(1000);
                    workerSet.ChatLogWorker.Reset();
                    workerSet.ChatLogWorker.StartScanning();
                });
        }

        private void MemoryHandler_OnMemoryHandlerDisposedEvent(object? sender, MemoryHandlerDisposedEvent e) {
            if (sender is not MemoryHandler memoryHandler) {
                return;
            }

            if (this._workerSets.TryRemove(memoryHandler.Configuration.ProcessModel.ProcessID, out WorkerSet workerSet)) {
                workerSet.StopMemoryWorkers();
            }

            this.ResultSets.TryRemove(memoryHandler.Configuration.ProcessModel.ProcessID, out ResultSet resultSet);
        }

        private void MemoryHandler_OnMemoryLocationsFoundEvent(object? sender, MemoryLocationsFoundEvent e) {
            if (sender is not MemoryHandler memoryHandler) {
                return;
            }

            foreach (KeyValuePair<string, MemoryLocation> kvp in e.MemoryLocations) {
                FlowDocHelper.AppendMessage(memoryHandler, $"Process[{memoryHandler.Configuration.ProcessModel.ProcessID}] -> {kvp.Key} => {kvp.Value.GetAddress():X}", DebugTabItem.TabItem.DebugLogReader._FDR);
            }
        }

        private void SetupDirectories() {
            AppViewModel.Instance.CachePath = Constants.Instance.CachePath;
            AppViewModel.Instance.ConfigurationsPath = Constants.Instance.ConfigurationsPath;
            AppViewModel.Instance.LogsPath = Constants.Instance.LogsPath;
            AppViewModel.Instance.SettingsPath = Constants.Instance.SettingsPath;
        }

        private void SetupResultSets() {
            foreach (MemoryHandler memoryHandler in SharlayanMemoryManager.Instance.GetHandlers()) {
                ResultSet resultSet = new ResultSet();
                this.ResultSets.AddOrUpdate(memoryHandler.Configuration.ProcessModel.ProcessID, resultSet, (k, v) => resultSet);
            }
        }

        private void SetupSharlayanManager() {
            foreach (Process process in this._gameInstances) {
                SharlayanConfiguration sharlayanConfiguration = new SharlayanConfiguration {
                    ProcessModel = new ProcessModel {
                        Process = process,
                    },
                };
                MemoryHandler handler = SharlayanMemoryManager.Instance.AddHandler(sharlayanConfiguration);
                handler.ExceptionEvent += this.MemoryHandler_OnExceptionEvent;
                handler.MemoryHandlerDisposedEvent += this.MemoryHandler_OnMemoryHandlerDisposedEvent;
                handler.MemoryLocationsFoundEvent += this.MemoryHandler_OnMemoryLocationsFoundEvent;
            }
        }

        private void SetupWorkerSets() {
            foreach (MemoryHandler memoryHandler in SharlayanMemoryManager.Instance.GetHandlers()) {
                WorkerSet workerSet = new WorkerSet(memoryHandler);
                this._workerSets.AddOrUpdate(memoryHandler.Configuration.ProcessModel.ProcessID, workerSet, (k, v) => workerSet);
            }
        }

        private void StartAllSharlayanWorkers() {
            this.StopAllSharlayanWorkers();

            foreach (WorkerSet workerSet in this._workerSets.Values.ToList()) {
                workerSet.StartMemoryWorkers();
            }
        }

        private void StopAllSharlayanWorkers() {
            foreach (WorkerSet workerSet in this._workerSets.Values.ToList()) {
                workerSet.StopMemoryWorkers();
            }
        }
    }
}