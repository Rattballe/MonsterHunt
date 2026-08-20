# Monster Hunt

**R.E.P.O. endless monster-hunting mode**.

The mode is designed around your idea:

- Use the monsters already in R.E.P.O.
- Spawn substantially more monsters per map
- Killing monsters is the only source of run money
- Normal valuables are set to $0
- No normal quota/extraction requirement
- Configurable kill rewards
- Configurable enemy multiplier
- Host-authoritative changes for multiplayer

## Current implementation

The project is a BepInEx/Harmony C# plugin. BepInEx is the normal Unity plugin framework and provides Harmony-based runtime patching. Game-specific assemblies still need to be referenced from the local R.E.P.O. installation; BepInEx's developer documentation explicitly recommends local references when game libraries are not available through NuGet. citeturn0search0turn0search9

### Project files

- `MonsterHunt/Plugin.cs` — configuration and plugin entry point
- `MonsterHunt/EnemyPatches.cs` — monster death rewards
- `MonsterHunt/SpawnPatches.cs` — increased vanilla enemy selection
- `MonsterHunt/EconomyPatch.cs` — disables normal valuable money
- `MonsterHunt/QuotaPatch.cs` — makes the normal quota effectively unreachable
- `MonsterHunt/MonsterHunt.csproj` — Visual Studio/.NET project

## Build it

Because R.E.P.O.'s private game assembly is not redistributed in this repository, copy these DLLs from your own installation into `lib/`:

```text
lib/
  BepInEx.dll
  0Harmony.dll
  Assembly-CSharp.dll
  UnityEngine.CoreModule.dll
  UnityEngine.PhysicsModule.dll
```

Then run:

```powershell
dotnet build MonsterHunt/MonsterHunt.csproj -c Release
```

Or install directly by supplying the game directory:

```powershell
dotnet build MonsterHunt/MonsterHunt.csproj -c Release -p:GameDir="C:\Path\To\R.E.P.O."
```

The output is:

```text
MonsterHunt/bin/Release/net472/MonsterHunt.dll
```

and, when `GameDir` is supplied:

```text
R.E.P.O./BepInEx/plugins/MonsterHunt/MonsterHunt.dll
```

## Config

After first launch, edit:

```text
BepInEx/config/rattballe.repo.monsterhunt.cfg
```

Defaults:

```text
EnemyMultiplier = 3
Tier1Reward = 100
Tier2Reward = 250
Tier3Reward = 750
DisableValuableMoney = true
DisableQuota = true
```

Set `EnemyMultiplier = 5` or higher for a much more chaotic mode, keeping in mind that very high values can hurt performance.

## Important

This is source-ready, but a trustworthy prebuilt DLL cannot be produced against R.E.P.O.'s private `Assembly-CSharp.dll` without building against the exact game assemblies from your installation. Do not download or commit someone else's game DLLs into the public repository.

R.E.P.O. has an active public modding ecosystem, and current community mods demonstrate the same `EnemyDirector.AmountSetup`, `EnemyHealth`, `RoundDirector.StartRoundLogic`, and `SemiFunc.StatGetRunCurrency/StatSetRunCurrency` APIs used here. citeturn2search2turn2search1turn2search5
