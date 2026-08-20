using NuGet.Versioning;

namespace SignedPackagePublisher;

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
	string Version);
