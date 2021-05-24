namespace BootstrappedWPF.SharlayanWrappers.Events {
    using Sharlayan;
    using Sharlayan.Core;

    public class NewJobResourcesContainerEvent : SharlayanDataEvent<JobResourcesContainer> {
        public NewJobResourcesContainerEvent(object sender, MemoryHandler memoryHandler, JobResourcesContainer eventData) : base(sender, memoryHandler, eventData) { }
    }
}