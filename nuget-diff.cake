
// SECTION: Arguments and Settings

var ROOT_DIR = MakeAbsolute((DirectoryPath)Argument("root", "."));
var ARTIFACTS_DIR = MakeAbsolute((DirectoryPath)Argument("artifacts", ROOT_DIR.Combine("output").FullPath));
var CACHE_DIR = MakeAbsolute((DirectoryPath)Argument("cache", ROOT_DIR.Combine("externals/api-diff").FullPath));
var OUTPUT_DIR = MakeAbsolute((DirectoryPath)Argument("output", ROOT_DIR.Combine("output/api-diff").FullPath));


// SECTION: Main Script

Information("");
Information("Script Arguments:");
Information("  Root directory: {0}", ROOT_DIR);
Information("  Artifacts directory: {0}", ARTIFACTS_DIR);
Information("  Cache directory: {0}", CACHE_DIR);
Information("  Output directory: {0}", OUTPUT_DIR);
Information("");


// SECTION: Diff NuGets

var nupkgs = GetFiles($"{ARTIFACTS_DIR}/*.nupkg");
if (!nupkgs.Any()) {
	Warning($"##vso[task.logissue type=warning]No NuGet packages were found.");
} else {
	Parallel.ForEach (nupkgs, nupkg => {
		// See https://github.com/xamarin/AndroidX/issues/916
		if (nupkg.FullPath.Contains ("Xamarin.AndroidX.Car.App.App") || nupkg.FullPath.Contains ("Xamarin.AndroidX.Fragment") || nupkg.FullPath.Contains ("Xamarin.AndroidX.Lifecycle.LiveData.Core"))
		  return;
		var version = "--latest";
		var versionFile = nupkg.FullPath + ".baseversion";
		if (FileExists(versionFile)) {
			version = "--version=" + System.IO.File.ReadAllText(versionFile).Trim();
		}
		var exitCode = StartProcess("dotnet", new ProcessSettings {
			Arguments = new ProcessArgumentBuilder()
				.Append("api-tools")
				.Append("nuget-diff")
				.AppendQuoted(nupkg.FullPath)
				.Append(version)
				.Append("--prerelease")
				.Append("--prefer-release")
				.Append("--group-ids")
				.Append("--ignore-unchanged")
				.Append("--compare-nuget-structure")
				.AppendSwitchQuoted("--output", OUTPUT_DIR.FullPath)
				.AppendSwitchQuoted("--cache", CACHE_DIR.Combine("package-cache").FullPath)
		});
		if (exitCode != 0)
			throw new Exception ($"api-tools exited with error code {exitCode} ({nupkg.FullPath}).");
	});
}

// SECTION: Create combined nuget-diff.md

var nu_diffs = GetFiles($"{OUTPUT_DIR.FullPath}/**/nuget-diff.md");

if (nu_diffs.Any()) {
  using (var output = System.IO.File.Create(System.IO.Path.Combine(OUTPUT_DIR.FullPath, "all-nuget-diffs.md"))) {
    foreach (var file in nu_diffs) {
      using (var input = System.IO.File.OpenRead(file.FullPath)) {
          input.CopyTo(output);
          Information("");
      }
      
      // Delete the individual file because it makes the assembly diffs hard to find
      System.IO.File.Delete(file.FullPath);
      
      var dir = System.IO.Path.GetDirectoryName(file.FullPath);
      
      if (System.IO.Directory.GetFileSystemEntries(dir).Length == 0)
          System.IO.Directory.Delete(dir);
    }
  }
}

// SECTION: Upload Diffs

var diffs = GetFiles($"{OUTPUT_DIR}/**/*.md");
if (!diffs.Any()) {
	Warning($"##vso[task.logissue type=warning]No NuGet diffs were found.");
} else {
	var temp = CACHE_DIR.Combine("md-files");
	EnsureDirectoryExists(temp);

	foreach (var diff in diffs) {
		var segments = diff.Segments.Reverse().ToArray();
		var nugetId = segments[2];
		var platform = segments[1];
		var assembly = ((FilePath)segments[0]).GetFilenameWithoutExtension().GetFilenameWithoutExtension();
		var breaking = segments[0].EndsWith(".breaking.md");

		// using non-breaking spaces
		var newName = breaking ? "[BREAKING]   " : "";
		newName += $"{nugetId}    {assembly} ({platform}).md";
		var newPath = temp.CombineWithFilePath(newName);

		CopyFile(diff, newPath);
	}

	var temps = GetFiles($"{temp}/**/*.md");
	foreach (var t in temps.OrderBy(x => x.FullPath)) {
		Information($"##vso[task.uploadsummary]{t}");
	}
}
