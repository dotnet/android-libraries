namespace SignedPackagePublisher;

public static class Program
{
	public static async Task<int> Main(string[] args)
	{
		try
		{
			Options options = Options.Parse(args);
			IReadOnlyList<SignedPackage> packages = await PackageInspector.InspectDirectoryAsync(
				options.PackagesDirectory,
				CancellationToken.None);
			await PublishingOutput.WriteInventoryAsync(
				options.InventoryPath,
				packages,
				CancellationToken.None);

			PublishingOutput.StagePackages(options.PackageOutputDirectory, packages);
			PublishingOutput.WriteManifest(options.ManifestPath, options.ManifestIdentity, packages);
			Console.WriteLine($"Inspected and prepared {packages.Count} signed packages.");
			return 0;
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine(exception);
			return 1;
		}
	}
}

public sealed record Options(
	string PackagesDirectory,
	string PackageOutputDirectory,
	string InventoryPath,
	string ManifestPath,
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

		return new Options(
			Required("packages"),
			Required("package-output"),
			Required("inventory"),
			Required("manifest"),
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
