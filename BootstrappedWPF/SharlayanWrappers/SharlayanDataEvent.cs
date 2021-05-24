namespace BootstrappedWPF.SharlayanWrappers {
    using System;

    using Sharlayan;

    public class SharlayanDataEvent<T> : EventArgs {
        public SharlayanDataEvent(object sender, MemoryHandler memoryHandler, T eventData) {
            this.Sender = sender;
            this.MemoryHandler = memoryHandler;
            this.EventData = eventData;
        }

        public T EventData { get; }

        public MemoryHandler MemoryHandler { get; }

        public object Sender { get; }
    }
}