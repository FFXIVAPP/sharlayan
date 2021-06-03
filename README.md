# sharlayan

Issue tracking, feature request and release repository.

# What is this?

This is the main memory module for FFXIVAPP split out into it's own repo. For enterprising people this means not having to wait for a full app update as this "should" be a drop in replacement for your existing one in your FFXIVAPP folder.

Pending anything catastrophic update-wise it should be good to go.

# How do I use it and what comes back?

- Add as a reference into your project.

That's the basic of it. For actual instantiation it works as follows:

```csharp
using Sharlayan;
using Sharlayan.Enums;
using Sharlayan.Models;

// DX11
Process[] processes = Process.GetProcessesByName("ffxiv_dx11");
if (processes.length)
{
    // supported: Global, Chinese, Korean
    GameRegion gameRegion = GameRegion.Global;
    GameLanguage gameLanguage = GameLanguage.English;
	// whether to always hit API on start to get the latest sigs based on patchVersion, or use the local json cache (if the file doesn't exist, API will be hit)
	bool useLocalCache = true;
	// patchVersion of game, or latest
	string patchVersion = "latest";
    Process process = processes[0];
    ProcessModel processModel = new ProcessModel {
        Process = process
    }
    SharlayanConfiguration configuration = new SharlayanConfiguration {
        ProcessModel = processModel,
        GameLanguage = gameLanguage,
        GameRegion = gameRegion,
        PatchVersion = patchVersion,
        UseLocalCache = Settings.Default.UseLocalMemoryJSONDataCache
    };
    MemoryHandler memoryHandler = SharlayanMemoryManager.Instance.AddHandler(configuration);
}
```

The memory module is now instantiated and is ready to read data. When switch processes you should call:

```csharp
SharlayanMemoryManager.Instance.RemoveHandler(processModel);
```

# Reading

The instantiated memory handler for the process comes with it's own reader, or you can create your own.

### Using Predefined

```csharp
MemoryHandler memoryHandler = SharlayanMemoryManager.Instance.GetHandler(processModel);
XResult result = memoryHandler.Reader.GetX();
```

## Actors (Monster, Player, NPC, etc) Reading

```csharp
using Sharlayan;

ActorReadResult readResult = memoryHandler.Reader.GetActors();

// Removed is list of ID's that were in the last scan
// New is all the new ID's added
// Also returned is the Current list of actors.

// The result is the following class
public class ActorReadResult
{
    public ActorReadResult()
    {
        RemovedMonster = new Dictionary<uint, uint>();
        RemovedNPC = new Dictionary<uint, uint>();
        RemovedPC = new Dictionary<uint, uint>();

        NewMonster = new List<uint>();
        NewNPC = new List<uint>();
        NewPC = new List<uint>();
    }

    public ConcurrentDictionary<uint, ActorItem> MonsterEntities => MonsterWorkerDelegate.EntitiesDictionary;
    public ConcurrentDictionary<uint, ActorItem> NPCEntities => NPCWorkerDelegate.EntitiesDictionary;
    public ConcurrentDictionary<uint, ActorItem> PCEntities => PCWorkerDelegate.EntitiesDictionary;
    public Dictionary<uint, uint> RemovedMonster { get; set; }
    public Dictionary<uint, uint> RemovedNPC { get; set; }
    public Dictionary<uint, uint> RemovedPC { get; set; }
    public List<UInt32> NewMonster { get; set; }
    public List<UInt32> NewNPC { get; set; }
    public List<UInt32> NewPC { get; set; }
}
```

## ChatLog Reading

```csharp
using Sharlayan;

// For chatlog you must locally store previous array offsets and indexes in order to pull the correct log from the last time you read it.
int _previousArrayIndex = 0;
int _previousOffset = 0;

ChatLogReadResult readResult = memoryHandler.Reader.GetChatLog(_previousArrayIndex, _previousOffset);

List<ChatLogEntry> chatLogEntries = readResult.ChatLogEntries;

_previousArrayIndex = readResult.PreviousArrayIndex;
_previousOffset = readResult.PreviousOffset;

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
using Sharlayan;

InventoryReadResult readResult = memoryHandler.Reader.GetInventoryItems();

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
using Sharlayan;

PartyInfoReadResult readResult = memoryHandler.Reader.GetPartyMembers();

// Removed is list of ID's that were in the last scan
// New is all the new ID's added
// Also returned is the Current list of actors.

// The result is the following class
public class PartyInfoReadResult
{
    public PartyInfoReadResult()
    {
        RemovedParty = new Dictionary<uint, uint>();

        NewParty = new List<uint>();
    }

    public ConcurrentDictionary<uint, PartyMember> PartyEntities => PartyInfoWorkerDelegate.EntitiesDictionary;
    public Dictionary<uint, uint> RemovedParty { get; set; }
    public List<UInt32> NewParty { get; set; }
}
```

## Player Info Reading

```csharp
using Sharlayan;

PlayerInfoReadResult readResult = memoryHandler.Reader.GetPlayerInfo();

// The result is the following class
public class PlayerInfoReadResult
{
    public PlayerInfoReadResult()
    {
        CurrentPlayer = new CurrentPlayer();
    }

    public CurrentPlayer CurrentPlayer { get; set; }
}
```

## Target Reading

```csharp
using Sharlayan;

TargetReadResult readResult = memoryHandler.Reader.GetTargetInfo();

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

# Roll your own app?

If you want to add your own signatures to scan for you can modify the json file directly that's automatically downloaded/saved into your application directory.

You can also override the built in like so:

```csharp
SharlayanConfiguration configuration = new SharlayanConfiguration {
    ProcessModel = new ProcessModel {
        Process = Process.GetProcessesByName("ffxiv_dx11").FirstOrDefault(),
    },
};
MemoryHandler memoryHandler = new MemoryHandler(configuration);
// it be default will pull in the memory signatures from the local file, backup from API (GitHub)
memoryHandler.Scanner.Locations.Clear(); // these are resolved MemoryLocation

var signatures = new List<Signature>();
// typical signature
signatures.Add(new Signature
{
	Key = "SOMETHING",
	Value = "0123456789ABCDEF",
	Offset = 0
});
// pointer path based (no signature)
signatures.Add(new Signature
{
	Key = "SOMETHING2",
	PointerPath = new List<long>
	{
		0x0123456789
	}
});
// Aseembly Signature Based
signatures.Add(new Signature
{
	Key = "SOMETING3",
	Value = "0123456789ABCDEF0123456789ABCDEF",
	ASMSignature = true,
	PointerPath = new List<long>
	{
		0L, // ASM assumes first pointer is always 0
		144L
	}
});

memoryHandler.Scanner.LoadOffsets(signatures);
```

Once this is complete you can reference this when reading like so:

```csharp
var somethingMap = memoryHandler.GetByteArray(memoryHandler.Scanner.Locations["SOMETHING"], 8);
```
