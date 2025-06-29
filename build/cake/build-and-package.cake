// Contains tasks for building and packaging projects

// Builds the NuGet packages
Task ("nuget")
    .IsDependentOn ("libs")
    .Does (() =>
{
    var settings = new DotNetMSBuildSettings ()
        .SetConfiguration (CONFIGURATION)
        .EnableBinaryLogger ($"./output/nuget.{CONFIGURATION}.binlog")
        .WithProperty ("NoBuild", "true")
        .WithProperty ("PackageOutputPath", MakeAbsolute ((DirectoryPath)"./output/").FullPath)
        .WithTarget ("Pack");

    settings.NodeReuse = false;

    DotNetBuild (
        "./generated/AndroidX.sln", 
        new DotNetBuildSettings { MSBuildSettings = settings }
    );
});

// Builds the .csproj projects
Task ("libs")
    .IsDependentOn("metadata-verify")
    .IsDependentOn ("libs-native")
    .Does(() =>
{
    var settings = new DotNetMSBuildSettings ()
        .SetConfiguration (CONFIGURATION)
        .EnableBinaryLogger ($"./output/libs.{CONFIGURATION}.binlog")
        .WithProperty("Verbosity", VERBOSITY.ToString ());

    settings.NodeReuse = false;

    DotNetBuild (
        "./generated/AndroidX.sln", 
        new DotNetBuildSettings { MSBuildSettings = settings }
    );
});

// Builds the Java libraries
Task ("libs-native")
    .Does (() =>
{
    // com.xamarin.google.android.material.extensions
    BuildGradleProject (
      "./source/com.google.android.material/material.extensions/",
      "./externals/com.xamarin.google.android.material.extensions/",
      false
    );
    
    // com.google.android.play/core.extensions
    BuildGradleProject (
      "./source/com.google.android.play/core.extensions/",
      "./externals/com.xamarin.google.android.play.core.extensions/",
      true
    );
    
    // com.google.android.play/asset.delivery.extensions
    BuildGradleProject (
      "./source/com.google.android.play/asset.delivery.extensions/",
      "./externals/com.xamarin.google.android.play.asset.delivery.extensions/",
      true
    );
      
    // com.google.android.play/feature.delivery.extensions
    BuildGradleProject (
      "./source/com.google.android.play/feature.delivery.extensions/",
      "./externals/com.xamarin.google.android.play.feature.delivery.extensions/",
      true
    );
});

void BuildGradleProject (string root, string outputDir, bool moveFile)
{
    RunGradle (root, "build");
    
    EnsureDirectoryExists (outputDir);
    CleanDirectories (outputDir);

    var full_file_path = $"{root}extensions-aar/build/outputs/aar/extensions-aar-release.aar";
    var file_name = System.IO.Path.GetFileName (full_file_path);
    
    CopyFileToDirectory (full_file_path, outputDir);
    Unzip($"{outputDir}/{file_name}", outputDir);
    
    if (moveFile)
        MoveFile ($"{outputDir}/classes.jar", $"{outputDir}/extensions.jar");
}

void RunGradle (DirectoryPath root, string target)
{
    root = MakeAbsolute (root);
    
    var proc = IsRunningOnWindows ()
        ? root.CombineWithFilePath ("gradlew.bat").FullPath
        : "bash";
    var args = IsRunningOnWindows ()
        ? ""
        : root.CombineWithFilePath ("gradlew").FullPath;
        
    args += $" {target} -p {root}";

    var exitCode = StartProcess (proc, args);
    
    if (exitCode != 0)
        throw new Exception ($"Gradle exited with code {exitCode}.");
}
