// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuresContainer.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   StructuresContainer.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public class StructuresContainer {
        public ActorItem ActorItem { get; set; } = new ActorItem();

        public ChatLogPointers ChatLogPointers { get; set; } = new ChatLogPointers();

        public EnmityItem EnmityItem { get; set; } = new EnmityItem();

        public HotBarItem HotBarItem { get; set; } = new HotBarItem();

        public InventoryContainer InventoryContainer { get; set; } = new InventoryContainer();

        public InventoryItem InventoryItem { get; set; } = new InventoryItem();

        public JobResources JobResources { get; set; } = new JobResources();

        public PartyMember PartyMember { get; set; } = new PartyMember();

        public PlayerInfo PlayerInfo { get; set; } = new PlayerInfo();

        public RecastItem RecastItem { get; set; } = new RecastItem();

        public StatusItem StatusItem { get; set; } = new StatusItem();

        public TargetInfo TargetInfo { get; set; } = new TargetInfo();
    }
}