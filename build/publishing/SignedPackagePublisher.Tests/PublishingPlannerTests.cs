using NuGet.Versioning;

namespace SignedPackagePublisher.Tests;

public sealed class PublishingPlannerTests
{
	[Test]
	public async Task FiltersExistingPackagesAndProducesDeterministicInventory()
	{
		var packages = new[] {
			Package("z.nupkg", "Z.Package", "2.0.0", "bb"),
			Package("a.nupkg", "A.Package", "1.0.0", "aa"),
		};
		var probe = new StubProbe(("A.Package", "1.0.0", true), ("Z.Package", "2.0.0", false));

		PublishingPlan plan = await new PublishingPlanner(probe, 2).CreateAsync(packages, CancellationToken.None);

		Assert.Multiple(() => {
			Assert.That(plan.Inventory.Select(entry => entry.Id), Is.EqualTo(new[] { "A.Package", "Z.Package" }));
			Assert.That(plan.Inventory.Select(entry => entry.Reason), Is.EqualTo(new[] { "already-exists", "missing-from-feed" }));
			Assert.That(plan.IncludedPackages.Select(package => package.Id), Is.EqualTo(new[] { "Z.Package" }));
		});
	}

	[Test]
	public async Task DeduplicatesIdenticalIdentityAndContent()
	{
		var packages = new[] {
			Package("b/copy.nupkg", "Example.Package", "1.0.0", "aa"),
			Package("a/original.nupkg", "Example.Package", "1.0.0", "aa"),
		};
		var probe = new StubProbe(("Example.Package", "1.0.0", false));

		PublishingPlan plan = await new PublishingPlanner(probe, 1).CreateAsync(packages, CancellationToken.None);

		Assert.Multiple(() => {
			Assert.That(plan.HasErrors, Is.False);
			Assert.That(plan.IncludedPackages.Single().RelativePath, Is.EqualTo("a/original.nupkg"));
			Assert.That(plan.Inventory.Single(entry => entry.SourceFile == "b/copy.nupkg").Reason, Is.EqualTo("duplicate-identical"));
		});
	}

	[Test]
	public async Task RejectsDuplicateIdentityWithDifferentContent()
	{
		var packages = new[] {
			Package("a.nupkg", "Example.Package", "1.0.0", "aa"),
			Package("b.nupkg", "Example.Package", "1.0.0", "bb"),
		};

		PublishingPlan plan = await new PublishingPlanner(new StubProbe(), 1)
			.CreateAsync(packages, CancellationToken.None);

		Assert.Multiple(() => {
			Assert.That(plan.HasErrors, Is.True);
			Assert.That(plan.Inventory, Has.All.Property(nameof(PackageInventoryEntry.Reason)).EqualTo("duplicate-identity-different-content"));
		});
	}

	[Test]
	public async Task RejectsDuplicateFilenameWithDifferentIdentities()
	{
		var packages = new[] {
			Package("a/same.nupkg", "First.Package", "1.0.0", "aa"),
			Package("b/same.nupkg", "Second.Package", "1.0.0", "bb"),
		};

		PublishingPlan plan = await new PublishingPlanner(new StubProbe(), 1)
			.CreateAsync(packages, CancellationToken.None);

		Assert.That(plan.Inventory, Has.All.Property(nameof(PackageInventoryEntry.Reason)).EqualTo("duplicate-filename-different-identity"));
	}

	[Test]
	public async Task AuditsIdentityDuplicatesRelatedToFilenameCollision()
	{
		var packages = new[] {
			Package("a/same.nupkg", "First.Package", "1.0.0", "aa"),
			Package("b/same.nupkg", "Second.Package", "1.0.0", "bb"),
			Package("c/other.nupkg", "First.Package", "1.0.0", "aa"),
		};

		PublishingPlan plan = await new PublishingPlanner(new StubProbe(), 1)
			.CreateAsync(packages, CancellationToken.None);

		Assert.Multiple(() => {
			Assert.That(plan.Inventory, Has.Count.EqualTo(3));
			Assert.That(
				plan.Inventory.Single(entry => entry.SourceFile == "c/other.nupkg").Reason,
				Is.EqualTo("duplicate-identity-in-conflicting-filename-set"));
		});
	}

	[Test]
	public async Task TreatsFeedFailuresAsErrorsNotExistingPackages()
	{
		var package = Package("a.nupkg", "Example.Package", "1.0.0", "aa");
		var probe = new StubProbe(new FeedQueryException("auth failed", new UnauthorizedAccessException()));

		PublishingPlan plan = await new PublishingPlanner(probe, 1)
			.CreateAsync(new[] { package }, CancellationToken.None);

		Assert.Multiple(() => {
			Assert.That(plan.HasErrors, Is.True);
			Assert.That(plan.Inventory.Single().Decision, Is.EqualTo(PackageDecision.Error));
			Assert.That(plan.Inventory.Single().Reason, Does.StartWith("feed-query-failed:"));
		});
	}

	private static SignedPackage Package(string relativePath, string id, string version, string hash)
		=> new(relativePath, relativePath, Path.GetFileName(relativePath), hash, id, NuGetVersion.Parse(version));

	private sealed class StubProbe : IPackageFeedProbe
	{
		private readonly Dictionary<(string Id, string Version), bool> results;
		private readonly Exception? exception;

		public StubProbe(params (string Id, string Version, bool Exists)[] results)
			=> this.results = results.ToDictionary(
				result => (result.Id, result.Version),
				result => result.Exists);

		public StubProbe(Exception exception)
		{
			results = [];
			this.exception = exception;
		}

		public Task<bool> ExistsAsync(string id, NuGetVersion version, CancellationToken cancellationToken)
			=> exception is null
				? Task.FromResult(results[(id, version.ToNormalizedString())])
				: Task.FromException<bool>(exception);
	}
}
