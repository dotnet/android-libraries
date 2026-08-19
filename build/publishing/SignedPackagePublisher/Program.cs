namespace SignedPackagePublisher;

public static class Program
{
	public static async Task<int> Main(string[] args)
	{
		try
		{
			Options options = Options.Parse(args);
			using var probe = new NuGetFeedProbe(
				options.Feed,
				GetOptionalEnvironmentVariable(options.FeedTokenEnvironmentVariable),
				options.MaxAttempts);

			IReadOnlyList<SignedPackage> packages = await PackageInspector.InspectDirectoryAsync(
				options.PackagesDirectory,
				CancellationToken.None);
			var planner = new PublishingPlanner(probe, options.MaxConcurrency);
			PublishingPlan plan = await planner.CreateAsync(packages, CancellationToken.None);
			await PublishingOutput.WriteInventoryAsync(
				options.InventoryPath,
				options.Feed,
				plan,
				CancellationToken.None);

			if (plan.HasErrors)
			{
				Console.Error.WriteLine("Package filtering failed. See the deterministic audit inventory for details.");
				return 2;
			}

			PublishingOutput.StageIncludedPackages(options.PackageOutputDirectory, plan.IncludedPackages);
			PublishingOutput.WriteManifest(options.ManifestPath, options.ManifestIdentity, plan.IncludedPackages);
			Console.WriteLine($"Inspected {packages.Count} signed packages; {plan.IncludedPackages.Count} are absent from the target feed.");
			Console.WriteLine($"##vso[task.setvariable variable=IncludedPackageCount]{plan.IncludedPackages.Count}");
			return 0;
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine(exception);
			return 1;
		}
	}

	private static string? GetOptionalEnvironmentVariable(string? name)
		=> string.IsNullOrWhiteSpace(name) ? null : Environment.GetEnvironmentVariable(name);
}

public sealed record Options(
	string PackagesDirectory,
	string PackageOutputDirectory,
	string InventoryPath,
	string ManifestPath,
	Uri Feed,
	string? FeedTokenEnvironmentVariable,
	int MaxConcurrency,
	int MaxAttempts,
	ManifestIdentity ManifestIdentity)
{
	public static Options Parse(string[] args)
	{
		var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		for (int index = 0; index < args.Length; index += 2)
		{
			if (index + 1 >= args.Length || !args[index].StartsWith("--", StringComparison.Ordinal))
				throw new ArgumentException("Arguments must be supplied as '--name value' pairs.");
			values.Add(args[index][2..], args[index + 1]);
		}

		string Required(string name)
			=> values.TryGetValue(name, out string? value) && !string.IsNullOrWhiteSpace(value)
				? value
				: throw new ArgumentException($"Missing required argument '--{name}'.");
		int RequiredInt(string name)
			=> int.TryParse(Required(name), out int value)
				? value
				: throw new ArgumentException($"Argument '--{name}' must be an integer.");
		int OptionalInt(string name, int fallback)
			=> values.TryGetValue(name, out string? value)
				? int.Parse(value, System.Globalization.CultureInfo.InvariantCulture)
				: fallback;

		return new Options(
			Required("packages"),
			Required("package-output"),
			Required("inventory"),
			Required("manifest"),
			new Uri(Required("feed"), UriKind.Absolute),
			values.GetValueOrDefault("feed-token-env"),
			OptionalInt("max-concurrency", 8),
			OptionalInt("max-attempts", 4),
			new ManifestIdentity(
				Required("repository-name"),
				Required("build-number"),
				Required("branch"),
				Required("commit"),
				Required("azure-collection-uri"),
				Required("azure-project"),
				RequiredInt("azure-build-id"),
				RequiredInt("azure-definition-id"),
				Required("azure-repository-uri")));
	}
}
