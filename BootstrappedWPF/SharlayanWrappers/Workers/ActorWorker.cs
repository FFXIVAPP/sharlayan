namespace BootstrappedWPF.SharlayanWrappers.Workers {
    using System;
    using System.Linq;
    using System.Timers;

    using BootstrappedWPF.Properties;

    using Sharlayan;
    using Sharlayan.Models.ReadResults;

    internal class ActorWorker : PropertyChangedBase, IDisposable {
        private readonly MemoryHandler _memoryHandler;

        private readonly Timer _scanTimer;

        private bool _isScanning;

        public ActorWorker(MemoryHandler memoryHandler) {
            this._memoryHandler = memoryHandler;
            this._scanTimer = new Timer(250);
            this._scanTimer.Elapsed += this.ScanTimerElapsed;
        }

        public void Dispose() {
            this._scanTimer.Elapsed -= this.ScanTimerElapsed;
        }

        ~ActorWorker() {
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

            this._scanTimer.Interval = Settings.Default.ActorWorkerTiming;

            this._isScanning = true;

            ActorResult result = this._memoryHandler.Reader.GetActors();

            EventHost.Instance.RaiseNewMonsterActorItemsEvent(this._memoryHandler, result.CurrentMonsters);
            EventHost.Instance.RaiseNewNPCActorItemsEvent(this._memoryHandler, result.CurrentNPCs);
            EventHost.Instance.RaiseNewPCActorItemsEvent(this._memoryHandler, result.CurrentPCs);

            if (result.NewMonsters.Any()) {
                EventHost.Instance.RaiseMonsterActorItemsAddedEvent(this._memoryHandler, result.NewMonsters);
            }

            if (result.NewNPCs.Any()) {
                EventHost.Instance.RaiseNPCActorItemsAddedEvent(this._memoryHandler, result.NewNPCs);
            }

            if (result.NewPCs.Any()) {
                EventHost.Instance.RaisePCActorItemsAddedEvent(this._memoryHandler, result.NewPCs);
            }

            if (result.RemovedMonsters.Any()) {
                EventHost.Instance.RaiseMonsterActorItemsRemovedEvent(this._memoryHandler, result.RemovedMonsters);
            }

            if (result.RemovedNPCs.Any()) {
                EventHost.Instance.RaiseNPCActorItemsRemovedEvent(this._memoryHandler, result.RemovedNPCs);
            }

            if (result.RemovedPCs.Any()) {
                EventHost.Instance.RaisePCActorItemsRemovedEvent(this._memoryHandler, result.RemovedPCs);
            }

            this._isScanning = false;
        }
    }
}