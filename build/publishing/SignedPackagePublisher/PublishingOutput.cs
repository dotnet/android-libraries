using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Microsoft.DotNet.Build.Manifest;

namespace SignedPackagePublisher;

public sealed record ManifestIdentity(
	string RepositoryName,
	string BuildNumber,
	string Branch,
	string Commit,
	string AzureCollectionUri,
	string AzureProject,
	int AzureBuildId,
	int AzureDefinitionId,
	string AzureRepositoryUri);

public sealed record InventoryDocument(
	int SchemaVersion,
	IReadOnlyList<PackageInventoryEntry> Packages);

[JsonSourceGenerationOptions(
	WriteIndented = true,
	PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
	UseStringEnumConverter = true)]
[JsonSerializable(typeof(InventoryDocument))]
internal sealed partial class InventoryJsonContext : JsonSerializerContext;

public static class PublishingOutput
{
	public static async Task WriteInventoryAsync(
		string path,
		PublishingPlan plan,
		CancellationToken cancellationToken)
	{
		EnsureParentDirectory(path);
		var document = new InventoryDocument(2, plan.Inventory);
		await using FileStream stream = File.Create(path);
		await JsonSerializer.SerializeAsync(
			stream,
			document,
			InventoryJsonContext.Default.InventoryDocument,
			cancellationToken);
		await stream.WriteAsync("\n"u8.ToArray(), cancellationToken);
	}

	public static void StageIncludedPackages(string outputDirectory, IReadOnlyList<SignedPackage> packages)
	{
		Directory.CreateDirectory(outputDirectory);
		foreach (SignedPackage package in packages)
		{
			string destination = Path.Combine(
				outputDirectory,
				$"{package.Id}.{package.Version.ToNormalizedString()}.nupkg");
			File.Copy(package.SourcePath, destination, overwrite: false);
		}
	}

	public static void WriteManifest(
		string path,
		ManifestIdentity identity,
		IReadOnlyList<SignedPackage> packages)
	{
		var build = new BuildModel(new BuildIdentity {
			PublishingVersion = PublishingInfraVersion.V3,
			Name = identity.RepositoryName,
			BuildId = identity.BuildNumber,
			Branch = identity.Branch,
			Commit = identity.Commit,
			IsStable = true,
			IsReleaseOnlyPackageVersion = false,
			InitialAssetsLocation = $"{identity.AzureCollectionUri.TrimEnd('/')}/{identity.AzureProject}/_apis/build/builds/{identity.AzureBuildId}/artifacts",
			AzureDevOpsAccount = GetAzureDevOpsAccount(identity.AzureCollectionUri),
			AzureDevOpsProject = identity.AzureProject,
			AzureDevOpsBuildNumber = identity.BuildNumber,
			AzureDevOpsRepository = identity.AzureRepositoryUri,
			AzureDevOpsBranch = identity.Branch,
			AzureDevOpsBuildId = identity.AzureBuildId,
			AzureDevOpsBuildDefinitionId = identity.AzureDefinitionId,
		});

		foreach (SignedPackage package in packages)
		{
			var asset = new PackageArtifactModel {
				Id = package.Id,
				Version = package.Version.ToNormalizedString(),
				NonShipping = false,
			};
			asset.Attributes["Category"] = "Package";
			build.Artifacts.Packages.Add(asset);
		}

		EnsureParentDirectory(path);
		File.WriteAllText(path, build.ToXml().ToString(SaveOptions.DisableFormatting));
	}

	private static void EnsureParentDirectory(string path)
	{
		string? directory = Path.GetDirectoryName(Path.GetFullPath(path));
		if (directory is not null)
			Directory.CreateDirectory(directory);
	}

	private static string GetAzureDevOpsAccount(string collectionUri)
	{
		var uri = new Uri(collectionUri);
		if (uri.Host.Equals("dev.azure.com", StringComparison.OrdinalIgnoreCase))
			return uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries).First();

		return uri.Host.Split('.', StringSplitOptions.RemoveEmptyEntries).First();
	}
}
