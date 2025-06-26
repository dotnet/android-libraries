using System.Text.Json;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace PackageAnalyzer;

public class PackageInfo
{
    public string NugetId { get; set; } = "";
    public string Version { get; set; } = "";
    public string NugetVersion { get; set; } = "";
    public string? Downloads { get; set; }
    public string? UsedBy { get; set; }
    public string PackageUrl { get; set; } = "";
}

public class ConfigArtifact
{
    public string groupId { get; set; } = "";
    public string artifactId { get; set; } = "";
    public string version { get; set; } = "";
    public string nugetVersion { get; set; } = "";
    public string nugetId { get; set; } = "";
    public bool dependencyOnly { get; set; }
}

public class Config
{
    public string slnFile { get; set; } = "";
    public bool strictRuntimeDependencies { get; set; }
    public List<string> additionalProjects { get; set; } = new();
    public List<ConfigArtifact> artifacts { get; set; } = new();
}

class Program
{
    private static readonly HttpClient httpClient = new();
    
    static async Task Main(string[] args)
    {
        bool testMode = args.Length > 0 && args[0] == "--test";
        
        // Read and parse config.json
        var configPath = Path.Combine("..", "..", "config.json");
        if (!File.Exists(configPath))
        {
            Console.WriteLine("config.json not found. Please run from tools/package-info directory.");
            return;
        }

        var configJson = await File.ReadAllTextAsync(configPath);
        var configs = JsonSerializer.Deserialize<List<Config>>(configJson, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        if (configs == null || configs.Count == 0 || configs[0].artifacts == null)
        {
            Console.WriteLine("Could not parse config.json or no artifacts found");
            return;
        }

        var config = configs[0]; // Take the first config object

        var artifactsToProcess = testMode ? config.artifacts.Take(5).ToList() : config.artifacts;
        
        Console.WriteLine($"Found {config.artifacts.Count} packages in config.json");
        if (testMode)
        {
            Console.WriteLine($"Running in test mode - processing first {artifactsToProcess.Count} packages only");
        }
        Console.WriteLine("Fetching package information from NuGet.org...\n");

        var packages = new List<PackageInfo>();
        var semaphore = new SemaphoreSlim(testMode ? 3 : 5); // Limit concurrent requests

        var tasks = artifactsToProcess.Select(async artifact =>
        {
            await semaphore.WaitAsync();
            try
            {
                var packageInfo = await FetchPackageInfo(artifact, testMode);
                lock (packages)
                {
                    packages.Add(packageInfo);
                    if (testMode)
                    {
                        Console.WriteLine($"Processed {packages.Count}/{artifactsToProcess.Count}: {packageInfo.NugetId}");
                        Console.WriteLine($"  Downloads: {packageInfo.Downloads}, Used By: {packageInfo.UsedBy}");
                    }
                    else if (packages.Count % 10 == 0)
                    {
                        Console.WriteLine($"Processed {packages.Count}/{artifactsToProcess.Count} packages... ({packages.Count * 100.0 / artifactsToProcess.Count:F1}%)");
                    }
                }
                return packageInfo;
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);

        // Generate markdown tables
        var outputFile = testMode ? "package-analysis-test.md" : "package-analysis.md";
        await GenerateMarkdownTables(packages, outputFile);
        
        Console.WriteLine($"\nCompleted! Generated {outputFile} with information for {packages.Count} packages.");
    }

    static async Task<PackageInfo> FetchPackageInfo(ConfigArtifact artifact, bool testMode = false)
    {
        var packageInfo = new PackageInfo
        {
            NugetId = artifact.nugetId,
            Version = artifact.version,
            NugetVersion = artifact.nugetVersion,
            PackageUrl = $"https://www.nuget.org/packages/{artifact.nugetId}"
        };

        try
        {
            if (!testMode)
            {
                Console.WriteLine($"  Fetching: {packageInfo.PackageUrl}");
            }
            var response = await httpClient.GetAsync(packageInfo.PackageUrl);
            if (response.IsSuccessStatusCode)
            {
                var html = await response.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Extract download count - try multiple selectors
                string? downloads = null;
                
                // Look for "Total X.XM" pattern in the text
                var allText = doc.DocumentNode.InnerText;
                
                // Handle multiline text with potential whitespace
                var downloadMatch = Regex.Match(allText, @"Total\s+([\d,.]+[KMB]?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (downloadMatch.Success)
                {
                    downloads = downloadMatch.Groups[1].Value;
                }

                packageInfo.Downloads = downloads ?? "N/A";

                // Extract "Used by" count - look for "NuGet packages (89)" pattern
                string? usedBy = null;
                
                // Look for "NuGet packages (X)" pattern in the all text
                var usedByMatch = Regex.Match(allText, @"NuGet packages[^\d]*\((\d+)\)", RegexOptions.IgnoreCase);
                if (usedByMatch.Success)
                {
                    usedBy = usedByMatch.Groups[1].Value;
                }
                
                packageInfo.UsedBy = usedBy ?? "N/A";

                // Clean up the values
                packageInfo.Downloads = CleanStatValue(packageInfo.Downloads);
                packageInfo.UsedBy = CleanStatValue(packageInfo.UsedBy);

                // Add a small delay to be respectful
                await Task.Delay(testMode ? 500 : 200);
            }
            else
            {
                packageInfo.Downloads = $"HTTP {response.StatusCode}";
                packageInfo.UsedBy = $"HTTP {response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            packageInfo.Downloads = $"Error: {ex.Message}";
            packageInfo.UsedBy = $"Error: {ex.Message}";
        }

        return packageInfo;
    }

    static string CleanStatValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "N/A";

        // Remove extra whitespace and newlines
        value = Regex.Replace(value, @"\s+", " ").Trim();
        
        // If it already looks like a formatted number (e.g., "52.9M", "1,234"), keep it as is
        if (Regex.IsMatch(value, @"^[\d,]+(\.\d+)?[KMB]?$", RegexOptions.IgnoreCase))
            return value;
        
        // Extract numbers from text like "1,234 downloads"
        var numberMatch = Regex.Match(value, @"([\d,]+(\.\d+)?[KMB]?)", RegexOptions.IgnoreCase);
        if (numberMatch.Success)
            return numberMatch.Groups[1].Value;

        return value;
    }

    static async Task GenerateMarkdownTables(List<PackageInfo> packages, string outputFile)
    {
        var markdown = new System.Text.StringBuilder();
        
        markdown.AppendLine("# NuGet Package Analysis");
        markdown.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        markdown.AppendLine($"Total packages analyzed: {packages.Count}");
        markdown.AppendLine();

        // Group packages by prefix for better organization
        var groupedPackages = packages
            .GroupBy(p => GetPackagePrefix(p.NugetId))
            .OrderBy(g => g.Key);

        foreach (var group in groupedPackages)
        {
            markdown.AppendLine($"## {group.Key}");
            markdown.AppendLine();
            markdown.AppendLine("| Package ID | Version | NuGet Version | Downloads | Used By | Package URL |");
            markdown.AppendLine("|------------|---------|---------------|-----------|---------|-------------|");

            foreach (var package in group.OrderBy(p => p.NugetId))
            {
                var escapedId = package.NugetId.Replace("|", "\\|");
                var packageLink = $"[{escapedId}]({package.PackageUrl})";
                
                markdown.AppendLine($"| {packageLink} | {package.Version} | {package.NugetVersion} | {package.Downloads} | {package.UsedBy} | {package.PackageUrl} |");
            }
            
            markdown.AppendLine();
        }

        // Add summary statistics
        markdown.AppendLine("## Summary Statistics");
        markdown.AppendLine();
        
        var validDownloads = packages
            .Select(p => p.Downloads?.Replace(",", ""))
            .Where(d => !string.IsNullOrEmpty(d) && long.TryParse(d, out _))
            .Select(d => long.Parse(d!))
            .ToList();
            
        var totalDownloads = validDownloads.Sum();
        var packagesWithStats = packages.Count(p => p.Downloads != "N/A" && !p.Downloads!.Contains("Error"));
        
        markdown.AppendLine($"- **Total packages analyzed:** {packages.Count}");
        markdown.AppendLine($"- **Packages with download stats:** {packagesWithStats}");
        markdown.AppendLine($"- **Total downloads (estimated):** {totalDownloads:N0}");
        markdown.AppendLine($"- **Average downloads per package:** {(validDownloads.Any() ? validDownloads.Average() : 0):N0}");
        markdown.AppendLine($"- **Analysis date:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        await File.WriteAllTextAsync(outputFile, markdown.ToString());
    }

    static string GetPackagePrefix(string packageId)
    {
        var parts = packageId.Split('.');
        if (parts.Length >= 3)
        {
            return string.Join(".", parts.Take(3)); // e.g., "Xamarin.AndroidX.Activity"
        }
        return packageId;
    }
}
