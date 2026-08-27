using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace AndroidBinderator;

public class MavenArtifactConfig
{
	public MavenArtifactConfig ()
	{
	}

	public MavenArtifactConfig (string groupId, string artifactId, string version, string? nugetPackageId = null, string? nugetVersion = null)
	{
		GroupId = groupId;
		ArtifactId = artifactId;
		Version = version;
		NugetVersion = nugetVersion;
		NugetPackageId = nugetPackageId;
	}

	[JsonProperty ("groupId")]
	public string? GroupId { get; set; }

	[JsonProperty ("artifactId")]
	public string? ArtifactId { get; set; }

	[JsonProperty ("version")]
	public string Version { get; set; } = string.Empty;

	string? nugetVersion = null;

	[JsonProperty ("nugetVersion")]
	public string? NugetVersion {
		get { return nugetVersion ?? Version; }
		set { nugetVersion = value; }
	}

	[JsonProperty ("nugetId")]
	public string? NugetPackageId { get; set; }

	[JsonProperty ("dependencyOnly")]
	public bool DependencyOnly { get; set; } = false;

	[JsonProperty ("frozen")]
	public bool Frozen { get; set; }

	[JsonProperty ("allowPrereleaseDependencies")]
	public bool AllowPrereleaseDependencies { get; set; }

	[JsonProperty ("assemblyName")]
	public string? AssemblyName { get; set; }

	[JsonProperty ("excludedRuntimeDependencies")]
	public string? ExcludedRuntimeDependencies { get; set; }

	[JsonProperty ("extraDependencies")]
	public string? ExtraDependencies { get; set; }

	[JsonProperty ("type")]
	public BindingType? BindingsType { get; set; }

	[JsonProperty ("mavenRepositoryType")]
	public MavenRepoType? MavenRepositoryType { get; set; }

	[JsonProperty ("mavenRepositoryLocation")]
	public string? MavenRepositoryLocation { get; set; } = null;

	[JsonProperty ("documentationType")]
	[DefaultValue (DocumentationType.None)]
	public DocumentationType DocumentationType { get; set; } = DocumentationType.None;

	[JsonProperty ("templateSet")]
	public string? TemplateSet { get; set; }

	[JsonProperty ("skipExtendedTests")]
	public bool SkipExtendedTests { get; set; }

	public bool ShouldSerializeOverrideLicenses () => OverrideLicenses.Count > 0;

	[JsonProperty ("overrideLicenses")]
	public List<string> OverrideLicenses { get; set; } = new List<string> ();

	[JsonProperty ("comments")]
	public string? Comments { get; set; }

	public bool ShouldSerializeMetadata () => Metadata.Count > 0;

	[JsonProperty ("metadata")]
	public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string> ();

	// The latest version available in Maven
	[JsonIgnore]
	public string LatestVersion { get; set; } = string.Empty;

	// NuGet version to match Maven LatestVersion
	[JsonIgnore]
	public string LatestNuGetVersion { get; set; } = string.Empty;

	[JsonIgnore]
	public string GroupAndArtifactId => $"{GroupId}.{ArtifactId}";

	[JsonIgnore]
	public bool NewVersionAvailable => LatestVersion != Version;

	[JsonIgnore]
	public bool NewNuGetVersionAvailable => LatestNuGetVersion != NugetVersion;
}
