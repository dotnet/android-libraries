using NuGet.Packaging.Core;

namespace SignedPackagePublisher;

public sealed class PublishingPlanner
{
	private static readonly PackageIdentityComparer IdentityComparer = PackageIdentityComparer.Default;

	public PublishingPlan Create(IReadOnlyList<SignedPackage> packages)
	{
		var entries = new Dictionary<string, PackageInventoryEntry>(StringComparer.Ordinal);
		var canonicalPackages = new List<SignedPackage>();

		foreach (IGrouping<string, SignedPackage> fileNameGroup in packages.GroupBy(
			package => package.FileName,
			StringComparer.OrdinalIgnoreCase))
		{
			var identities = fileNameGroup
				.Select(package => new PackageIdentity(package.Id, package.Version))
				.Distinct(IdentityComparer)
				.ToArray();
			if (identities.Length > 1)
			{
				foreach (SignedPackage package in fileNameGroup)
					entries[package.RelativePath] = Entry(package, PackageDecision.Error, "duplicate-filename-different-identity");
			}
		}

		foreach (IGrouping<PackageIdentity, SignedPackage> identityGroup in packages.GroupBy(
			package => new PackageIdentity(package.Id, package.Version),
			IdentityComparer))
		{
			SignedPackage[] ordered = identityGroup.OrderBy(package => package.RelativePath, StringComparer.Ordinal).ToArray();
			SignedPackage[] unrecorded = ordered.Where(package => !entries.ContainsKey(package.RelativePath)).ToArray();
			if (unrecorded.Length == 0)
				continue;
			if (unrecorded.Length != ordered.Length)
			{
				foreach (SignedPackage package in unrecorded)
					entries[package.RelativePath] = Entry(package, PackageDecision.Error, "duplicate-identity-in-conflicting-filename-set");
				continue;
			}

			if (ordered.Select(package => package.Sha256).Distinct(StringComparer.Ordinal).Count() > 1)
			{
				foreach (SignedPackage package in ordered)
					entries[package.RelativePath] = Entry(package, PackageDecision.Error, "duplicate-identity-different-content");
				continue;
			}

			SignedPackage canonical = ordered[0];
			canonicalPackages.Add(canonical);
			entries[canonical.RelativePath] = Entry(canonical, PackageDecision.Include, "included");
			foreach (SignedPackage duplicate in ordered.Skip(1))
			{
				entries[duplicate.RelativePath] = Entry(
					duplicate,
					PackageDecision.Exclude,
					"duplicate-identical",
					canonical.RelativePath);
			}
		}

		PackageInventoryEntry[] inventory = entries.Values
			.OrderBy(entry => entry.Id, StringComparer.OrdinalIgnoreCase)
			.ThenBy(entry => entry.Version, StringComparer.OrdinalIgnoreCase)
			.ThenBy(entry => entry.SourceFile, StringComparer.Ordinal)
			.ToArray();

		SignedPackage[] included = canonicalPackages
			.OrderBy(package => package.Id, StringComparer.OrdinalIgnoreCase)
			.ThenBy(package => package.Version)
			.ToArray();

		return new PublishingPlan(inventory, included);
	}

	private static PackageInventoryEntry Entry(
		SignedPackage package,
		PackageDecision decision,
		string reason,
		string? canonicalSourceFile = null)
		=> new(
			package.RelativePath,
			package.FileName,
			package.Sha256,
			package.Id,
			package.Version.ToNormalizedString(),
			decision,
			reason,
			canonicalSourceFile);
}
