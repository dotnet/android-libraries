#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Seeds Maven dependencies into the dnceng dotnet-public-maven feed.

.DESCRIPTION
    CI reads the feed anonymously. New upstream artifacts return 401 until an
    authenticated developer request causes Azure Artifacts to cache them.
    Run `az login`, then use this script for a Gradle project or explicit Maven
    coordinates, or -PullRequest to audit an update PR without checking it out.
    Tokens are sent only in the Authorization header and are not written to output.

.EXAMPLE
    pwsh ./build/gradle/mirror-dependencies.ps1 `
        -ProjectDir source/com.google.android.play/asset.delivery.extensions `
        -Task build

.EXAMPLE
    pwsh ./build/gradle/mirror-dependencies.ps1 `
        -MavenArtifact 'com.google.code.gson:gson:2.11.0'

.EXAMPLE
    pwsh ./build/gradle/mirror-dependencies.ps1 -PullRequest 1572 -ReportPath ./mirror-report.json
#>
[CmdletBinding(DefaultParameterSetName = 'Gradle')]
param (
    [Parameter(Mandatory, ParameterSetName = 'Gradle')]
    [string] $ProjectDir,

    [Parameter(Mandatory, ParameterSetName = 'Gradle')]
    [string] $Task,

    [Parameter(Mandatory, ParameterSetName = 'MavenArtifact')]
    [string[]] $MavenArtifact,

    [Parameter(Mandatory, ParameterSetName = 'PullRequest')]
    [ValidateRange(1, [int]::MaxValue)]
    [int] $PullRequest,

    [Parameter(ParameterSetName = 'PullRequest')]
    [string] $Repository = 'dotnet/android-libraries',

    [Parameter(ParameterSetName = 'PullRequest')]
    [string[]] $CiLogPath = @(),

    [Parameter(ParameterSetName = 'PullRequest')]
    [int[]] $AzureBuildId = @(),

    [Parameter(ParameterSetName = 'PullRequest')]
    [string] $ReportPath,

    [Parameter(ParameterSetName = 'Gradle')]
    [string] $AndroidHome = $env:ANDROID_HOME,

    [Parameter(ParameterSetName = 'Gradle')]
    [int] $MaxIterations = 15
)

$ErrorActionPreference = 'Stop'
$feedBaseUrl = 'https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-public-maven/maven/v1'
$azDevOpsResource = '499b84ac-1321-427f-aa17-267ca6975798'
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '../..') | Select-Object -ExpandProperty Path
$anonymousSession = [Microsoft.PowerShell.Commands.WebRequestSession]::new()
$authenticatedSession = [Microsoft.PowerShell.Commands.WebRequestSession]::new()

function Get-AzDevOpsToken {
    $token = az account get-access-token --resource $azDevOpsResource --query accessToken -o tsv 2>$null
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrEmpty($token)) {
        throw "Could not get an Azure DevOps access token. Run 'az login' first."
    }
    $token
}

function Get-MirrorHeaders {
    $credential = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(":$(Get-AzDevOpsToken)"))
    @{ Authorization = "Basic $credential" }
}

function Invoke-MirrorRequest([string] $Url, [hashtable] $Headers = @{}, [switch] $WaitForAnonymous) {
    # Reuse connections without allowing authenticated session state into anonymous checks.
    $session = if ($Headers.ContainsKey('Authorization')) { $authenticatedSession } else { $anonymousSession }
    $refreshedToken = $false
    for ($attempt = 0; $attempt -lt 4; $attempt++) {
        try {
            # Use GET, not HEAD: verify that the payload can actually be downloaded.
            $response = Invoke-WebRequest -Uri $Url -Headers $Headers -WebSession $session -SkipHttpErrorCheck -TimeoutSec 120
            $status = [int] $response.StatusCode
        }
        catch [System.Net.Http.HttpRequestException], [System.Threading.Tasks.TaskCanceledException] {
            if ($attempt -eq 3) { throw "Download failed after retries: $Url" }
            Start-Sleep -Seconds ($attempt + 1)
            continue
        }
        if ($status -eq 401 -and $Headers.ContainsKey('Authorization') -and !$refreshedToken -and $attempt -lt 3) {
            $Headers.Authorization = (Get-MirrorHeaders).Authorization
            $refreshedToken = $true
            continue
        }
        if ($attempt -lt 3 -and ($status -in 408, 429 -or $status -ge 500 -or ($WaitForAnonymous -and $status -in 401, 403, 404))) {
            Start-Sleep -Seconds ($attempt + 1)
            continue
        }
        return $response
    }
}

function Invoke-MirrorUrls([string[]] $Urls) {
    $headers = Get-MirrorHeaders
    foreach ($url in $Urls | Sort-Object -Unique) {
        $response = Invoke-MirrorRequest $url $headers
        Write-Host "$($response.StatusCode) $url"
        # Explicit/Gradle mode can probe optional files, so only 404 is non-fatal.
        if ([int] $response.StatusCode -eq 404) { continue }
        if ([int] $response.StatusCode -ne 200) { throw "Authenticated mirror request failed: $url ($($response.StatusCode))" }
        $anonymous = Invoke-MirrorRequest $url -WaitForAnonymous
        if ([int] $anonymous.StatusCode -ne 200) { throw "Anonymous verification failed: $url ($($anonymous.StatusCode))" }
    }
}

function Invoke-Gh([string[]] $Arguments) {
    $result = & gh @Arguments
    if ($LASTEXITCODE -ne 0) { throw "GitHub discovery failed: gh $($Arguments -join ' ')" }
    return ($result -join "`n")
}

function Save-AzureFailureLogs([uri] $BuildUrl, [int] $BuildId, [string] $Directory) {
    if ($BuildUrl.Host -ne 'dev.azure.com') { throw "Unsupported Azure build host: $($BuildUrl.Host)" }
    $parts = $BuildUrl.AbsolutePath.Trim('/').Split('/')
    if ($parts.Count -lt 3) { throw "Invalid Azure build URL: $BuildUrl" }
    $organization = "https://dev.azure.com/$($parts[0])"
    $project = $parts[1]
    $routes = @("project=$project", "buildId=$BuildId")
    $json = & az devops invoke --organization $organization --area build --resource timeline --route-parameters @routes --api-version 7.1 -o json --only-show-errors
    if ($LASTEXITCODE -ne 0) { throw "Could not list Azure build $BuildId failures" }
    $timeline = ($json -join "`n") | ConvertFrom-Json
    $failures = @($timeline.records | Where-Object { $_.type -eq 'Task' -and $_.result -in 'failed', 'canceled', 'abandoned' })
    if ($failures.Count -eq 0) { throw "Azure build $BuildId has no downloadable failed task logs" }
    foreach ($failure in $failures) {
        if (!$failure.log.id) { throw "Azure build $BuildId task '$($failure.name)' has no log" }
        $path = Join-Path $Directory "azure-$BuildId-$($failure.log.id).log"
        & az devops invoke --organization $organization --area build --resource logs --route-parameters @routes "logId=$($failure.log.id)" `
            --api-version 7.1 --accept-media-type text/plain --out-file $path -o none
        if ($LASTEXITCODE -ne 0 -or !(Test-Path $path) -or (Get-Item $path).Length -eq 0) {
            throw "Could not download Azure build $BuildId log $($failure.log.id)"
        }
        $path
    }
}

function Invoke-PullRequestMirror {
    $directory = Join-Path ([IO.Path]::GetTempPath()) "pr-maven-mirror-$([Guid]::NewGuid().ToString('N'))"
    New-Item -ItemType Directory -Path $directory | Out-Null
    $errors = [Collections.Generic.List[string]]::new()
    $ciSources = [Collections.Generic.List[string]]::new()
    if (!$ReportPath) { $ReportPath = Join-Path ([IO.Path]::GetTempPath()) "pr-$PullRequest-maven-mirror.json" }
    $ReportPath = [IO.Path]::GetFullPath($ReportPath)
    $pr = $null
    $plan = $null
    try {
        $pr = Invoke-Gh @('api', "repos/$Repository/pulls/$PullRequest") | ConvertFrom-Json
        $mergeBase = Invoke-Gh @('api', "repos/$Repository/compare/$($pr.base.sha)...$($pr.head.sha)", '--jq', '.merge_base_commit.sha')
        if ($mergeBase -notmatch '^[a-f0-9]{40}$') { throw 'Could not discover the PR merge base.' }
        $checks = Invoke-Gh @('pr', 'view', "$PullRequest", '--repo', $Repository, '--json', 'statusCheckRollup', '--jq', '.statusCheckRollup') | ConvertFrom-Json
        foreach ($ref in @(@('head', $pr.head.sha), @('base', $mergeBase))) {
            $text = Invoke-Gh @('api', "repos/$Repository/contents/config.json?ref=$($ref[1])", '-H', 'Accept: application/vnd.github.raw+json')
            Set-Content -LiteralPath (Join-Path $directory "$($ref[0]).json") -Value $text -Encoding utf8
        }
        $logs = [Collections.Generic.List[string]]::new()
        foreach ($path in $CiLogPath) {
            $logs.Add((Resolve-Path -LiteralPath $path).Path)
            $ciSources.Add("local log: $path")
        }
        $azureUrls = @{}
        foreach ($check in $checks) {
            $state = if ($check.state) { $check.state } else { $check.conclusion }
            if ($state -in 'SUCCESS', 'NEUTRAL', 'SKIPPED') { continue }
            $url = if ($check.targetUrl) { $check.targetUrl } else { $check.detailsUrl }
            if ($state -in 'PENDING', '' -or !$state) {
                $errors.Add("CI discovery incomplete: check still pending ($url)")
                continue
            }
            try {
                if ($url -match '^https://dev\.azure\.com/[^/]+/[^/]+/_build/results\?buildId=(\d+)') {
                    $azureUrls[[int] $Matches[1]] = $url
                }
                elseif ($url -match '^https://github\.com/[^/]+/[^/]+/actions/runs/(\d+)') {
                    $run = $Matches[1]
                    $log = Invoke-Gh @('run', 'view', $run, '--repo', $Repository, '--log-failed')
                    if (!$log) { throw "No failure logs returned for run $run" }
                    $path = Join-Path $directory "github-$run.log"
                    Set-Content -LiteralPath $path -Value $log
                    $logs.Add($path)
                    $ciSources.Add($url)
                }
                else { throw "Unsupported failed check log source: $url" }
            }
            catch { $errors.Add($_.Exception.Message) }
        }
        foreach ($id in $AzureBuildId) {
            $azureUrls[$id] = "https://dev.azure.com/dnceng-public/public/_build/results?buildId=$id"
        }
        foreach ($entry in $azureUrls.GetEnumerator()) {
            try {
                foreach ($path in @(Save-AzureFailureLogs ([uri] $entry.Value) $entry.Key $directory)) { $logs.Add($path) }
                $ciSources.Add($entry.Value)
            }
            catch { $errors.Add($_.Exception.Message) }
        }
        $ciLog = Join-Path $directory 'ci.log'
        Set-Content -LiteralPath $ciLog -Value (($logs | ForEach-Object { Get-Content -LiteralPath $_ -Raw }) -join "`n")
        Write-Host "Discovering PR diff, latest/held candidates and dependency closure for $($pr.head.sha)..."
        $head = Get-Content -LiteralPath (Join-Path $directory 'head.json') -Raw | ConvertFrom-Json
        $baseline = Get-Content -LiteralPath (Join-Path $directory 'base.json') -Raw | ConvertFrom-Json
        $scratchConfig = Join-Path $directory 'update.json'
        Copy-Item -LiteralPath (Join-Path $directory 'head.json') -Destination $scratchConfig
        $updaterLog = Join-Path $directory 'update.log'
        & dotnet run --project (Join-Path $repoRoot 'util/Xamarin.AndroidBinderator/Xamarin.AndroidBinderator.Tool') -- `
            update --config-file $scratchConfig *> $updaterLog
        if ($LASTEXITCODE -ne 0) {
            Get-Content -LiteralPath $updaterLog -Tail 20 | Write-Host
            $errors.Add('Binderator update discovery failed; see BUILDING.md for tool prerequisites.')
        }
        Add-Type -Path (Join-Path $repoRoot 'tools/Microsoft.Android.Sdk.Windows.35.0.61/tools/Java.Interop.Tools.Maven.dll')
        . (Join-Path $PSScriptRoot 'maven-mirror-plan.ps1')
        $plan = New-MavenMirrorPlan $head[0] $baseline[0] (Get-Content -LiteralPath $updaterLog -Raw) (Get-Content -LiteralPath $ciLog -Raw)
        Write-Host "Verifying $(@($plan.Artifacts).Count) artifact/parent/BOM versions anonymously..."
        foreach ($errorMessage in $plan.Errors) { $errors.Add($errorMessage) }
        $headers = Get-MirrorHeaders
        $index = 0
        foreach ($artifact in $plan.Artifacts) {
            Write-Progress -Activity 'Verifying Maven feed' -Status $artifact.Coordinate -PercentComplete (100 * $index++ / $plan.Artifacts.Count)
            foreach ($file in $artifact.Files) {
                $url = "$feedBaseUrl/$($file.Path)"
                try {
                    $anonymous = Invoke-MirrorRequest $url
                    if ([int] $anonymous.StatusCode -eq 200) {
                        $file.Status = 'already accessible'
                        continue
                    }
                    if ([int] $anonymous.StatusCode -notin 401, 403, 404) { throw "Anonymous HTTP $($anonymous.StatusCode): $url" }
                    $upstream = Invoke-MirrorRequest $file.Upstream
                    if ([int] $upstream.StatusCode -eq 404) {
                        $file.Status = 'unavailable upstream'
                        $errors.Add("Unavailable upstream: $($file.Upstream)")
                        continue
                    }
                    if ([int] $upstream.StatusCode -ne 200) { throw "Upstream HTTP $($upstream.StatusCode): $($file.Upstream)" }
                    $response = Invoke-MirrorRequest $url $headers
                    if ([int] $response.StatusCode -ne 200) { throw "Authenticated HTTP $($response.StatusCode): $url" }
                    $verified = Invoke-MirrorRequest $url -WaitForAnonymous
                    if ([int] $verified.StatusCode -ne 200) { throw "Anonymous verification HTTP $($verified.StatusCode): $url" }
                    $file.Status = 'newly mirrored'
                }
                catch {
                    $file.Status = 'unresolved'
                    $errors.Add($_.Exception.Message)
                }
            }
        }
        Write-Progress -Activity 'Verifying Maven feed' -Completed
        $current = Invoke-Gh @('api', "repos/$Repository/pulls/$PullRequest", '--jq', '.head.sha')
        if ($current -ne $pr.head.sha) { $errors.Add('PR head changed during audit; rerun against the new head.') }
    }
    catch { $errors.Add($_.Exception.Message) }
    finally {
        Remove-Item -LiteralPath $directory -Recurse -Force
    }
    foreach ($file in $plan.Artifacts.Files | Where-Object Status -eq 'pending') { $file.Status = 'unresolved' }
    $report = [ordered]@{
        PullRequest = if ($pr) { $pr.html_url } else { "$Repository#$PullRequest" }
        Head = $pr.head.sha
        Base = $mergeBase
        CiSources = @($ciSources)
        Scope = if ($plan) { $plan.Scope } else { 'Discovery did not complete; no availability claim.' }
        Complete = $errors.Count -eq 0
        Disclaimer = 'Feed availability does not mean CI is fixed.'
        Artifacts = @($plan.Artifacts | Where-Object { $null -ne $_ })
        MissingMappings = @($plan.MissingMappings | Where-Object { $null -ne $_ })
        Errors = @($errors)
    }
    $report | ConvertTo-Json -Depth 30 | Set-Content -LiteralPath $ReportPath -Encoding utf8
    Write-Host "Report: $ReportPath"
    $plan.Artifacts.Files | Group-Object Status | ForEach-Object { Write-Host "$($_.Name): $($_.Count) file(s)" }
    Write-Host "Missing binding/configuration mappings: $($report.MissingMappings.Count)"
    Write-Host $report.Disclaimer
    if ($errors.Count -gt 0) { throw "Mirror audit incomplete ($($errors.Count) error(s)); see $ReportPath" }
}

if ($PSCmdlet.ParameterSetName -eq 'PullRequest') {
    Invoke-PullRequestMirror
    return
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
