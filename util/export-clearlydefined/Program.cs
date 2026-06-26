using Mono.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

const   string  AppName     = "export-clearlydefined";

bool    showHelp	= false;
int     verbosity	= 0;
string? curatedDataPath = null;
string? manifestPath    = null;
var     groupIdPrefixes = new HashSet<string> {
	"com.android",
	"com.google",
	"org.chromium",
	"org.jetbrains",
	"org.tensorflow",
	"org.ow2.asm",
};

var     mavenGoogleGroupIdPrefixes = new HashSet<string> {
	"com.android",
	"org.chromium",
	"com.google",
};

var     excludeGroupIdPrefixes  = new HashSet<string> {
	"androidx",
};

var options     = new OptionSet {
	$"Usage: {AppName} [OPTIONS]+",
	"",
	"Update clearlydefined/curated-data to contain bound packages and versions.",
	"",
	"Available Options:",
	{ "clear-builtin-group-ids",
	  $"Do not process the group id prefixes:\n{string.Join (", ", groupIdPrefixes)}",
	  v => groupIdPrefixes.Clear () },
	{ "g|group-id=",
	  "Process only groups starting with {GROUP_ID}",
	  v => groupIdPrefixes.Add (v) },
	{ "m|manifest=",
	  "{FILE} path to cgmanifest.json to process",
	  v => manifestPath = v },
	{ "o=",
	  "clearlydefined/curated-data checkout {DIRECTORY}",
	  v => curatedDataPath = v },
	{ "v|verbose:",
	  "Increase verbosity of messages, or set verbosity to {LEVEL}",
	  (int? v) => verbosity = v.HasValue ? v.Value : verbosity + 1 },
	{ "h|?|help",
	  "Show this help message and exit",
	  v => showHelp = v != null },
};

try {
	options.Parse (args);
	if (showHelp) {
		options.WriteOptionDescriptions (Console.Out);
		return;
	}

	if (ErrorMissingOption ("o", curatedDataPath) ||
			ErrorMissingOption ("manifest", manifestPath)) {
		return;
	}

	Directory.CreateDirectory (curatedDataPath);

	using var manifest = ReadJsonFile (manifestPath);
	foreach (var registration in manifest.RootElement.GetProperty ("registrations").EnumerateArray ()) {
		if (!registration.TryGetProperty ("component", out var component) ||
				!component.TryGetProperty ("type", out var type) || type.GetString () != "maven" ||
				!component.TryGetProperty ("maven", out var maven)) {
			continue;
		}

		var groupId     = maven.GetProperty ("groupId").GetString ();
		if (ShouldSkipGroupId (groupId)) {
			continue;
		}

		var artifactId  = maven.GetProperty ("artifactId").GetString ();
		var version     = maven.GetProperty ("version").GetString ();
		if (string.IsNullOrWhiteSpace (artifactId) || string.IsNullOrWhiteSpace (version)) {
			continue;
		}
		var license = registration.TryGetProperty ("license", out var licenseProp)
			? licenseProp.GetString ()
			: null;
		AddOtherLicense (groupId, artifactId, version, GetLicenseId (license));
	}
}
catch (OptionException e) {
	Environment.ExitCode = 1;
	Console.Error.WriteLine ($"{AppName}: {(verbosity > 0 ? e.ToString () : e.Message)}");
	Console.Error.WriteLine ($"Try `{AppName} --help` for more information.");
	return;
}

static bool ErrorMissingOption (string option, [NotNullWhen (false)] string? value)
{
	if (!string.IsNullOrWhiteSpace (value)) {
		return false;
	}
	Environment.ExitCode = 1;
	Console.Error.WriteLine ($"{AppName}: Missing required option '{option}'.");
	Console.Error.WriteLine ($"Try `{AppName} --help` for more information.");
	return true;
}

static JsonDocument ReadJsonFile (string path)
{
	var         options = new JsonDocumentOptions {
	};
	using var   file    = File.OpenRead (path);
	return JsonDocument.Parse (file, options);
}

bool ShouldSkipGroupId ([NotNullWhen (false)] string? groupId)
{
	if (string.IsNullOrWhiteSpace (groupId)) {
		return true;
	}
	foreach (var prefix in excludeGroupIdPrefixes) {
		if (groupId.StartsWith (prefix, StringComparison.OrdinalIgnoreCase)) {
			return true;
		}
	}
	foreach (var prefix in groupIdPrefixes) {
		if (groupId.StartsWith (prefix, StringComparison.OrdinalIgnoreCase)) {
			return false;
		}
	}
	return groupIdPrefixes.Count > 0;
}

string GetLicenseId (string? license)
{
	return license?.ToLowerInvariant () switch {
		"the apache software license, version 2.0"
		    => "Apache-2.0",
		"chromium and built-in dependencies"
		    => "BSD-3-Clause",
		_   => "OTHER",
	};
}

void AddOtherLicense (string groupId, string artifactId, string version, string licenseId)
{
	var provider    = GetMavenDirectoryPart (groupId);
	var dir         = Path.Combine (curatedDataPath, "curations", "maven", provider, groupId);
	Directory.CreateDirectory (dir);
	var path        = Path.Combine (dir, $"{artifactId}.yaml");
	var newPath     = !File.Exists (path);

	if (!newPath && YamlHasVersion (path, version)) {
		return;
	}

	using var o = File.AppendText (path);
	if (newPath) {
		o.WriteLine ($"coordinates:");
		o.WriteLine ($"  name: {artifactId}");
		o.WriteLine ($"  namespace: {groupId}");
		o.WriteLine ($"  provider: {provider}");
		o.WriteLine ($"  type: maven");
		o.WriteLine ($"revisions:");
	}
	o.WriteLine ($"  {version}:");
	o.WriteLine ($"    licensed:");
	o.WriteLine ($"      declared: {licenseId}");
}

string GetMavenDirectoryPart (string groupId)
{
	foreach (var prefix in mavenGoogleGroupIdPrefixes) {
		if (groupId.StartsWith (prefix, StringComparison.OrdinalIgnoreCase)) {
			return "mavengoogle";
		}
	}
	return "mavencentral";
}

static bool YamlHasVersion (string path, string version)
{
	var     expectedVersion     = $"  {version}:";
	bool	sawRevisions        = false;

	foreach (var line in File.ReadLines (path)) {
		if (line == "revisions:") {
			sawRevisions = true;
			continue;
		}
		if (!sawRevisions) {
			continue;
		}
		if (line == expectedVersion) {
			return true;
		}
	}
	return false;
}
