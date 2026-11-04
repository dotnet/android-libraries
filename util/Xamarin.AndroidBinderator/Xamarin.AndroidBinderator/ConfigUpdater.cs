using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MavenNet.Models;
using NuGet.Versioning;

namespace AndroidBinderator;

public class ConfigUpdater
{
	static HttpClient? client;

	public static async Task Update (BindingConfig config, List<string> dependencyConfigs)
	{
		await MavenFactory.Initialize (config);

		// Check for artifact updates
		foreach (var art in config.MavenArtifacts.Where (a => !a.DependencyOnly)) {
			art.LatestVersion = art.Version;
			art.LatestNuGetVersion = art.NugetVersion ?? string.Empty;

			var a = FindMavenArtifact (config, art);

			if (a is null)
				continue;

			if (HasUpdate (art, a)) {
				var new_version = GetLatestVersion (a)?.ToString () ?? string.Empty;
				var needs_prefix = NeedsPrefix (art.NugetVersion!, art.Version);

				art.LatestVersion = new_version;
				art.LatestNuGetVersion = new_version;

				if (needs_prefix) {
					if (new_version.IndexOf ('.') == 1)
						art.LatestNuGetVersion = $"10{new_version}";
					else
						art.LatestNuGetVersion = $"1{new_version}";
				}
			}
		}

		// Check for artifact dependency updates
		var dependencies = await GetExternalDependencies (dependencyConfigs);

		foreach (var art in config.MavenArtifacts.Where (a => a.DependencyOnly)) {
			art.LatestVersion = art.Version;
			art.LatestNuGetVersion = art.NugetVersion ?? string.Empty;

			var dep = dependencies.FirstOrDefault (d => d.NugetPackageId == art.NugetPackageId);

			if (dep is null)
				continue;

			// We need to ensure the NuGet dependency has actually been published
			if (dep.NugetPackageId is not null && dep.NugetVersion is not null && await DoesNuGetPackageAlreadyExist (dep.NugetPackageId, dep.NugetVersion)) {
				art.LatestVersion = dep.Version;
				art.LatestNuGetVersion = dep.NugetVersion;
			}
		}
	}

	static Artifact? FindMavenArtifact (BindingConfig config, MavenArtifactConfig artifact)
	{
		var repo = MavenFactory.GetMavenRepository (config, artifact);
		var group = repo.Groups.FirstOrDefault (g => artifact.GroupId == g.Id);

		return group?.Artifacts?.FirstOrDefault (a => artifact.ArtifactId == a.Id);
	}

	static bool HasUpdate (MavenArtifactConfig model, Artifact artifact)
	{
		// Get latest stable version
		var latest = GetLatestVersion (artifact);

		// No stable version
		if (latest is null)
			return false;

		// Already on latest
		var current = GetVersion (model.Version);

		if (latest <= current)
			return false;

		return true;
	}

	static SemanticVersion? GetLatestVersion (Artifact artifact, bool includePrerelease = false)
	{
		var versions = artifact.Versions.Select (v => GetVersion (v));

		// Handle 'com.google.guava.guava' which ships its release packages looking like prerelease, ex: '33.1.0-android'
		if (!includePrerelease)
			versions = versions.Where (v => !v.IsPrerelease || v.Release == "android");

		if (!versions.Any ())
			return null;

		return versions.Max ();
	}

	static SemanticVersion GetVersion (string s)
	{
		var hyphen = s.IndexOf ('-');
		var version = hyphen >= 0 ? s.Substring (0, hyphen) : s;
		var tag = hyphen >= 0 ? s.Substring (hyphen) : string.Empty;

		if (tag.Contains ("_"))
			tag = string.Empty;

		// Stuff like: 1.1.1d-alpha-1
		if (version.Any (c => char.IsLetter (c)))
			return new SemanticVersion (0, 0, 0);

		if (version.Count (c => c == '.') == 0)
			version += ".0.0";

		if (version.Count (c => c == '.') == 1)
			version += ".0";

		// SemanticVersion can't handle more than 3 parts, like '0.11.91.1'
		if (version.Count (c => c == '.') > 2)
			return new SemanticVersion (0, 0, 0);

		// Leading zeros are forbidden in SemanticVersion
		if (version == "22.12.06")
			return new SemanticVersion (0, 0, 0);

		return SemanticVersion.Parse (version + tag);
	}

	static bool NeedsPrefix (string nugetVersion, string artifactVersion)
	{
		var nuget = Extensions.GetThreePartVersion (nugetVersion);
		var artifact = Extensions.GetThreePartVersion (artifactVersion);

		var nuget_major = int.Parse (nuget.Split ('.') [0]);
		var artifact_major = int.Parse (artifact.Split ('.') [0]);

		return nuget_major == artifact_major + 100;
	}

	static async Task<List<MavenArtifactConfig>> GetExternalDependencies (List<string> externalFiles)
	{
		var list = new List<MavenArtifactConfig> ();

		foreach (var file in externalFiles) {
			var config = await BindingConfig.Load (file);

			list.AddRange (config.MavenArtifacts.Where (a => !a.DependencyOnly));
		}

		return list;
	}

	public static async Task<bool> DoesNuGetPackageAlreadyExist (string package, string version)
	{
		var packagePath = package.ToLowerInvariant ();
		var versionPath = version.ToLowerInvariant ();
		var url = $"https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-public/nuget/v3/flat2/{packagePath}/{versionPath}/{packagePath}.{versionPath}.nupkg";

		client ??= new HttpClient ();

		var result = await client.GetAsync (url);
		return result.StatusCode != System.Net.HttpStatusCode.NotFound;
	}
}
