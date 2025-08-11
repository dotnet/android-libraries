using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Java.Interop.Tools.Maven.Models;

namespace AndroidBinderator;

static class BindingProjectDependencyVerifier
{
	/// <summary>
	/// Automatic dependency mappings for artifact migrations.
	/// Key: "groupId:artifactId" of the old artifact
	/// Value: (newGroupId, newArtifactId, minimumVersion)
	/// </summary>
	static readonly Dictionary<string, (string GroupId, string ArtifactId, string MinimumVersion)> Remappings = new()
	{
		["com.squareup.okhttp:okhttp"] = ("com.squareup.okhttp3", "okhttp", "3.4.0"),
	};
	public static void Verify (BindingConfig config, List<BindingProjectModel> artifacts)
	{
		var exceptions = new List<Exception> ();

		foreach (var artifact in artifacts)
			VerifyArtifact (config, artifact, exceptions);

		if (exceptions.Count > 0)
			throw new AggregateException (exceptions);
	}

	static void VerifyArtifact (BindingConfig config, BindingProjectModel projectModel, List<Exception> exceptions)
	{
		var artifact = projectModel.ToArtifact ();

		// Ensure every artifact dependency is satisfied
		foreach (var mavenDep in projectModel.MavenDependencies) {
			mavenDep.GroupId = mavenDep.GroupId?.Replace ("${project.groupId}", artifact.GroupId);
			mavenDep.Version = mavenDep.Version?.Replace ("${project.version}", artifact.Version);

			// Apply automatic mappings for known artifact migrations
			ApplyRemappings (mavenDep);

			var depMapping = config.MavenArtifacts.FirstOrDefault (
				ma => !string.IsNullOrEmpty (ma.Version)
				&& ma.GroupId == mavenDep.GroupId
				&& ma.ArtifactId == mavenDep.ArtifactId
				&& mavenDep.Satisfies (ma.Version));

			if (depMapping is null) {
				AddDependencyException (exceptions, artifact, mavenDep.ToArtifact ());
				continue;
			}

			projectModel.NuGetDependencies.Add (new NuGetDependencyModel {
				IsProjectReference = !depMapping.DependencyOnly,
				NuGetPackageId = depMapping.NugetPackageId,
				NuGetVersionBase = depMapping.NugetVersion,
				NuGetVersionSuffix = config.NugetVersionSuffix,
				MavenArtifactConfig = depMapping,

				MavenArtifact = new MavenArtifactModel {
					MavenGroupId = mavenDep.GroupId,
					MavenArtifactId = mavenDep.ArtifactId,
					MavenArtifactVersion = mavenDep.Version,
					MavenArtifactMd5 = projectModel.MavenArtifacts.First ().MavenArtifactMd5,
					MavenArtifactSha256 = projectModel.MavenArtifacts.First ().MavenArtifactSha256
				}
			});
		}
	}

	static void AddDependencyException (List<Exception> exceptions, Artifact artifact, Artifact dependency)
	{
		var sb = new StringBuilder ();

		sb.AppendLine ($"");
		sb.AppendLine ($"No matching artifact config found for: ");
		sb.AppendLine ($"			{dependency.VersionedArtifactString}");
		sb.AppendLine ($"to satisfy dependency of: ");
		sb.AppendLine ($"			{artifact.VersionedArtifactString}");
		sb.AppendLine ($"");
		sb.AppendLine ($"	Please add following json snippet to config.json:");
		sb.AppendLine ($"");
		sb.AppendLine
		($@"
		   {{
		     ""groupId"": ""{dependency.GroupId}"",
		     ""artifactId"": ""{dependency.Id}"",
		     ""version"": ""{dependency.Version}"",
		     ""nugetVersion"": ""CHECK PREFIX {dependency.Version}"",
		     ""nugetId"": ""CHECK NUGET ID"",
		     ""dependencyOnly"": true/false
		   }}
		");
		sb.AppendLine ($"");

		exceptions.Add (new Exception (sb.ToString ()));
	}

	internal static void ApplyRemappings (Dependency dependency)
	{
		var key = $"{dependency.GroupId}:{dependency.ArtifactId}";
		
		if (!Remappings.TryGetValue(key, out var mapping)) {
			return; // No mapping found for this dependency
		}
		
		// Apply the mapping
		dependency.GroupId = mapping.GroupId;
		dependency.ArtifactId = mapping.ArtifactId;
		
		// Handle version with minimum version enforcement
		if (!string.IsNullOrEmpty (dependency.Version)) {
			var requestedVersion = dependency.Version;
			
			// Handle version ranges - parse and potentially adjust lower bound to meet minimum
			if (requestedVersion.Contains ('[') || requestedVersion.Contains ('(') || requestedVersion.Contains (']') || requestedVersion.Contains (')')) {
				dependency.Version = AdjustVersionRange(requestedVersion, mapping.MinimumVersion);
				return;
			}
			
			try {
				var parsed = MavenVersion.Parse (requestedVersion);
				var minimum = MavenVersion.Parse (mapping.MinimumVersion);
				
				// If the requested version is lower than minimum, upgrade to minimum
				if (parsed.CompareTo (minimum) < 0) {
					dependency.Version = mapping.MinimumVersion;
				}
			} catch {
				// If version parsing fails, default to minimum version
				dependency.Version = mapping.MinimumVersion;
			}
		} else {
			// No version specified, use minimum version
			dependency.Version = mapping.MinimumVersion;
		}
	}

	/// <summary>
	/// Adjusts a Maven version range to ensure the lower bound meets the minimum version requirement.
	/// Examples:
	/// - "[2.0.0,5.0.0)" with minimum "3.4.0" becomes "[3.4.0,5.0.0)"
	/// - "(1.0.0,4.0.0]" with minimum "3.4.0" becomes "[3.4.0,4.0.0]"
	/// </summary>
	static string AdjustVersionRange(string versionRange, string minimumVersion)
	{
		try {
			// Simple regex-based parsing for common Maven version range patterns
			// Handles: [1.0.0,2.0.0), (1.0.0,2.0.0], [1.0.0,2.0.0], (1.0.0,2.0.0)
			var pattern = @"^([\[\(])([^,]+),([^\]\)]+)([\]\)])$";
			var match = System.Text.RegularExpressions.Regex.Match(versionRange, pattern);
			
			if (!match.Success) {
				// Not a standard range format, return as-is
				return versionRange;
			}
			
			var lowerBracket = match.Groups[1].Value;
			var lowerVersion = match.Groups[2].Value.Trim();
			var upperVersion = match.Groups[3].Value.Trim();
			var upperBracket = match.Groups[4].Value;
			
			// Parse versions for comparison
			var parsedLower = MavenVersion.Parse(lowerVersion);
			var parsedMinimum = MavenVersion.Parse(minimumVersion);
			
			// If lower bound is below minimum, adjust it
			if (parsedLower.CompareTo(parsedMinimum) < 0) {
				lowerVersion = minimumVersion;
				// Change exclusive lower bound to inclusive if we're setting it to minimum
				if (lowerBracket == "(") {
					lowerBracket = "[";
				}
			}
			
			return $"{lowerBracket}{lowerVersion},{upperVersion}{upperBracket}";
		} catch (Exception ex) {
			Console.WriteLine($"Error adjusting version range '{versionRange}' with minimum '{minimumVersion}': {ex}");

			// If parsing fails, return the original range
			return versionRange;
		}
	}
}
