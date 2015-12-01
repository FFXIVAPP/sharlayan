# ffxivapp-memory
Issue tracking, feature request and release repository.

# What is this?
This is the main memory module for FFXIVAPP split out into it's own repo. For enterprising people this means not having to wait for a full app update as this "should" be a drop in replacement for your existing one in your FFXIVAPP folder.

Pending anything catastrophic update-wise it should be good to go.

# How do I use it and what comes back?
- Add as a reference into your project.

That's the basic of it. For actual instantiation it works as follows:

```csharp
using FFXIVAPP.Memory;
using FFXIVAPP.Memory.Models;

// DX9
Process[] processes = Process.GetProcessesByName("ffxiv");
if (processes.length)
{
    // supported: English, Chinese, Japanese, French, German
    string gameLanguage = "English";
    Process process = processes[0];
    ProcessModel processModel = new ProcessModel
    {
        Process = process
    }
    MemoryHandler.Instance.SetProcess(processModel, gameLanguage);
}

// DX11
Process[] processes = Process.GetProcessesByName("ffxiv_dx11");
if (processes.length)
{
    // supported: English, Chinese, Japanese, French, German
    string gameLanguage = "English";
    Process process = processes[0];
    ProcessModel processModel = new ProcessModel
    {
        Process = process,
        IsWin64 = true
    }
    MemoryHandler.Instance.SetProcess(processModel, gameLanguage);
}
```

The memory module is now instantiated and is ready to read data. If you are switching to a new process remember to call SetProcess again.

# Reading data
The following functions are available:

## Actors (Monster, Player, NPC, etc) Reading

```csharp
using FFXIVAPP.Memory;

ActorReadResult readResult = Reader.GetActors();

// Previous is list of ID's that were in the last scan
// New is all the new ID's added
// Also returned is the Current list of actors.

// The result is the following class
public class ActorReadResult
{
    public ActorReadResult()
    {
        PreviousMonster = new Dictionary<uint, uint>();
        PreviousNPC = new Dictionary<uint, uint>();
        PreviousPC = new Dictionary<uint, uint>();

        NewMonster = new List<uint>();
        NewNPC = new List<uint>();
        NewPC = new List<uint>();
    }

    public ConcurrentDictionary<uint, ActorEntity> MonsterEntities => MonsterWorkerDelegate.EntitiesDictionary;
    public ConcurrentDictionary<uint, ActorEntity> NPCEntities => NPCWorkerDelegate.EntitiesDictionary;
    public ConcurrentDictionary<uint, ActorEntity> PCEntities => PCWorkerDelegate.EntitiesDictionary;
    public Dictionary<uint, uint> PreviousMonster { get; set; }
    public Dictionary<uint, uint> PreviousNPC { get; set; }
    public Dictionary<uint, uint> PreviousPC { get; set; }
    public List<UInt32> NewMonster { get; set; }
    public List<UInt32> NewNPC { get; set; }
    public List<UInt32> NewPC { get; set; }
}
```

## ChatLog Reading

```csharp
using FFXIVAPP.Memory;

// For chatlog you must locally store previous array offsets and indexes in order to pull the correct log from the last time you read it.
int previousArrayOffset = 0;
int previousIndex = 0;

ChatLogReadResult readResult = Reader.GetChatLog(previousArrayOffset, previousIndex);

List<ChatLogEntry> chatLogEntries = readResult.ChatLogEntries;

previousArrayOffset = readResult.PreviousArrayOffset;
previousIndex = readResult.PreviousIndex;

// The result is the following class
public class ChatLogReadResult
{
    public ChatLogReadResult()
    {
        ChatLogEntries = new List<ChatLogEntry>();
    }

    public List<ChatLogEntry> ChatLogEntries { get; set; }
    public int PreviousArrayIndex { get; set; }
    public int PreviousOffset { get; set; }
}
```

## Inventory Reading

```csharp
using FFXIVAPP.Memory;

InventoryReadResult readResult = Reader.GetInventoryItems();

// The result is the following class
public class InventoryReadResult
{
    public InventoryReadResult()
    {
        InventoryEntities = new List<InventoryEntity>();
    }

    public List<InventoryEntity> InventoryEntities { get; set; }
}
```

## Party Reading

```csharp
using FFXIVAPP.Memory;

PartyInfoReadResult readResult = Reader.GetPartyMembers();

// Previous is list of ID's that were in the last scan
// New is all the new ID's added
// Also returned is the Current list of actors.

// The result is the following class
public class PartyInfoReadResult
{
    public PartyInfoReadResult()
    {
        PreviousParty = new Dictionary<uint, uint>();

        NewParty = new List<uint>();
    }

    public ConcurrentDictionary<uint, PartyEntity> PartyEntities => PartyInfoWorkerDelegate.EntitiesDictionary;
    public Dictionary<uint, uint> PreviousParty { get; set; }
    public List<UInt32> NewParty { get; set; }
}
```

## Player Info Reading

```csharp
using FFXIVAPP.Memory;

PlayerInfoReadResult readResult = Reader.GetPlayerInfo();

// The result is the following class
public class PlayerInfoReadResult
{
    public PlayerInfoReadResult()
    {
        PlayerEntity = new PlayerEntity();
    }

    public PlayerEntity PlayerEntity { get; set; }
}
```

## Target Reading

```csharp
using FFXIVAPP.Memory;

TargetReadResult readResult = Reader.GetTargetInfo();

// TargetsFound means at least 1 was found

// The result is the following class
public class TargetReadResult
{
    public TargetReadResult()
    {
        TargetEntity = new TargetEntity();
    }

    public TargetEntity TargetEntity { get; set; }
    public bool TargetsFound { get; set; }
}
```
