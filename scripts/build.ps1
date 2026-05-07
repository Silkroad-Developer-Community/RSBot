# Usage:
# `scripts\build.ps1
# -Clean[False, optional]
# -DoNotStart[False, optional]
# -Configuration[Debug, optional]`

param(
    [string]$Configuration = "Debug",
    [switch]$Clean,
    [switch]$DoNotStart
)

if (-not (Test-Path ".\SDUI")) {
    Write-Output "SDUI submodule is missing. Initializing and updating submodules..."
    git submodule update --init --recursive
}

taskkill /F /IM RSBot.exe
taskkill /F /IM sro_client.exe

if ($Clean) {
    Write-Output "Performing a clean build..."
    New-Item  -ItemType Directory ".\temp" -ErrorAction SilentlyContinue > $null
    Move-Item ".\Build\User" ".\temp" -ErrorAction SilentlyContinue > $null
    Remove-Item -Recurse -Force ".\Build" -ErrorAction SilentlyContinue > $null
}

Write-Output "Building with '$Configuration' configuration..."
$vsPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath
$msBuildPath = Join-Path $vsPath "MSBuild\Current\Bin\MSBuild.exe"
& $msBuildPath /p:Configuration=$Configuration /p:Platform=x86 RSBot.sln | Tee-Object -FilePath build.log
$buildExitCode = $LASTEXITCODE

if ($Clean) {
    Move-Item ".\temp\User" ".\Build\User" -ErrorAction SilentlyContinue > $null
    Remove-Item -Recurse -Force ".\temp" -ErrorAction SilentlyContinue > $null
}

if ($buildExitCode -eq 0) {
    # TODO: move linkage logic to msbuild for vs devs
    Write-Output "Fetching navigation linkage data..."
    $linkageUrl = "https://raw.githubusercontent.com/Silkroad-Developer-Community/Silkroad-NavLink/main/navigation_linkage.json"
    $linkagePath = ".\Build\Data\navigation_linkage.json"
    if (-not (Test-Path ".\Build\Data")) {
        New-Item -ItemType Directory ".\Build\Data" -Force | Out-Null
    }
    $linkageTempPath = "$linkagePath.tmp"
    try {
        Invoke-WebRequest -Uri $linkageUrl -OutFile $linkageTempPath -UseBasicParsing -ErrorAction Stop
        $jsonContent = Get-Content -Path $linkageTempPath -Raw
        $null = $jsonContent | ConvertFrom-Json -ErrorAction Stop
        Move-Item -Path $linkageTempPath -Destination $linkagePath -Force
        Write-Output "Successfully updated navigation linkage data."
    }
    catch {
        Write-Warning "Failed to update navigation linkage data: $($_.Exception.Message). Keeping existing file if it exists."
        if (Test-Path $linkageTempPath) {
            Remove-Item $linkageTempPath -Force
        }
    }

    if (!$DoNotStart) {
        Write-Output "Starting OasisBot..."
        & ".\Build\OasisBot.exe"
    }
}
else {
    Write-Output "Build failed. Check build.log for details."
    exit $buildExitCode
}