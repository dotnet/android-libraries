using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RazorLight;

namespace AndroidBinderator;

public class Engine
{
	public static Task BinderateAsync (string configFile, string? basePath = null)
	{
		var config = Newtonsoft.Json.JsonConvert.DeserializeObject<BindingConfig> (File.ReadAllText (configFile))
			?? throw new ArgumentException ("Could not deserialize config file");

		if (!string.IsNullOrEmpty (basePath))
			config.BasePath = basePath;

		return BinderateAsync (config);
	}

	public static async Task BinderateAsync (BindingConfig config)
	{
		if (config.DownloadExternals) {
			var artifactDir = Path.Combine (config.BasePath!, config.ExternalsDir);
			if (!Directory.Exists (artifactDir))
				Directory.CreateDirectory (artifactDir);
		}

		await ProcessConfig (config);
	}

	static async Task ProcessConfig (BindingConfig config)
	{
		Console.WriteLine("Processing config...");
		var slnProjModels = new Dictionary<string, BindingProjectModel> ();

		Console.WriteLine("	converting config...");
		var models = BindingProjectConverter.Convert (config);

		Console.WriteLine("	verifying config...");
		BindingProjectDependencyVerifier.Verify (config, models);
		BindingProjectConsistencyVerifier.Verify (models);

		if (config.DownloadExternals)
			MavenArtifactDownloader.Download (config, models);

		// This isn't really correct, as it could be .aar, but it'll do until we hit that case and need to fix it
		foreach (var artifact in models.Where (a => a.MavenArtifacts.First ().MavenArtifactPackaging == "bundle"))
			artifact.MavenArtifacts.First ().MavenArtifactPackaging = "jar";

		// Look for Proguard files extracted from AARs
		foreach (var model in models) {
			var ma = model.MavenArtifacts.First ();

			if (!ma.ProguardFile.HasValue ())
				continue;

			if (File.Exists (ma.ProguardFile))
				ma.ProguardFile = Extensions.GetRelativePath (ma.ProguardFile, config.BasePath ?? "").Replace ("/", "\\");
			else
				ma.ProguardFile = null;
		}

		if (config.Debug.DumpModels) {
			var json = Newtonsoft.Json.JsonConvert.SerializeObject (models);
			File.WriteAllText (Path.Combine (config.BasePath!, "models.json"), json);
		}

		var engine = new RazorLightEngineBuilder ()
			.UseMemoryCachingProvider ()
			.UseFileSystemProject (config.BasePath)
			.Build ();

		foreach (var model in models) {
			var template_set = config.GetTemplateSet (model.MavenArtifacts.FirstOrDefault ()?.MavenArtifactConfig?.TemplateSet);

			foreach (var template in template_set.Templates) {
				AssignMetadata (model, template);

				var outputFile = new FileInfo (template.GetOutputFile (config, model))!;
				if (!outputFile.Directory!.Exists)
					outputFile.Directory.Create ();

				var result = await engine.CompileRenderAsync (template.TemplateFile, model);

				File.WriteAllText (outputFile.FullName, result);

				// We want to collect all the models for the .csproj's so we can add them to a .sln file after
				if (!slnProjModels.ContainsKey (outputFile.FullName) && template.OutputFileRule.EndsWith (".csproj"))
					slnProjModels.Add (outputFile.FullName, model);
			}
		}

		if (!string.IsNullOrEmpty (config.SolutionFile)) {
			var slnPath = Path.Combine (config.BasePath ?? AppDomain.CurrentDomain.BaseDirectory, config.SolutionFile);
			var sln = SolutionFileBuilder.Build (config, slnProjModels);
			File.WriteAllText (slnPath, sln);
		}
	}

	static void AssignMetadata (BindingProjectModel project, TemplateConfig template)
	{
		// Calculate metadata from the config file and template file
		var baseMetadata = new Dictionary<string, string> ();

		MergeValues (baseMetadata, project.Config?.Metadata);
		MergeValues (baseMetadata, template.Metadata);

		// Add metadata for artifact
		var artifactMetadata = new Dictionary<string, string> ();
		MergeValues (artifactMetadata, baseMetadata);

		if (project.MavenArtifacts.FirstOrDefault () is MavenArtifactModel artifact)
			MergeValues (artifactMetadata, artifact.MavenArtifactConfig?.Metadata);

		project.Metadata = artifactMetadata;

		foreach (var art in project.MavenArtifacts)
			art.Metadata = artifactMetadata;

		// Add metadata for dependency
		var dependencyMetadata = new Dictionary<string, string> ();
		MergeValues (dependencyMetadata, baseMetadata);

		if (project.NuGetDependencies.FirstOrDefault () is NuGetDependencyModel depMapping)
			MergeValues (dependencyMetadata, depMapping.MavenArtifactConfig?.Metadata);

		foreach (var dep in project.NuGetDependencies) {
			dep.Metadata = dependencyMetadata;
			dep.MavenArtifact!.Metadata = dependencyMetadata;
		}
	}

	static Dictionary<string, string> MergeValues (Dictionary<string, string>? dest, Dictionary<string, string>? src)
	{
		dest = dest ?? new Dictionary<string, string> ();
		if (src != null) {
			foreach (var kvp in src) {
				dest [kvp.Key] = kvp.Value;
			}
		}
		return dest;
	}
}
