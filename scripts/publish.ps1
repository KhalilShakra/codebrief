param(
    [switch]$SkipInstaller
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

Write-Host "Publishing CodeBrief (win-x64, self-contained)..."
dotnet publish src/CodeBrief.Desktop/CodeBrief.Desktop.csproj `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:PublishTrimmed=false `
    -o publish/win-x64

if (-not $SkipInstaller) {
    $iscc = @(
        "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
        "$env:ProgramFiles\Inno Setup 6\ISCC.exe",
        "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe"
    ) | Where-Object { Test-Path $_ } | Select-Object -First 1

    if (-not $iscc) {
        Write-Warning "Inno Setup hittades inte. Zippar publish/win-x64 istället."
        New-Item -ItemType Directory -Force dist | Out-Null
        Compress-Archive -Path publish/win-x64\* -DestinationPath dist/CodeBrief-1.0.0-win-x64.zip -Force
        return
    }

    New-Item -ItemType Directory -Force dist | Out-Null
    & $iscc installer\codebrief.iss
}

Write-Host "Klart. Se publish/win-x64 och dist/"
