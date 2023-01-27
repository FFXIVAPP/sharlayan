// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using Sharlayan.Delegates;
    using Sharlayan.Utilities;

    public partial class Reader {
        private ActorItemResolver _actorItemResolver;

        private ChatLogReader _chatLogReader;

        private ChatLogWorkerDelegate _chatLogWorkerDelegate = new ChatLogWorkerDelegate();

        private JobResourceResolver _jobResourceResolver;

        private MonsterWorkerDelegate _monsterWorkerDelegate = new MonsterWorkerDelegate();

        private NPCWorkerDelegate _npcWorkerDelegate = new NPCWorkerDelegate();

        private PartyWorkerDelegate _partyWorkerDelegate = new PartyWorkerDelegate();

        private PCWorkerDelegate _pcWorkerDelegate = new PCWorkerDelegate();

        public Reader(MemoryHandler memoryHandler) {
            this._memoryHandler = memoryHandler;

            this._chatLogReader = new ChatLogReader(this._memoryHandler);

            this._chatLogWorkerDelegate = new ChatLogWorkerDelegate();
            this._monsterWorkerDelegate = new MonsterWorkerDelegate();
            this._npcWorkerDelegate = new NPCWorkerDelegate();
            this._partyWorkerDelegate = new PartyWorkerDelegate();
            this._pcWorkerDelegate = new PCWorkerDelegate();

            this._actorItemResolver = new ActorItemResolver(this._memoryHandler, this._pcWorkerDelegate, this._npcWorkerDelegate, this._monsterWorkerDelegate);
            this._playerInfoResolver = new PlayerInfoResolver(this._memoryHandler);
            this._partyMemberResolver = new PartyMemberResolver(this._memoryHandler, this._pcWorkerDelegate, this._npcWorkerDelegate, this._monsterWorkerDelegate);
            this._jobResourceResolver = new JobResourceResolver(this._memoryHandler);
        }

        private MemoryHandler _memoryHandler { get; }

        private PartyMemberResolver _partyMemberResolver { get; }

        private PlayerInfoResolver _playerInfoResolver { get; }
    }
}