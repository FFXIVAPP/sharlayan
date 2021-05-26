namespace BootstrappedElectron.SharlayanWrappers {
    using System;
    using System.Collections.Concurrent;
    using System.Linq;

    using ElectronNET.API;

    using Sharlayan;
    using Sharlayan.Core;

    public class EventHost {
        public delegate void SharlayanEventHandler<T>(object sender, MemoryHandler memoryHandler, T eventData);

        private static Lazy<EventHost> _instance = new Lazy<EventHost>(() => new EventHost());

        public static EventHost Instance => _instance.Value;

        public event SharlayanEventHandler<ConcurrentDictionary<uint, ActorItem>> OnMonsterActorItemsAdded = delegate { };

        public event SharlayanEventHandler<ConcurrentDictionary<uint, ActorItem>> OnMonsterActorItemsRemoved = delegate { };

        public event SharlayanEventHandler<ConcurrentBag<ActionContainer>> OnNewActionContainers = delegate { };

        public event SharlayanEventHandler<ChatLogItem> OnNewChatLogItem = delegate { };

        public event SharlayanEventHandler<ActorItem> OnNewCurrentUser = delegate { };

        public event SharlayanEventHandler<ConcurrentBag<InventoryContainer>> OnNewInventoryContainers = delegate { };

        public event SharlayanEventHandler<JobResourcesContainer> OnNewJobResourcesContainer = delegate { };

        public event SharlayanEventHandler<ConcurrentDictionary<uint, ActorItem>> OnNewMonsterActorItems = delegate { };

        public event SharlayanEventHandler<ConcurrentDictionary<uint, ActorItem>> OnNewNPCActorItems = delegate { };

        public event SharlayanEventHandler<ConcurrentDictionary<uint, PartyMember>> OnNewPartyMembers = delegate { };

        public event SharlayanEventHandler<ConcurrentDictionary<uint, ActorItem>> OnNewPCActorItems = delegate { };

        public event SharlayanEventHandler<PlayerInfo> OnNewPlayerInfo = delegate { };

        public event SharlayanEventHandler<TargetInfo> OnNewTargetInfo = delegate { };

        public event SharlayanEventHandler<ConcurrentDictionary<uint, ActorItem>> OnNPCActorItemsAdded = delegate { };

        public event SharlayanEventHandler<ConcurrentDictionary<uint, ActorItem>> OnNPCActorItemsRemoved = delegate { };

        public event SharlayanEventHandler<ConcurrentDictionary<uint, PartyMember>> OnPartyMembersAdded = delegate { };

        public event SharlayanEventHandler<ConcurrentDictionary<uint, PartyMember>> OnPartyMembersRemoved = delegate { };

        public event SharlayanEventHandler<ConcurrentDictionary<uint, ActorItem>> OnPCActorItemsAdded = delegate { };

        public event SharlayanEventHandler<ConcurrentDictionary<uint, ActorItem>> OnPCActorItemsRemoved = delegate { };

        public virtual void RaiseMonsterActorItemsAddedEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            this.OnMonsterActorItemsAdded?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnMonsterActorItemsAdded", eventData);
        }

        public virtual void RaiseMonsterActorItemsRemovedEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            this.OnMonsterActorItemsRemoved?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnMonsterActorItemsRemoved", eventData);
        }

        public virtual void RaiseNewActionContainersEvent(MemoryHandler memoryHandler, ConcurrentBag<ActionContainer> eventData) {
            this.OnNewActionContainers?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnNewActionContainers", eventData);
        }

        public virtual void RaiseNewChatLogItemEvent(MemoryHandler memoryHandler, ChatLogItem eventData) {
            this.OnNewChatLogItem?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnNewChatLogItem", eventData);
        }

        public virtual void RaiseNewCurrentUserEvent(MemoryHandler memoryHandler, ActorItem eventData) {
            this.OnNewCurrentUser?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnNewCurrentUser", eventData);
        }

        public virtual void RaiseNewInventoryContainersEvent(MemoryHandler memoryHandler, ConcurrentBag<InventoryContainer> eventData) {
            this.OnNewInventoryContainers?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnNewInventoryContainers", eventData);
        }

        public virtual void RaiseNewJobResourcesContainerEvent(MemoryHandler memoryHandler, JobResourcesContainer eventData) {
            this.OnNewJobResourcesContainer?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnNewJobResourcesContainer", eventData);
        }

        public virtual void RaiseNewMonsterActorItemsEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            this.OnNewMonsterActorItems?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnNewMonsterActorItems", eventData);
        }

        public virtual void RaiseNewNPCActorItemsEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            this.OnNewNPCActorItems?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnNewNPCActorItems", eventData);
        }

        public virtual void RaiseNewPartyMembersEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, PartyMember> eventData) {
            this.OnNewPartyMembers?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnNewPartyMembers", eventData);
        }

        public virtual void RaiseNewPCActorItemsEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            this.OnNewPCActorItems?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnNewPCActorItems", eventData);
        }

        public virtual void RaiseNewPlayerInfoEvent(MemoryHandler memoryHandler, PlayerInfo eventData) {
            this.OnNewPlayerInfo?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnNewPlayerInfo", eventData);
        }

        public virtual void RaiseNewTargetInfoEvent(MemoryHandler memoryHandler, TargetInfo eventData) {
            this.OnNewTargetInfo?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnNewTargetInfo", eventData);
        }

        public virtual void RaiseNPCActorItemsAddedEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            this.OnNPCActorItemsAdded?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnNPCActorItemsAdded", eventData);
        }

        public virtual void RaiseNPCActorItemsRemovedEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            this.OnNPCActorItemsRemoved?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnNPCActorItemsRemoved", eventData);
        }

        public virtual void RaisePartyMembersAddedEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, PartyMember> eventData) {
            this.OnPartyMembersAdded?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnPartyMembersAdded", eventData);
        }

        public virtual void RaisePartyMembersRemovedEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, PartyMember> eventData) {
            this.OnPartyMembersRemoved?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnPartyMembersRemoved", eventData);
        }

        public virtual void RaisePCActorItemsAddedEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            this.OnPCActorItemsAdded?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnPCActorItemsAdded", eventData);
        }

        public virtual void RaisePCActorItemsRemovedEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            this.OnPCActorItemsRemoved?.Invoke(this, memoryHandler, eventData);

            RaiseElectronEvent("OnPCActorItemsRemoved", eventData);
        }

        private static void RaiseElectronEvent<T>(string channel, T value) {
            BrowserWindow? window = Electron.WindowManager.BrowserWindows.FirstOrDefault();
            if (window is not null) {
                Electron.IpcMain.Send(window, channel, value);
            }
        }
    }
}