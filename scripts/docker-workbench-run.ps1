[CmdletBinding()]
param(
    [string]$ImageName = "nscript-workbench",
    [string]$RepositoryPath = (Join-Path $PSScriptRoot ".."),
    [string]$WorkspacePath = "/workspace",
    [string[]]$Mount = @(),
    [string[]]$EnvVar = @(),
    [switch]$EnableHostDockerAccess,
    [switch]$DisableGeneratedArtifactIsolation,
    [switch]$RunAsRoot,
    [string[]]$ContainerCommand = @("pwsh", "-NoLogo")
)

$ErrorActionPreference = "Stop"

function Resolve-MountArguments {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Spec
    )

    $parts = $Spec -split "=", 2
    if ($parts.Count -ne 2) {
        throw "Mount spec '$Spec' must be in the form '<host-path>=<container-path>'."
    }

    $hostPath = (Resolve-Path $parts[0]).Path
    $containerPath = $parts[1]

    return @("--mount", "type=bind,source=$hostPath,target=$containerPath")
}

function Test-IsRepoManagedPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RepoRoot,

        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $roslynRoot = Join-Path $RepoRoot "roslyn"
    return -not $Path.StartsWith($roslynRoot, [System.StringComparison]::OrdinalIgnoreCase)
}

function Test-IsNestedNodeModulesPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    return $Path -match '[\\/]+node_modules([\\/]|$)'
}

function Convert-ToContainerPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RepoRoot,

        [Parameter(Mandatory = $true)]
        [string]$WorkspaceRoot,

        [Parameter(Mandatory = $true)]
        [string]$FullPath
    )

    $relativePath = [System.IO.Path]::GetRelativePath($RepoRoot, $FullPath) -replace "\\", "/"
    if ([string]::IsNullOrWhiteSpace($relativePath) -or $relativePath -eq ".") {
        return $WorkspaceRoot
    }

    return "$WorkspaceRoot/$relativePath"
}

function Get-IsolatedDotNetArtifactTargets {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RepoRoot,

        [Parameter(Mandatory = $true)]
        [string]$WorkspaceRoot
    )

    $targets = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    $projectFiles = Get-ChildItem -Path $RepoRoot -Recurse -File -Filter *.csproj |
        Where-Object { Test-IsRepoManagedPath -RepoRoot $RepoRoot -Path $_.FullName }

    foreach ($projectFile in $projectFiles) {
        $containerProjectDirectory = Convert-ToContainerPath -RepoRoot $RepoRoot -WorkspaceRoot $WorkspaceRoot -FullPath $projectFile.Directory.FullName
        [void]$targets.Add("$containerProjectDirectory/bin")
        [void]$targets.Add("$containerProjectDirectory/obj")
    }

    [void]$targets.Add("$WorkspaceRoot/NScriptToolSet/bin")
    [void]$targets.Add("$WorkspaceRoot/NScriptToolSet/lib")

    return @($targets)
}

function Get-IsolatedNodeArtifactTargets {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RepoRoot,

        [Parameter(Mandatory = $true)]
        [string]$WorkspaceRoot
    )

    $targets = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    $packageFiles = Get-ChildItem -Path $RepoRoot -Recurse -File -Filter package.json |
        Where-Object {
            (Test-IsRepoManagedPath -RepoRoot $RepoRoot -Path $_.FullName) -and
            (-not (Test-IsNestedNodeModulesPath -Path $_.FullName))
        }

    foreach ($packageFile in $packageFiles) {
        $containerDirectory = Convert-ToContainerPath -RepoRoot $RepoRoot -WorkspaceRoot $WorkspaceRoot -FullPath $packageFile.Directory.FullName
        [void]$targets.Add("$containerDirectory/node_modules")
    }

    return @($targets)
}

function Get-IsolatedGeneratedArtifactTargets {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RepoRoot,

        [Parameter(Mandatory = $true)]
        [string]$WorkspaceRoot
    )

    $targets = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    $generatedDirs = Get-ChildItem -Path $RepoRoot -Recurse -Directory -Filter GeneratedScripts |
        Where-Object { Test-IsRepoManagedPath -RepoRoot $RepoRoot -Path $_.FullName }

    foreach ($generatedDir in $generatedDirs) {
        $containerDirectory = Convert-ToContainerPath -RepoRoot $RepoRoot -WorkspaceRoot $WorkspaceRoot -FullPath $generatedDir.FullName
        [void]$targets.Add($containerDirectory)
    }

    return @($targets)
}

$repoRoot = (Resolve-Path $RepositoryPath).Path

$dockerArgs = @(
    "run",
    "--rm",
    "-it",
    "--workdir",
    $WorkspacePath,
    "--mount",
    "type=bind,source=$repoRoot,target=$WorkspacePath"
)

$isolatedArtifactTargets = @()
$isolatedArtifactTargets += Get-IsolatedDotNetArtifactTargets -RepoRoot $repoRoot -WorkspaceRoot $WorkspacePath
$isolatedArtifactTargets += Get-IsolatedNodeArtifactTargets -RepoRoot $repoRoot -WorkspaceRoot $WorkspacePath
$isolatedArtifactTargets += Get-IsolatedGeneratedArtifactTargets -RepoRoot $repoRoot -WorkspaceRoot $WorkspacePath
$isolatedArtifactTargets = $isolatedArtifactTargets | Sort-Object -Unique

foreach ($envSpec in $EnvVar) {
    $dockerArgs += @("-e", $envSpec)
}

foreach ($mountSpec in $Mount) {
    $dockerArgs += Resolve-MountArguments -Spec $mountSpec
}

if (-not $DisableGeneratedArtifactIsolation) {
    $dockerArgs += @("-e", "WORKBENCH_FIXUP_PATHS=$($isolatedArtifactTargets -join '|')")
    foreach ($target in $isolatedArtifactTargets) {
        $dockerArgs += @("--mount", "type=volume,target=$target")
    }
}

if ($EnableHostDockerAccess) {
    $dockerArgs += @("--mount", "type=bind,source=/var/run/docker.sock,target=/var/run/docker.sock")
}

if ($RunAsRoot) {
    $dockerArgs += @("-e", "WORKBENCH_RUN_AS_ROOT=true", "--user", "root")
}

$dockerArgs += $ImageName
$dockerArgs += $ContainerCommand

Write-Host "Running: docker $($dockerArgs -join ' ')"
& docker @dockerArgs

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}
