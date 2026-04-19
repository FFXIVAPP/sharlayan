#Requires -Version 7.0
<#
.SYNOPSIS
    Build and run the Sharlayan validation harness against a running FFXIV client.

.DESCRIPTION
    Builds Sharlayan unmerged (-NoILRepack, required so the harness can see FFXIVClientStructs
    types) then runs the harness. Writes the report to harness-report.txt at the repo root
    so you can paste it back to the session. FFXIV must already be running when you invoke
    this script.

.PARAMETER Configuration
    Debug (default) or Release.

.PARAMETER SkipSubmoduleUpdate
    Pass-through to build.ps1. Default: skip (so you aren't pulling upstream mid-session).

.EXAMPLE
    .\harness.ps1
    Full build + run. Report written to .\harness-report.txt
#>

[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',

    [switch]$PullSubmodule
)

$ErrorActionPreference = 'Stop'
$repoRoot = $PSScriptRoot

# Build Sharlayan (unmerged) + the harness project.
$buildArgs = @{
    Configuration = $Configuration
    NoILRepack    = $true
}
if (-not $PullSubmodule) { $buildArgs.SkipSubmoduleUpdate = $true }

& (Join-Path $repoRoot 'build.ps1') @buildArgs
if ($LASTEXITCODE -ne 0) { throw 'Sharlayan build failed — aborting harness.' }

Write-Host ''
Write-Host '==> Building Sharlayan.Harness' -ForegroundColor Cyan
$harnessCsproj = Join-Path $repoRoot 'Sharlayan.Harness\Sharlayan.Harness.csproj'
dotnet build $harnessCsproj --configuration $Configuration -nowarn:CS1591 --nologo
if ($LASTEXITCODE -ne 0) { throw 'Harness build failed.' }

$reportPath = Join-Path $repoRoot 'harness-report.txt'
Write-Host ''
Write-Host "==> Running Sharlayan.Harness (report -> $reportPath)" -ForegroundColor Cyan
dotnet run --no-build --project $harnessCsproj --configuration $Configuration -- "--out=$reportPath"
if ($LASTEXITCODE -ne 0) {
    Write-Warning "Harness exited with code $LASTEXITCODE — see above for details."
    exit $LASTEXITCODE
}

Write-Host ''
Write-Host "✓ Report saved to $reportPath — paste its contents back to the refactor session." -ForegroundColor Green
