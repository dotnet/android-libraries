using NuGet.Versioning;

namespace SignedPackagePublisher;

public enum PackageDecision
{
	Include,
	Exclude,
	Error,
}

public sealed record SignedPackage(
	string SourcePath,
	string RelativePath,
	string FileName,
	string Sha256,
	string Id,
	NuGetVersion Version);

public sealed record PackageInventoryEntry(
	string SourceFile,
	string FileName,
	string Sha256,
	string Id,
	string Version,
	PackageDecision Decision,
	string Reason,
	string? CanonicalSourceFile = null);

public sealed record PublishingPlan(
	IReadOnlyList<PackageInventoryEntry> Inventory,
	IReadOnlyList<SignedPackage> IncludedPackages)
{
	public bool HasErrors => Inventory.Any(entry => entry.Decision == PackageDecision.Error);
}

public enum FeedFailureKind
{
	Transient,
	Authentication,
	Unknown,
}

public sealed class FeedQueryException(string message, Exception innerException)
	: Exception(message, innerException);
