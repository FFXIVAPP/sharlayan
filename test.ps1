#Requires -Version 7.0
<#
.SYNOPSIS
    Build and run Sharlayan.Tests.

.DESCRIPTION
    Ensures the FFXIVClientStructs submodule is built, then runs Sharlayan's test suite.
    Disables ILRepack for this build so tests can see Sharlayan + FFXIVClientStructs as
    separate assemblies (merging would hide FFXIVClientStructs behind internalization).

.PARAMETER Configuration
    Debug or Release. Default: Debug.

.PARAMETER SkipSubmoduleUpdate
    Skip git submodule sync/update (pass-through to build.ps1).

.PARAMETER Filter
    xUnit filter expression, e.g. -Filter "FullyQualifiedName~Provider".
#>

[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',

    [switch]$SkipSubmoduleUpdate,

    [string]$Filter
)

$ErrorActionPreference = 'Stop'
$repoRoot = $PSScriptRoot

# Build Sharlayan (unmerged — required for tests to see FFXIVClientStructs types).
$buildArgs = @{
    Configuration = $Configuration
    NoILRepack    = $true
}
if ($SkipSubmoduleUpdate) { $buildArgs.SkipSubmoduleUpdate = $true }

& (Join-Path $repoRoot 'build.ps1') @buildArgs
if ($LASTEXITCODE -ne 0) { throw 'Sharlayan build failed — skipping tests.' }

Write-Host ''
Write-Host '==> Running Sharlayan.Tests' -ForegroundColor Cyan
$testCsproj = Join-Path $repoRoot 'Sharlayan.Tests\Sharlayan.Tests.csproj'

$testArgs = @(
    'test', $testCsproj,
    '--configuration', $Configuration,
    '-p:DisableILRepack=true',
    '--nologo'
)
if ($Filter) { $testArgs += '--filter'; $testArgs += $Filter }

dotnet @testArgs
if ($LASTEXITCODE -ne 0) { throw 'Sharlayan.Tests failed.' }
