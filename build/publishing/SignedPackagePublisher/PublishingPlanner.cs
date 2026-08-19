using NuGet.Packaging.Core;
using System.Collections.Concurrent;

namespace SignedPackagePublisher;

public sealed class PublishingPlanner(IPackageFeedProbe feedProbe, int maxConcurrency)
{
	private static readonly PackageIdentityComparer IdentityComparer = PackageIdentityComparer.Default;

	public async Task<PublishingPlan> CreateAsync(
		IReadOnlyList<SignedPackage> packages,
		CancellationToken cancellationToken)
	{
		if (maxConcurrency < 1)
			throw new ArgumentOutOfRangeException(nameof(maxConcurrency));

		var entries = new ConcurrentDictionary<string, PackageInventoryEntry>(StringComparer.Ordinal);
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
			foreach (SignedPackage duplicate in ordered.Skip(1))
			{
				entries[duplicate.RelativePath] = Entry(
					duplicate,
					PackageDecision.Exclude,
					"duplicate-identical",
					canonical.RelativePath);
			}
		}

		using var gate = new SemaphoreSlim(maxConcurrency, maxConcurrency);
		await Task.WhenAll(canonicalPackages.Select(async package => {
			await gate.WaitAsync(cancellationToken);
			try
			{
				bool exists = await feedProbe.ExistsAsync(package.Id, package.Version, cancellationToken);
				entries[package.RelativePath] = Entry(
					package,
					exists ? PackageDecision.Exclude : PackageDecision.Include,
					exists ? "already-exists" : "missing-from-feed");
			}
			catch (Exception exception) when (exception is not OperationCanceledException
				|| !cancellationToken.IsCancellationRequested)
			{
				entries[package.RelativePath] = Entry(
					package,
					PackageDecision.Error,
					$"feed-query-failed: {exception.Message}");
			}
			finally
			{
				gate.Release();
			}
		}));

		PackageInventoryEntry[] inventory = entries.Values
			.OrderBy(entry => entry.Id, StringComparer.OrdinalIgnoreCase)
			.ThenBy(entry => entry.Version, StringComparer.OrdinalIgnoreCase)
			.ThenBy(entry => entry.SourceFile, StringComparer.Ordinal)
			.ToArray();

		SignedPackage[] included = canonicalPackages
			.Where(package => entries[package.RelativePath].Decision == PackageDecision.Include)
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
