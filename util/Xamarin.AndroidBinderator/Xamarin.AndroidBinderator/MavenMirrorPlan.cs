using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Java.Interop.Tools.Maven.Models;

namespace AndroidBinderator;

public sealed class MavenMirrorPlan
{
	public List<MirrorArtifact> Artifacts { get; } = [];
	public List<string> Errors { get; } = [];
	public HashSet<string> MissingMappings { get; } = [];
	public bool Complete => Errors.Count == 0;
	public string Scope => "PR version diff, newer stable candidates (including frozen), CI failures, and parent/BOM + compile/runtime closure; includes optional dependencies without exclusions or conflict mediation.";
	public string Disclaimer => "Feed availability does not mean CI is fixed.";

	public static async Task<MavenMirrorPlan> Create (BindingConfig config, BindingConfig? baseline,
		IReadOnlyDictionary<string, string> latest, string ciLog, HttpClient client)
	{
		var plan = new MavenMirrorPlan ();
		var resolver = new MirrorResolver (config, plan, client);
		foreach (var art in config.MavenArtifacts) {
			if (baseline is null || !baseline.MavenArtifacts.Any (a => a.GroupId == art.GroupId && a.ArtifactId == art.ArtifactId && a.Version == art.Version))
				resolver.Add (art.GroupId!, art.ArtifactId!, art.Version, "PR diff head version", payload: true);
			if (latest.TryGetValue ($"{art.GroupId}:{art.ArtifactId}", out var version))
				resolver.Add (art.GroupId!, art.ArtifactId!, version, art.Frozen ? "latest stable (held/frozen)" : "latest stable", payload: true);
		}
		if (baseline is not null) {
			foreach (var art in baseline.MavenArtifacts.Where (a => !config.MavenArtifacts.Any (
				h => h.GroupId == a.GroupId && h.ArtifactId == a.ArtifactId && h.Version == a.Version)))
				resolver.Add (art.GroupId!, art.ArtifactId!, art.Version, "PR diff base version", payload: true);
		}
		foreach (var coordinate in ParseCiCoordinates (ciLog))
			resolver.Add (coordinate.GroupId, coordinate.Id, coordinate.Version, "CI failure", payload: true);
		foreach (Match diagnostic in Regex.Matches (ciLog, @"(?im)No matching artifact config found for:[^\r\n]*(?:\r?\n[^\r\n]*){0,2}"))
			if (!ParseCiCoordinates (diagnostic.Value).Any ())
				plan.Errors.Add ($"Unparsed CI dependency failure: {diagnostic.Value.Trim ()}");
		await resolver.Run ();
		return plan;
	}

	public static IEnumerable<Artifact> ParseCiCoordinates (string text)
	{
		var found = new Dictionary<string, Artifact> ();
		// Build logs also list successful downloads and entire configs. Only failure diagnostics
		// and their continuation lines contribute candidates, not every coordinate in the log.
		text = string.Join ("\n", Regex.Matches (text,
			@"(?im)^.*(?:No matching artifact config found|to satisfy dependency of|No POM found|Could not|Unable to|\berror\b|\bfailed\b|\b40[134]\b|\b50[0234]\b).*(?:\r?\n[^\r\n]*){0,2}")
			.Select (m => m.Value));
		// Binderator coordinates, including versions in "No matching artifact config found" blocks.
		foreach (Match match in Regex.Matches (text, @"(?<![\w:/])([A-Za-z_][\w.-]*):([\w.-]+):(\[?[0-9][\w.+,\[\]()-]*)(?![\w])")) {
			var artifact = new Artifact (match.Groups [1].Value, match.Groups [2].Value, match.Groups [3].Value);
			found.TryAdd (artifact.VersionedArtifactString, artifact);
		}
		// HTTP failures may contain only a Maven URL, not a colon-separated coordinate.
		foreach (Match match in Regex.Matches (text, @"https://[^\s'""<>]+/(?:maven/v1|maven2)/([^\s'""<>?]+)")) {
			var parts = match.Groups [1].Value.TrimEnd (')', '.', ',').Split ('/');
			if (parts.Length < 4)
				continue;
			var artifact = new Artifact (string.Join (".", parts.Take (parts.Length - 3)), parts [^3], parts [^2]);
			found.TryAdd (artifact.VersionedArtifactString, artifact);
		}
		return found.Values;
	}
}

public sealed class MirrorArtifact
{
	public string Coordinate { get; set; } = "";
	public List<string> Reasons { get; } = [];
	public List<MirrorFile> Files { get; } = [];
	public string? Error { get; set; }
}

public sealed class MirrorFile
{
	public string Path { get; set; } = "";
	public string Upstream { get; set; } = "";
	public string Status { get; set; } = "pending";
}

internal sealed class MirrorResolver
{
	const string google = "https://dl.google.com/dl/android/maven2";
	const string central = "https://repo.maven.apache.org/maven2";
	readonly BindingConfig config;
	readonly MavenMirrorPlan plan;
	readonly HttpClient client;
	readonly Dictionary<string, string?> responses = [];
	readonly Dictionary<string, MirrorArtifact> artifacts = [];
	readonly Dictionary<string, EffectivePom> poms = [];
	readonly HashSet<string> resolving = [];
	readonly Queue<(Artifact artifact, string extension, string classifier)> pending = [];
	readonly HashSet<string> visited = [];
	readonly Dictionary<string, string[]> repositories = [];

	internal MirrorResolver (BindingConfig config, MavenMirrorPlan plan, HttpClient client)
	{
		this.config = config;
		this.plan = plan;
		this.client = client;
	}

	internal void Add (string group, string id, string version, string reason, bool payload, string extension = "", string classifier = "")
	{
		Validate (group, id, version);
		if (!Regex.IsMatch (classifier, @"^[\w.-]*$") || classifier.Contains (".."))
			throw new InvalidOperationException ($"Unsupported dependency classifier '{classifier}'");
		var artifact = new Artifact (group, id, version);
		var entry = Entry (artifact);
		if (!entry.Reasons.Contains (reason))
			entry.Reasons.Add (reason);
		if (payload)
			pending.Enqueue ((artifact, extension, classifier));
	}

	MirrorArtifact Entry (Artifact artifact)
	{
		if (!artifacts.TryGetValue (artifact.VersionedArtifactString, out var entry)) {
			entry = new MirrorArtifact { Coordinate = artifact.VersionedArtifactString };
			artifacts.Add (entry.Coordinate, entry);
			plan.Artifacts.Add (entry);
		}
		return entry;
	}

	internal async Task Run ()
	{
		while (pending.TryDequeue (out var next)) {
			var key = $"{next.artifact}:{next.extension}:{next.classifier}";
			if (!visited.Add (key))
				continue;
			var mapping = config.MavenArtifacts.Any (a => a.GroupId == next.artifact.GroupId && a.ArtifactId == next.artifact.Id && a.Version == next.artifact.Version);
			if (Entry (next.artifact).Reasons.Contains ("CI failure") && !mapping)
				plan.MissingMappings.Add ($"{next.artifact} (named in CI; no exact config mapping)");
			try {
				var (xml, repository) = await GetPom (next.artifact);
				var root = XDocument.Parse (xml).Root ?? throw new InvalidOperationException ("Empty POM");
				if (root.Name.LocalName != "project")
					throw new InvalidOperationException ($"Response is not a Maven POM for {next.artifact}");
				var rawPackaging = Value (root, "packaging").OrDefault ("jar");
				if (!rawPackaging.Contains ("${"))
					AddPayload (next.artifact, next.extension.OrDefault (rawPackaging), next.classifier, repository);
				var pom = await Resolve (next.artifact);
				AddPayload (next.artifact, next.extension.OrDefault (pom.Packaging), next.classifier, pom.Repository);
				foreach (var dep in pom.Dependencies.Values) {
					try {
						pom.Managed.TryGetValue (dep.Key, out var defaults);
						var scope = ExpandProperties (dep.Scope.OrDefault (defaults?.Scope ?? ""), pom.Properties);
						if (scope != "" && scope != "compile" && scope != "runtime" && scope != "test" && scope != "provided" && scope != "system")
							throw new InvalidOperationException ($"Unsupported or unresolved dependency scope '{scope}' for {dep.Group}:{dep.Id}");
						if (scope != "" && scope != "compile" && scope != "runtime")
							continue;
						var requirementVersion = ExpandProperties (dep.Version.OrDefault (defaults?.Version ?? ""), pom.Properties);
						var version = await ResolveVersion (dep.Group, dep.Id, requirementVersion);
						Add (dep.Group, dep.Id, version, $"dependency of {next.artifact}", true, dep.Type, dep.Classifier);
						var requirement = new Dependency { GroupId = dep.Group, ArtifactId = dep.Id, Version = requirementVersion };
						if (!config.MavenArtifacts.Any (a => a.GroupId == dep.Group && a.ArtifactId == dep.Id && requirement.Satisfies (a.Version))) {
							plan.MissingMappings.Add ($"{dep.Group}:{dep.Id}:{requirementVersion} (required by {next.artifact})");
						}
					} catch (Exception ex) when (IsResolutionError (ex)) {
						RecordError (next.artifact, ex.Message);
					}
				}
			} catch (Exception ex) when (IsResolutionError (ex)) {
				RecordError (next.artifact, ex.Message);
			}
		}
	}

	static bool IsResolutionError (Exception ex) => ex is InvalidOperationException || ex is HttpRequestException || ex is System.Xml.XmlException || ex is ArgumentException || ex is TaskCanceledException;

	void RecordError (Artifact artifact, string message)
	{
		Entry (artifact).Error = message;
		var error = $"{artifact}: {message}";
		if (!plan.Errors.Contains (error))
			plan.Errors.Add (error);
	}

	void AddPayload (Artifact artifact, string packaging, string classifier, string repository)
	{
		if (packaging == "pom")
			return;
		var extension = packaging == "bundle" || packaging == "test-jar" ? "jar" : packaging;
		var suffix = classifier.HasValue () ? $"-{classifier}" : packaging == "test-jar" ? "-tests" : "";
		if (extension != "jar" && extension != "aar")
			throw new InvalidOperationException ($"Unsupported payload packaging '{packaging}'");
		AddFile (artifact, $"{artifact.Id}-{artifact.Version}{suffix}.{extension}", repository);
	}

	async Task<EffectivePom> Resolve (Artifact artifact)
	{
		Validate (artifact.GroupId, artifact.Id, artifact.Version);
		var key = artifact.VersionedArtifactString;
		if (poms.TryGetValue (key, out var existing))
			return existing;
		if (!resolving.Add (key))
			throw new InvalidOperationException ($"Cyclic parent/BOM reference at {key}");
		try {
			var (xml, repository) = await GetPom (artifact);
			var root = XDocument.Parse (xml).Root ?? throw new InvalidOperationException ("Empty POM");
			if (root.Name.LocalName != "project")
				throw new InvalidOperationException ($"Response is not a Maven POM for {artifact}");
			var ns = root.Name.Namespace;
			var properties = new Dictionary<string, string> ();
			var managed = new Dictionary<string, PomDependency> ();
			var dependencies = new Dictionary<string, PomDependency> ();
			var parent = root.Element (ns + "parent");
			if (parent is not null) {
				var pa = new Artifact (Value (parent, "groupId"), Value (parent, "artifactId"), Value (parent, "version"));
				Add (pa.GroupId, pa.Id, pa.Version, $"parent of {artifact}", false);
				var inherited = await Resolve (pa);
				properties = new (inherited.Properties);
				managed = new (inherited.Managed);
				dependencies = new (inherited.Dependencies);
			}
			foreach (var property in root.Element (ns + "properties")?.Elements () ?? [])
				properties [property.Name.LocalName] = property.Value.Trim ();
			foreach (var prefix in new [] { "project", "pom" }) {
				properties [$"{prefix}.groupId"] = artifact.GroupId;
				properties [$"{prefix}.artifactId"] = artifact.Id;
				properties [$"{prefix}.version"] = artifact.Version;
				if (parent is not null) {
					properties [$"{prefix}.parent.groupId"] = Value (parent, "groupId");
					properties [$"{prefix}.parent.version"] = Value (parent, "version");
				}
			}
			string Expand (string value) => ExpandProperties (value, properties);
			PomDependency Read (XElement element)
			{
				return new (Expand (Value (element, "groupId")), Expand (Value (element, "artifactId")),
					Value (element, "version"), Value (element, "scope"),
					Expand (Value (element, "type")), Expand (Value (element, "classifier")));
			}
			var management = root.Element (ns + "dependencyManagement")?.Element (ns + "dependencies")?.Elements (ns + "dependency").Select (Read).ToList () ?? [];
			var imports = new Dictionary<string, PomDependency> ();
			foreach (var dep in management.Where (d => Expand (d.Scope) == "import" && d.Type == "pom")) {
				var version = await ResolveVersion (dep.Group, dep.Id, Expand (dep.Version));
				var bom = new Artifact (dep.Group, dep.Id, version);
				Add (bom.GroupId, bom.Id, bom.Version, $"BOM imported by {artifact}", false);
				var imported = await Resolve (bom);
				foreach (var item in imported.Managed)
					imports.TryAdd (item.Key, item.Value with {
						Version = ExpandProperties (item.Value.Version, imported.Properties),
						Scope = ExpandProperties (item.Value.Scope, imported.Properties)
					});
			}
			foreach (var item in imports)
				managed [item.Key] = item.Value;
			foreach (var dep in management.Where (d => Expand (d.Scope) != "import"))
				managed [dep.Key] = dep;
			foreach (var element in root.Element (ns + "dependencies")?.Elements (ns + "dependency") ?? []) {
				var dep = Read (element);
				dependencies [dep.Key] = dep;
			}
			var profileDependencies = root.Element (ns + "profiles")?.Elements (ns + "profile").SelectMany (p =>
				(p.Element (ns + "dependencies")?.Elements (ns + "dependency") ?? []).Concat (
					p.Element (ns + "dependencyManagement")?.Element (ns + "dependencies")?.Elements (ns + "dependency") ?? []));
			if (profileDependencies?.Any (d => Value (d, "scope") is not ("test" or "provided" or "system")) == true)
				throw new InvalidOperationException ("POM contains profile-dependent dependencies; profile evaluation is not supported");
			var relocation = root.Element (ns + "distributionManagement")?.Element (ns + "relocation");
			if (relocation is not null)
				Add (Expand (Value (relocation, "groupId")).OrDefault (artifact.GroupId),
					Expand (Value (relocation, "artifactId")).OrDefault (artifact.Id),
					Expand (Value (relocation, "version")).OrDefault (artifact.Version), $"relocation of {artifact}", true);
			var result = new EffectivePom (properties, managed, dependencies, Expand (Value (root, "packaging")).OrDefault ("jar"), repository);
			poms.Add (key, result);
			return result;
		} finally {
			resolving.Remove (key);
		}
	}

	static string Value (XElement element, string name) => element.Element (element.Name.Namespace + name)?.Value.Trim () ?? "";

	internal static string ExpandProperties (string value, Dictionary<string, string> properties)
	{
		for (var i = 0; i < 30 && value.Contains ("${"); i++) {
			var expanded = Regex.Replace (value, @"\$\{([^}]+)\}", m => properties.TryGetValue (m.Groups [1].Value, out var replacement) ? replacement : m.Value);
			if (expanded == value)
				break;
			value = expanded;
		}
		return value;
	}

	async Task<string> ResolveVersion (string group, string id, string version)
	{
		if (version.Contains ("${") || !version.HasValue () || version.StartsWith ("unspecified", StringComparison.OrdinalIgnoreCase))
			throw new InvalidOperationException ($"Unresolved dependency version {group}:{id}:{version}");
		if (!version.Contains ('[') && !version.Contains ('('))
			return version;
		var ranges = MavenVersionRange.Parse (version).ToList ();
		foreach (var repository in Repositories (group, id)) {
			var text = await Download ($"{repository}/{group.Replace ('.', '/')}/{id}/maven-metadata.xml");
			if (text is null)
				continue;
			var match = XDocument.Parse (text).Descendants ("version").Select (e => e.Value)
				.Where (v => ranges.Any (r => r.ContainsVersion (MavenVersion.Parse (v))))
				.OrderByDescending (v => MavenVersion.Parse (v)).FirstOrDefault ();
			if (match is not null)
				return match;
		}
		throw new InvalidOperationException ($"No upstream version satisfies {group}:{id}:{version}");
	}

	async Task<(string xml, string repository)> GetPom (Artifact artifact)
	{
		var filename = $"{artifact.Id}-{artifact.Version}.pom";
		foreach (var repository in Repositories (artifact.GroupId, artifact.Id)) {
			var path = ArtifactPath (artifact, filename);
			var text = await Download ($"{repository}/{path}");
			if (text is null)
				continue;
			AddFile (artifact, filename, repository);
			return (text, repository);
		}
		Entry (artifact).Error = "POM unavailable upstream";
		throw new InvalidOperationException ($"POM unavailable upstream for {artifact}");
	}

	string[] Repositories (string group, string id)
	{
		var key = $"{group}:{id}";
		if (repositories.TryGetValue (key, out var cached))
			return cached;
		var art = config.MavenArtifacts.FirstOrDefault (a => a.GroupId == group && a.ArtifactId == id);
		var template = art is null ? null : config.GetTemplateSet (art.TemplateSet);
		var type = art?.MavenRepositoryType ?? template?.MavenRepositoryType ?? config.MavenRepositoryType;
		var location = art?.MavenRepositoryType is not null ? art.MavenRepositoryLocation :
			template?.MavenRepositoryType is not null ? template.MavenRepositoryLocation : config.MavenRepositoryLocation;
		var preferred = type switch {
			MavenRepoType.Google => google,
			MavenRepoType.MavenCentral => central,
			MavenRepoType.Url when Uri.TryCreate (location, UriKind.Absolute, out var url) && url.Scheme == "https" => location!.TrimEnd ('/'),
			_ => throw new InvalidOperationException ($"Unsupported upstream repository for {key}")
		};
		return repositories [key] = new [] { preferred, google, central }.Distinct ().ToArray ();
	}

	void AddFile (Artifact artifact, string filename, string repository)
	{
		var entry = Entry (artifact);
		var path = ArtifactPath (artifact, filename);
		if (!entry.Files.Any (f => f.Path == path))
			entry.Files.Add (new MirrorFile { Path = path, Upstream = $"{repository}/{path}" });
	}

	static string ArtifactPath (Artifact artifact, string filename) => $"{artifact.GroupId.Replace ('.', '/')}/{artifact.Id}/{artifact.Version}/{filename}";

	static void Validate (string group, string id, string version)
	{
		if (!Regex.IsMatch (group, @"^[\w.-]+$") || !Regex.IsMatch (id, @"^[\w.-]+$") || !Regex.IsMatch (version, @"^[\w.+-]+$") ||
			group.Contains ("..") || id.Contains ("..") || version.Contains (".."))
			throw new InvalidOperationException ($"Invalid or unresolved Maven coordinate '{group}:{id}:{version}'");
	}

	sealed record EffectivePom (Dictionary<string, string> Properties, Dictionary<string, PomDependency> Managed,
		Dictionary<string, PomDependency> Dependencies, string Packaging, string Repository);
	sealed record PomDependency (string Group, string Id, string Version, string Scope, string Type, string Classifier)
	{
		public string Key => $"{Group}:{Id}:{Type.OrDefault ("jar")}:{Classifier}";
	}

	async Task<string?> Download (string url)
	{
		if (responses.TryGetValue (url, out var cached))
			return cached;
		for (var attempt = 0; ; attempt++) {
			try {
				using var response = await client.GetAsync (url);
				if (response.StatusCode == HttpStatusCode.NotFound)
					return responses [url] = null;
				if ((response.StatusCode == HttpStatusCode.RequestTimeout || (int) response.StatusCode == 429 || (int) response.StatusCode >= 500) && attempt < 3) {
					await Task.Delay (TimeSpan.FromSeconds (attempt + 1));
					continue;
				}
				response.EnsureSuccessStatusCode ();
				return responses [url] = await response.Content.ReadAsStringAsync ();
			} catch (HttpRequestException ex) when (attempt < 3 && ex.StatusCode is null) {
				await Task.Delay (TimeSpan.FromSeconds (attempt + 1));
			} catch (TaskCanceledException) when (attempt < 3) {
				await Task.Delay (TimeSpan.FromSeconds (attempt + 1));
			}
		}

	}
}

internal static class MirrorStringExtensions
{
	public static string OrDefault (this string value, string fallback) => value.HasValue () ? value : fallback;
}
