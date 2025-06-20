using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Java.Interop.Tools.Maven;
using Java.Interop.Tools.Maven.Models;
using Java.Interop.Tools.Maven.Repositories;

namespace AndroidBinderator;

public static class Extensions
{
	public static bool HasValue ([NotNullWhen (true)] this string? str) => !string.IsNullOrWhiteSpace (str);

	public static string OrEmpty (this string? value) => value ?? string.Empty;

	public static string GroupAndArtifactId (this Dependency dependency) => $"{dependency.GroupId}.{dependency.ArtifactId}";

	public static bool IsCompileDependency (this Dependency dependency) => string.IsNullOrWhiteSpace (dependency.Scope) || dependency.Scope.ToLowerInvariant ().Equals ("compile");

	public static bool IsRuntimeDependency (this Dependency dependency) => dependency?.Scope != null && dependency.Scope.ToLowerInvariant ().Equals ("runtime");

	public static Artifact ToArtifact (this MavenArtifactConfig config) => new Artifact (config.GroupId!, config.ArtifactId!, config.Version);

	public static Artifact ToArtifact (this Parent parent) => new Artifact (parent.GroupId!, parent.ArtifactId!, parent.Version!);

	public static Artifact ToArtifact (this BindingProjectModel parent) => new Artifact (parent.MavenGroupId!, parent.MavenArtifacts.First ().MavenArtifactId!, parent.MavenArtifacts.First ().MavenArtifactVersion!);

	public static Dependency? FindParentDependency (this Project project, Dependency dependency)
	{
		return project.DependencyManagement?.Dependencies?.FirstOrDefault (
			d => d.GroupAndArtifactId () == dependency.GroupAndArtifactId () && d.Classifier != "sources");
	}

	public async static Task<Project> GetProjectAsync (this MavenRepository repository, string groupId, string artifactId, string version)
	{
		var project = await repository.GetProjectAsync (groupId, artifactId, version);

		if (project is null)
			throw new InvalidOperationException ($"Could not find project {groupId}:{artifactId}:{version}");
		return project;
	}

	public static string GetRelativePath (string filespec, string folder)
	{
		Uri pathUri = new Uri (filespec);
		// Folders must end in a slash
		if (!folder.EndsWith (Path.DirectorySeparatorChar.ToString (), StringComparison.OrdinalIgnoreCase))
			folder += Path.DirectorySeparatorChar;
		Uri folderUri = new Uri (folder);
		return Uri.UnescapeDataString (folderUri.MakeRelativeUri (pathUri).ToString ().Replace ('/', Path.DirectorySeparatorChar));
	}

	public static bool Satisfies (this Dependency dependency, string value)
	{
		if (!dependency.Version.HasValue ())
			throw new Exception ("no value");

		// Example:
		// No matching artifact config found for:
		//     org.jetbrains.kotlin:kotlin-stdlib:unspecified.0
		// to satisfy dependency of:
		//     androidx.compose.ui:ui-text-android:1.8.3
		if (dependency.Version.StartsWith ("unspecified", StringComparison.OrdinalIgnoreCase))
			return true;

		var version = MavenVersion.Parse (value);
		var range = MavenVersionRange.Parse (dependency.Version);

		return range.Any (r => r.ContainsVersion (version));
	}

	public static string GetThreePartVersion (string version)
	{
		// Change 121.0.0.0-beta1 to 121.0.0
		var hyphen = version.IndexOf ('-');
		version = hyphen >= 0 ? version.Substring (0, hyphen) : version;

		var parts = version.Split ('.');

		if (parts.Count () < 3)
			return version;

		return $"{parts [0]}.{parts [1]}.{parts [2]}";
	}

	public static (int x, int y, int z) GetThreePartVersionAsIntegers (string version)
	{
		// Return 121.0.0 as (121, 0, 0), handles shorter versions like 121.0
		var hyphen = version.IndexOf ('-');
		version = hyphen >= 0 ? version.Substring (0, hyphen) : version;

		var parts = version.Split ('.');
		var x = 0;
		var y = 0;
		var z = 0;

		if (parts.Length > 2)
			z = int.Parse (parts[2]);

		if (parts.Length > 1)
			y = int.Parse (parts [1]);

		if (parts.Length > 0)
			x = int.Parse (parts [0]);

		return (x, y, z);
	}
}

public class ProjectResolver : IProjectResolver
{
	readonly CachedMavenRepository repository;

	public Dictionary<string, string> ResolvedPoms { get; } = new Dictionary<string, string> ();

	public ProjectResolver (CachedMavenRepository repository)
	{
		this.repository = repository;
	}

	public Project Resolve (Artifact artifact)
	{
		if (repository.TryGetFilePath (artifact, $"{artifact.Id}-{artifact.Version}.pom", out var path)) {
			using (var stream = File.OpenRead (path)) {
				var pom = Project.Load (stream) ?? throw new InvalidOperationException ($"Could not deserialize POM for {artifact}");

				// Use index instead of Add to handle duplicates
				ResolvedPoms [artifact.VersionedArtifactString] = path;

				return pom;
			}
		}

		throw new InvalidOperationException ($"No POM found for {artifact}");
	}
}
