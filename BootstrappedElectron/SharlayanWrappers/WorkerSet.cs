namespace BootstrappedElectron.SharlayanWrappers {
    using BootstrappedElectron.SharlayanWrappers.Workers;

    using Sharlayan;

    public class WorkerSet {
        private readonly MemoryHandler _memoryHandler;

        internal ActionWorker ActionWorker;

        internal ActorWorker ActorWorker;

        internal ChatLogWorker ChatLogWorker;

        internal CurrentPlayerWorker CurrentPlayerWorker;

        internal InventoryWorker InventoryWorker;

        internal JobResourceWorker JobResourceWorker;

        internal PartyWorker PartyWorker;

        internal TargetWorker TargetWorker;

        public WorkerSet(MemoryHandler memoryHandler) {
            this._memoryHandler = memoryHandler;
        }

        public void StartMemoryWorkers() {
            this.StopMemoryWorkers();

            this.ActionWorker = new ActionWorker(this._memoryHandler);
            this.ActorWorker = new ActorWorker(this._memoryHandler);
            this.ChatLogWorker = new ChatLogWorker(this._memoryHandler);
            this.CurrentPlayerWorker = new CurrentPlayerWorker(this._memoryHandler);
            this.InventoryWorker = new InventoryWorker(this._memoryHandler);
            this.JobResourceWorker = new JobResourceWorker(this._memoryHandler);
            this.PartyWorker = new PartyWorker(this._memoryHandler);
            this.TargetWorker = new TargetWorker(this._memoryHandler);

            this.ActionWorker.StartScanning();
            this.ActorWorker.StartScanning();
            this.ChatLogWorker.StartScanning();
            this.CurrentPlayerWorker.StartScanning();
            this.InventoryWorker.StartScanning();
            this.JobResourceWorker.StartScanning();
            this.PartyWorker.StartScanning();
            this.TargetWorker.StartScanning();
        }

        public void StopMemoryWorkers() {
            if (this.ActionWorker is not null) {
                this.ActionWorker.StopScanning();
                this.ActionWorker.Dispose();
            }

            if (this.ActorWorker is not null) {
                this.ActorWorker.StopScanning();
                this.ActorWorker.Dispose();
            }

            if (this.ChatLogWorker is not null) {
                this.ChatLogWorker.StopScanning();
                this.ChatLogWorker.Dispose();
            }

            if (this.CurrentPlayerWorker is not null) {
                this.CurrentPlayerWorker.StopScanning();
                this.CurrentPlayerWorker.Dispose();
            }

            if (this.InventoryWorker is not null) {
                this.InventoryWorker.StopScanning();
                this.InventoryWorker.Dispose();
            }

            if (this.JobResourceWorker is not null) {
                this.JobResourceWorker.StopScanning();
                this.JobResourceWorker.Dispose();
            }

            if (this.PartyWorker is not null) {
                this.PartyWorker.StopScanning();
                this.PartyWorker.Dispose();
            }

            if (this.TargetWorker is not null) {
                this.TargetWorker.StopScanning();
                this.TargetWorker.Dispose();
            }
        }
    }
}