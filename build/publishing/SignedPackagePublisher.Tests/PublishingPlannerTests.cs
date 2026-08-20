using NuGet.Versioning;

namespace SignedPackagePublisher.Tests;

public sealed class PublishingPlannerTests
{
	[Test]
	public void IncludesEveryCanonicalPackageAndProducesDeterministicInventory()
	{
		var packages = new[] {
			Package("z.nupkg", "Z.Package", "2.0.0", "bb"),
			Package("a.nupkg", "A.Package", "1.0.0", "aa"),
		};

		PublishingPlan plan = new PublishingPlanner().Create(packages);

		Assert.Multiple(() => {
			Assert.That(plan.Inventory.Select(entry => entry.Id), Is.EqualTo(new[] { "A.Package", "Z.Package" }));
			Assert.That(plan.Inventory.Select(entry => entry.Reason), Is.All.EqualTo("included"));
			Assert.That(plan.IncludedPackages.Select(package => package.Id), Is.EqualTo(new[] { "A.Package", "Z.Package" }));
		});
	}

	[Test]
	public void DeduplicatesIdenticalIdentityAndContent()
	{
		var packages = new[] {
			Package("b/copy.nupkg", "Example.Package", "1.0.0", "aa"),
			Package("a/original.nupkg", "Example.Package", "1.0.0", "aa"),
		};
		PublishingPlan plan = new PublishingPlanner().Create(packages);

		Assert.Multiple(() => {
			Assert.That(plan.HasErrors, Is.False);
			Assert.That(plan.IncludedPackages.Single().RelativePath, Is.EqualTo("a/original.nupkg"));
			Assert.That(plan.Inventory.Single(entry => entry.SourceFile == "b/copy.nupkg").Reason, Is.EqualTo("duplicate-identical"));
		});
	}

	[Test]
	public void RejectsDuplicateIdentityWithDifferentContent()
	{
		var packages = new[] {
			Package("a.nupkg", "Example.Package", "1.0.0", "aa"),
			Package("b.nupkg", "Example.Package", "1.0.0", "bb"),
		};

		PublishingPlan plan = new PublishingPlanner().Create(packages);

		Assert.Multiple(() => {
			Assert.That(plan.HasErrors, Is.True);
			Assert.That(plan.Inventory, Has.All.Property(nameof(PackageInventoryEntry.Reason)).EqualTo("duplicate-identity-different-content"));
		});
	}

	[Test]
	public void RejectsDuplicateFilenameWithDifferentIdentities()
	{
		var packages = new[] {
			Package("a/same.nupkg", "First.Package", "1.0.0", "aa"),
			Package("b/same.nupkg", "Second.Package", "1.0.0", "bb"),
		};

		PublishingPlan plan = new PublishingPlanner().Create(packages);

		Assert.That(plan.Inventory, Has.All.Property(nameof(PackageInventoryEntry.Reason)).EqualTo("duplicate-filename-different-identity"));
	}

	[Test]
	public void AuditsIdentityDuplicatesRelatedToFilenameCollision()
	{
		var packages = new[] {
			Package("a/same.nupkg", "First.Package", "1.0.0", "aa"),
			Package("b/same.nupkg", "Second.Package", "1.0.0", "bb"),
			Package("c/other.nupkg", "First.Package", "1.0.0", "aa"),
		};

		PublishingPlan plan = new PublishingPlanner().Create(packages);

		Assert.Multiple(() => {
			Assert.That(plan.Inventory, Has.Count.EqualTo(3));
			Assert.That(
				plan.Inventory.Single(entry => entry.SourceFile == "c/other.nupkg").Reason,
				Is.EqualTo("duplicate-identity-in-conflicting-filename-set"));
		});
	}

	private static SignedPackage Package(string relativePath, string id, string version, string hash)
		=> new(relativePath, relativePath, Path.GetFileName(relativePath), hash, id, NuGetVersion.Parse(version));
}
