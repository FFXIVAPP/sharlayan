#Requires -Version 7.0
<#
.SYNOPSIS
    Build Sharlayan with the latest FFXIVClientStructs from the submodule.

.DESCRIPTION
    1. Ensures the FFXIVClientStructs git submodule is initialised and up-to-date.
    2. Builds FFXIVClientStructs (runs its source generators, produces DLLs).
    3. Copies FFXIVClientStructs.dll + InteropGenerator.Runtime.dll into the location
       Sharlayan.csproj references.
    4. Builds Sharlayan (which ILRepacks the FFXIVClientStructs outputs into Sharlayan.dll).

.PARAMETER Configuration
    Debug or Release. Default: Debug.

.PARAMETER SkipSubmoduleUpdate
    Skip git submodule sync/update. Useful during iterative local development when you
    want to test against a pinned FFXIVClientStructs state without pulling upstream.

.PARAMETER NoILRepack
    Build Sharlayan WITHOUT merging FFXIVClientStructs into the output DLL. Leaves the
    dependency DLLs alongside Sharlayan.dll. Useful for debugging reference issues.

.PARAMETER Version
    Override the NuGet package version. When omitted the version from version.props is
    used. Accepts any valid SemVer string, e.g. "9.0.4" or "9.0.4-preview.42".

.EXAMPLE
    .\build.ps1
    Full pipeline, Debug configuration.

.EXAMPLE
    .\build.ps1 -Configuration Release
    Release build with merged DLL.

.EXAMPLE
    .\build.ps1 -SkipSubmoduleUpdate
    Skip pulling new FFXIVClientStructs commits.

.EXAMPLE
    .\build.ps1 -Configuration Release -Version "9.0.4-preview.99"
    Release build published as a NuGet prerelease.
#>

[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',

    [switch]$SkipSubmoduleUpdate,

    [switch]$NoILRepack,

    [string]$Version = ''
)

$ErrorActionPreference = 'Stop'
$repoRoot = $PSScriptRoot
$sharlayanDir = Join-Path $repoRoot 'Sharlayan'
$submoduleDir = Join-Path $sharlayanDir 'FFXIVClientStructs'
$csCsproj = Join-Path $submoduleDir 'FFXIVClientStructs\FFXIVClientStructs.csproj'
$sharlayanCsproj = Join-Path $sharlayanDir 'Sharlayan.csproj'

function Write-Step {
    param([string]$Message)
    Write-Host ''
    Write-Host "==> $Message" -ForegroundColor Cyan
}

function Assert-ToolAvailable {
    param([string]$Name, [string]$VersionArg = '--version')
    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        throw "Required tool '$Name' not found in PATH."
    }
    Write-Verbose "$Name : $(& $Name $VersionArg 2>&1 | Select-Object -First 1)"
}

# ---- 1. Tool check ---------------------------------------------------------
Write-Step 'Checking required tools'
Assert-ToolAvailable -Name git
Assert-ToolAvailable -Name dotnet --version

# ---- 2. Submodule sync -----------------------------------------------------
if (-not $SkipSubmoduleUpdate) {
    Write-Step 'Updating FFXIVClientStructs submodule'
    Push-Location $repoRoot
    try {
        git submodule sync --recursive
        if ($LASTEXITCODE -ne 0) { throw 'git submodule sync failed.' }

        git submodule update --init --recursive --remote -- Sharlayan/FFXIVClientStructs
        if ($LASTEXITCODE -ne 0) { throw 'git submodule update failed.' }
    }
    finally {
        Pop-Location
    }
}
else {
    Write-Step 'Skipping submodule update (-SkipSubmoduleUpdate)'
}

if (-not (Test-Path $csCsproj)) {
    throw "FFXIVClientStructs project not found at $csCsproj. Submodule may not be initialised."
}

# ---- 3. Build FFXIVClientStructs ------------------------------------------
Write-Step "Building FFXIVClientStructs ($Configuration)"
dotnet build $csCsproj --configuration $Configuration --nologo
if ($LASTEXITCODE -ne 0) { throw 'FFXIVClientStructs build failed.' }

# FFXIVClientStructs.csproj writes to ..\bin\<Configuration>\ (relative to its own csproj)
# so the output lands at: <submodule>/bin/<Configuration>/
$csBuildOutput = Join-Path $submoduleDir "bin\$Configuration"
$csDll = Join-Path $csBuildOutput 'FFXIVClientStructs.dll'
$interopDll = Join-Path $csBuildOutput 'InteropGenerator.Runtime.dll'

if (-not (Test-Path $csDll)) { throw "FFXIVClientStructs.dll not produced at $csDll" }
if (-not (Test-Path $interopDll)) { throw "InteropGenerator.Runtime.dll not produced at $interopDll" }

Write-Host "    FFXIVClientStructs.dll      $([Math]::Round((Get-Item $csDll).Length / 1KB)) KB"
Write-Host "    InteropGenerator.Runtime.dll $([Math]::Round((Get-Item $interopDll).Length / 1KB)) KB"

# ---- 4. Build Sharlayan ----------------------------------------------------
Write-Step "Building Sharlayan ($Configuration)"

$ilRepackArg = if ($NoILRepack) { '-p:DisableILRepack=true' } else { $null }

$buildArgs = @(
    'build', $sharlayanCsproj,
    '--configuration', $Configuration,
    '-nowarn:CS1591',
    '--nologo'
)
if ($ilRepackArg) { $buildArgs += $ilRepackArg }
if ($Version)     { $buildArgs += "-p:Version=$Version" }

dotnet @buildArgs
if ($LASTEXITCODE -ne 0) { throw 'Sharlayan build failed.' }

# ---- 5. Report -------------------------------------------------------------
Write-Step 'Build complete'
# Sharlayan.csproj targets a single TFM (net10.0 at time of writing). SDK appends the
# TFM subfolder to OutputPath, so Sharlayan.dll lives one level deeper than the raw
# OutputPath. Glob for it rather than hardcoding the TFM string.
$sharlayanOutputRoot = Join-Path $sharlayanDir "bin\$Configuration"
$sharlayanDll = Get-ChildItem -Path $sharlayanOutputRoot -Filter 'Sharlayan.dll' -Recurse -File -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if ($sharlayanDll) {
    $sizeKb = [Math]::Round($sharlayanDll.Length / 1KB)
    $merged = if ($NoILRepack) { '(unmerged)' } else { '(merged with FFXIVClientStructs)' }
    Write-Host "    Sharlayan.dll : $sizeKb KB $merged"
    Write-Host "    Location      : $($sharlayanDll.FullName)"
}
else {
    Write-Warning "Sharlayan.dll not found under $sharlayanOutputRoot"
}
