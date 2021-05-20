// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionWorker.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ActionWorker.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.SharlayanWrappers.Workers {
    using System;
    using System.Threading.Tasks;
    using System.Timers;

    using Sharlayan;
    using Sharlayan.Models.ReadResults;

    using AppContext = BootstrappedWPF.AppContext;

    internal class ActionWorker : PropertyChangedBase, IDisposable {
        private readonly MemoryHandler _memoryHandler;

        private readonly Timer _scanTimer;

        private bool _isScanning;

        public ActionWorker(MemoryHandler memoryHandler) {
            this._memoryHandler = memoryHandler;
            this._scanTimer = new Timer(250);
            this._scanTimer.Elapsed += this.ScanTimerElapsed;
        }

        ~ActionWorker() {
            this.Dispose();
        }

        public void Dispose() {
            this._scanTimer.Elapsed -= this.ScanTimerElapsed;
        }

        public void StartScanning() {
            this._scanTimer.Enabled = true;
        }

        public void StopScanning() {
            this._scanTimer.Enabled = false;
        }

        private void ScanTimerElapsed(object sender, ElapsedEventArgs e) {
            if (this._isScanning) {
                return;
            }

            this._isScanning = true;

            Task.Run(
                () => {
                    ActionResult result = this._memoryHandler.Reader.GetActions();

                    if (AppContext.Instance.ResultSets.TryGetValue(this._memoryHandler.Configuration.ProcessModel.ProcessID, out ResultSet resultSet)) {
                        resultSet.ActionContainers = result.ActionContainers;
                    }

                    this._isScanning = false;
                });
        }
    }
}