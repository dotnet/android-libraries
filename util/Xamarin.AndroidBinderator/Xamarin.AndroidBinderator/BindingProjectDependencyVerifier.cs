using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Java.Interop.Tools.Maven.Models;

namespace AndroidBinderator;

static class BindingProjectDependencyVerifier
{
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
}
