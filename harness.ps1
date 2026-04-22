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

# FFXIV's ACG blocks PROCESS_VM_READ from non-admin processes — must run elevated.
$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Error "harness.ps1 must be run as Administrator (FFXIV blocks VM reads from non-elevated processes). Right-click your terminal and choose 'Run as administrator', then retry."
    exit 1
}

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

$reportsDir = Join-Path $repoRoot 'reports'
if (-not (Test-Path $reportsDir)) { New-Item -ItemType Directory -Path $reportsDir | Out-Null }

$timestamp   = Get-Date -Format 'yyyyMMdd-HHmmss'
$stampedPath = Join-Path $reportsDir "harness-$timestamp.txt"
$latestPath  = Join-Path $repoRoot 'harness-report.txt'

Write-Host ''
Write-Host "==> Running Sharlayan.Harness (report -> $stampedPath)" -ForegroundColor Cyan
dotnet run --no-build --project $harnessCsproj --configuration $Configuration -- "--out=$stampedPath"
if ($LASTEXITCODE -ne 0) {
    Write-Warning "Harness exited with code $LASTEXITCODE — see above for details."
    exit $LASTEXITCODE
}

Copy-Item -Path $stampedPath -Destination $latestPath -Force

Write-Host ''
Write-Host "✓ Report saved to $stampedPath" -ForegroundColor Green
Write-Host "✓ Latest copy  -> $latestPath" -ForegroundColor Green
