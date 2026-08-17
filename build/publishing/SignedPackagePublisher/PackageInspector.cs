using System.Security.Cryptography;
using NuGet.Packaging;

namespace SignedPackagePublisher;

public static class PackageInspector
{
	public static async Task<IReadOnlyList<SignedPackage>> InspectDirectoryAsync(
		string packagesDirectory,
		CancellationToken cancellationToken)
	{
		string root = Path.GetFullPath(packagesDirectory);
		if (!Directory.Exists(root))
			throw new DirectoryNotFoundException($"Signed package directory '{root}' does not exist.");

		string[] packagePaths = Directory.GetFiles(root, "*.nupkg", SearchOption.AllDirectories);
		if (packagePaths.Length == 0)
			throw new InvalidOperationException($"No signed .nupkg files were found in '{root}'.");

		var packages = new List<SignedPackage>(packagePaths.Length);
		foreach (string packagePath in packagePaths.Order(StringComparer.Ordinal))
		{
			cancellationToken.ThrowIfCancellationRequested();
			string hash = await ComputeSha256Async(packagePath, cancellationToken);

			using var reader = new PackageArchiveReader(packagePath);
			var identity = await reader.GetIdentityAsync(cancellationToken)
				?? throw new InvalidDataException($"Package '{packagePath}' has no NuGet identity.");

			packages.Add(new SignedPackage(
				packagePath,
				Path.GetRelativePath(root, packagePath).Replace('\\', '/'),
				Path.GetFileName(packagePath),
				hash,
				identity.Id,
				identity.Version));
		}

		return packages;
	}

	private static async Task<string> ComputeSha256Async(string path, CancellationToken cancellationToken)
	{
		await using FileStream stream = File.OpenRead(path);
		byte[] hash = await SHA256.HashDataAsync(stream, cancellationToken);
		return Convert.ToHexStringLower(hash);
	}
}
