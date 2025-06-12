// Contains tasks for running binderator

var binderator_project = "util/Xamarin.AndroidBinderator/Xamarin.AndroidBinderator.Tool/Xamarin.AndroidBinderator.Tool.csproj";

// Runs `binderator`
Task ("binderate")
    .IsDependentOn ("javadocs-gps")
    .Does (() =>
{
    EnsureDirectoryExists("./output");
       
    var configFile = MakeAbsolute (new FilePath ("./config.json")).FullPath;
    var basePath = MakeAbsolute (new DirectoryPath ("./")).FullPath;

    // Run the binderator project
    var args = new ProcessArgumentBuilder ()
        .Append ("binderate")
        .Append ("--config-file")
        .Append (configFile)
        .Append ("--base-path")
        .Append (basePath);
    
    DotNetRun (binderator_project, args);

    // format the targets file so they are pretty in the package
    var targetsFiles = GetFiles ("generated/**/*.targets");
    var xmlns = (XNamespace)"http://schemas.microsoft.com/developer/msbuild/2003";
    foreach (var targets in targetsFiles) {
        var xdoc = XDocument.Load (targets.FullPath);
        xdoc.Save (targets.FullPath);
    }

    // different lint.jar files in artifacts causing R8 errors
    foreach (var file in GetFiles ("./externals/**/lint.jar")) {
        Information($"Deleting: {file}");
        DeleteFile(file);

        foreach (var aar in GetFiles ($"{file.GetDirectory ()}/../*.aar")) {
            Information ($"Deleting: lint.jar from {aar}");
            using (var zipFile = new ICSharpCode.SharpZipLib.Zip.ZipFile (aar.ToString ())) {
                zipFile.BeginUpdate ();
                var entry = zipFile.GetEntry ("lint.jar");
                if (entry != null) {
                    Information ($"		Deleting lint.jar from {aar}");
                    zipFile.Delete (entry);
                }
                zipFile.CommitUpdate ();
            }
        }
    }
});
