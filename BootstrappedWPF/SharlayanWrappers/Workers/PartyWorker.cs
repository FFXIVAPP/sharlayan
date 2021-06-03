namespace BootstrappedWPF.SharlayanWrappers.Workers {
    using System;
    using System.Linq;
    using System.Timers;

    using BootstrappedWPF.Properties;

    using Sharlayan;
    using Sharlayan.Models.ReadResults;

    internal class PartyWorker : PropertyChangedBase, IDisposable {
        private readonly MemoryHandler _memoryHandler;

        private readonly Timer _scanTimer;

        private bool _isScanning;

        public PartyWorker(MemoryHandler memoryHandler) {
            this._memoryHandler = memoryHandler;
            this._scanTimer = new Timer(250);
            this._scanTimer.Elapsed += this.ScanTimerElapsed;
        }

        private bool _partyReferencesSet { get; set; }

        public void Dispose() {
            this._scanTimer.Elapsed -= this.ScanTimerElapsed;
        }

        ~PartyWorker() {
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

            this._scanTimer.Interval = Settings.Default.PartyWorkerTiming;

            this._isScanning = true;

            PartyResult result = this._memoryHandler.Reader.GetPartyMembers();

            if (!this._partyReferencesSet) {
                this._partyReferencesSet = true;
                EventHost.Instance.RaiseNewPartyMembersEvent(this._memoryHandler, result.PartyMembers);
            }

            if (result.NewPartyMembers.Any()) {
                EventHost.Instance.RaisePartyMembersAddedEvent(this._memoryHandler, result.NewPartyMembers);
            }

            if (result.RemovedPartyMembers.Any()) {
                EventHost.Instance.RaisePartyMembersRemovedEvent(this._memoryHandler, result.RemovedPartyMembers);
            }

            this._isScanning = false;
        }
    }
}