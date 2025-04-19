using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Java.Interop.Tools.Maven.Models;
using Java.Interop.Tools.Maven.Repositories;

namespace AndroidBinderator;

static class MavenArtifactDownloader
{
	public static void Download (BindingConfig config, List<BindingProjectModel> artifacts)
	{
		var externals_dir = Path.Combine (config.BasePath!, config.ExternalsDir);
		Directory.CreateDirectory (externals_dir);

		foreach (var artifact in artifacts) {
			Console.WriteLine($"{artifact.MavenGroupId}:{artifact.Name}:{artifact.Id}");
			var maven_artifact = artifact.MavenArtifacts.First ();
			var config_artifact = maven_artifact.MavenArtifactConfig;

			if (config_artifact is null)
				throw new InvalidOperationException ($"No MavenArtifactConfig found for {artifact}");

			if (config_artifact.DependencyOnly)
				continue;

			// Find and download the artifact payload
			maven_artifact.DownloadedArtifact = DownloadPayload (config, artifact);

			// Download additional files
			DownloadAdditionalFiles (config, artifact);
		}

		// Copy files to externals directory and extract .aar files
		Parallel.ForEach (artifacts, artifact => {

			var maven_artifact = artifact.MavenArtifacts.First ();
			var config_artifact = maven_artifact.MavenArtifactConfig!;
			var artifact_file = maven_artifact.DownloadedArtifact!;

			// Copy to externals directory
			var output_dir = Path.Combine (externals_dir, artifact.MavenGroupId!);
			Directory.CreateDirectory (output_dir);

			var output_file = Path.Combine (output_dir, $"{config_artifact.ArtifactId}{Path.GetExtension (artifact_file)}");
			File.Copy (artifact_file, output_file, overwrite: true);

			// Extract .aar files
			if (Path.GetExtension (artifact_file) == ".aar") {
				var extract_dir = Path.Combine (output_dir, config_artifact.ArtifactId!);

				if (Directory.Exists (extract_dir))
					Directory.Delete (extract_dir, true);

				Directory.CreateDirectory (extract_dir);
				ZipFile.ExtractToDirectory (artifact_file, extract_dir);
			}
		});
	}

	static string DownloadPayload (BindingConfig config, BindingProjectModel artifact)
	{
		var artifact_dir = Path.Combine (config.BasePath!, config.ExternalsDir, artifact.MavenGroupId!);
		Directory.CreateDirectory (artifact_dir);

		var artifact_model = artifact.MavenArtifacts.First ();
		var repository = MavenFactory2.GetMavenRepository (config, artifact_model.MavenArtifactConfig!);
		var a = artifact.ToArtifact ();

		if (artifact_model.MavenArtifactPackaging == "jar" || artifact_model.MavenArtifactPackaging == "aar") {
			if (repository.TryGetFilePath (a, FormatPayloadFileName (a, artifact_model.MavenArtifactPackaging), out var payload_path))
				return payload_path;

			throw new InvalidOperationException ($"Failed to download {a} from {repository.Name}");
		}

		// Sometimes the "MavenArtifactPackaging" isn't useful, like "bundle" for Guava or "pom" for KotlinX Coroutines.
		// In this case we're going to try "jar" and "aar" to try to find the real payload
		if (repository.TryGetFilePath (a, FormatPayloadFileName (a, "jar"), out var jar_path)) {
			artifact_model.MavenArtifactPackaging = "jar";
			return jar_path;
		}

		if (repository.TryGetFilePath (a, FormatPayloadFileName (a, "aar"), out var aar_path)) {
			artifact_model.MavenArtifactPackaging = "aar";
			return aar_path;
		}

		throw new Exception ($"Could not find artifact payload {a.VersionedArtifactString}. [Packaging was {artifact_model.MavenArtifactPackaging}.]");
	}

	static void DownloadAdditionalFiles (BindingConfig config, BindingProjectModel artifact)
	{
		var art = artifact.ToArtifact ();
		var artifact_model = artifact.MavenArtifacts.First ();
		var artifact_dir = Path.Combine (config.BasePath!, config.ExternalsDir, art.GroupId!);
		var base_output_file_name = config.DownloadExternalsWithFullName ? $"{art.GroupId}.{art.Id}" : $"{art.Id}";
		var repository = MavenFactory2.GetMavenRepository (config, artifact_model.MavenArtifactConfig!);

		if (config.DownloadJavaSourceJars) {
			var source_file = FormatMavenFileName (art) + "-sources.jar";
			var dest_file = base_output_file_name + "-sources.jar";
			TryDownloadFile (repository, art, source_file, Path.Combine (artifact_dir, dest_file));
		}

		if (config.DownloadPoms) {
			var pom_file = FormatMavenFileName (art) + ".pom";
			var dest_file = base_output_file_name + ".pom";
			TryDownloadFile (repository, art, pom_file, Path.Combine (artifact_dir, dest_file));
		}

		if (config.DownloadJavaDocJars) {
			var javadoc_file = FormatMavenFileName (art) + "-javadoc.jar";
			var dest_file = base_output_file_name + "-javadoc.jar";
			TryDownloadFile (repository, art, javadoc_file, Path.Combine (artifact_dir, dest_file));
		}
	}

	static bool TryDownloadFile (CachedMavenRepository repository, Artifact artifact, string filename, string outputFile)
	{
		try {
			if (repository.TryGetFilePath (artifact, filename, out var path)) {
				File.Copy (path, outputFile, overwrite: true);
				return true;
			}
		} catch (Exception) {
			// None of these files are required, so we can ignore any exceptions
			return false;
		}

		return false;
	}

	static string FormatPayloadFileName (Artifact artifact, string packaging)
		=> FormatMavenFileName (artifact) + "." + packaging.ToLowerInvariant ().TrimStart ('.');

	static string FormatMavenFileName (Artifact artifact)
		=> artifact.Id + "-" + artifact.Version;
}
