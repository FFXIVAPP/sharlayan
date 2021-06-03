namespace BootstrappedElectron.SharlayanWrappers.Workers {
    using System;
    using System.Timers;

    using Sharlayan;
    using Sharlayan.Models.ReadResults;

    internal class CurrentPlayerWorker : IDisposable {
        private readonly MemoryHandler _memoryHandler;

        private readonly Timer _scanTimer;

        private bool _isScanning;

        public CurrentPlayerWorker(MemoryHandler memoryHandler) {
            this._memoryHandler = memoryHandler;
            this._scanTimer = new Timer(250);
            this._scanTimer.Elapsed += this.ScanTimerElapsed;
        }

        public void Dispose() {
            this._scanTimer.Elapsed -= this.ScanTimerElapsed;
        }

        ~CurrentPlayerWorker() {
            this.Dispose();
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

            // this._scanTimer.Interval = Settings.Default.CurrentPlayerWorkerTiming;

            this._isScanning = true;

            CurrentPlayerResult result = this._memoryHandler.Reader.GetCurrentPlayer();

            EventHost.Instance.RaiseNewCurrentUserEvent(this._memoryHandler, result.Entity);
            EventHost.Instance.RaiseNewPlayerInfoEvent(this._memoryHandler, result.PlayerInfo);

            this._isScanning = false;
        }
    }
}