param(
    [Parameter(Mandatory=$false)]
    [string]$ApiKey
)

# If API key not provided, prompt for it
if (-not $ApiKey) {
    $ApiKey = Read-Host "Enter your NuGet API key"
}

# Validate API key is not empty
if (-not $ApiKey) {
    Write-Error "API key is required"
    exit 1
}

# Get all 1.0.4 nupkg files in current directory
$packages = Get-ChildItem -Filter "*.1.0.4.nupkg" -File

if ($packages.Count -eq 0) {
    Write-Error "No packages found matching *.1.0.4.nupkg in current directory"
    exit 1
}

Write-Host "Found $($packages.Count) package(s) to publish" -ForegroundColor Green
Write-Host ""

# Push each package
$successCount = 0
$failureCount = 0

foreach ($package in $packages) {
    Write-Host "Publishing: $($package.Name)" -ForegroundColor Cyan

    dotnet nuget push "$($package.FullName)" `
        --api-key $ApiKey `
        --source https://api.nuget.org/v3/index.json

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Published successfully" -ForegroundColor Green
        $successCount++
    } else {
        Write-Host "Failed to publish" -ForegroundColor Red
        $failureCount++
    }
    Write-Host ""
}

# Summary
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "Published: $successCount successful, $failureCount failed" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
