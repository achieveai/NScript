[CmdletBinding()]
param(
    [switch]$SkipDebugBuild,
    [switch]$SkipReleaseBuild,
    [switch]$SkipBrowserValidation
)

$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$testWebAppDir = Join-Path $repoRoot "Test\Framework\TestWebApplication"

Set-Location $repoRoot

function Invoke-WorkbenchStep {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name,

        [Parameter(Mandatory = $true)]
        [scriptblock]$Command,

        [string]$WorkingDirectory
    )

    Write-Host "==> $Name"

    if ([string]::IsNullOrWhiteSpace($WorkingDirectory)) {
        & $Command
    }
    else {
        Push-Location $WorkingDirectory
        try {
            & $Command
        }
        finally {
            Pop-Location
        }
    }

    if ($LASTEXITCODE -ne 0) {
        throw "Step '$Name' failed with exit code $LASTEXITCODE."
    }
}

Invoke-WorkbenchStep "tool versions" {
    dotnet --list-sdks
    dotnet --list-runtimes
    node --version
    npm --version
    pwsh --version
    python3 --version
    uv --version
    uvx --version
    docker --version
}

Invoke-WorkbenchStep "agent CLI checks" {
    $npmRoot = (& npm root -g).Trim()
    $claudeCliPath = Join-Path $npmRoot "@anthropic-ai/claude-agent-sdk/cli.js"

    if (-not (Test-Path $claudeCliPath)) {
        throw "Claude Agent SDK entrypoint was not found at '$claudeCliPath'."
    }

    $claudeVersion = (& node -e "const path = require('path'); const pkg = path.join(process.argv[1], '@anthropic-ai', 'claude-agent-sdk', 'package.json'); console.log(require(pkg).version);" $npmRoot).Trim()
    if ([string]::IsNullOrWhiteSpace($claudeVersion)) {
        throw "Could not determine Claude Agent SDK package version."
    }

    Write-Host "Claude Agent SDK package version: $claudeVersion"
    claude --version
    copilot --version
    codex --version
}

if (-not $SkipDebugBuild) {
    Invoke-WorkbenchStep "debug build" {
        dotnet build NScript_Full.sln -c Debug --nologo
    }
}

if (-not $SkipReleaseBuild) {
    Invoke-WorkbenchStep "release build" {
        dotnet build NScript_Full.sln -c Release --nologo
    }
}

if (-not $SkipBrowserValidation) {
    Invoke-WorkbenchStep "browser test dependencies" -WorkingDirectory $testWebAppDir {
        npm ci
    }

    Invoke-WorkbenchStep "Playwright browser install" -WorkingDirectory $testWebAppDir {
        npx playwright install chromium
    }

    Invoke-WorkbenchStep "browser tests" -WorkingDirectory $testWebAppDir {
        npm test
    }
}
