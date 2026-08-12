#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Seeds Maven dependencies into the dnceng dotnet-public-maven feed.

.DESCRIPTION
    CI reads the feed anonymously. New upstream artifacts return 401 until an
    authenticated developer request causes Azure Artifacts to cache them.
    Run `az login`, then use this script for a Gradle project or explicit Maven
    coordinates. Tokens are sent only in the Authorization header and are not
    written to output.

.EXAMPLE
    pwsh ./build/gradle/mirror-dependencies.ps1 `
        -ProjectDir source/com.google.android.play/asset.delivery.extensions `
        -Task build

.EXAMPLE
    pwsh ./build/gradle/mirror-dependencies.ps1 `
        -MavenArtifact 'com.google.code.gson:gson:2.11.0'
#>
[CmdletBinding(DefaultParameterSetName = 'Gradle')]
param (
    [Parameter(Mandatory, ParameterSetName = 'Gradle')]
    [string] $ProjectDir,

    [Parameter(Mandatory, ParameterSetName = 'Gradle')]
    [string] $Task,

    [Parameter(Mandatory, ParameterSetName = 'MavenArtifact')]
    [string[]] $MavenArtifact,

    [Parameter(ParameterSetName = 'Gradle')]
    [string] $AndroidHome = $env:ANDROID_HOME,

    [Parameter(ParameterSetName = 'Gradle')]
    [int] $MaxIterations = 15
)

$ErrorActionPreference = 'Stop'
$feedBaseUrl = 'https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-public-maven/maven/v1'
$azDevOpsResource = '499b84ac-1321-427f-aa17-267ca6975798'
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '../..') | Select-Object -ExpandProperty Path

function Get-AzDevOpsToken {
    $token = az account get-access-token --resource $azDevOpsResource --query accessToken -o tsv 2>$null
    if ([string]::IsNullOrEmpty($token)) {
        throw "Could not get an Azure DevOps access token. Run 'az login' first."
    }
    $token
}

function Invoke-MirrorUrls([string[]] $Urls) {
    $token = Get-AzDevOpsToken
    $credential = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(":$token"))
    $headers = @{ Authorization = "Basic $credential" }
    foreach ($url in $Urls | Sort-Object -Unique) {
        $response = Invoke-WebRequest -Uri $url -Headers $headers -SkipHttpErrorCheck
        Write-Host "$($response.StatusCode) $url"
    }
}

Get-AzDevOpsToken | Out-Null

if ($PSCmdlet.ParameterSetName -eq 'MavenArtifact') {
    $urls = foreach ($artifact in $MavenArtifact) {
        $parts = $artifact.Split(':', 4)
        if ($parts.Count -lt 3) {
            throw "Invalid Maven artifact '$artifact'. Expected group:artifact:version[:filename]."
        }
        $group = $parts[0].Replace('.', '/')
        $name = $parts[1]
        $version = $parts[2]
        $files = if ($parts.Count -eq 4) {
            @($parts[3])
        } else {
            @("$name-$version.pom", "$name-$version.jar", "$name-$version.aar", "$name-$version.module")
        }
        foreach ($file in $files) {
            "$feedBaseUrl/$group/$name/$version/$file"
        }
    }
    Invoke-MirrorUrls $urls
    return
}

$project = Resolve-Path (Join-Path $repoRoot $ProjectDir) | Select-Object -ExpandProperty Path
$wrapper = Join-Path $project $(if ($IsWindows -or $env:OS -eq 'Windows_NT') { 'gradlew.bat' } else { 'gradlew' })
$wrapper = Resolve-Path $wrapper | Select-Object -ExpandProperty Path
$env:RUNNINGONCI = 'true'
if ($AndroidHome) {
    $env:ANDROID_HOME = $AndroidHome
}

Push-Location $project
$logs = @()
try {
    for ($iteration = 1; $iteration -le $MaxIterations; $iteration++) {
        $log = Join-Path ([IO.Path]::GetTempPath()) "gradle-mirror-$PID-$iteration.log"
        $logs += $log
        & $wrapper $Task --no-daemon --refresh-dependencies *>&1 | Tee-Object -FilePath $log | Out-Null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Gradle succeeded after $iteration iteration(s)."
            return
        }

        $urls = Select-String -Path $log -Pattern "Could not (?:GET|HEAD) '(https://pkgs\.dev\.azure\.com/dnceng/[^']+)'" -AllMatches |
            ForEach-Object { $_.Matches.Groups[1].Value } |
            Sort-Object -Unique
        if ($urls.Count -eq 0) {
            Get-Content $log -Tail 30
            throw "Gradle failed without any mirror URLs to seed."
        }
        Invoke-MirrorUrls $urls
    }
    throw "Gradle did not succeed after $MaxIterations iterations."
}
finally {
    Pop-Location
    Remove-Item $logs -ErrorAction SilentlyContinue
}
