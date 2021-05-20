// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceWorker.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceWorker.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.SharlayanWrappers.Workers {
    using System;
    using System.Threading.Tasks;
    using System.Timers;

    using Sharlayan;
    using Sharlayan.Models.ReadResults;

    using AppContext = BootstrappedWPF.AppContext;

    internal class JobResourceWorker : PropertyChangedBase, IDisposable {
        private readonly MemoryHandler _memoryHandler;

        private readonly Timer _scanTimer;

        private bool _isScanning;

        public JobResourceWorker(MemoryHandler memoryHandler) {
            this._memoryHandler = memoryHandler;
            this._scanTimer = new Timer(250);
            this._scanTimer.Elapsed += this.ScanTimerElapsed;
        }

        ~JobResourceWorker() {
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
                    JobResourceResult result = this._memoryHandler.Reader.GetJobResources();

                    if (AppContext.Instance.ResultSets.TryGetValue(this._memoryHandler.Configuration.ProcessModel.ProcessID, out ResultSet resultSet)) {
                        resultSet.Astrologian = result.Astrologian;
                        resultSet.Bard = result.Bard;
                        resultSet.BlackMage = result.BlackMage;
                        resultSet.Dancer = result.Dancer;
                        resultSet.DarkKnight = result.DarkKnight;
                        resultSet.Dragoon = result.Dragoon;
                        resultSet.GunBreaker = result.GunBreaker;
                        resultSet.Machinist = result.Machinist;
                        resultSet.Monk = result.Monk;
                        resultSet.Ninja = result.Ninja;
                        resultSet.Paladin = result.Paladin;
                        resultSet.RedMage = result.RedMage;
                        resultSet.Samurai = result.Samurai;
                        resultSet.Scholar = result.Scholar;
                        resultSet.Summoner = result.Summoner;
                        resultSet.Warrior = result.Warrior;
                        resultSet.WhiteMage = result.WhiteMage;
                    }

                    this._isScanning = false;
                });
        }
    }
}