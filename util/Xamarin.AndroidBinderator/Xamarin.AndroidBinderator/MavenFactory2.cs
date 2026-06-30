using System;
using System.Collections.Generic;
using System.IO;
using Java.Interop.Tools.Maven.Models;
using Java.Interop.Tools.Maven.Repositories;

namespace AndroidBinderator;

public static class MavenFactory2
{
	static string cache_location = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "maven-cache");
	static readonly Dictionary<string, CachedMavenRepository> repositories = new Dictionary<string, CachedMavenRepository> ();
	static readonly Dictionary<string, ProjectResolver> project_resolvers = new Dictionary<string, ProjectResolver> ();

	public static CachedMavenRepository GetMavenRepository (BindingConfig config, MavenArtifactConfig artifact)
	{
		var (type, location) = GetMavenInfoForArtifact (config, artifact);
		var repo = GetOrCreateRepository (type, location);

		return repo;
	}

	public static ProjectResolver GetProjectResolver (BindingConfig config, MavenArtifactConfig artifact)
	{
		var (type, location) = GetMavenInfoForArtifact (config, artifact);
		var resolver = GetOrCreateProjectResolver (type, location);

		return resolver;
	}

	public static Project GetPomForArtifact (BindingConfig config, MavenArtifactConfig artifact)
	{
		var resolver = GetProjectResolver (config, artifact);
		var project = resolver.Resolve (artifact.ToArtifact ());

		return project;
	}

	public static Project GetPomForArtifactParent (BindingConfig config, Artifact parentArtifact, MavenArtifactConfig originalArtifact)
	{
		// We assume the parent artifact is in the same repository as the original artifact
		var resolver = GetProjectResolver (config, originalArtifact);
		var project = resolver.Resolve (parentArtifact);

		return project;
	}

	static (MavenRepoType type, string location) GetMavenInfoForArtifact (BindingConfig config, MavenArtifactConfig artifact)
	{
		// Precendence: Artifact > TemplateSet > Config
		if (artifact.MavenRepositoryType.HasValue)
			return (artifact.MavenRepositoryType.Value, artifact.MavenRepositoryLocation!);

		var template = config.GetTemplateSet (artifact.TemplateSet);

		if (template.MavenRepositoryType.HasValue)
			return (template.MavenRepositoryType.Value, template.MavenRepositoryLocation!);

		return (config.MavenRepositoryType, config.MavenRepositoryLocation!);
	}

	static CachedMavenRepository GetOrCreateRepository (MavenRepoType type, string location)
	{
		var key = $"{type}|{location}";

		if (repositories.TryGetValue (key, out var repository))
			return repository;

		MavenRepository maven;

		if (type == MavenRepoType.Directory)
			throw new ArgumentException ("Directory repository type not supported");
		else if (type == MavenRepoType.Url)
			maven = new MavenRepository (location, new Uri(location).GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Scheme, UriFormat.UriEscaped));
		else if (type == MavenRepoType.MavenCentral)
			maven = MavenRepository.Central;
		else
			maven = MavenRepository.Google;

		var cached_maven = new CachedMavenRepository (cache_location, maven);

		repositories.Add (key, cached_maven);

		return cached_maven;
	}

	static ProjectResolver GetOrCreateProjectResolver (MavenRepoType type, string location)
	{
		var key = $"{type}|{location}";

		if (project_resolvers.TryGetValue (key, out ProjectResolver? pr))
			return pr;

		var resolver = new ProjectResolver (GetOrCreateRepository (type, location));

		project_resolvers.Add (key, resolver);

		return resolver;
	}
}
