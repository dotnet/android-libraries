# NuGet Package Analyzer
# This script analyzes all NuGet packages from config.json and generates a report

param(
    [switch]$Test,
    [switch]$Help
)

if ($Help) {
    Write-Host "NuGet Package Analyzer" -ForegroundColor Green
    Write-Host "=====================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Usage:"
    Write-Host "  .\run-analysis.ps1        - Analyze all 600+ packages (takes 10-15 minutes)"
    Write-Host "  .\run-analysis.ps1 -Test  - Test mode: analyze first 5 packages only"
    Write-Host "  .\run-analysis.ps1 -Help  - Show this help"
    Write-Host ""
    Write-Host "Output:"
    Write-Host "  package-analysis.md       - Full analysis report"
    Write-Host "  package-analysis-test.md  - Test mode report"
    exit 0
}

Write-Host "NuGet Package Analyzer" -ForegroundColor Green
Write-Host "=====================" -ForegroundColor Green

# Change to the tools/package-info directory
Set-Location (Split-Path $MyInvocation.MyCommand.Path)

# Check if we're in the right directory
if (-not (Test-Path "../../config.json")) {
    Write-Error "config.json not found. Please run this script from util/package-info directory."
    exit 1
}

Write-Host "Building the analyzer..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to build the analyzer"
    exit 1
}

if ($Test) {
    Write-Host "Running in TEST MODE - analyzing first 5 packages only..." -ForegroundColor Cyan
    dotnet run -- --test
    $outputFile = "package-analysis-test.md"
} else {
    Write-Host "Running FULL ANALYSIS of 600+ packages..." -ForegroundColor Yellow
    Write-Host "This will take approximately 10-15 minutes..." -ForegroundColor Cyan
    Write-Host "Progress will be displayed every 10 packages processed." -ForegroundColor Cyan
    dotnet run
    $outputFile = "package-analysis.md"
}

if ($LASTEXITCODE -eq 0) {
    Write-Host "Analysis complete!" -ForegroundColor Green
    Write-Host "Results saved to: $outputFile" -ForegroundColor Green
    
    if (Test-Path $outputFile) {
        $fileSize = (Get-Item $outputFile).Length
        Write-Host "Report file size: $([math]::Round($fileSize / 1KB, 1)) KB" -ForegroundColor Cyan
        
        # Count number of packages analyzed
        $content = Get-Content $outputFile
        $packageCount = ($content | Select-String -Pattern "^\| \[").Count
        Write-Host "Packages in report: $packageCount" -ForegroundColor Cyan
    }
} else {
    Write-Error "Analysis failed"
    exit 1
}
