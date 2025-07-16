# NuGet Package Analyzer

This tool analyzes all NuGet packages defined in the `config.json` file and generates a comprehensive report with download statistics and usage information from NuGet.org.

## Quick Start

```powershell
# Test with first 5 packages (recommended first)
.\run-analysis.ps1 -Test

# Full analysis of all 600+ packages (takes 10-15 minutes)
.\run-analysis.ps1

# Show help
.\run-analysis.ps1 -Help
```

## Features

- ✅ Extracts all NuGet package IDs from `config.json` (658 packages)
- ✅ Fetches download counts from NuGet.org (e.g., "52.9M", "1.2K")
- ✅ Extracts "Used By" statistics (number of dependent packages)
- ✅ Generates single table sorted by download count (most popular first)
- ✅ Provides summary statistics and progress reporting
- ✅ **Intelligent retry logic** with exponential backoff for rate limiting
- ✅ **Respects NuGet.org rate limits** with automatic retry after 429 errors
- ✅ Handles concurrent request management (max 3 concurrent)
- ✅ Test mode for quick verification

## Sample Output

| Package ID | Version | NuGet Version | Downloads | Used By | Package URL |
|------------|---------|---------------|-----------|---------|-------------|
| [Xamarin.AndroidX.Activity](https://www.nuget.org/packages/Xamarin.AndroidX.Activity) | 1.10.1 | 1.10.1.2 | 52.9M | 89 | https://www.nuget.org/packages/Xamarin.AndroidX.Activity |
| [Xamarin.AndroidX.Activity.Compose](https://www.nuget.org/packages/Xamarin.AndroidX.Activity.Compose) | 1.10.1 | 1.10.1.2 | 224.0K | 29 | https://www.nuget.org/packages/Xamarin.AndroidX.Activity.Compose |

## Usage

### Prerequisites

- .NET 9.0 SDK (or compatible)
- Internet connection to fetch data from NuGet.org
- PowerShell (for the convenience script)

### Running the Analysis

1. **Quick Test** (recommended first):

   ```powershell
   cd util/package-info
   .\run-analysis.ps1 -Test
   ```

   This analyzes the first 5 packages in about 10 seconds.

2. **Full Analysis**:

   ```powershell
   cd util/package-info
   .\run-analysis.ps1
   ```

   This analyzes all 658 packages and takes 10-15 minutes.

3. **Manual Execution**:

   ```powershell
   cd util/package-info
   dotnet build
   dotnet run              # Full analysis
   dotnet run -- --test    # Test mode
   ```

### Output Files

- **`package-analysis.md`**: Full analysis report (~500KB)
- **`package-analysis-test.md`**: Test mode report (~5KB)

## Report Structure

The generated report includes:

### 1. Package Tables by Family

- **AndroidX packages**: Activity, AppCompat, Camera, Compose, etc.
- **Firebase packages**: Auth, Database, Analytics, etc.
- **Google Play Services**: Ads, Maps, Auth, etc.
- **ML Kit packages**: Vision, Translation, etc.

### 2. Package Information

- **Package ID**: With direct link to NuGet.org
- **Version**: Maven artifact version
- **NuGet Version**: .NET package version (includes revision number)
- **Downloads**: Total download count (e.g., "52.9M", "1.2K")
- **Used By**: Number of packages that depend on this package
- **Package URL**: Direct link to package page

### 3. Summary Statistics

- Total packages analyzed
- Packages with download statistics
- Total estimated downloads
- Average downloads per package
- Analysis timestamp

## Technical Details

### How It Works

1. **Config Parsing**: Reads `config.json` and extracts all artifacts with `nugetId` fields
2. **Web Scraping**: For each package, fetches the NuGet.org page and extracts:
   - Download count using regex: `Total\s+([\d,.]+[KMB]?)`
   - "Used By" count using regex: `NuGet packages[^\d]*\((\d+)\)`
3. **Rate Limit Handling**:
   - Detects HTTP 429 (Too Many Requests) responses
   - Implements exponential backoff retry strategy (2s, 4s, 8s delays)
   - Respects `Retry-After` headers when provided by NuGet.org
   - Falls back to "Rate Limited" status after 3 failed attempts
4. **Data Processing**: Cleans and formats the extracted data
5. **Report Generation**: Creates single table sorted by download count (highest first)
6. **Concurrent Control**: Uses semaphore to limit concurrent requests (max 3)

### Performance

- **Test Mode**: 5 packages in ~15 seconds (with retry logic)
- **Full Mode**: 658 packages in ~15-25 minutes (with automatic retries)
- **Concurrent Requests**: Limited to 3 to respect NuGet.org rate limits
- **Request Delay**: 300ms between requests (1000ms after retries)
- **Retry Logic**: Up to 3 retries with exponential backoff for rate limiting
- **Rate Limit Handling**: Automatic detection and respect for HTTP 429 responses

## Troubleshooting

### Common Issues

- **Build errors**: Ensure .NET 9.0 SDK is installed
- **Network errors**: Check internet connection and try again
- **Missing data**: Some packages may not have complete statistics on NuGet.org
- **Rate limiting**: The tool automatically handles this, but very slow responses may indicate temporary NuGet.org issues

### Package Analysis Issues

- **"N/A" Downloads**: Package may be new, unlisted, or statistics not available
- **"N/A" Used By**: Package may not have dependencies or the Used By section may be empty
- **HTTP errors**: Network issues or temporary NuGet.org problems

### Performance Optimization

- Test mode is recommended for initial testing
- The tool uses optimized regex patterns for better performance
- Progress reporting every 10 packages in full mode
- Automatic cleanup of statistics formatting

## Development

### Project Structure

```text
util/package-info/
├── PackageAnalyzer.csproj  # Project file with dependencies
├── Program.cs              # Main analysis logic
├── run-analysis.ps1        # PowerShell convenience script
└── README.md              # This file
```

### Dependencies

- **HtmlAgilityPack**: For HTML parsing
- **Newtonsoft.Json**: For JSON serialization (alternative to System.Text.Json)
- **.NET 9.0**: Target framework

### Extending the Tool

To modify the analysis:

1. **Add new data extraction**: Update the regex patterns in `FetchPackageInfo()`
2. **Change report format**: Modify `GenerateMarkdownTables()`
3. **Add new package families**: Update `GetPackagePrefix()` logic
4. **Improve error handling**: Enhance exception handling in the fetch methods
