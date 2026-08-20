using System.Text.Json;
using System.Xml.Linq;

namespace SignedPackagePublisher.Tests;

public sealed class PublishingOutputTests
{
	[Test]
	public async Task InventoryIsByteForByteDeterministic()
	{
		using var directory = new TemporaryDirectory();
		var packages = new[] {
			Package("z.nupkg", "Z.Package", "2.0.0", "bb"),
			Package("a.nupkg", "A.Package", "1.0.0", "aa"),
		};
		string first = Path.Combine(directory.Path, "first.json");
		string second = Path.Combine(directory.Path, "second.json");

		await PublishingOutput.WriteInventoryAsync(first, packages, CancellationToken.None);
		await PublishingOutput.WriteInventoryAsync(second, packages.Reverse().ToArray(), CancellationToken.None);

		Assert.That(await File.ReadAllBytesAsync(first), Is.EqualTo(await File.ReadAllBytesAsync(second)));
		using JsonDocument document = JsonDocument.Parse(await File.ReadAllTextAsync(first));
		Assert.Multiple(() => {
			Assert.That(document.RootElement.GetProperty("schemaVersion").GetInt32(), Is.EqualTo(2));
			Assert.That(document.RootElement.TryGetProperty("feed", out _), Is.False);
			Assert.That(
				document.RootElement.GetProperty("packages").EnumerateArray()
					.Select(package => package.GetProperty("id").GetString()),
				Is.EqualTo(new[] { "A.Package", "Z.Package" }));
		});
	}

	[Test]
	public void ManifestMarksAssetsAsShippingPackageCategory()
	{
		using var directory = new TemporaryDirectory();
		string manifestPath = Path.Combine(directory.Path, "Manifest.xml");
		SignedPackage package = Package("a.nupkg", "A.Package", "1.0.0", "aa");

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

	private static SignedPackage Package(string path, string id, string version, string hash)
		=> new(path, path, Path.GetFileName(path), hash, id, NuGet.Versioning.NuGetVersion.Parse(version));
}
