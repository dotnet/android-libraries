param(
	[Parameter(Mandatory = $true)][int] $BuildId,
	[Parameter(Mandatory = $true)][string] $ChannelName,
	[Parameter(Mandatory = $true)][int] $ExpectedChannelId,
	[Parameter(Mandatory = $true)][string] $AzdoToken,
	[Parameter(Mandatory = $false)][string] $MaestroApiEndpoint = 'https://maestro.dot.net'
)

$ErrorActionPreference = 'Stop'

try {
	$ci = $true
	$disableConfigureToolsetImport = $true
	. $PSScriptRoot\..\..\eng\common\tools.ps1

	$darc = Get-Darc
	$channelsJson = & $darc get-channels `
		--output-format json `
		--bar-uri $MaestroApiEndpoint `
		--ci
	if ($LastExitCode -ne 0) {
		throw "Darc could not list BAR channels."
	}

	$channel = @($channelsJson | ConvertFrom-Json) |
		Where-Object { $_.name -eq $ChannelName }
	if ($channel.Count -ne 1 -or $channel[0].id -ne $ExpectedChannelId) {
		throw "BAR channel '$ChannelName' must resolve uniquely to id $ExpectedChannelId."
	}

	& $darc add-build-to-channel `
		--id $BuildId `
		--channel $ChannelName `
		--publishing-infra-version 3 `
		--source-branch main `
		--azdev-pat $AzdoToken `
		--bar-uri $MaestroApiEndpoint `
		--ci `
		--verbose
	if ($LastExitCode -ne 0) {
		throw "Darc failed to promote BAR build $BuildId to '$ChannelName'."
	}
}
catch {
	Write-Host $_
	Write-PipelineTelemetryError -Category 'PromoteBuild' -Message "Failed to promote BAR build '$BuildId' to '$ChannelName'."
	ExitWithExitCode 1
}
