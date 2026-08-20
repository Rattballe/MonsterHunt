param(
    [Parameter(Mandatory=$true)]
    [string]$GameDir
)

$ErrorActionPreference = "Stop"

$lib = Join-Path $PSScriptRoot "lib"
$managed = Join-Path $GameDir "REPO_Data\Managed"

$required = @(
    @{ Name = "Assembly-CSharp.dll"; Source = (Join-Path $managed "Assembly-CSharp.dll") },
    @{ Name = "UnityEngine.CoreModule.dll"; Source = (Join-Path $managed "UnityEngine.CoreModule.dll") },
    @{ Name = "UnityEngine.PhysicsModule.dll"; Source = (Join-Path $managed "UnityEngine.PhysicsModule.dll") }
)

foreach ($item in $required) {
    if (!(Test-Path $item.Source)) {
        throw "Missing $($item.Name) at $($item.Source)"
    }
    Copy-Item $item.Source (Join-Path $lib $item.Name) -Force
}

$bep = Join-Path $GameDir "BepInEx\core\BepInEx.dll"
$harm = Join-Path $GameDir "BepInEx\core\0Harmony.dll"

if (!(Test-Path $bep)) { throw "Missing BepInEx.dll at $bep" }
if (!(Test-Path $harm)) { throw "Missing 0Harmony.dll at $harm" }
Copy-Item $bep (Join-Path $lib "BepInEx.dll") -Force
Copy-Item $harm (Join-Path $lib "0Harmony.dll") -Force

dotnet build (Join-Path $PSScriptRoot "MonsterHunt\MonsterHunt.csproj") -c Release -p:GameDir=$GameDir

Write-Host ""
Write-Host "Monster Hunt built successfully."
Write-Host "Installed to $GameDir\BepInEx\plugins\MonsterHunt\MonsterHunt.dll"
