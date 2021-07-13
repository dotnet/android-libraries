// Tools needed by cake addins
#tool nuget:?package=Cake.CoreCLR
#tool nuget:?package=vswhere&version=2.8.4

// Cake Addins
#addin nuget:?package=Cake.FileHelpers&version=4.0.1
#addin nuget:?package=Newtonsoft.Json&version=12.0.3
#addin nuget:?package=Cake.MonoApiTools&version=3.0.5
#addin nuget:?package=CsvHelper&version=12.2.1
#addin nuget:?package=SharpZipLib&version=1.2.0

// #addin nuget:?package=NuGet.Protocol&loaddependencies=true&version=5.6.0
// #addin nuget:?package=NuGet.Versioning&loaddependencies=true&version=5.6.0
// #addin nuget:?package=Microsoft.Extensions.Logging&loaddependencies=true&version=3.0.0

using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CsvHelper;

// The main configuration points
var TARGET = Argument ("t", Argument ("target", "Default"));
var CONFIGURATION = Argument ("c", Argument ("configuration", "Release"));
var VERBOSITY = Argument ("v", Argument ("verbosity", Verbosity.Normal));

// Lists all the artifacts and their versions for com.android.support.*
// https://dl.google.com/dl/android/maven2/com/android/support/group-index.xml
// Master list of all the packages in the repo:
// https://dl.google.com/dl/android/maven2/master-index.xml

var REF_DOCS_URL = "https://bosstoragemirror.blob.core.windows.net/android-docs-scraper/a7/a712886a8b4ee709f32d51823223039883d38734/androidx.zip";

// In order to create the type mapping, we need to get the AndroidSupport.Merged.dll
var SUPPORT_MERGED_DLL_URL = EnvironmentVariable("SUPPORT_MERGED_DLL_URL") ?? $"https://github.com/xamarin/AndroidSupportComponents/releases/download/28.0.0.3/AndroidSupport.Merged.dll";

var JAVA_INTEROP_ZIP_URL = "https://github.com/xamarin/java.interop/archive/d16-7.zip";

var SUPPORT_CONFIG_URL = "https://raw.githubusercontent.com/xamarin/AndroidSupportComponents/master/config.json";

var VISUAL_STUDIO_ROOT = EnvironmentVariable ("VISUAL_STUDIO_ROOT") ?? Argument ("vs", "");
if (IsRunningOnWindows() && string.IsNullOrEmpty(VISUAL_STUDIO_ROOT))
    VISUAL_STUDIO_ROOT = VSWhereLatest(new VSWhereLatestSettings { Requires = "Component.Xamarin", IncludePrerelease = true }).FullPath;

// Resolve Xamarin.Android installation
var XAMARIN_ANDROID_PATH = EnvironmentVariable ("XAMARIN_ANDROID_PATH");
var ANDROID_SDK_BASE_VERSION = "v1.0";
var ANDROID_SDK_VERSION = "v10.0";
var AndroidSdkBuildTools = $"29.0.2";

if (string.IsNullOrEmpty(XAMARIN_ANDROID_PATH)) {
    if (IsRunningOnWindows()) {
        XAMARIN_ANDROID_PATH = VISUAL_STUDIO_ROOT + @"\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid";
    } else {
        if (DirectoryExists("/Library/Frameworks/Xamarin.Android.framework/Versions/Current/lib/xamarin.android/xbuild-frameworks/MonoAndroid"))
            XAMARIN_ANDROID_PATH = "/Library/Frameworks/Xamarin.Android.framework/Versions/Current/lib/xamarin.android/xbuild-frameworks/MonoAndroid";
        else
            XAMARIN_ANDROID_PATH = "/Library/Frameworks/Xamarin.Android.framework/Versions/Current/lib/xbuild-frameworks/MonoAndroid";
    }
}
if (!DirectoryExists($"{XAMARIN_ANDROID_PATH}/{ANDROID_SDK_VERSION}"))
    throw new Exception($"Unable to find Xamarin.Android {ANDROID_SDK_VERSION} at {XAMARIN_ANDROID_PATH}.");

// Load all the git variables
var BUILD_COMMIT = EnvironmentVariable("BUILD_COMMIT") ?? "DEV";
var BUILD_NUMBER = EnvironmentVariable("BUILD_NUMBER") ?? "DEBUG";
var BUILD_TIMESTAMP = DateTime.UtcNow.ToString();

var REQUIRED_DOTNET_TOOLS = new [] {
    "xamarin-android-binderator",
    "xamarin.androidx.migration.tool"
};

string JAVA_HOME = EnvironmentVariable ("JAVA_HOME") ?? Argument ("java_home", "");
string ANDROID_HOME = EnvironmentVariable ("ANDROID_HOME") ?? Argument ("android_home", "");
string ANDROID_SDK_ROOT = EnvironmentVariable ("ANDROID_SDK_ROOT") ?? Argument ("android_sdk_root", "");
string MSBUILD_PATH = IsRunningOnWindows() && !string.IsNullOrEmpty(VISUAL_STUDIO_ROOT)
    ? VISUAL_STUDIO_ROOT + @"\MSBuild\Current\Bin\MSBuild.exe"
    : null;

// Log some variables
Information ($"JAVA_HOME            : {JAVA_HOME}");
Information ($"ANDROID_HOME         : {ANDROID_HOME}");
Information ($"ANDROID_SDK_ROOT     : {ANDROID_SDK_ROOT}");
Information ($"VISUAL_STUDIO_ROOT   : {VISUAL_STUDIO_ROOT}");
Information ($"XAMARIN_ANDROID_PATH : {XAMARIN_ANDROID_PATH}");
Information ($"MSBUILD_PATH         : {MSBUILD_PATH}");
Information ($"ANDROID_SDK_VERSION  : {ANDROID_SDK_VERSION}");
Information ($"BUILD_COMMIT         : {BUILD_COMMIT}");
Information ($"BUILD_NUMBER         : {BUILD_NUMBER}");
Information ($"BUILD_TIMESTAMP      : {BUILD_TIMESTAMP}");


// You shouldn't have to configure anything below here
// ######################################################

var MIGRATION_PACKAGE_VERSION = GetNuGetVersion("Xamarin.AndroidX.Migration");
var MULTIDEX_PACKAGE_VERSION = GetNuGetVersion("Xamarin.AndroidX.MultiDex");

void RunProcess(FilePath fileName, string processArguments)
{
    var exitCode = StartProcess(fileName, processArguments);
    if (exitCode != 0)
        throw new Exception ($"Process {fileName} exited with code {exitCode}.");
}

string[] RunProcessWithOutput(FilePath fileName, string processArguments)
{
    var exitCode = StartProcess(fileName, new ProcessSettings {
        Arguments = processArguments,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    }, out var procOut);
    if (exitCode != 0)
        throw new Exception ($"Process {fileName} exited with code {exitCode}.");
    return procOut.ToArray();;
}

void RunGradle(DirectoryPath root, string target)
{
    root = MakeAbsolute(root);
    var proc = IsRunningOnWindows()
        ? root.CombineWithFilePath("gradlew.bat").FullPath
        : "bash";
    var args = IsRunningOnWindows()
        ? ""
        : root.CombineWithFilePath("gradlew").FullPath;
    args += $" {target} -p {root}";

    var exitCode = StartProcess(proc, args);
    if (exitCode != 0)
        throw new Exception($"Gradle exited with code {exitCode}.");
}

string GetNuGetVersion(string nugetId, string configJson = "./config.json")
{
    var json = JToken.Parse(FileReadText(configJson));

    if (json.Type == JTokenType.Array)
        json = ((JArray)json)[0];

    var artifacts = json["artifacts"];
    var artifact = artifacts.FirstOrDefault(j => (string)j["nugetId"] == nugetId);

    if (artifact == null)
    {
        /*
        NEEDS INVESTIGATION
        nugetID Xamarin.Jetbrains.Annotations was found as dependency, but it was not listed in config.json

        buildTransitive might be the issue.
        
        workaround was to add following snippet to config.json:

        {
            "groupId": "org.jetbrains",
            "artifactId": "annotations",
            "version": "13.0",
            "nugetVersion": "13.0.0.4",
            "nugetId": "Xamarin.Jetbrains.Annotations",
            "dependencyOnly": true
        },
        */
    }

    return (string)(artifact["nugetVersion"] ?? artifact["version"]);
}

string GetNuGetId(string assemblyName, string configJson = "./config.json")
{
    var json = JToken.Parse(FileReadText(configJson));

    if (json.Type == JTokenType.Array)
        json = ((JArray)json)[0];

    var artifacts = json["artifacts"];
    var artifact =
        artifacts.FirstOrDefault(j => (string)j["assemblyName"] == assemblyName) ??
        artifacts.FirstOrDefault(j => (string)j["nugetId"] == assemblyName);

    if (artifact == null)
        return assemblyName;

    return (string)artifact["nugetId"];
}

IEnumerable<string> GetArtifacts(string configJson = "./config.json")
{
    var json = JToken.Parse(FileReadText(configJson));

    if (json.Type == JTokenType.Array)
        json = ((JArray)json)[0];

    var artifacts = json["artifacts"];
    foreach (var artifact in artifacts) {
        yield return $"{artifact["groupId"]}.{artifact["artifactId"]}";
    }
}

// Preparation

Task("inject-variables")
    .WithCriteria(!BuildSystem.IsLocalBuild)
    .Does(() =>
{
    var glob = "./source/AssemblyInfo.cs";

    ReplaceTextInFiles(glob, "{BUILD_COMMIT}", BUILD_COMMIT);
    ReplaceTextInFiles(glob, "{BUILD_NUMBER}", BUILD_NUMBER);
    ReplaceTextInFiles(glob, "{BUILD_TIMESTAMP}", BUILD_TIMESTAMP);
});

Task("check-tools")
    .Does(() =>
{
    var installedTools = RunProcessWithOutput("dotnet", "tool list -g");
    foreach (var toolName in REQUIRED_DOTNET_TOOLS) {
        if (installedTools.All(l => l.IndexOf(toolName, StringComparison.OrdinalIgnoreCase) == -1))
            throw new Exception ($"Missing dotnet tool: {toolName}");
    }
});


Task("tools-update")
	.Does
	(
		() =>
		{
			/*
			dotnet tool uninstall   -g Cake.Tool
			dotnet tool install     -g Cake.Tool
			dotnet tool uninstall   -g xamarin.androidbinderator.tool
			dotnet tool install     -g xamarin.androidbinderator.tool
			dotnet tool uninstall   -g xamarin.androidx.migration.tool
			dotnet tool install     -g xamarin.androidx.migration.tool

			StartProcess("dotnet", "tool uninstall   -g Cake.Tool");
			StartProcess("dotnet", "tool install     -g Cake.Tool");
			*/
			StartProcess("dotnet", "tool uninstall   -g xamarin.androidbinderator.tool");
			StartProcess("dotnet", "tool install     -g xamarin.androidbinderator.tool");
			StartProcess("dotnet", "tool uninstall   -g xamarin.androidx.migration.tool");
			StartProcess("dotnet", "tool install     -g xamarin.androidx.migration.tool");
		}
	);

// Android X

Task("javadocs")
    .Does(() =>
{
    EnsureDirectoryExists("./externals/");

    if (!FileExists("./externals/docs.zip"))
        DownloadFile(REF_DOCS_URL, "./externals/docs.zip");

    if (!DirectoryExists("./externals/docs"))
        Unzip ("./externals/docs.zip", "./externals/docs");

    var astJar = new FilePath("./util/JavaASTParameterNames-1.0.jar");
    var sourcesJars = GetFiles("./externals/**/*-sources.jar");

    foreach (var srcJar in sourcesJars) {
        var srcJarPath = MakeAbsolute(srcJar).FullPath;
        var outTxtPath = srcJarPath.Replace("-sources.jar", "-paramnames.txt");
        var outXmlPath = srcJarPath.Replace("-sources.jar", "-paramnames.xml");

        RunProcess("java", "-jar \"" + MakeAbsolute(astJar).FullPath + "\" --text \"" + srcJarPath + "\" \"" + outTxtPath + "\"");
        RunProcess("java", "-jar \"" + MakeAbsolute(astJar).FullPath + "\" --xml \"" + srcJarPath + "\" \"" + outXmlPath + "\"");
    }
});

Task ("binderate")
    .IsDependentOn("binderate-config-verify")
    .Does (() =>
{
    var configFile = MakeAbsolute(new FilePath("./config.json")).FullPath;
    var basePath = MakeAbsolute(new DirectoryPath ("./")).FullPath;

    // Run the dotnet tool for binderator
    RunProcess("xamarin-android-binderator",
        $"--config=\"{configFile}\" --basepath=\"{basePath}\"");

    // format the targets file so they are pretty in the package
    var targetsFiles = GetFiles("generated/**/*.targets");
    var xmlns = (XNamespace)"http://schemas.microsoft.com/developer/msbuild/2003";
    foreach (var targets in targetsFiles) {
        var xdoc = XDocument.Load(targets.FullPath);
        xdoc.Save(targets.FullPath);
    }

    // different lint.jar files in artifacts causing R8 errors
    foreach (var file in GetFiles("./externals/**/lint.jar")) {
        Information($"Deleting: {file}");
        DeleteFile(file);

        foreach (var aar in GetFiles($"{file.GetDirectory()}/../*.aar")) {
            Information($"Deleting: lint.jar from {aar}");
            using (var zipFile = new ICSharpCode.SharpZipLib.Zip.ZipFile(aar.ToString())) {
                zipFile.BeginUpdate();
                var entry = zipFile.GetEntry("lint.jar");
                if (entry != null) {
                    Information($"		Deleting lint.jar from {aar}");
                    zipFile.Delete(entry);
                }
                zipFile.CommitUpdate();
            }
        }
    }
});

string version_suffix = "";
string nuget_version_template = $"x.y.z.w{version_suffix}";
JArray binderator_json_array = null;

Task("binderate-config-verify")
    .IsDependentOn("binderate-fix")
    .Does
    (
        () =>
        {
            if (!(binderator_json_array?.Count > 0)) 
            {
                using (StreamReader reader = System.IO.File.OpenText(@"./config.json"))
                {
                    JsonTextReader jtr = new JsonTextReader(reader);
                    binderator_json_array = (JArray)JToken.ReadFrom(jtr);
                }
            }

            Information("config.json verification...");
            foreach(JObject jo in binderator_json_array[0]["artifacts"])
            {
                bool? dependency_only = (bool?) jo["dependencyOnly"];
                if ( dependency_only == true)
                {
                    continue;
                }
                string artifact_version = (string) jo["version"];
                string nuget_version  	= (string) jo["nugetVersion"];

                string[] artifact_version_parts = artifact_version.Split(new string[]{ "-" }, StringSplitOptions.RemoveEmptyEntries);
                string[] nuget_version_parts = nuget_version.Split(new string[]{ "-" }, StringSplitOptions.RemoveEmptyEntries);

                string nuget_version_prefix  	= nuget_version_parts[0];
                string artifact_version_prefix  = artifact_version_parts[0];
                string nuget_version_suffix  	= null;
                string artifact_version_suffix  = null;

                if (nuget_version_parts.Length > 1)
                {
                    nuget_version_suffix  	= nuget_version_parts[1];
                }
                if (artifact_version_parts.Length > 1)
                {
                    artifact_version_suffix  = artifact_version_parts[1];
                }

                Information($"groupId                   = {jo["groupId"]}");
                Information($"artifactId                = {jo["artifactId"]}");
                Information($"artifact_version          = {artifact_version}");
                Information($"artifact_version_prefix   = {artifact_version_prefix}");
                Information($"artifact_version_suffix   = {artifact_version_suffix}");
                Information($"nuget_version             = {nuget_version}");
                Information($"nuget_version_prefix      = {nuget_version_prefix}");
                Information($"nuget_version_suffix      = {nuget_version_suffix}");
                Information($"nugetId                   = {jo["nugetId"]}");


                string[] artifact_version_prefix_parts = artifact_version_prefix.Split(new string[]{ "." }, StringSplitOptions.RemoveEmptyEntries);
                string[] nuget_version_prefix_parts = nuget_version_prefix.Split(new string[]{ "." }, StringSplitOptions.RemoveEmptyEntries);
                string x = nuget_version_prefix_parts[0];
                string y = nuget_version_prefix_parts[1];
                string z = nuget_version_prefix_parts[2];

                string w = null;
                if (nuget_version_prefix_parts.Length > 3)
                {
                    w = nuget_version_prefix_parts[3];
                }

                string nuget_version_new = nuget_version_template;
                nuget_version_new = nuget_version_new.Replace("x", x);
                nuget_version_new = nuget_version_new.Replace("y", y);
                nuget_version_new = nuget_version_new.Replace("z", z);
                nuget_version_new = nuget_version_new.Replace("w", w);

                if ( ! string.IsNullOrEmpty(nuget_version_suffix) )
                {
                    nuget_version_new    += $"-{nuget_version_suffix}";
                }

                Information($"nuget_version_new         = {nuget_version_new}");

                if
                    (
                        ! nuget_version_new.StartsWith($"{artifact_version_prefix}")
                        &&
                        ! nuget_version_new.EndsWith($"{artifact_version_suffix}")
                    )
                {
                    Error("check config.json for nuget id");
                    Error  ($"		groupId           = {jo["groupId"]}");
                    Error  ($"		artifactId        = {jo["artifactId"]}");
                    Error  ($"		artifact_version  = {artifact_version}");
                    Error  ($"		nuget_version     = {nuget_version}");
                    Error  ($"		nuget_version_new = {nuget_version_new}");
                    Error  ($"		nugetId           = {jo["nugetId"]}");

                    Warning($"	expected : ");
                    Warning($"		nuget_version = {nuget_version_new}");
                    throw new Exception("check config.json for nuget id");
                }
            }

            return;
        }
    );

Task("binderate-diff")
	.IsDependentOn("binderate")
    .Does
    (
        () =>
        {
			EnsureDirectoryExists("./output/");

			// "git diff master:config.json config.json" > ./output/config.json.diff-from-master.txt"
			string process = "git";
			string process_args = "diff master:config.json config.json";
			IEnumerable<string> redirectedStandardOutput;
			ProcessSettings process_settings = new ProcessSettings ()
			{
             Arguments = process_args,
             RedirectStandardOutput = true
         	};
			int exitCodeWithoutArguments = StartProcess(process, process_settings, out redirectedStandardOutput);
			System.IO.File.WriteAllLines("./output/config.json.diff-from-master.txt", redirectedStandardOutput.ToArray());
			Information("Exit code: {0}", exitCodeWithoutArguments);
		}
	);

Task("binderate-fix")
    .Does
    (
        () =>
        {
            if (!(binderator_json_array?.Count > 0)) 
            {
                using (StreamReader reader = System.IO.File.OpenText(@"./config.json"))
                {
                    JsonTextReader jtr = new JsonTextReader(reader);
                    binderator_json_array = (JArray)JToken.ReadFrom(jtr);
                }
            }

            Warning("config.json fixing missing folder strucutre ...");
            foreach(JObject jo in binderator_json_array[0]["artifacts"])
            {
                string groupId      = (string) jo["groupId"];
                string artifactId   = (string) jo["artifactId"];

                Information($"  Verifying files for     :");
                Information($"              group       : {groupId}");
                Information($"              artifact    : {artifactId}");

                bool? dependency_only = (bool?) jo["dependencyOnly"];
                if ( dependency_only == true)
                {
                    continue;
                }


                string dir_group = $"source/{groupId}";
                if ( ! DirectoryExists(dir_group) )
                {
                    Warning($"  Creating {dir_group}");
                    CreateDirectory(dir_group);
                }
                string dir_artifact = $"{dir_group}/artifactId";
                if ( ! DirectoryExists(dir_group) )
                {
                    Warning($"      Creating artifact folder : {dir_artifact}");
                    CreateDirectory(dir_group);
                }

            }

            return;
        }
    );


// using System.Threading;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Logging.Abstractions;

// using NuGet.Protocol.Core.Types;
// using NuGet.Versioning;
// using NuGet.Protocol.Core.Types;

Task("binderate-nuget-check")
    .Does
    (
        () =>
        {
            if (!(binderator_json_array?.Count > 0)) 
            {
                using (StreamReader reader = System.IO.File.OpenText(@"./config.json"))
                {
                    JsonTextReader jtr = new JsonTextReader(reader);
                    binderator_json_array = (JArray)JToken.ReadFrom(jtr);
                }
            }

            Warning("config.json fixing missing folder structure ...");
            foreach(JObject jo in binderator_json_array[0]["artifacts"])
            {
                string groupId      = (string) jo["groupId"];
                string artifactId   = (string) jo["artifactId"];
                string nugetId      = (string) jo["nugetId"];
                string nugetVersion = (string) jo["nugetVersion"];

                Information($"  Verifying nuget                 :");
                Information($"            nugetId               : {nugetId}");
                Information($"            config.json veriosn   : {nugetVersion}");

                // ILogger logger = NullLogger.Instance;
                // CancellationToken cancellationToken = CancellationToken.None;

                // SourceCacheContext cache = new SourceCacheContext();
                // SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
                // FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>().Result;

                // IEnumerable<NuGetVersion> versions = resource.GetAllVersionsAsync
                //                                                         (
                //                                                             nugetId,
                //                                                             cache,
                //                                                             logger,
                //                                                             cancellationToken
                //                                                         ).Result;

                // foreach (NuGetVersion version in versions)
                // {
                //     Information($"              Found version {version}");
                // }

            }

            return;
        }
    );


System.Xml.XmlDocument xmldoc = null;
System.Xml.XmlNamespaceManager ns = null;

Task("metadata-verify")
    .Does
    (
        () =>
        {
            FilePathCollection metadata_files = null;

            //  namespace is required, otherwise NRE
            string xml_namespace_name = "ax"; // could be "apixml" but it is irrelevant, keeping it short

            string xpath_expression_nodes_to_find =
                        //$@"//attr[contains(@path,'interface') and contains(@name ,'visibility')]"
                        $@"//attr[contains(@path,'interface')]"
                        ;

            metadata_files = GetFiles($"./generated/**/Metadata*.xml");
            foreach(FilePath fp in metadata_files)
            {
                Information($"Metadata = {fp}");
                throw new Exception("Move this file to source");
            }
            metadata_files = GetFiles($"./source/**/Metadata*.xml");
            foreach(FilePath fp in metadata_files)
            {
                Information($"Metadata = {fp}");
                xmldoc = new System.Xml.XmlDocument();
                xmldoc.Load(fp.ToString());
                ns = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);

                List<(string Path, bool IsPublic)> result = GetXmlMetadata(xpath_expression_nodes_to_find, ns).ToList();
                foreach((string Path, bool IsPublic) r in result)
                {
                    Information($"		Found:");
                    Information($"			Path: {r.Path}");
                    Information($"			IsPublic: {r.IsPublic}");
                    if (r.IsPublic)
                    {
                        throw new Exception("Preventing exposing/surfacing interfaces with default package accessibility as public");
                    }
                }
            }
        }
    );

private IEnumerable<(string Path, bool IsPublic)> GetXmlMetadata(string xpath, System.Xml.XmlNamespaceManager xml_namespace)
{
    System.Xml.XmlNodeList node_list = xmldoc.SelectNodes(xpath, xml_namespace);

    foreach (System.Xml.XmlNode node in node_list)
    {
        string name = node.Attributes["name"].Value;
        string inner_text = node.InnerText;	//.Value;
        string path = node.Attributes["path"].Value;

        Information($"	path        = {path}");

        if (path.Contains("MediaRouteProvider.DynamicGroupRouteController"))
        {
            Information($"		Found:");
            Information($"			Name: {name}");
            Information($"			Visibility: {inner_text}");
            throw new Exception("MediaRouteProvider.DynamicGroupRouteController");
        }

        if (string.Equals(name, "visibility") && inner_text.Contains("public"))
        {
            Information($"		Visibility  = {inner_text}");

            bool is_public = inner_text.Contains("public") ? true : false;

            yield return (Path: path, IsPublic: is_public);
        }
    }
}

Task("libs")
    .IsDependentOn("metadata-verify")
    .IsDependentOn("libs-native")
    .Does(() =>
{
    var settings = new DotNetCoreMSBuildSettings()
        .SetConfiguration(CONFIGURATION)
        .SetMaxCpuCount(0)
        .EnableBinaryLogger($"./output/libs.{CONFIGURATION}.binlog")
        .WithProperty("MigrationPackageVersion", MIGRATION_PACKAGE_VERSION)
        .WithProperty("DesignTimeBuild", "false")
        .WithProperty("AndroidSdkBuildToolsVersion", $"{AndroidSdkBuildTools}");

    if (!string.IsNullOrEmpty(ANDROID_HOME))
        settings.WithProperty("AndroidSdkDirectory", $"{ANDROID_HOME}");

    DotNetCoreRestore("./generated/AndroidX.sln", new DotNetCoreRestoreSettings
    {
        MSBuildSettings = settings.EnableBinaryLogger("./output/restore.binlog")
    });

    DotNetCoreMSBuild("./generated/AndroidX.sln", settings);
});

Task("libs-native")
    .Does(() =>
{
    string root = "./source/com.google.android.material/material.extensions/";

    RunGradle(root, "build");

    string outputDir = "./externals/com.xamarin.google.android.material.extensions/";
    EnsureDirectoryExists(outputDir);
    CleanDirectories(outputDir);

    CopyFileToDirectory($"{root}/extensions-aar/build/outputs/aar/extensions-aar-release.aar", outputDir);
    Unzip($"{outputDir}/extensions-aar-release.aar", outputDir);
});

Task("nuget")
    .IsDependentOn("libs")
    .Does(() =>
{
    var settings = new DotNetCoreMSBuildSettings()
        .SetConfiguration(CONFIGURATION)
        .SetMaxCpuCount(0)
        .EnableBinaryLogger($"./output/nuget.{CONFIGURATION}.binlog")
        .WithProperty("MigrationPackageVersion", MIGRATION_PACKAGE_VERSION)
        .WithProperty("NoBuild", "true")
        .WithProperty("PackageRequireLicenseAcceptance", "true")
        .WithProperty("PackageOutputPath", MakeAbsolute ((DirectoryPath)"./output/").FullPath)
        .WithTarget("Pack");

    if (!string.IsNullOrEmpty(ANDROID_HOME))
        settings.WithProperty("AndroidSdkDirectory", $"{ANDROID_HOME}");

    DotNetCoreMSBuild("./generated/AndroidX.sln", settings);
});

Task("samples-generate-all-targets")
    .Does(() =>
{
    // make a big .targets file that pulls in everything
    var xmlns = (XNamespace)"http://schemas.microsoft.com/developer/msbuild/2003";
    var itemGroup = new XElement(xmlns + "ItemGroup");
    foreach (var nupkg in GetFiles("./output/*.nupkg")) {
        Information($"NuGet package = {nupkg}");

        // Skip Wear as it has special implications requiring more packages to be used properly in an app
        if (nupkg.FullPath.Contains(".Wear."))
            continue;
        // Skip the migration packages as that is not meant forto be used here
        if (nupkg.FullPath.Contains("Xamarin.AndroidX.Migration"))
            continue;

        var filename = nupkg.GetFilenameWithoutExtension();
        var match = Regex.Match(filename.ToString(), @"(.+?)\.(\d+[\.0-9\-a-zA-Z]+)");
        itemGroup.Add(new XElement(xmlns + "PackageReference",
            new XAttribute("Include", match.Groups[1]),
            new XAttribute("Version", match.Groups[2])));

    }

    // R8 ACW errors about missing classes
    // TODO: could have been grabbed from config.json
    itemGroup.Add(new XElement(xmlns + "PackageReference",
        new XAttribute("Include", "Xamarin.Google.Guava"),
        new XAttribute("Version", "27.1.0.4")));
    itemGroup.Add(new XElement(xmlns + "PackageReference",
        new XAttribute("Include", "Xamarin.Google.Guava.FailureAccess"),
        new XAttribute("Version", "1.0.1.2")));
    itemGroup.Add(new XElement(xmlns + "PackageReference",
        new XAttribute("Include", "Xamarin.Google.Guava.ListenableFuture"),
        new XAttribute("Version", "1.0.0.2")));

    var xdoc = new XDocument(new XElement(xmlns + "Project", itemGroup));
    xdoc.Save("./output/AllPackages.targets");
});

Task("samples")
    .IsDependentOn("nuget")
    .IsDependentOn("samples-generate-all-targets")
    .IsDependentOn("migration-nuget")
    .Does(() =>
{
    // clear the packages folder so we always use the latest
    var packagesPath = MakeAbsolute((DirectoryPath)"./samples/packages").FullPath;
    EnsureDirectoryExists(packagesPath);
    CleanDirectories(packagesPath);

    // build the samples
    var settings = new MSBuildSettings()
        .SetConfiguration(CONFIGURATION)
        .SetVerbosity(VERBOSITY)
        .SetMaxCpuCount(0)
        .EnableBinaryLogger($"./output/samples.{CONFIGURATION}.binlog")
        .WithRestore()
        .WithProperty("RestorePackagesPath", packagesPath)
        .WithProperty("DesignTimeBuild", "false")
        .WithProperty("AndroidSdkBuildToolsVersion", $"{AndroidSdkBuildTools}");

    if (!string.IsNullOrEmpty(ANDROID_HOME))
        settings.WithProperty("AndroidSdkDirectory", $"{ANDROID_HOME}");

    if (!string.IsNullOrEmpty(MSBUILD_PATH))
        settings.ToolPath = MSBUILD_PATH;

    MSBuild("./samples/BuildAll/BuildAll.sln", settings);
});

// Migration Preparation

Task("generate-mapping")
    .IsDependentOn("merge")
    .IsDependentOn("nuget")
    .Does(() =>
{
    EnsureDirectoryExists("./output/mappings/");

    // download the AndroidSupport.Merged.dll from a past build
    if (!FileExists("./output/AndroidSupport.Merged.dll"))
        DownloadFile(SUPPORT_MERGED_DLL_URL, "./output/AndroidSupport.Merged.dll");

    // copy the overrides
    CopyFileToDirectory("./mappings/override-mapping.csv", "./output/mappings/");

    // generate the mapping
    Information("Generating the androidx-mapping.csv file...");
    RunProcess("androidx-migrator",
        $"generate -v " +
        $"  --support ./output/AndroidSupport.Merged.dll" +
        $"  --androidx ./output/AndroidX.Merged.dll" +
        $"  --output ./output/mappings/androidx-mapping.csv");
    var mappingLines = FileReadLines("./output/mappings/androidx-mapping.csv");
    mappingLines = mappingLines.Where(l => !l.Contains("InjectedAssemblyNameAttribute")).ToArray();
    FileWriteLines("./output/mappings/androidx-mapping.csv", mappingLines);
    CopyFileToDirectory("./output/mappings/androidx-mapping.csv", "./mappings/");

    // generate the dependency tree
    Information("Generating the dependencies.json file...");
    RunProcess("androidx-migrator",
        $"deptree -v " +
        $"  --directory ./output/" +
        $"  --output ./output/mappings/dependencies.json");
    using (var jsonReader = System.IO.File.OpenText("./output/mappings/dependencies.json")) {
        var o = (JObject)JToken.ReadFrom(new JsonTextReader(jsonReader));
        // special case for the fact that we can't actually read the real nuget package yet
        foreach (var p in o["packages"]) {
            if (p["id"].Value<string>() == "Xamarin.AndroidX.Migration") {
                p["dependencies"] = new JArray(new [] {
                    "Xamarin.AndroidX.MultiDex"
                });
            }
        }
        var j = o.ToString();
        jsonReader.Dispose();
        FileWriteText("./output/mappings/dependencies.json", j);
    }
    CopyFileToDirectory("./output/mappings/dependencies.json", "./mappings/");

    // generate the nuget mappings
    Information("Generating the androidx-assemblies.csv file...");
    var support = "./externals/android.support";
    var supportJson = support + "/config.json";
    EnsureDirectoryExists(support);
    if (!FileExists(supportJson))
        DownloadFile(SUPPORT_CONFIG_URL, supportJson);

    // read the newly generated mapping file
    var csvReader = new StreamReader("./mappings/androidx-mapping.csv");
    var csv = new CsvReader(csvReader);
    var records = csv.GetRecords<dynamic>()
        .Cast<IDictionary<string, object>>()
        .Select(r => $"{r["Support .NET assembly"]}|{r["AndroidX .NET assembly"]}")
        .Union(new [] {
            "Xamarin.Android.Support.v4|Xamarin.AndroidX.Legacy.Support.V4",
            "Xamarin.Android.Support.MultiDex|Xamarin.AndroidX.MultiDex",
        })
        .Distinct()
        .Select(r => new {
            Support = r.Split("|")[0],
            AndroidX = r.Split("|")[1],
        })
        .Where(r => !string.IsNullOrEmpty(r.Support))
        .OrderBy(r => r.AndroidX)
        .OrderBy(r => r.Support)
        .ToArray();

    var lines = new List<string> {
        "Support .NET assembly," +
        "AndroidX .NET assembly," +
        "Support NuGet," +
        "AndroidX NuGet," +
        "AndroidX NuGet Version",
    };
    foreach (var record in records) {
        var androidxNuget = GetNuGetId(record.AndroidX);
        lines.Add(
            record.Support + "," +
            record.AndroidX + "," +
            GetNuGetId(record.Support, supportJson) + "," +
            androidxNuget + "," +
            GetNuGetVersion(androidxNuget));
    }
    FileWriteLines("./output/mappings/androidx-assemblies.csv", lines.ToArray());
    CopyFileToDirectory("./output/mappings/androidx-assemblies.csv", "./mappings/");

    // update the .props
    Information("Generating the Xamarin.AndroidX.Migration.props file...");
    var propsPath = "./source/migration/BuildTasks/Xamarin.AndroidX.Migration.props";
    var xprops = XDocument.Load(propsPath);
    var xmlns = xprops.Root.Name.Namespace;
    var xitems = xprops.Descendants(xmlns + "_AndroidXSupportAssembly");
    var xparent = xitems.FirstOrDefault().Parent;
    xitems.Remove();
    xparent.Add(records.OrderBy(r => r.Support).Select(r => r.Support).Distinct().Select(r =>
        new XElement(xmlns + "_AndroidXSupportAssembly",
            new XAttribute("Include", r))));
    xitems = xprops.Descendants(xmlns + "_AndroidXAssembly");
    xparent = xitems.FirstOrDefault().Parent;
    xitems.Remove();
    xparent.Add(records.OrderBy(r => r.AndroidX).Select(r => r.AndroidX).Distinct().Select(r =>
        new XElement(xmlns + "_AndroidXAssembly",
            new XAttribute("Include", r))));
    xitems = xprops.Descendants(xmlns + "_AndroidXMavenArtifact");
    xparent = xitems.FirstOrDefault().Parent;
    xitems.Remove();
    xparent.Add(GetArtifacts().OrderBy(r => r).Select(r => r).Distinct().Select(r =>
        new XElement(xmlns + "_AndroidXMavenArtifact",
            new XAttribute("Include", r))));
    var xmlSettings = new XmlWriterSettings {
        Encoding = new UTF8Encoding(),
        Indent = true,
        IndentChars = "    ",
        NewLineChars = "\n",
    };
    using (var xmlWriter = XmlWriter.Create(propsPath, xmlSettings)) {
        xprops.Save(xmlWriter);
    }
    FileAppendText(propsPath, "\n");
    CopyFileToDirectory(propsPath, "./output/mappings/");
});

Task ("merge")
    .IsDependentOn ("libs")
    .Does (() =>
{
    // find all the dlls
    var allDlls = GetFiles($"./generated/*/bin/Release/monoandroid*/Xamarin.*.dll");
    var mergeDlls = allDlls
        .GroupBy(d => d.GetFilename().FullPath)
        .Where(g => !g.Key.Contains("Xamarin.AndroidX.Migration"))
        .Select(g => g.FirstOrDefault())
        .ToList();

    // merge them all
    EnsureDirectoryExists("./output/");
    RunProcess("androidx-migrator",
        $"merge" +
        $"  --assembly " + string.Join(" --assembly ", mergeDlls) +
        $"  --output ./output/AndroidX.Merged.dll" +
        $"  --search \"{XAMARIN_ANDROID_PATH}/{ANDROID_SDK_VERSION}\" " +
        $"  --search \"{XAMARIN_ANDROID_PATH}/{ANDROID_SDK_BASE_VERSION}\" " +
        $"  --inject-assemblyname");
});

// Migration External Assets

Task("jetifier-wrapper")
    .Does(() =>
{
    var root = "./source/migration/JetifierWrapper/";

    RunGradle(root, "jar");

    var outputDir = MakeAbsolute((DirectoryPath)"./output/JetifierWrapper");
    EnsureDirectoryExists(outputDir);

    CopyFileToDirectory($"{root}build/libs/jetifierWrapper-1.0.jar", outputDir);
});

// Migration

Task("migration-externals")
    .Does(() =>
{
    var interop = "./externals/java.interop";

    EnsureDirectoryExists(interop);
    if (!FileExists(interop + "/source.zip")) {
        DownloadFile(JAVA_INTEROP_ZIP_URL, interop + "/source.zip");
        Unzip(interop + "/source.zip", interop);
        MoveDirectory(interop + "/java.interop-" + System.IO.Path.GetFileNameWithoutExtension(JAVA_INTEROP_ZIP_URL), interop + "/java.interop");
    }
});

Task("migration-libs")
    .IsDependentOn("jetifier-wrapper")
    .IsDependentOn("generate-mapping")
    .IsDependentOn("migration-externals")
    .Does(() =>
{
    var settings = new MSBuildSettings()
        .SetConfiguration(CONFIGURATION)
        .SetVerbosity(VERBOSITY)
        .SetMaxCpuCount(0)
        .EnableBinaryLogger($"./output/migration-libs.{CONFIGURATION}.binlog")
        .WithRestore()
        .WithProperty("PackageVersion", MIGRATION_PACKAGE_VERSION);

    if (!string.IsNullOrEmpty(ANDROID_HOME))
        settings.WithProperty("AndroidSdkDirectory", $"{ANDROID_HOME}");

    if (!string.IsNullOrEmpty(MSBUILD_PATH))
        settings.ToolPath = MSBUILD_PATH;

    MSBuild("./source/migration/Xamarin.AndroidX.Migration.sln", settings);
});

Task("migration-nuget")
    .IsDependentOn("migration-libs")
    .Does(() =>
{
    var settings = new MSBuildSettings()
        .SetConfiguration(CONFIGURATION)
        .SetVerbosity(VERBOSITY)
        .SetMaxCpuCount(0)
        .EnableBinaryLogger($"./output/migration-nuget.{CONFIGURATION}.binlog")
        .WithProperty("NoBuild", "true")
        .WithRestore()
        .WithProperty("PackageVersion", MIGRATION_PACKAGE_VERSION)
        .WithProperty("MultiDexVersion", MULTIDEX_PACKAGE_VERSION)
        .WithProperty("PackageRequireLicenseAcceptance", "true")
        .WithProperty("PackageOutputPath", MakeAbsolute((DirectoryPath)"./output/").FullPath)
        .WithTarget("Pack");

    if (!string.IsNullOrEmpty(ANDROID_HOME))
        settings.WithProperty("AndroidSdkDirectory", $"{ANDROID_HOME}");

    if (!string.IsNullOrEmpty(MSBUILD_PATH))
        settings.ToolPath = MSBUILD_PATH;

    MSBuild("./source/migration/BuildTasks/Xamarin.AndroidX.Migration.BuildTasks.csproj", settings);

    settings.EnableBinaryLogger($"./output/migration-tool-nuget.{CONFIGURATION}.binlog");

    MSBuild("./source/migration/Tool/Xamarin.AndroidX.Migration.Tool.csproj", settings);
});

// Migration Tests

Task("migration-tests-externals")
    .Does(() =>
{
    // download the facebook sdk to test with
    //{
    //    var sdkRoot = "./externals/test-assets/facebook-sdk/";
    //    var facebookFilename = "facebook-android-sdk";
    //    var facebookVersion = "4.40.0";
    //    var facebookFullName = $"{facebookFilename}-{facebookVersion}";
    //    var facebookTestUrl = $"https://origincache.facebook.com/developers/resources/?id={facebookFullName}.zip";
    //    var zipName = $"{sdkRoot}{facebookFilename}.zip";
    //    EnsureDirectoryExists(sdkRoot);
    //    if (!FileExists(zipName)) {
    //        DownloadFile(facebookTestUrl, zipName);
    //        Unzip(zipName, sdkRoot);
    //    }
    //}

    // downlaod some dodgy dlls
    {
        var sdkRoot = "./externals/test-assets/active-directory/";
        var adUrl = "https://www.nuget.org/api/v2/package/Microsoft.IdentityModel.Clients.ActiveDirectory/5.2.4";
        var zipName = $"{sdkRoot}/ad.zip";
        EnsureDirectoryExists(sdkRoot);
        if (!FileExists(zipName)) {
            DownloadFile(adUrl, zipName);
            Unzip(zipName, sdkRoot);
        }
    }

    // build the special test projects
    {
        var nativeProjects = new [] {
            "./tests/AndroidXMigrationTests/Aarxersise.Java.AndroidX/",
            "./tests/AndroidXMigrationTests/Aarxersise.Java.Support/",
        };
        foreach (var native in nativeProjects) {
            RunGradle (native, "assembleDebug");
        }
    }
});

Task("migration-tests")
    .IsDependentOn("migration-tests-externals")
    .IsDependentOn("migration-libs")
    .Does(() =>
{
    // build
    var settings = new MSBuildSettings()
        .SetConfiguration(CONFIGURATION)
        .SetVerbosity(VERBOSITY)
        .SetMaxCpuCount(0)
        .EnableBinaryLogger($"./output/migration-tests.{CONFIGURATION}.binlog")
        .WithRestore();

    if (!string.IsNullOrEmpty(ANDROID_HOME))
        settings.WithProperty("AndroidSdkDirectory", $"{ANDROID_HOME}");

    if (!string.IsNullOrEmpty(MSBUILD_PATH))
        settings.ToolPath = MSBUILD_PATH;

    MSBuild("./tests/AndroidXMigrationTests.sln", settings);

    // test
    DotNetCoreTest("Xamarin.AndroidX.Migration.Tests.csproj", new DotNetCoreTestSettings {
        Configuration = CONFIGURATION,
        NoBuild = true,
        Logger = "trx;LogFileName=Xamarin.AndroidX.Migration.Tests.trx",
        WorkingDirectory = "./tests/AndroidXMigrationTests/Tests/",
        ResultsDirectory = "./output/test-results/",
    });
});


var TF_MONIKER = "monoandroid90";
string SUPPORT_MERGED_DLL = "./output/AndroidSupport.Merged.dll";
string ANDROIDX_MERGED_DLL = "./output/AndroidX.Merged.dll";
string MAPPING_URL = "https://raw.githubusercontent.com/xamarin/AndroidX/master/mappings/androidx-mapping.csv";
string MAPPING_LOCAL = "./output/androidx-mapping.csv";
string API_INFO_OLD = "./output/AndroidSupport.Merged.api-info.xml";
string API_INFO_OLD_MIGRATED = "./output/AndroidSupport.Merged.Migrated.api-info.xml";
string API_INFO_NEW = "./output/AndroidX.Merged.api-info.xml";


var MONODROID_BASE_PATH = (DirectoryPath)"/Library/Frameworks/Xamarin.Android.framework/Versions/Current/lib/xbuild-frameworks/MonoAndroid/";
if (IsRunningOnWindows ()) {
    var vsInstallPath = VSWhereLatest (new VSWhereLatestSettings { Requires = "Component.Xamarin", IncludePrerelease = true });
    MONODROID_BASE_PATH = vsInstallPath.Combine ("Common7/IDE/ReferenceAssemblies/Microsoft/Framework/MonoAndroid/");
}
var MONODROID_PATH = MONODROID_BASE_PATH.Combine(ANDROID_SDK_VERSION);


Task("api-info-migrate")
    .Does
    (
        () =>
        {
            EnsureDirectoryExists ("./output/");

            DownloadFile (SUPPORT_MERGED_DLL_URL, SUPPORT_MERGED_DLL);

            var result1 = StartProcess($"api-tools", $"api-info {SUPPORT_MERGED_DLL}");
            if (result1 != 0)
                throw new Exception($"The androidxmapper failed with error code {result1}.");


            DownloadFile (MAPPING_URL, MAPPING_LOCAL);
            string[] lines_mapping_local = FileReadLines(MAPPING_LOCAL);
            List<(string, string)> mapping = new List<(string, string)>();
            lines_mapping_local.ToList()
                            .ForEach( x => { var r = x.Split(new char[] {','}); mapping.Add( (r[0], r[2]) ); } );
            mapping.GroupBy(g => new Tuple<string, string>(g.Item1, g.Item2))
                .Select(g => g.First())
                .ToList()
                .ForEach( x => { Console.WriteLine(x); } );


            string[] lines_api_info = FileReadLines(API_INFO_OLD);
            // lines_api_info.ToList()
            //                .ForEach( x => { Console.WriteLine(x); } );
            List<string> lines_api_info_migrated =  new List<string>();
            foreach(string l in lines_api_info)
            {
                string line_new = null;

                foreach((string, string) m in mapping)
                {
                    if (string.IsNullOrEmpty(m.Item1))
                    {
                        continue;
                    }
                    if( l.Contains(m.Item1) )
                    {
                        line_new = l.Replace(m.Item1, m.Item2);
                        break;
                    }
                    else
                    {
                        line_new = l;
                    }
                }
                lines_api_info_migrated.Add(line_new);
            }

            FileWriteLines(API_INFO_OLD_MIGRATED, lines_api_info_migrated.ToArray());
        }
    );


Task("merge-fresh")
    .Does
    (
        () =>
        {
            var allDlls = GetFiles ($"./generated/*/bin/Release/{TF_MONIKER}/Xamarin.AndroidX.*.dll");
            var mergeDlls = allDlls
                                .GroupBy(d => new FileInfo(d.FullPath).Name)
                                .Select(g => g.FirstOrDefault())
                                .ToList();
            mergeDlls.ForEach( x => { Console.WriteLine(x); });
            var result2 = StartProcess
                            (
                                "api-tools",
                                "merge -n Xamarin.AndroidX.Merged" +
                                $" {string.Join(" ", mergeDlls)} " +
                                $" -s /Library/Frameworks/Xamarin.Android.framework/Versions/9.1.0-17/lib/xamarin.android/xbuild-frameworks/MonoAndroid/v9.0/" +
                                $" -s /Library/Frameworks/Xamarin.Android.framework/Versions/10.1.0.29/lib/xamarin.android/xbuild-frameworks/MonoAndroid/v1.0/" +
                                $" -o " + MakeAbsolute((FilePath)"./output/AndroidX.Merged.dll") +
                                $" --inject-assemblyname"
                            );
            var result1 = StartProcess($"api-tools", $"api-info {ANDROIDX_MERGED_DLL}");
            if (result1 != 0)
                throw new Exception($"The androidxmapper failed with error code {result1}.");

            return;
        }
    );


Task("api-diff")
    .IsDependentOn ("merge")
    .IsDependentOn ("api-info-migrate")
    .IsDependentOn ("merge-fresh")
    .IsDependentOn ("bindings-verify")    
    .Does
    (
        () =>
        {
            // https://github.com/Redth/Cake.MonoApiTools/
            MonoApiDiff(API_INFO_OLD_MIGRATED, API_INFO_NEW, "./output/api-info-diff.xml");
            string[] lines_xml = FileReadLines("./output/api-info-diff.xml");
            List<string> lines_xml_new = new List<string>();
            foreach(string line in lines_xml)
            {
                if (line.Contains(@"Java.Interop.IJavaPeerable"))
                {
                    continue;
                }

                lines_xml_new.Add(line);
            }
            FileWriteLines("./output/api-info-diff.cleaned.xml", lines_xml_new.ToArray());


            MonoApiHtmlColorized(API_INFO_OLD_MIGRATED, API_INFO_NEW, "./output/api-info-diff.html");
            string[] lines_html = FileReadLines("./output/api-info-diff.html");

            List<string> lines_html_new = new List<string>();
            List<(string Class, string ClassFullyQualified)> removed_types = new List<(string Class, string ClassFullyQualified)>();
            List<(string Class, string ClassFullyQualified)> new_types = new List<(string Class, string ClassFullyQualified)>();

            foreach(string line in lines_html)
            {
                if (line.Contains(@"Java.Interop.IJavaPeerable"))
                {
                    continue;
                }
                if (line.Contains("<h3>Removed Type <span class='breaking' data-is-breaking>"))
                {
                    string t = line.Replace("<h3>Removed Type <span class='breaking' data-is-breaking>", "");
                    t = t.Replace("</span></h3>",",");
                    t = Regex.Replace(t, ",</div> <!-- (.*?) -->", "");
                    t = Regex.Replace(t, ",<div> <!-- (.*?) -->", "");
                    string[] types = t.Split( new string[] { "," }, StringSplitOptions.RemoveEmptyEntries );
                    foreach(string t1 in types)
                    {
                        string c  = t1.Substring(t1.LastIndexOf(".") + 1);
                        removed_types.Add((Class: c, ClassFullyQualified: t1));
                    }
                }
                if (line.Contains("<h3>New Type "))
                {
                    string t = line.Replace("<h3>New Type ", "");
                    t = t.Replace("</h3>", "");
                    Information($"{t}");
                    string c  = t.Substring(t.LastIndexOf(".") + 1);
                    new_types.Add((Class: c, ClassFullyQualified:t));
                }
                lines_html_new.Add(line);
            }

            FileWriteLines("./output/api-info-diff.cleaned.html", lines_html_new.ToArray());
            FileWriteLines("./output/removed-types.csv", removed_types.Select(i => $"{i.Class},{i.ClassFullyQualified}").ToArray());
            FileWriteLines("./output/new-types.csv", new_types.Select(i => $"{i.Class},{i.ClassFullyQualified}").ToArray());

            List<int> indices_new = new List<int>();
            List<int> indices_removed = new List<int>();
            List<string> moved_types = new List<string>();
            moved_types.Add($"# Class,ClassFullyQualified Removed, ClassFullyQualified New");

            for( int idx1 = 0; idx1 < removed_types.Count; idx1++)
            {
                (string Class, string ClassFullyQualified) tr = removed_types[idx1];
                int idx2 = new_types.FindIndex(tn => tn.Class == tr.Class);
                if (idx2 < 0)
                {
                    continue;
                }
                indices_removed.Add(idx1);
                indices_new.Add(idx2);
                string c = removed_types[idx1].Class;
                string cr_fq = removed_types[idx1].ClassFullyQualified;
                string cn_fq = new_types[idx2].ClassFullyQualified;
                moved_types.Add($"{c},{cr_fq},{cn_fq}");
            }

            var indices_new_sorted = indices_new.OrderByDescending(t => t);
            for (int i = 0; i < indices_new_sorted.Count(); i++)
            {
                new_types.RemoveAt(indices_new_sorted.ElementAt(i));
            }
            var indices_removed_sorted = indices_removed.OrderByDescending(t => t);
            for (int i = 0; i < indices_removed_sorted.Count(); i++)
            {
                removed_types.RemoveAt(indices_removed_sorted.ElementAt(i));
            }

            FileWriteLines("./output/removed-types.final.csv", removed_types.Select(i => $"{i.Class},{i.ClassFullyQualified}").ToArray());
            FileWriteLines("./output/new-types.final.csv", new_types.Select(i => $"{i.Class},{i.ClassFullyQualified}").ToArray());
            FileWriteLines("./output/moved-types.final.csv", moved_types.ToArray());

            return;
        }
    );

Task ("diff")
    .IsDependentOn ("merge")
    .IsDependentOn ("bindings-verify")    
    .Does (() =>
{
    var SEARCH_DIRS = new DirectoryPath [] {
        MONODROID_BASE_PATH.Combine("v1.0"),
        MONODROID_PATH,
    };

    EnsureDirectoryExists("./output/");
    MonoApiInfo ("./output/AndroidX.Merged.dll", "./output/api-info.xml", new MonoApiInfoToolSettings {
        SearchPaths = SEARCH_DIRS
    });

    //DownloadFile (BASE_API_INFO_URL, "./output/api-info.previous.xml");

    // Now diff against current released api info
    MonoApiDiff ("./output/api-info.previous.xml", "./output/api-info.xml", "./output/api-diff.xml");

    // Now let's make pretty files
    MonoApiHtml ("./output/api-info.previous.xml", "./output/api-info.xml", "./output/api-diff.html");
    MonoApiMarkdown ("./output/api-info.previous.xml", "./output/api-info.xml", "./output/api-diff.md");
});


Task("bindings-verify")
    .Does
    (
        () =>
        {
            List<string> missing_java_type = new List<string>();
            List<string> missing_dotnet_type = new List<string>();
            List<string> missing_dotnet_override_type = new List<string>();

            string[] lines = System.IO.File.ReadAllLines("./mappings/androidx-mapping.csv");
            foreach(string line in lines)
            {
                if (line.StartsWith(",,,,,,,,,,WARNING: Unable to find AndroidX type for Java type "))
                {
                    missing_java_type.Add(line);
                }
                else if (line.StartsWith(",,,,,,,,,,WARNING: Unable to find AndroidX type for .NET type "))
                {
                    missing_dotnet_type.Add(line);

                }
                else if (line.StartsWith(",,,,,,,,,,WARNING: Unable to find override type for type "))
                {
                    missing_dotnet_override_type.Add(line);
                }
            }

            System.IO.File.WriteAllLines("./output/missing_java_type.csv", missing_java_type.ToArray());
            System.IO.File.WriteAllLines("./output/missing_dotnet_type.csv", missing_dotnet_type.ToArray());
            System.IO.File.WriteAllLines("./output/missing_dotnet_override_type.csv", missing_dotnet_override_type.ToArray());
        }
    );

Task ("clean")
    .Does (() =>
{
    if (DirectoryExists ("./externals"))
        DeleteDirectory ("./externals", new DeleteDirectorySettings { Recursive = true, Force = true });

    if (DirectoryExists ("./generated"))
        DeleteDirectory ("./generated", new DeleteDirectorySettings { Recursive = true, Force = true });

    CleanDirectories ("./**/packages");
});

Task ("packages")
    .IsDependentOn ("binderate")
    .IsDependentOn ("nuget");

Task ("full-run")
    .IsDependentOn ("binderate")
    .IsDependentOn ("nuget")
    .IsDependentOn ("samples");

Task ("nuget-and-migration")
    .IsDependentOn ("nuget")
    .IsDependentOn ("generate-mapping")
    .IsDependentOn ("migration-nuget")
    .IsDependentOn ("migration-tests");

Task ("ci")
    .IsDependentOn ("check-tools")
    .IsDependentOn ("inject-variables")
    .IsDependentOn ("binderate")
    .IsDependentOn ("nuget")
    .IsDependentOn ("generate-mapping")
    .IsDependentOn ("migration-nuget")
    .IsDependentOn ("migration-tests")
    .IsDependentOn ("bindings-verify")
    .IsDependentOn ("samples");

// for local builds, conditionally do the first binderate
if (FileExists ("./generated/AndroidX.sln")) {
    Task ("Default")
        .IsDependentOn ("nuget");
} else {
    Task ("Default")
        .IsDependentOn ("binderate")
        .IsDependentOn ("nuget");
}

RunTarget (TARGET);
