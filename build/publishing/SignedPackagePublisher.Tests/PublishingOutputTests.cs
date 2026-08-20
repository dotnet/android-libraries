using System.Text.Json;
using System.Xml.Linq;

namespace SignedPackagePublisher.Tests;

public sealed class PublishingOutputTests
{
	[Test]
	public async Task InventoryIsByteForByteDeterministic()
	{
		using var directory = new TemporaryDirectory();
		var plan = new PublishingPlan(
			new[] {
				new PackageInventoryEntry("a.nupkg", "a.nupkg", "aa", "A.Package", "1.0.0", PackageDecision.Include, "included"),
			},
			Array.Empty<SignedPackage>());
		string first = Path.Combine(directory.Path, "first.json");
		string second = Path.Combine(directory.Path, "second.json");

		await PublishingOutput.WriteInventoryAsync(first, plan, CancellationToken.None);
		await PublishingOutput.WriteInventoryAsync(second, plan, CancellationToken.None);

		Assert.That(await File.ReadAllBytesAsync(first), Is.EqualTo(await File.ReadAllBytesAsync(second)));
		using JsonDocument document = JsonDocument.Parse(await File.ReadAllTextAsync(first));
		Assert.Multiple(() => {
			Assert.That(document.RootElement.GetProperty("schemaVersion").GetInt32(), Is.EqualTo(2));
			Assert.That(document.RootElement.TryGetProperty("feed", out _), Is.False);
			Assert.That(document.RootElement.GetProperty("packages").GetArrayLength(), Is.EqualTo(1));
		});
	}

	[Test]
	public void ManifestMarksAssetsAsShippingPackageCategory()
	{
		using var directory = new TemporaryDirectory();
		string manifestPath = Path.Combine(directory.Path, "Manifest.xml");
		var package = new SignedPackage(
			"a.nupkg",
			"a.nupkg",
			"a.nupkg",
			"aa",
			"A.Package",
			NuGet.Versioning.NuGetVersion.Parse("1.0.0"));

		PublishingOutput.WriteManifest(
			manifestPath,
			new ManifestIdentity(
				"dotnet/android-libraries",
				"20260817.1",
				"refs/heads/main",
				new string('a', 40),
				"https://dev.azure.com/devdiv/",
				"DevDiv",
				123,
				456,
				"https://dev.azure.com/devdiv/DevDiv/_git/android-libraries"),
			new[] { package });

		string xml = File.ReadAllText(manifestPath);
		XElement root = XElement.Parse(xml);
		XAttribute isStable = root.DescendantsAndSelf().Attributes("IsStable").Single();
		Assert.Multiple(() => {
			Assert.That(xml, Does.Contain("PublishingVersion=\"3\""));
			Assert.That((bool?)isStable, Is.True);
			Assert.That(xml, Does.Contain("Id=\"A.Package\""));
			Assert.That(xml, Does.Contain("NonShipping=\"False\""));
			Assert.That(xml, Does.Contain("Category=\"Package\""));
		});
	}
}
