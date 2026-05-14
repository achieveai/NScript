<#
.SYNOPSIS
    Publishes NScript NuGet packages from the current directory.

.DESCRIPTION
    Discovers .nupkg files in the script directory and pushes them to a NuGet
    feed (defaults to nuget.org).

    Default selection strategy: for each package id, picks the highest semver
    version found on disk. This makes the typical workflow ("build everything,
    publish the latest") work without parameter churn while staying safe against
    accidental republishes of old artifacts that linger in NScriptToolSet/.

    Override with -Versions to publish a specific list of versions instead, or
    -PackageIds to restrict the publish set to particular package ids.

.PARAMETER ApiKey
    NuGet feed API key. Prompted interactively if omitted.

.PARAMETER Source
    NuGet feed URL or local-feed path. Defaults to https://api.nuget.org/v3/index.json.

.PARAMETER Versions
    Comma-separated list of versions to publish (e.g., "1.1.0,1.1.1"). When
    specified, only packages whose version matches one of these entries are
    pushed. Overrides the "highest version per id" default.

.PARAMETER PackageIds
    Comma-separated list of package ids to restrict the publish set
    (e.g., "Mcqdb.NScript.Cs2Jsc,Mcqdb.NScript.Sdk").

.PARAMETER DryRun
    Print the publish plan and exit without pushing anything.

.EXAMPLE
    ./Publish-Packages.ps1 -ApiKey $key
    Publishes the highest version of every package id in the directory to nuget.org.

.EXAMPLE
    ./Publish-Packages.ps1 -ApiKey $key -Versions "1.1.0,1.1.1"
    Publishes only packages whose versions match 1.1.0 or 1.1.1.

.EXAMPLE
    ./Publish-Packages.ps1 -Source "C:\Users\me\local-feed" -DryRun
    Previews what would be published to a local feed.
#>
param(
    [Parameter(Mandatory=$false)]
    [string]$ApiKey,

    [Parameter(Mandatory=$false)]
    [string]$Source = "https://api.nuget.org/v3/index.json",

    [Parameter(Mandatory=$false)]
    [string]$Versions,

    [Parameter(Mandatory=$false)]
    [string]$PackageIds,

    [Parameter(Mandatory=$false)]
    [switch]$DryRun
)

# nupkg filenames follow the pattern <id>.<version>.nupkg where version is
# SemVer (e.g., 1.1.0 or 1.1.1-preview.2). Capture both halves with a regex
# anchored on the trailing .nupkg.
$nupkgNameRegex = '^(?<id>.+?)\.(?<version>\d+\.\d+\.\d+(?:[-+].+)?)\.nupkg$'

function Parse-NupkgName {
    param([System.IO.FileInfo]$File)

    if ($File.Name -match $nupkgNameRegex) {
        [pscustomobject]@{
            File = $File
            Id = $matches['id']
            Version = $matches['version']
        }
    }
}

function ConvertTo-VersionSortKey {
    param([string]$Version)

    # Sort by the numeric core first (1.10.0 > 1.9.0), then prerelease tag
    # (1.1.1 > 1.1.1-preview). Returns a tuple-like array for comparison.
    $parts = $Version -split '[-+]', 2
    $core = [Version]::Parse($parts[0])
    $isRelease = $parts.Length -eq 1
    @($core, $isRelease, ($parts | Select-Object -Skip 1) -join '')
}

# Discover packages in the script directory.
$scriptDir = $PSScriptRoot
if (-not $scriptDir) { $scriptDir = (Get-Location).Path }
$nupkgs = Get-ChildItem -Path $scriptDir -Filter "*.nupkg" -File |
    ForEach-Object { Parse-NupkgName $_ } |
    Where-Object { $_ -ne $null }

if (-not $nupkgs) {
    Write-Error "No .nupkg files found in $scriptDir"
    exit 1
}

# Apply optional filters.
if ($PackageIds) {
    $idSet = $PackageIds -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ }
    $nupkgs = $nupkgs | Where-Object { $idSet -contains $_.Id }
}

if ($Versions) {
    $versionSet = $Versions -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ }
    $selected = $nupkgs | Where-Object { $versionSet -contains $_.Version }
} else {
    # Default: highest version per package id.
    $selected = $nupkgs |
        Group-Object -Property Id |
        ForEach-Object {
            $_.Group |
                Sort-Object -Property @{ Expression = { ConvertTo-VersionSortKey $_.Version } } -Descending |
                Select-Object -First 1
        }
}

if (-not $selected) {
    Write-Error "No packages matched the supplied filters."
    exit 1
}

$selected = $selected | Sort-Object Id

Write-Host "Source : $Source" -ForegroundColor Yellow
Write-Host "Plan   : $($selected.Count) package(s)" -ForegroundColor Yellow
foreach ($pkg in $selected) {
    Write-Host "         $($pkg.Id) $($pkg.Version)" -ForegroundColor Gray
}
Write-Host ""

if ($DryRun) {
    Write-Host "DryRun: skipping push." -ForegroundColor Cyan
    exit 0
}

# API key only required for remote feeds. Local-feed pushes (folder path) accept
# it but do not use it.
$isLocalFeed = Test-Path -LiteralPath $Source -PathType Container
if (-not $isLocalFeed -and -not $ApiKey) {
    $ApiKey = Read-Host "Enter your NuGet API key"
    if (-not $ApiKey) {
        Write-Error "API key is required when publishing to $Source"
        exit 1
    }
}

$successCount = 0
$failureCount = 0

foreach ($pkg in $selected) {
    Write-Host "Publishing: $($pkg.File.Name)" -ForegroundColor Cyan

    $args = @('nuget', 'push', $pkg.File.FullName, '--source', $Source)
    if ($ApiKey) { $args += @('--api-key', $ApiKey) }

    & dotnet @args

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Published successfully" -ForegroundColor Green
        $successCount++
    } else {
        Write-Host "Failed to publish" -ForegroundColor Red
        $failureCount++
    }
    Write-Host ""
}

Write-Host "========================================" -ForegroundColor Yellow
Write-Host "Published: $successCount successful, $failureCount failed" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow

if ($failureCount -gt 0) { exit 1 }
