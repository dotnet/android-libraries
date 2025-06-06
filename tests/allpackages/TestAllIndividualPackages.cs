using System.Text;
using System.Xml;
using CliWrap;
using CliWrap.Buffered;
using NUnit.Framework;

namespace AllPackagesTests;

[TestFixture]
public class TestAllIndividualPackages
{
	static string base_dir = "";
	static string test_dir = Path.Combine ("output", "tests" , "allpackages");
	static string configuration = "Release";
	static string platform_version = "29";
	static string net_version = "net8.0";

	// These packages are ignored because they contain the same Java code as
	// other packages, which causes a conflict when building the project.
	static List<string> ignored_packages = [
		"Xamarin.Google.Guava.ListenableFuture",
		"Xamarin.Protobuf.Lite",
		"Xamarin.GoogleAndroid.Libraries.Places.Compat",

		// PlayServices packages
		// - Duplicate managed types (due to Games.V2)
		"Xamarin.GooglePlayServices.Games",

		// - Duplicate Java types
		"Xamarin.GooglePlayServices.Ads.Base",
		"Xamarin.GooglePlayServices.Ads.Lite",
		"Xamarin.GooglePlayServices.Ads.Api",
		"Xamarin.GooglePlayServices.Gass",
		"Xamarin.GooglePlayServices.Measurement.Base",
		"Xamarin.GooglePlayServices.Measurement.Sdk",
		"Xamarin.GooglePlayServices.Analytics",
		"Xamarin.GooglePlayServices.Analytics.Impl",
		"Xamarin.GooglePlayServices.TagManager.Api",
		"Xamarin.GooglePlayServices.TagManager.V4.Impl",

		// Firebase packages
		// - Duplicate managed types (due to Xamarin.AndroidX.DataStore.Core.Android and Xamarin.AndroidX.DataStore.Core.Jvm)
		"Xamarin.Firebase.Crashlytics",
		"Xamarin.Firebase.Crashlytics.Ktx",
		"Xamarin.Firebase.Crashlytics.NDK",
		"Xamarin.Firebase.Perf",
		"Xamarin.Firebase.Perf.Ktx",
		"Xamarin.Firebase.Sessions",

		// - Duplicate Java types
		"Xamarin.Firebase.Analytics",
		"Xamarin.Firebase.Analytics.Impl",
		"Xamarin.Firebase.Analytics.Ktx",
		"Xamarin.Firebase.AppIndexing",
		"Xamarin.Firebase.Firestore",
		"Xamarin.Firebase.Firestore.Ktx",
		"Xamarin.Firebase.InAppMessaging",
		"Xamarin.Firebase.InAppMessaging.Display",
		"Xamarin.Firebase.InAppMessaging.Display.Ktx",
		"Xamarin.Firebase.InAppMessaging.Ktx",
		"Xamarin.Firebase.ML.Vision",
		"Xamarin.Firebase.ML.Vision.AutoML",
		"Xamarin.Firebase.ML.Vision.BarCode.Model",
		"Xamarin.Firebase.ML.Vision.Face.Model",
		"Xamarin.Firebase.ML.Vision.Image.Label.Model",
		"Xamarin.Firebase.ML.Vision.Internal.Vkp",
		"Xamarin.Firebase.ML.Vision.Object.Detection.Model",
		"Xamarin.Firebase.ProtoliteWellKnownTypes",

		// MLKit packages
		// - Duplicate Java types
		"Xamarin.Google.MLKit.FaceDetection",

		// Google Play packages
		// - Split into separate packages, these older ones cause duplicate bound types
		"Xamarin.Google.Android.Play.Core",
		"Xamarin.Google.Android.Play.Core.Common",
		"Xamarin.Google.Android.Play.Core.Ktx",

		// - Causes Kotlin.Stdlib version conflicts
		"Xamarin.Google.Android.Play.App.Update.Ktx",
		"Xamarin.Google.Android.Play.Asset.Delivery.Ktx",
		"Xamarin.Google.Android.Play.Core.Ktx",
		"Xamarin.Google.Android.Play.Feature.Delivery.Ktx",
		"Xamarin.Google.Android.Play.Review.Ktx",

		// Tensorflow LiteRT packages
		// - Duplicate Java types
		"Xamarin.Google.AI.Edge.LiteRT",
		"Xamarin.Google.AI.Edge.LiteRT.API",
		"Xamarin.Google.AI.Edge.LiteRT.GPU",
		"Xamarin.Google.AI.Edge.LiteRT.GPU.API",
		"Xamarin.Google.AI.Edge.LiteRT.Metadata",
		"Xamarin.Google.AI.Edge.LiteRT.Support",
		"Xamarin.Google.AI.Edge.LiteRT.Support.API",

		"Xamarin.CodeHaus.Mojo.AnimalSnifferAnnotations",

	];

	static TestAllIndividualPackages ()
	{
		// Find the repo base directory
		while (!File.Exists (Path.Combine (base_dir, "config.json")))
			base_dir = Path.Combine ("..", base_dir);

		// Create the test directory
		Directory.CreateDirectory (Path.Combine (base_dir, test_dir));

		// We need a Directory.Build.props file so we don't use the root one, it also
		// needs to turn off NuGet Central Package Management.
		var directory_props = Path.Combine (base_dir, test_dir, "Directory.Build.props");

		var props_content = """
		<Project>
		  <PropertyGroup>
		    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
		  </PropertyGroup>
		</Project>
		""";

		if (!File.Exists (directory_props))
			File.WriteAllText (directory_props, props_content);

		// Set up a NuGet.config file that allows us to use the locally built NuGet packages.
		var nuget_config_src = Path.Combine (base_dir, "tests", "common", "NuGet.config");
		var nuget_config_dst = Path.Combine (base_dir, test_dir, "NuGet.config");

		if (!File.Exists (nuget_config_dst)) {
			var contents = File.ReadAllText (nuget_config_src);
			contents = contents.Replace ("../output", "../..");
			File.WriteAllText (nuget_config_dst, contents);
		}
	}

	[Test]
	[Category ("Android")]
	public Task TestAndroidDotNetAllPackages ()
		=> TestAllPackages ("android", false);

	[Test]
	[Category ("Android")]
	public Task TestAndroidDotNetAllGPSPackages ()
		=> TestAllPackages ("android", true);

	[Test]
	[Category ("MAUI")]
	public Task TestMauiAllPackages ()
		=> TestAllPackages ("maui", false);

	[Test]
	[Category ("MAUI")]
	public Task TestMauiAllGPSPackages ()
		=> TestAllPackages ("maui", true);

	async Task TestAllPackages (string template, bool isGps)
	{
		var case_dir = Path.Combine (base_dir, test_dir, template, $"AllPackagesTest{(isGps ? "-GPS" : "")}");

		// Test the package
		try {
			if (Directory.Exists (case_dir))
				Directory.Delete (case_dir, true);
		} catch {
			// Ignore
		}
		Directory.CreateDirectory (case_dir);

		// Create new dotnet project
		await RunAndAssertSuccess ($"new {template}", case_dir);

		// - Replace <SupportedOSPlatformVersion> with the maximum version some packages require
		// - Remove the target frameworks that are not 'android'
		var proj_file = Directory.GetFiles (case_dir, "*.csproj").FirstOrDefault ();

		if (proj_file is null) {
			Assert.Fail ("Could not find the project file.");
			return;
		}

		XmlDocument xd = new ();
		xd.Load (proj_file);

		XmlNodeList nl = xd.SelectNodes("//*[starts-with(name(), 'TargetFramework')]");

		if (nl is not null) {
			foreach (XmlNode node in nl) {
				node.InnerText = $"{net_version}-android";
			}
		}
		xd.Save(proj_file);

    ReplaceInFile (proj_file, ">21</SupportedOSPlatformVersion>", $">{platform_version}</SupportedOSPlatformVersion>");
		ReplaceInFile (proj_file, ">21.0</SupportedOSPlatformVersion>", $">{platform_version}</SupportedOSPlatformVersion>");
		ReplaceInFile (proj_file, $";{net_version}-ios", "");
		ReplaceInFile (proj_file, $";{net_version}-maccatalyst", "");
		ReplaceInFile (proj_file, $";{net_version}-windows10.0.19041.0", "");

		// Get all packages to test
		var packages = GetAllPackages (isGps);

		// Add the package
		AddPackagesToProjectFile (proj_file, packages.ToArray ());

		// Build the project
		await RunAndAssertSuccess ($"build -c {configuration} -bl", case_dir, true);

		// If we're here, everything succeeded, so try to clean up the project directory
		try {
			Directory.Delete (case_dir, true);
		} catch {
			// Ignore
		}
	}

	static IEnumerable<BinderatorConfigFileParser.ArtifactModel> GetAllPackages (bool isGps)
	{
		var config_file = Path.Combine (base_dir, "config.json");
		var config = BinderatorConfigFileParser.ParseConfigurationFile (config_file).Result;

		var packages = config.FirstOrDefault ()?.Artifacts?.Where (a => !a.DependencyOnly) ?? [];

		if (isGps)
			return packages.Where (p => p.TemplateSet == "gps");

		return packages.Where (p => p.TemplateSet != "gps");
	}

	static void ReplaceInFile (string filename, string oldValue, string newValue)
	{
		var contents = File.ReadAllText (filename);
		contents = contents.Replace (oldValue, newValue);
		File.WriteAllText (filename, contents);
	}

	static void AddPackagesToProjectFile (string filename, BinderatorConfigFileParser.ArtifactModel [] packages)
	{
		var xml = new XmlDocument ();
		xml.Load (filename);

		var root = xml.DocumentElement!;
		var item_group = xml.CreateElement ("ItemGroup");
		item_group.SetAttribute ("Condition", "$(TargetFramework.Contains('-android')) == true");
		root.AppendChild (item_group);

		foreach (var package in packages) {
			if (ignored_packages.Contains (package.NugetId))
				continue;

			var package_ref = xml.CreateElement ("PackageReference");
			package_ref.SetAttribute ("Include", package.NugetId);
			package_ref.SetAttribute ("Version", package.NugetVersion);
			item_group.AppendChild (package_ref);
		}

		// Add <NoWarn> and <JavaMaximumHeapSize>
		var property_group = root ["PropertyGroup"]!;
		var no_warn = xml.CreateElement ("NoWarn");
		no_warn.InnerText = "NU1603;NU1605;NU1608";
		property_group.AppendChild (no_warn);

		var heap_size = xml.CreateElement ("JavaMaximumHeapSize");
		heap_size.InnerText = "4G";
		property_group.AppendChild (heap_size);

		xml.Save (filename);
	}

	static async Task RunAndAssertSuccess (string arguments, string workingDir, bool isMSBuild = false)
	{
		var result = await Cli.Wrap ("dotnet")
			.WithArguments (arguments)
			.WithWorkingDirectory (workingDir)
			.WithValidation (CommandResultValidation.None)
			.ExecuteBufferedAsync ();

		if (result.ExitCode == 0)
			return;

		var sb = new StringBuilder ();

		sb.AppendLine ($"Command '{arguments}' failed with exit code {result.ExitCode}.");

		if (!isMSBuild) {
			sb.AppendLine ("Output:");
			sb.AppendLine (result.StandardOutput);
			sb.AppendLine ();
			sb.AppendLine ("Error:");
			sb.AppendLine (result.StandardError);

			Assert.Fail (sb.ToString ());
		}

		var errors = new List<string> ();
		var warnings = new List<string> ();

		using (var sr = new StringReader (result.StandardOutput)) {
			string? line;

			while ((line = sr.ReadLine ()) != null) {

				// MSBuild prints all the messages out again after this message
				if (line == "Build succeeded." || line == "Build FAILED.")
					break;

				if (CanonicalError.Parse (line) is CanonicalError.Parts parts) {
					var message = $"{parts.code}: {parts.text}";

					if (parts.category == CanonicalError.Parts.Category.Warning)
						warnings.Add (message);
					else
						errors.Add (message);
				}
			}
		}

		if (errors.Count > 0) {
			sb.AppendLine ("Errors:");
			errors.ForEach (e => sb.AppendLine (e));
		}

		if (warnings.Count > 0) {
			sb.AppendLine ("Warnings:");
			warnings.ForEach (w => sb.AppendLine (w));
		}

		Assert.Fail (sb.ToString ());
	}
}
