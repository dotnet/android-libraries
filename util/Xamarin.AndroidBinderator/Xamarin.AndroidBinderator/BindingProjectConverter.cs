using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Java.Interop.Tools.Maven.Models;

namespace AndroidBinderator;

static class BindingProjectConverter
{
	public static List<BindingProjectModel> Convert (BindingConfig config)
	{
		var exceptions = new List<Exception> ();
		var models = new List<BindingProjectModel> ();

		foreach (var artifact in config.MavenArtifacts.Where (a => !a.DependencyOnly))
			models.Add (ConvertArtifact (config, artifact, exceptions));

		if (exceptions.Count > 0)
			throw new AggregateException (exceptions);

		return models;
	}

	// Converts an artifact from config.json to the model used to generate files
	static BindingProjectModel ConvertArtifact (BindingConfig config, MavenArtifactConfig mavenArtifact, List<Exception> exceptions)
	{
		Console.WriteLine ($"{mavenArtifact.GroupAndArtifactId}:{mavenArtifact.Version}");
		var mavenProject = MavenFactory2.GetPomForArtifact (config, mavenArtifact);

		var projectModel = new BindingProjectModel {
			Name = mavenArtifact.ArtifactId,
			NuGetPackageId = mavenArtifact.NugetPackageId,
			NuGetVersionBase = mavenArtifact.NugetVersion,
			NuGetVersionSuffix = config.NugetVersionSuffix,
			MavenGroupId = mavenArtifact.GroupId,
			AssemblyName = mavenArtifact.AssemblyName,
			Config = config,
			MavenDescription = mavenProject.Description,
			MavenUrl = mavenProject.Url,
			JavaSourceRepository = mavenProject.Scm?.Url,
			Type = mavenArtifact.BindingsType ?? config.DefaultBindingsType,
			AllowPrereleaseDependencies = mavenArtifact.AllowPrereleaseDependencies,
		};

		foreach (var developer in mavenProject.Developers)
			if (developer.Name is not null)
				projectModel.Developers.Add (new MavenArtifactDeveloper (developer.Name));

		ConvertPayload (config, projectModel, mavenArtifact, mavenProject);
		ConvertLicenses (config, mavenArtifact, exceptions, mavenProject, projectModel);
		ConvertDependencies (config, projectModel, mavenArtifact, mavenProject);

		return projectModel;
	}

	private static void ConvertLicenses (BindingConfig config, MavenArtifactConfig mavenArtifact, List<Exception> exceptions, Project mavenProject, BindingProjectModel projectModel)
	{
		var licenses = FindLicenses (config, mavenArtifact, mavenProject);
		var had_unknown_license = false;

		// Verify that we have known licenses
		foreach (var license in licenses) {
			var license_config = config.Licenses.FirstOrDefault (l => string.Compare (l.Name, license.Name, true) == 0);

			if (license_config is null) {
				exceptions.Add (new Exception ($"Unknown license '{license.Name}' for artifact '{mavenArtifact.GroupAndArtifactId}'. This license must be added to 'config.json'."));
				had_unknown_license = true;
				continue;
			}

			projectModel.Licenses.Add (new MavenArtifactLicense (license.Name ?? "", license.Url ?? "", license_config));
		}

		if (projectModel.Licenses.Count == 0 && !had_unknown_license)
			exceptions.Add (new Exception ($"No license(s) could be found for artifact '{mavenArtifact.GroupAndArtifactId}'. Use 'overrideLicenses' in 'config.json' to specify with format 'name|url' ('url' is optional)."));

		// Load license text
		foreach (var license in projectModel.Licenses) {
			var license_file = Path.Combine (config.BasePath!, license.LicenseConfig.File);
			license.Text = File.ReadAllText (license_file);
		}

		// Ensure we use Xamarin.Build.Download for any proprietary licenses
		if (projectModel.Type != BindingType.XamarinBuildDownload)
			foreach (var l in projectModel.Licenses)
				if (l.LicenseConfig.Proprietary)
					exceptions.Add (new Exception ($"Artifact '{mavenArtifact.GroupAndArtifactId}' has proprietary license '{l.Name}' and must use the `xbd` artifact type."));
	}

	static Collection<License> FindLicenses (BindingConfig config, MavenArtifactConfig mavenArtifact, Project mavenProject)
	{
		Collection<License>? licenses;

		// OverrideLicenses ignores any licenses in the POM
		if (mavenArtifact.OverrideLicenses.Count > 0) {
			licenses = new Collection<License> ();

			foreach (var l in mavenArtifact.OverrideLicenses) {
				var parts = l.Split ('|');
				licenses.Add (new License {
					Name = parts [0],
					Url = parts.Length > 1 ? parts [1] : string.Empty
				});
			}

			return licenses;
		}

		licenses = mavenProject.Licenses;
		var project = mavenProject;

		// If we didn't find any licenses, try the parent POM
		while (licenses.Count == 0 && project.Parent is not null) {
			project = MavenFactory2.GetPomForArtifactParent (config, project.Parent.ToArtifact (), mavenArtifact);
			licenses = project.Licenses;
		}

		return licenses ?? [];
	}

	// Creates a model for a .jar/.aar artifact payload
	static void ConvertPayload (BindingConfig config, BindingProjectModel projectModel, MavenArtifactConfig mavenArtifact, Project mavenProject)
	{
		var artifactDir = Path.Combine (config.BasePath!, config.ExternalsDir, mavenArtifact.GroupId!);
		var artifactFile = Path.Combine (artifactDir, $"{mavenArtifact.ArtifactId}.{mavenProject.Packaging}");
		var md5File = artifactFile + ".md5";
		var sha256File = artifactFile + ".sha256";
		var md5 = File.Exists (md5File) ? File.ReadAllText (md5File) : string.Empty;
		var sha256 = File.Exists (sha256File) ? File.ReadAllText (sha256File) : string.Empty;
		var artifactExtractDir = Path.Combine (artifactDir, mavenArtifact.ArtifactId!);

		var proguardFile = Path.Combine (artifactExtractDir, "proguard.txt");

		var artifact = new MavenArtifactModel {
			MavenGroupId = mavenArtifact.GroupId,
			MavenArtifactId = mavenArtifact.ArtifactId,
			MavenArtifactPackaging = mavenProject.Packaging,
			MavenArtifactVersion = mavenArtifact.Version,
			MavenArtifactMd5 = md5,
			MavenArtifactSha256 = sha256,
			ProguardFile = proguardFile,
			MavenArtifactConfig = mavenArtifact,
			DocumentationType = mavenArtifact.DocumentationType,
		};

		projectModel.MavenArtifacts.Add (artifact);
	}

	static void ConvertDependencies (BindingConfig config, BindingProjectModel model, MavenArtifactConfig mavenArtifact, Project mavenProject)
	{
		// Find all the POM specified dependencies that we need to consider
		foreach (var mavenDep in mavenProject.Dependencies) {
			FixDependency (config, mavenArtifact, mavenDep, mavenProject);

			if (!ShouldIncludeDependency (config, mavenArtifact, mavenDep))
				continue;

			model.MavenDependencies.Add (mavenDep);
		}

		// Add any "extraDependencies"
		model.MavenDependencies.AddRange (ParseExtraDependencies (mavenArtifact.ExtraDependencies));
	}

	static void FixDependency (BindingConfig config, MavenArtifactConfig mavenArtifact, Dependency dependency, Project project)
	{
		// Handle Parent POM
		if ((string.IsNullOrEmpty (dependency.Version) || string.IsNullOrEmpty (dependency.Scope)) && project.Parent != null) {
			var parent_pom = MavenFactory2.GetPomForArtifactParent (config, project.Parent.ToArtifact (), mavenArtifact);
			var parent_dependency = parent_pom.FindParentDependency (dependency);

			// Try to fish a version out of the parent POM
			if (string.IsNullOrEmpty (dependency.Version))
				dependency.Version = ReplaceVersionProperties (parent_pom, parent_dependency?.Version);

			// Try to fish a scope out of the parent POM
			if (string.IsNullOrEmpty (dependency.Scope))
				dependency.Scope = parent_dependency?.Scope;
		}

		var version = dependency.Version;

		// If <version> was empty, look for a matching <dependencyManagement> entry
		if (string.IsNullOrWhiteSpace (version)) {
			var dep_man_dep = project.DependencyManagement?.Dependencies?.FirstOrDefault (d => d.GroupAndArtifactId () == dependency.GroupAndArtifactId ());

			if (dep_man_dep is not null)
				version = dep_man_dep.Version;
		}

		if (string.IsNullOrWhiteSpace (version))
			return;

		version = ReplaceVersionProperties (project, version);

		// VersionRange.Parse cannot handle single number versions that we sometimes see in Maven, like "1".
		// Fix them to be "1.0".
		// https://github.com/NuGet/Home/issues/10342
		if (!version.Contains ('.'))
			version += ".0";

		dependency.Version = version;
	}

	[return: NotNullIfNotNull (nameof (version))]
	static string? ReplaceVersionProperties (Project project, string? version)
	{
		// Handle versions with Properties, like:
		// <properties>
		//   <java.version>1.8</java.version>
		//   <gson.version>2.8.6</gson.version>
		// </properties>
		// <dependencies>
		//   <dependency>
		//     <groupId>com.google.code.gson</groupId>
		//     <artifactId>gson</artifactId>
		//     <version>${gson.version}</version>
		//   </dependency>
		// </dependencies>
		if (string.IsNullOrWhiteSpace (version) || project?.Properties == null)
			return version;

		foreach (var prop in project.Properties.Any)
			version = version!.Replace ($"${{{prop.Name.LocalName}}}", prop.Value);

		return version;
	}

	static bool ShouldIncludeDependency (BindingConfig config, MavenArtifactConfig artifact, Dependency dependency)
	{
		// Check 'artifact' list
		if (artifact.ExcludedRuntimeDependencies.OrEmpty ().Split (',').Contains (dependency.GroupAndArtifactId (), StringComparer.OrdinalIgnoreCase))
			return false;

		// Check 'global' list
		if (config.ExcludedRuntimeDependencies.OrEmpty ().Split (',').Contains (dependency.GroupAndArtifactId (), StringComparer.OrdinalIgnoreCase))
			return false;

		// We always care about 'compile' scoped dependencies
		if (dependency.IsCompileDependency ())
			return true;

		// If we're not processing Runtime dependencies then ignore the rest
		if (!config.StrictRuntimeDependencies)
			return false;

		// The only other thing we may care about is 'runtime', bail if this isn't 'runtime'
		if (!dependency.IsRuntimeDependency ())
			return false;

		return true;
	}

	static IEnumerable<Dependency> ParseExtraDependencies (string? dependencies)
	{
		if (string.IsNullOrWhiteSpace (dependencies))
			yield break;

		// Format is: "groupid.artifactid:version"
		// Version is optional
		var deps = dependencies.Split (new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);

		foreach (var dep in deps) {
			var id = dep;
			var version = string.Empty;

			if (dep.Contains (':')) {
				id = dep.Substring (0, dep.IndexOf (':'));
				version = dep.Substring (dep.IndexOf (':') + 1);
			}

			var result = new Dependency {
				GroupId = id.Substring (0, id.LastIndexOf ('.')),
				ArtifactId = id.Substring (id.LastIndexOf ('.') + 1),
				Version = string.IsNullOrWhiteSpace (version) ? "0.0.0" : version
			};

			yield return result;
		}
	}
}
