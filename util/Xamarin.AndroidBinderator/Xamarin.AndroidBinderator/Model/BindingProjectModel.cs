using System;
using System.Collections.Generic;
using System.Linq;
using Java.Interop.Tools.Maven.Models;

namespace AndroidBinderator;

public class BindingProjectModel
{
	public string Id { get; private set; } = Guid.NewGuid ().ToString ().ToUpperInvariant ();

	public string? Name { get; set; }

	public string? MavenGroupId { get; set; }

	public BindingType Type { get; set; }

	public List<MavenArtifactModel> MavenArtifacts { get; set; } = new List<MavenArtifactModel> ();
	public List<Dependency> MavenDependencies { get; set; } = new List<Dependency> ();

	public string? NuGetPackageId { get; set; }
	public string? NuGetVersionBase { get; set; }
	public string? NuGetVersionSuffix { get; set; }

	public string? NuGetVersion =>
		string.IsNullOrWhiteSpace (NuGetVersionSuffix)
			? NuGetVersionBase
			: NuGetVersionBase + NuGetVersionSuffix;

	public string? AssemblyName { get; set; }

	public List<NuGetDependencyModel> NuGetDependencies { get; set; } = new List<NuGetDependencyModel> ();

	public List<string> ProjectReferences { get; set; } = new List<string> ();

	public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string> ();

	public BindingConfig? Config { get; set; }

	public string? MavenDescription { get; set; }
	public string? MavenUrl { get; set; }
	public string? JavaSourceRepository { get; set; }
	public bool AllowPrereleaseDependencies { get; set; }

	public List<MavenArtifactLicense> Licenses { get; } = new List<MavenArtifactLicense> ();

	public List<MavenArtifactDeveloper> Developers { get; } = new List<MavenArtifactDeveloper> ();

	public string GetAssemblyName () => AssemblyName.HasValue () ? AssemblyName : NuGetPackageId ?? string.Empty;

	public string GetArtifactVersion () => MavenArtifacts.FirstOrDefault ()?.MavenArtifactVersion ?? string.Empty;

	public string GetRootNamespace ()
	{
		if (Metadata.TryGetValue ("rootNamespace", out var rootNamespace))
			return rootNamespace;

		return NuGetPackageId!.Replace ("Xamarin.", "");
	}

	// TODO: Move this to config.json
	// Whether to bind the Java .jar/.aar
	public bool ShouldBindArtifact => NuGetPackageId != "Xamarin.AndroidX.DataStore.Core.Jvm" && NuGetPackageId != "Xamarin.Android.Glide.Annotations";

	// TODO: Move this to config.json
	// Whether to include the Java .jar/.aar in the NuGet package
	public bool ShouldIncludeArtifact => NuGetPackageId != "Xamarin.AndroidX.DataStore.Core.Jvm";

	// Proprietary licenses aren't supported by SPDX
	public bool CanUseSpdx => !Licenses.Any (s => string.IsNullOrWhiteSpace (s.Spdx));

	public string GetSpdxExpression ()
	{
		if (!CanUseSpdx)
			throw new InvalidOperationException ("One or more licenses do not have an SPDX expression");

		// Our SPDX expression is MIT for Microsoft code, plus any
		// other licenses that govern the artifact.
		var licenses = new string [] { "MIT" }.Concat (Licenses.Select (s => s.Spdx)).Distinct ().ToList ();

		return string.Join (" AND ", licenses);
	}
}

public class MavenArtifactLicense
{
	public string Name { get; set; }
	public string Url { get; set; }
	public string Spdx { get; set; }
	public string? Text { get; set; }

	public LicenseConfig LicenseConfig { get; set; }

	public MavenArtifactLicense (string name, string url, LicenseConfig licenseConfig)
	{
		Name = name;
		Url = url;
		LicenseConfig = licenseConfig;
		Spdx = licenseConfig.Spdx;
	}
}

public class MavenArtifactDeveloper
{
	public string Name { get; set; }

	public MavenArtifactDeveloper (string name)
	{
		Name = name;
	}
}
