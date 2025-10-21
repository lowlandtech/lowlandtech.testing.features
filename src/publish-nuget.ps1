<#
.SYNOPSIS
    Builds and publishes a NuGet package with automatic versioning.
.DESCRIPTION
    This script automatically increments the package version, builds the project,
    and outputs the NuGet package to a local directory.
.PARAMETER VersionIncrement
    Specifies which version component to increment: Major, Minor, or Patch (default: Patch)
.PARAMETER OutputPath
    The directory where the NuGet package will be published (default: C:\Workspaces\Packages)
.EXAMPLE
    .\publish-nuget.ps1
    .\publish-nuget.ps1 -VersionIncrement Minor
#>

param(
    [Parameter()]
    [ValidateSet("Major", "Minor", "Patch")]
    [string]$VersionIncrement = "Patch",
    
    [Parameter()]
    [string]$OutputPath = "C:\Workspaces\Packages",
    
    [Parameter()]
    [string]$ProjectFile = "lowlandtech.testing.features\LowlandTech.Testing.Features.csproj"
)

# Ensure output directory exists
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
    Write-Host "Created output directory: $OutputPath" -ForegroundColor Green
}

# Read the project file
$csprojPath = Join-Path $PSScriptRoot $ProjectFile
if (-not (Test-Path $csprojPath)) {
    Write-Error "Project file not found: $csprojPath"
    exit 1
}

[xml]$csproj = Get-Content $csprojPath

# Find the Version element
$versionNode = $csproj.Project.PropertyGroup.Version | Where-Object { $_ -ne $null } | Select-Object -First 1
$packageVersionNode = $csproj.Project.PropertyGroup.PackageVersion | Where-Object { $_ -ne $null } | Select-Object -First 1

if (-not $versionNode) {
    Write-Error "Version element not found in project file"
    exit 1
}

$currentVersion = $versionNode
Write-Host "Current version: $currentVersion" -ForegroundColor Cyan

# Parse version (supports both semantic versioning and date-based versioning)
$versionParts = $currentVersion -split '\.'
if ($versionParts.Count -lt 3) {
    Write-Error "Invalid version format. Expected format: Major.Minor.Patch"
    exit 1
}

$major = [int]$versionParts[0]
$minor = [int]$versionParts[1]
$patch = [int]$versionParts[2]

# Increment version based on parameter
switch ($VersionIncrement) {
    "Major" {
        $major++
        $minor = 0
        $patch = 0
    }
    "Minor" {
        $minor++
        $patch = 0
    }
    "Patch" {
        $patch++
    }
}

$newVersion = "$major.$minor.$patch"
Write-Host "New version: $newVersion" -ForegroundColor Green

# Update version in project file
$csproj.Project.PropertyGroup | ForEach-Object {
    if ($_.Version) {
        $_.Version = $newVersion
    }
    if ($_.PackageVersion) {
        $_.PackageVersion = $newVersion
    }
}

# Save the updated project file
$csproj.Save($csprojPath)
Write-Host "Updated project file with new version" -ForegroundColor Green

# Clean previous builds
Write-Host "`nCleaning previous builds..." -ForegroundColor Cyan
dotnet clean $csprojPath --configuration Release --verbosity quiet

# Restore packages
Write-Host "Restoring packages..." -ForegroundColor Cyan
dotnet restore $csprojPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed"
    exit 1
}

# Build the project
Write-Host "Building project..." -ForegroundColor Cyan
dotnet build $csprojPath --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
}

# Pack the NuGet package (with or without icon)
Write-Host "`nPacking NuGet package..." -ForegroundColor Cyan

# Check if icon file exists
$iconPath = Join-Path (Split-Path $PSScriptRoot -Parent) "icon.png"
$packArgs = @(
    "pack", $csprojPath,
    "--configuration", "Release",
    "--no-build",
    "--output", $OutputPath,
    "/p:Version=$newVersion",
    "/p:PackageVersion=$newVersion"
)

if (-not (Test-Path $iconPath)) {
    Write-Warning "Icon file not found at: $iconPath"
    Write-Warning "Packaging without icon. To add an icon, place 'icon.png' in the repository root."
    # Remove icon from package
    $packArgs += "/p:PackageIcon="
}

$packResult = & dotnet $packArgs

if ($LASTEXITCODE -ne 0) {
    Write-Error "Pack failed. See output above for details."
    exit 1
}

# Display results
Write-Host "`n========================================" -ForegroundColor Green
Write-Host "✓ Package successfully created!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "Version: $newVersion" -ForegroundColor Cyan
Write-Host "Output: $OutputPath" -ForegroundColor Cyan
Write-Host "`nPackage files:" -ForegroundColor Cyan
Get-ChildItem -Path $OutputPath -Filter "LowlandTech.Testing.Features.$newVersion.*" | ForEach-Object {
    Write-Host "  - $($_.Name)" -ForegroundColor White
}

# Optional: Commit version change to git
$commitChanges = Read-Host "`nDo you want to commit the version change to git? (y/n)"
if ($commitChanges -eq 'y') {
    git add $csprojPath
    git commit -m "Bump version to $newVersion"
    Write-Host "Version change committed to git" -ForegroundColor Green
}