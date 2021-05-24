namespace BootstrappedWPF.SharlayanWrappers {
    using System;
    using System.Collections.Concurrent;

    using BootstrappedWPF.SharlayanWrappers.Events;

    using Sharlayan;
    using Sharlayan.Core;

    public class EventHost {
        private static Lazy<EventHost> _instance = new Lazy<EventHost>(() => new EventHost());

        public event EventHandler<ActorItemsAddedEvent> OnMonsterActorItemsAdded = delegate { };

        public event EventHandler<ActorItemsRemovedEvent> OnMonsterActorItemsRemoved = delegate { };

        public event EventHandler<NewActionContainersEvent> OnNewActionContainers = delegate { };

        public event EventHandler<NewChatLogItemEvent> OnNewChatLogItem = delegate { };

        public event EventHandler<NewCurrentUserEvent> OnNewCurrentUser = delegate { };

        public event EventHandler<NewInventoryContainersEvent> OnNewInventoryContainers = delegate { };

        public event EventHandler<NewJobResourcesContainerEvent> OnNewJobResourcesContainer = delegate { };

        public event EventHandler<NewActorItemsEvent> OnNewMonsterActorItems = delegate { };

        public event EventHandler<NewActorItemsEvent> OnNewNPCActorItems = delegate { };

        public event EventHandler<NewPartyMembersEvent> OnNewPartyMembers = delegate { };

        public event EventHandler<NewActorItemsEvent> OnNewPCActorItems = delegate { };

        public event EventHandler<NewPlayerInfoEvent> OnNewPlayerInfo = delegate { };

        public event EventHandler<NewTargetInfoEvent> OnNewTargetInfo = delegate { };

        public event EventHandler<ActorItemsAddedEvent> OnNPCActorItemsAdded = delegate { };

        public event EventHandler<ActorItemsRemovedEvent> OnNPCActorItemsRemoved = delegate { };

        public event EventHandler<PartyMembersAddedEvent> OnPartyMembersAdded = delegate { };

        public event EventHandler<PartyMembersRemovedEvent> OnPartyMembersRemoved = delegate { };

        public event EventHandler<ActorItemsAddedEvent> OnPCActorItemsAdded = delegate { };

        public event EventHandler<ActorItemsRemovedEvent> OnPCActorItemsRemoved = delegate { };

        public static EventHost Instance => _instance.Value;

        public virtual void RaiseMonsterActorItemsAddedEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            ActorItemsAddedEvent newEvent = new ActorItemsAddedEvent(this, memoryHandler, eventData);
            EventHandler<ActorItemsAddedEvent> handler = this.OnMonsterActorItemsAdded;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaiseMonsterActorItemsRemovedEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            ActorItemsRemovedEvent newEvent = new ActorItemsRemovedEvent(this, memoryHandler, eventData);
            EventHandler<ActorItemsRemovedEvent> handler = this.OnMonsterActorItemsRemoved;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaiseNewActionContainersEvent(MemoryHandler memoryHandler, ConcurrentBag<ActionContainer> eventData) {
            NewActionContainersEvent newEvent = new NewActionContainersEvent(this, memoryHandler, eventData);
            EventHandler<NewActionContainersEvent> handler = this.OnNewActionContainers;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaiseNewChatLogItemEvent(MemoryHandler memoryHandler, ChatLogItem eventData) {
            NewChatLogItemEvent newEvent = new NewChatLogItemEvent(this, memoryHandler, eventData);
            EventHandler<NewChatLogItemEvent> handler = this.OnNewChatLogItem;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaiseNewCurrentUserEvent(MemoryHandler memoryHandler, ActorItem eventData) {
            NewCurrentUserEvent newEvent = new NewCurrentUserEvent(this, memoryHandler, eventData);
            EventHandler<NewCurrentUserEvent> handler = this.OnNewCurrentUser;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaiseNewInventoryContainersEvent(MemoryHandler memoryHandler, ConcurrentBag<InventoryContainer> eventData) {
            NewInventoryContainersEvent newEvent = new NewInventoryContainersEvent(this, memoryHandler, eventData);
            EventHandler<NewInventoryContainersEvent> handler = this.OnNewInventoryContainers;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaiseNewJobResourcesContainerEvent(MemoryHandler memoryHandler, JobResourcesContainer eventData) {
            NewJobResourcesContainerEvent newEvent = new NewJobResourcesContainerEvent(this, memoryHandler, eventData);
            EventHandler<NewJobResourcesContainerEvent> handler = this.OnNewJobResourcesContainer;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaiseNewMonsterActorItemsEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            NewActorItemsEvent newEvent = new NewActorItemsEvent(this, memoryHandler, eventData);
            EventHandler<NewActorItemsEvent> handler = this.OnNewMonsterActorItems;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaiseNewNPCActorItemsEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            NewActorItemsEvent newEvent = new NewActorItemsEvent(this, memoryHandler, eventData);
            EventHandler<NewActorItemsEvent> handler = this.OnNewNPCActorItems;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaiseNewPartyMembersEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, PartyMember> eventData) {
            NewPartyMembersEvent newEvent = new NewPartyMembersEvent(this, memoryHandler, eventData);
            EventHandler<NewPartyMembersEvent> handler = this.OnNewPartyMembers;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaiseNewPCActorItemsEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            NewActorItemsEvent newEvent = new NewActorItemsEvent(this, memoryHandler, eventData);
            EventHandler<NewActorItemsEvent> handler = this.OnNewPCActorItems;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaiseNewPlayerInfoEvent(MemoryHandler memoryHandler, PlayerInfo eventData) {
            NewPlayerInfoEvent newEvent = new NewPlayerInfoEvent(this, memoryHandler, eventData);
            EventHandler<NewPlayerInfoEvent> handler = this.OnNewPlayerInfo;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaiseNewTargetInfoEvent(MemoryHandler memoryHandler, TargetInfo eventData) {
            NewTargetInfoEvent newEvent = new NewTargetInfoEvent(this, memoryHandler, eventData);
            EventHandler<NewTargetInfoEvent> handler = this.OnNewTargetInfo;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaiseNPCActorItemsAddedEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            ActorItemsAddedEvent newEvent = new ActorItemsAddedEvent(this, memoryHandler, eventData);
            EventHandler<ActorItemsAddedEvent> handler = this.OnNPCActorItemsAdded;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaiseNPCActorItemsRemovedEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            ActorItemsRemovedEvent newEvent = new ActorItemsRemovedEvent(this, memoryHandler, eventData);
            EventHandler<ActorItemsRemovedEvent> handler = this.OnNPCActorItemsRemoved;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaisePartyMembersAddedEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, PartyMember> eventData) {
            PartyMembersAddedEvent newEvent = new PartyMembersAddedEvent(this, memoryHandler, eventData);
            EventHandler<PartyMembersAddedEvent> handler = this.OnPartyMembersAdded;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaisePartyMembersRemovedEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, PartyMember> eventData) {
            PartyMembersRemovedEvent newEvent = new PartyMembersRemovedEvent(this, memoryHandler, eventData);
            EventHandler<PartyMembersRemovedEvent> handler = this.OnPartyMembersRemoved;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaisePCActorItemsAddedEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            ActorItemsAddedEvent newEvent = new ActorItemsAddedEvent(this, memoryHandler, eventData);
            EventHandler<ActorItemsAddedEvent> handler = this.OnPCActorItemsAdded;
            handler?.Invoke(this, newEvent);
        }

        public virtual void RaisePCActorItemsRemovedEvent(MemoryHandler memoryHandler, ConcurrentDictionary<uint, ActorItem> eventData) {
            ActorItemsRemovedEvent newEvent = new ActorItemsRemovedEvent(this, memoryHandler, eventData);
            EventHandler<ActorItemsRemovedEvent> handler = this.OnPCActorItemsRemoved;
            handler?.Invoke(this, newEvent);
        }
    }
}