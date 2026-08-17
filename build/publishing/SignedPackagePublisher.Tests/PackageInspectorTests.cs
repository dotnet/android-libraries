using System.IO.Compression;

namespace SignedPackagePublisher.Tests;

public sealed class PackageInspectorTests
{
	[Test]
	public async Task ReadsIdentityAndNormalizesVersionFromNuspec()
	{
		using var directory = new TemporaryDirectory();
		CreatePackage(Path.Combine(directory.Path, "not-the-identity.nupkg"), "Example.Package", "1.2.3.0");

		IReadOnlyList<SignedPackage> packages = await PackageInspector.InspectDirectoryAsync(
			directory.Path,
			CancellationToken.None);

		Assert.Multiple(() => {
			Assert.That(packages, Has.Count.EqualTo(1));
			Assert.That(packages[0].Id, Is.EqualTo("Example.Package"));
			Assert.That(packages[0].Version.ToNormalizedString(), Is.EqualTo("1.2.3"));
			Assert.That(packages[0].Sha256, Has.Length.EqualTo(64));
		});
	}

	internal static void CreatePackage(string path, string id, string version, string content = "content")
	{
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		using ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Create);
		ZipArchiveEntry nuspec = archive.CreateEntry($"{id}.nuspec");
		using (var writer = new StreamWriter(nuspec.Open()))
		{
			writer.Write($"""
				<?xml version="1.0"?>
				<package>
				  <metadata>
				    <id>{id}</id>
				    <version>{version}</version>
				    <authors>Test</authors>
				    <description>Test</description>
				  </metadata>
				</package>
				""");
		}
		using var contentWriter = new StreamWriter(archive.CreateEntry("content.txt").Open());
		contentWriter.Write(content);
	}
}

internal sealed class TemporaryDirectory : IDisposable
{
	public TemporaryDirectory()
	{
		Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"signed-package-publisher-{Guid.NewGuid():N}");
		Directory.CreateDirectory(Path);
	}

	public string Path { get; }

	public void Dispose() => Directory.Delete(Path, recursive: true);
}
