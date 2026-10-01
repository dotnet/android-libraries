// Copyright (c) 2015-2016 Xamarin Inc.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Mono.Cecil;
using Xamarin.ContentPipeline.Tests;
using Xamarin.MacDev;
using Xunit;

namespace NativeLibraryDownloaderTests
{
	public class Test : TestsBase
	{
		public static string Configuration = "Release";
		public static readonly string[] DEFAULT_IGNORE_PATTERNS = { "*.overridetasks", "*.tasks" };
		const string DotNetPublicMavenGson = "https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-public-maven/maven/v1/com/google/code/gson/gson/2.11.0/gson-2.11.0.jar";

		// Serves raw deflate entry ranges, matching the bytes selected from a ZIP archive.
		sealed class PartialZipTestServer : IDisposable
		{
			readonly CancellationTokenSource cancellation = new ();
			readonly TcpListener listener = new (IPAddress.Loopback, 0);
			readonly byte [] payload;
			readonly Task serverTask;

			public PartialZipTestServer (params byte [][] entries)
			{
				var ranges = new List<(long Start, long End)> ();
				using (var stream = new MemoryStream ()) {
					foreach (var entry in entries) {
						var start = stream.Position;
						using (var deflate = new DeflateStream (stream, CompressionLevel.Optimal, leaveOpen: true))
							deflate.Write (entry, 0, entry.Length);
						ranges.Add ((start, stream.Position - 1));
					}
					payload = stream.ToArray ();
				}
				Ranges = ranges;

				listener.Start ();
				Url = $"http://127.0.0.1:{((IPEndPoint) listener.LocalEndpoint).Port}/fixture.zip";
				serverTask = Task.Run (ServeAsync);
			}

			public IReadOnlyList<(long Start, long End)> Ranges { get; }
			public string Url { get; }

			async Task ServeAsync ()
			{
				while (!cancellation.IsCancellationRequested) {
					TcpClient client;
					try {
						client = await listener.AcceptTcpClientAsync (cancellation.Token);
					} catch (OperationCanceledException) when (cancellation.IsCancellationRequested) {
						break;
					} catch (SocketException) when (cancellation.IsCancellationRequested) {
						break;
					}

					using (client)
						await RespondAsync (client);
				}
			}

			async Task RespondAsync (TcpClient client)
			{
				var stream = client.GetStream ();
				using var reader = new StreamReader (stream, Encoding.ASCII, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
				string rangeHeader = null;
				string line;
				while (!string.IsNullOrEmpty (line = await reader.ReadLineAsync ())) {
					if (line.StartsWith ("Range: bytes=", StringComparison.OrdinalIgnoreCase))
						rangeHeader = line.Substring ("Range: bytes=".Length);
				}

				var range = rangeHeader?.Split ('-');
				if (range?.Length != 2 ||
					!long.TryParse (range [0], out var start) ||
					!long.TryParse (range [1], out var end) ||
					start < 0 ||
					end < start ||
					end >= payload.Length)
					throw new InvalidOperationException ($"Invalid range header: {rangeHeader}");

				var length = checked((int) (end - start + 1));
				var headers = Encoding.ASCII.GetBytes (
					$"HTTP/1.1 206 Partial Content\r\nContent-Length: {length}\r\nContent-Range: bytes {start}-{end}/{payload.Length}\r\nConnection: close\r\n\r\n");
				await stream.WriteAsync (headers);
				await stream.WriteAsync (payload.AsMemory (checked((int) start), length));
				await stream.FlushAsync ();
			}

			public void Dispose ()
			{
				cancellation.Cancel ();
				listener.Stop ();
				serverTask.GetAwaiter ().GetResult ();
				cancellation.Dispose ();
			}
		}

		void AddCoreTargets (ProjectRootElement el)
		{
			var baseDir = new Uri(System.Reflection.Assembly.GetExecutingAssembly().Location).LocalPath;

			var props = Path.Combine (baseDir, "..", "..", "source", "Xamarin.Build.Download", "bin", Configuration, "netstandard20", "Xamarin.Build.Download.props");

			if (!File.Exists(props))
				props = Path.Combine(baseDir, "..", "Xamarin.Build.Download.props");

			el.AddImport (props);
			var targets = Path.Combine (baseDir, "..", "..", "source", "Xamarin.Build.Download", "bin", Configuration, "netstandard20", "Xamarin.Build.Download.targets");
			if (!File.Exists(targets))
				targets = Path.Combine(baseDir, "..", "Xamarin.Build.Download.targets");

			el.AddImport (targets);

		}

		[Fact]
		public void NoArchivesOrTargets ()
		{
			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);

			var log = new MSBuildTestLogger ();
			var success = BuildProject (engine, project, "_XamarinBuildDownload", log);

			AssertNoMessagesOrWarnings (log);
			Assert.True (success);
		}

		[Fact]
		public void InvalidArchiveMetadata ()
		{
			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			var unpackDir = GetTempPath ("unpacked");
			prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);

			prel.AddItem ("XamarinBuildDownload", "-----");

			prel.AddItem (
				"XamarinBuildDownload", "foo-1.2", new Dictionary<string, string> {
				});

			prel.AddItem (
				"XamarinBuildDownload", "bar-1.9", new Dictionary<string, string> {
					{ "Url", "https://www.example.com/bar.zip" },
					{ "Kind", "Cabbage" }
				});

			prel.AddItem (
				"XamarinBuildDownload", "baz-1.9", new Dictionary<string, string> {
					{ "Url", "https://www.example.com/bar.unknown" }
				});

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();

			var success = BuildProject (engine, project, "_XamarinBuildDownload", log);

			var errors = log.Errors.Select (e => e.Message).ToList ();
			Assert.Equal (4, errors.Count);
			Assert.Equal ("Invalid item ID -----", errors[0]);
			Assert.Equal ("Unknown archive kind 'Cabbage' for 'https://www.example.com/bar.zip'", errors[1]);
			Assert.Equal ("Unknown archive kind '' for 'https://www.example.com/bar.unknown'", errors[2]);
			Assert.Equal ("Missing required Url metadata on item foo-1.2", errors[3]);

			Assert.False (success);
		}

		[Fact]
		public void TestZipDownload ()
		{
			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			var unpackDir = GetTempPath ("unpacked");
			prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);

			prel.AddItem (
				"XamarinBuildDownload", "ILRepack-2.0.10", new Dictionary<string, string> {
					{ "Url", "https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-public/nuget/v3/flat2/ilrepack/2.0.10/ilrepack.2.0.10.nupkg" },
					{ "Kind", "Zip" }
				});

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();

			var success = BuildProject (engine, project, "_XamarinBuildDownload", log);


			AssertNoMessagesOrWarnings (log, DEFAULT_IGNORE_PATTERNS);
			Assert.True (success);

			Assert.True (File.Exists (Path.Combine (unpackDir, "ILRepack-2.0.10", "ILRepack.nuspec")));
		}

		[Fact]
		public void TestTgzDownload ()
		{
			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			var unpackDir = GetTempPath ("unpacked");
			prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);

			prel.AddItem (
				"XamarinBuildDownload", "GoogleSymbolUtilities-1.0.3", new Dictionary<string, string> {
					{ "Url", "https://www.gstatic.com/cpdc/a060f37adbad54ea-GoogleSymbolUtilities-1.0.3.tar.gz" },
					{ "Kind", "Tgz" }
				});

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();

			var success = BuildProject (engine, project, "_XamarinBuildDownload", log);

			AssertNoMessagesOrWarnings (log, DEFAULT_IGNORE_PATTERNS);
			Assert.True (success);

			Assert.True (File.Exists (Path.Combine (unpackDir, "GoogleSymbolUtilities-1.0.3", "Libraries", "libGSDK_Overload.a")));
		}

		[Fact]
		public void Test7ZipExtractionIsNonInteractive ()
		{
			var method = typeof (Xamarin.Build.Download.XamarinDownloadArchives).GetMethod (
				"Build7ZipExtractionArgs",
				BindingFlags.NonPublic | BindingFlags.Static);
			Assert.True (method != null, "Could not find XamarinDownloadArchives.Build7ZipExtractionArgs via reflection.");
			var sevenZipPath = Path.Combine (TempDir, "7z.exe");
			File.WriteAllText (sevenZipPath, string.Empty);

			var args = method.Invoke (null, new object [] {
				"archive.tgz",
				TempDir,
				sevenZipPath,
				false,
				null,
			});

			Assert.Contains ("-y", args.ToString ());
		}

		[Fact]
		public void TestUncompressedNamedDownload ()
		{
			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			var unpackDir = GetTempPath ("unpacked");
			prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);

			prel.AddItem (
				"XamarinBuildDownload", "Gson-2.11.0", new Dictionary<string, string> {
					{ "Url", DotNetPublicMavenGson },
					{ "Kind", "Uncompressed" },
					{ "ToFile", "gson.jar" }
				});

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();

			var success = BuildProject (engine, project, "_XamarinBuildDownload", log);

			AssertNoMessagesOrWarnings (log, DEFAULT_IGNORE_PATTERNS);
			Assert.True (success);

			Assert.True (File.Exists (Path.Combine (unpackDir, "Gson-2.11.0", "gson.jar")));
		}

		[Fact]
		public void TestUncompressedUnnamedDownload ()
		{
			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			var unpackDir = GetTempPath ("unpacked");
			prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);

			prel.AddItem (
				"XamarinBuildDownload", "Gson-2.11.0", new Dictionary<string, string> {
					{ "Url", DotNetPublicMavenGson },
					{ "Kind", "Uncompressed" },
				});

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();

			var success = BuildProject (engine, project, "_XamarinBuildDownload", log);

			AssertNoMessagesOrWarnings (log, DEFAULT_IGNORE_PATTERNS);
			Assert.True (success);

			Assert.True (File.Exists (Path.Combine (unpackDir, "Gson-2.11.0", "Gson-2.11.0.uncompressed")));
		}

		//in google maps, the tar inside the tgz doesn't match the tgz name
		[Fact]
		public void TestGMapsDownload ()
		{
			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			var unpackDir = GetTempPath ("unpacked");
			prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);

			prel.AddItem (
				"XamarinBuildDownload", "GMaps-1.11.1", new Dictionary<string, string> {
					{ "Url", "https://www.gstatic.com/cpdc/c0e534927c0c955e-GoogleMaps-1.11.1.tar.gz" },
					{ "Kind", "Tgz" }
				});

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();

			var success = BuildProject (engine, project, "_XamarinBuildDownload", log);

			AssertNoMessagesOrWarnings (log, DEFAULT_IGNORE_PATTERNS);
			Assert.True (success);

			Assert.True (File.Exists (Path.Combine (unpackDir, "GMaps-1.11.1", "CHANGELOG")));
		}

		[Fact]
		public void TestCastAssemblyResources ()
		{
			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			var asm = AssemblyDefinition.CreateAssembly (
				new AssemblyNameDefinition ("Foo", new Version (1, 0, 0, 0)),
				"Main",
				ModuleKind.Dll
			);
			var dll = Path.Combine (TempDir, "Foo.dll");
			asm.Write (dll);

			var unpackDir = GetTempPath ("unpacked");
			prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);
			prel.SetProperty ("TargetFrameworkIdentifier", "Xamarin.iOS");
			prel.SetProperty ("OutputType", "Exe");
			prel.SetProperty ("IntermediateOutputPath", Path.Combine (TempDir, "obj"));

			prel.AddItem (
				"XamarinBuildDownload", "AppInvites-1.0.2", new Dictionary<string, string> {
					{ "Url", "https://www.gstatic.com/cpdc/278f79fcd3b365e3-AppInvites-1.0.2.tar.gz" },
					{ "Kind", "Tgz" }
				});

			prel.AddItem (
				"ReferencePath",
				dll
			);

			var bundlePath = Path.Combine (unpackDir, "AppInvites-1.0.2", "Frameworks", "GINInvite.framework", "Versions", "A", "Resources", "GINInviteResources.bundle");

			var plist = Path.Combine (bundlePath, "Info.plist");
			string resourceName = "__monotouch_content_GINInviteResources.bundle_fInfo.plist";
			prel.AddItem (
				"RestoreAssemblyResource",
				plist,
				new Dictionary<string,string> {
					{ "AssemblyName", "Foo" },
					{ "LogicalName", resourceName }
				}
			);

			var image = Path.Combine (bundlePath, "ic_sms_24@3x.png");
			resourceName = "__monotouch_content_GINInviteResources.bundle_fic__sms__24%403x.png";
			prel.AddItem (
				"RestoreAssemblyResource",
				image,
				new Dictionary<string, string> {
					{ "AssemblyName", "Foo" },
					{ "LogicalName", resourceName }
				}
			);

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();

			var success = BuildProject (engine, project, "_XamarinBuildCastAssemblyResources", log);

			var ignoreMessages = new List<string> { "Enumeration yielded no results" };
			ignoreMessages.AddRange (DEFAULT_IGNORE_PATTERNS);
			AssertNoMessagesOrWarnings (log, ignoreMessages.ToArray());
			Assert.True (success);

			var plistExists = File.Exists (plist);
			Assert.True (plistExists);

			var imageExists = File.Exists (image);
			Assert.True (imageExists);

			// Check if BundleResources were generated correctly.
			var bundleResources = project.GetItems ("BundleResource");
			Assert.True (bundleResources != null);

			// Check if Optimize metadata was generated correctly.
			var imageResource = bundleResources.Single (b => b.GetMetadataValue ("Identity").ToLower ().EndsWith (".png"));
			Assert.True (imageResource.GetMetadataValue ("Optimize") == "False");
		}

		[Fact]
		public void TestAndroidAarAdded()
		{
			var unpackDir = GetTempPath("unpacked");
			var artifactXbdId = "gpsbasement-16.2.0";

			var r = AndroidAarAdd(unpackDir, artifactXbdId, "https://dl.google.com/dl/android/maven2/com/google/android/gms/play-services-basement/16.2.0/play-services-basement-16.2.0.aar", true);

			AssertNoMessagesOrWarnings(r.logs, DEFAULT_IGNORE_PATTERNS);
			Assert.True(r.success);

			var aarPath = Path.Combine(unpackDir, artifactXbdId, artifactXbdId + ".aar");
			Assert.True(File.Exists(aarPath));

			Assert.Contains(r.project.Items, i => i.ItemType == "AndroidAarLibrary" && i.EvaluatedInclude == aarPath);
		}

		[Fact]
		public void TestAndroidAarIdeTooOld()
		{
			var unpackDir = GetTempPath("unpacked");
			var artifactXbdId = "gpsbasement-16.2.0";

			var r = AndroidAarAdd(unpackDir, artifactXbdId, "https://dl.google.com/dl/android/maven2/com/google/android/gms/play-services-basement/16.2.0/play-services-basement-16.2.0.aar", false);

			// Check for the error
			Assert.NotEmpty(r.logs.Errors);

			Assert.False(r.success);
		}

		(bool success, ProjectInstance project, MSBuildTestLogger logs) AndroidAarAdd(string unpackDir, string artifactXbdId, string url, bool androidAarLibraryAvailableItem)
		{
			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);


			prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);
			prel.SetProperty ("TargetFrameworkIdentifier", "MonoAndroid");
			prel.SetProperty ("TargetFrameworkVersion", "v9.0");
			prel.SetProperty ("OutputType", "Exe");
			prel.SetProperty ("IntermediateOutputPath", Path.Combine (TempDir, "obj"));

			if (androidAarLibraryAvailableItem)
				prel.AddItem("AvailableItemName", "AndroidAarLibrary");


			var item = prel.AddItem (
				"XamarinBuildDownload", artifactXbdId, new Dictionary<string, string> {
					{ "Url", url },
					{ "Kind", "Uncompressed" },
					{ "ToFile", artifactXbdId + ".aar" }
				});

			prel.AddItem (
				"XamarinBuildDownloadAndroidAarLibrary",
				"$(XamarinBuildDownloadDir)" + artifactXbdId + "\\" + artifactXbdId + ".aar",
				new Dictionary<string, string> { }
			);

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();
			log.Verbosity = Microsoft.Build.Framework.LoggerVerbosity.Diagnostic;

			var success = BuildProject (engine, project, "_XamarinBuildDownloadAarInclude", log);

			return (success, project, log);
		}

		[Fact]
		public void TestDisallowUnsafeGetItemsToDownload ()
		{
			var itemUrl = "http://example.com/artifact.aar";
			var zipUrl = "http://dl-ssl.google.com/android/repository/android_m2repository_r40.zip";

			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			var unpackDir = GetTempPath ("unpacked");
			prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);

			prel.AddItem (
				"XamarinBuildDownload", "Gson-2.11.0", new Dictionary<string, string> {
					{ "Url", itemUrl },
					{ "Kind", "Uncompressed" },
				});

			prel.AddItem (
				"XamarinBuildDownloadPartialZip", "androidsupport-25.0.1/cardview.v7", new Dictionary<string, string> {
					{ "Url", zipUrl },
					{ "ToFile", "cardview.v7.aar" },
					{ "RangeStart", "196438127" },
					{ "RangeEnd", "196460160" },
				});

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();

			var success = BuildProject (engine, project, "XamarinBuildDownloadGetItemsToDownload", log);

			var itemToDownload = project.GetItems ("XamarinBuildDownloadItemToDownload");

			Assert.Empty (itemToDownload);
		}

		[Fact]
		public void TestAllowUnsafeGetItemsToDownload ()
		{
			var itemUrl = "http://example.com/artifact.aar";
			var zipUrl = "http://dl-ssl.google.com/android/repository/android_m2repository_r40.zip";

			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			var unpackDir = GetTempPath ("unpacked");
			prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);
			prel.SetProperty ("XamarinBuildDownloadAllowUnsecure", "true");

			prel.AddItem (
				"XamarinBuildDownload", "Gson-2.11.0", new Dictionary<string, string> {
					{ "Url", itemUrl },
					{ "Kind", "Uncompressed" },
				});

			prel.AddItem (
				"XamarinBuildDownloadPartialZip", "androidsupport-25.0.1/cardview.v7", new Dictionary<string, string> {
					{ "Url", zipUrl },
					{ "ToFile", "cardview.v7.aar" },
					{ "RangeStart", "196438127" },
					{ "RangeEnd", "196460160" },
				});

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();

			var success = BuildProject (engine, project, "XamarinBuildDownloadGetItemsToDownload", log);

			var itemToDownload = project.GetItems ("XamarinBuildDownloadItemToDownload");

			Assert.Equal (2, itemToDownload.Count);
			Assert.Contains(itemToDownload, i => i.GetMetadata ("Url").EvaluatedValue == itemUrl);
			Assert.Contains(itemToDownload, i => i.GetMetadata ("Url").EvaluatedValue == zipUrl);
		}

		[Fact]
		public void TestGetItemsToDownload ()
		{
			var itemUrl = DotNetPublicMavenGson;

			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			var unpackDir = GetTempPath ("unpacked");
			prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);

			prel.AddItem (
				"XamarinBuildDownload", "Gson-2.11.0", new Dictionary<string, string> {
					{ "Url", itemUrl },
					{ "Kind", "Uncompressed" },
				});

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();

			var success = BuildProject (engine, project, "XamarinBuildDownloadGetItemsToDownload", log);

			AssertNoMessagesOrWarnings (log, DEFAULT_IGNORE_PATTERNS);
			Assert.True (success);

			var itemToDownload = project.GetItems ("XamarinBuildDownloadItemToDownload");

			Assert.Single (itemToDownload);
			Assert.True (itemToDownload.First ().GetMetadata ("Url").EvaluatedValue == itemUrl);
		}

		[Fact]
		public void TestGoogleMavenRepositoryOverride ()
		{
			const string googleMavenUrl = "https://dl.google.com/dl/android/maven2/com/google/android/gms/play-services-base/17.6.0/play-services-base-17.6.0.aar";
			const string otherUrl = "https://example.com/com/example/library/1.0.0/library-1.0.0.aar";
			const string repository = "https://packages.example.com/maven/v1";

			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);
			prel.SetProperty ("XamarinBuildDownloadGoogleMavenRepository", repository);
			prel.AddItem ("XamarinBuildRestoreResources", "_AddRepositoryOverrideTestItems");

			var restoreTarget = prel.AddTarget ("_AddRepositoryOverrideTestItems");
			var itemGroup = restoreTarget.AddItemGroup ();
			var googleItem = itemGroup.AddItem ("XamarinBuildDownload", "gpsbase-17.6.0");
			googleItem.AddMetadata ("Url", googleMavenUrl);
			googleItem.AddMetadata ("Kind", "Uncompressed");
			googleItem.AddMetadata ("ToFile", "play-services-base.aar");
			googleItem.AddMetadata ("Sha256", "0123456789abcdef");
			googleItem.AddMetadata ("CustomErrorCode", "TEST001");
			googleItem.AddMetadata ("CustomErrorMessage", "Test error");
			var otherItem = itemGroup.AddItem ("XamarinBuildDownload", "other-1.0.0");
			otherItem.AddMetadata ("Url", otherUrl);
			otherItem.AddMetadata ("Kind", "Uncompressed");

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();
			var success = BuildProject (engine, project, "_XamarinBuildApplyRepositoryOverrides", log);

			AssertNoMessagesOrWarnings (log, DEFAULT_IGNORE_PATTERNS);
			Assert.True (success);

			var downloads = project.GetItems ("XamarinBuildDownload");
			Assert.Equal (2, downloads.Count);

			var rewritten = downloads.Single (item => item.EvaluatedInclude == "gpsbase-17.6.0");
			Assert.Equal (
				repository + "/com/google/android/gms/play-services-base/17.6.0/play-services-base-17.6.0.aar",
				rewritten.GetMetadataValue ("Url"));
			Assert.Equal ("Uncompressed", rewritten.GetMetadataValue ("Kind"));
			Assert.Equal ("play-services-base.aar", rewritten.GetMetadataValue ("ToFile"));
			Assert.Equal ("0123456789abcdef", rewritten.GetMetadataValue ("Sha256"));
			Assert.Equal ("TEST001", rewritten.GetMetadataValue ("CustomErrorCode"));
			Assert.Equal ("Test error", rewritten.GetMetadataValue ("CustomErrorMessage"));

			var unchanged = downloads.Single (item => item.EvaluatedInclude == "other-1.0.0");
			Assert.Equal (otherUrl, unchanged.GetMetadataValue ("Url"));
		}

		[Fact]
		public void TestGoogleMavenRepositoryOverrideDefaultsToDisabled ()
		{
			const string googleMavenUrl = "https://dl.google.com/dl/android/maven2/com/google/android/gms/play-services-base/17.6.0/play-services-base-17.6.0.aar";

			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);
			prel.AddItem ("XamarinBuildRestoreResources", "_AddRepositoryOverrideTestItems");

			var restoreTarget = prel.AddTarget ("_AddRepositoryOverrideTestItems");
			var googleItem = restoreTarget.AddItemGroup ().AddItem ("XamarinBuildDownload", "gpsbase-17.6.0");
			googleItem.AddMetadata ("Url", googleMavenUrl);
			googleItem.AddMetadata ("Kind", "Uncompressed");

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();
			var success = BuildProject (engine, project, "_XamarinBuildApplyRepositoryOverrides", log);

			AssertNoMessagesOrWarnings (log, DEFAULT_IGNORE_PATTERNS);
			Assert.True (success);
			Assert.Equal (googleMavenUrl, project.GetItems ("XamarinBuildDownload").Single ().GetMetadataValue ("Url"));
		}

		[Fact]
		public void TestGoogleMavenRepositoryOverrideGetItemsToDownload ()
		{
			const string googleMavenUrl = "https://dl.google.com/dl/android/maven2/com/google/android/gms/play-services-base/17.6.0/play-services-base-17.6.0.aar";
			const string partialZipUrl = "https://dl-ssl.google.com/android/repository/android_m2repository_r40.zip";
			const string repository = "https://packages.example.com/maven/v1/";

			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);
			prel.SetProperty ("XamarinBuildDownloadDir", GetTempPath ("unpacked"));
			prel.SetProperty ("XamarinBuildDownloadGoogleMavenRepository", repository);
			prel.AddItem ("XamarinBuildRestoreResources", "_AddRepositoryOverrideTestItems");

			var restoreTarget = prel.AddTarget ("_AddRepositoryOverrideTestItems");
			var itemGroup = restoreTarget.AddItemGroup ();
			var googleItem = itemGroup.AddItem ("XamarinBuildDownload", "gpsbase-17.6.0");
			googleItem.AddMetadata ("Url", googleMavenUrl);
			googleItem.AddMetadata ("Kind", "Uncompressed");
			var partialZipItem = itemGroup.AddItem ("XamarinBuildDownloadPartialZip", "androidsupport-25.0.1/cardview.v7");
			partialZipItem.AddMetadata ("Url", partialZipUrl);
			partialZipItem.AddMetadata ("ToFile", "cardview.v7.aar");
			partialZipItem.AddMetadata ("RangeStart", "196438127");
			partialZipItem.AddMetadata ("RangeEnd", "196460160");

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();
			var success = BuildProject (engine, project, "XamarinBuildDownloadGetItemsToDownload", log);

			AssertNoMessagesOrWarnings (log, DEFAULT_IGNORE_PATTERNS);
			Assert.True (success);

			var itemsToDownload = project.GetItems ("XamarinBuildDownloadItemToDownload");
			Assert.Equal (2, itemsToDownload.Count);
			Assert.Contains (
				itemsToDownload,
				item => item.GetMetadataValue ("Url") == repository + "com/google/android/gms/play-services-base/17.6.0/play-services-base-17.6.0.aar");
			Assert.Contains (itemsToDownload, item => item.GetMetadataValue ("Url") == partialZipUrl);
		}

		[Fact]
		public void TestDeduplicateGetItemsToDownload ()
		{
			var itemUrl = DotNetPublicMavenGson;

			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			var unpackDir = GetTempPath ("unpacked");
			prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);

			prel.AddItem (
				"XamarinBuildDownload", "FacebookAndroid-4.17.0", new Dictionary<string, string> {
					{ "Url", itemUrl },
					{ "Kind", "Uncompressed" },
				});

			prel.AddItem (
				"XamarinBuildDownload", "FacebookAndroid-4.17.0", new Dictionary<string, string> {
					{ "Url", itemUrl },
					{ "Kind", "Uncompressed" },
				});

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();

			var success = BuildProject (engine, project, "XamarinBuildDownloadGetItemsToDownload", log);

			AssertNoMessagesOrWarnings (log, DEFAULT_IGNORE_PATTERNS);
			Assert.True (success);

			var itemToDownload = project.GetItems ("XamarinBuildDownloadItemToDownload");

			Assert.Single (itemToDownload);
			Assert.True (itemToDownload.First ().GetMetadata ("Url").EvaluatedValue == itemUrl);
		}

		[Fact]
		public void TestTimesOutWaitingOnExclusiveLock ()
		{
			var unpackDir = GetTempPath ("unpacked");
			System.IO.Directory.CreateDirectory (unpackDir);

			var lockFile = Path.Combine(unpackDir, "GMaps-1.11.1.locked");
			using (var lockStream = File.Open (lockFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)) {

				var engine = new ProjectCollection ();
				var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

				prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);

				prel.AddItem (
					"XamarinBuildDownload", "GMaps-1.11.1", new Dictionary<string, string> {
					{ "Url", "https://www.gstatic.com/cpdc/c0e534927c0c955e-GoogleMaps-1.11.1.tar.gz" },
					{ "Kind", "Tgz" },
					{ "ExclusiveLockTimeout", "1" }
					});

				AddCoreTargets (prel);

				var project = new ProjectInstance (prel);
				var log = new MSBuildTestLogger ();

				var success = BuildProject (engine, project, "_XamarinBuildDownload", log);

				Assert.False (success);
				Assert.Null (log.Errors.FirstOrDefault (err => err.Code == "XBD005"));
			}
		}

		[Fact]
		public void TestSinglePartialZipDownload ()
		{
			using var server = new PartialZipTestServer (Encoding.UTF8.GetBytes ("manifest"));
			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			var unpackDir = GetTempPath ("unpacked");
			prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);
			prel.SetProperty ("XamarinBuildDownloadAllowUnsecure", "true");

			prel.AddItem (
				"XamarinBuildDownloadPartialZip", "fixture-1.0.0/manifest", new Dictionary<string, string> {
					{ "Url", server.Url },
					{ "ToFile", "manifest.mf" },
					{ "RangeStart", server.Ranges [0].Start.ToString () },
					{ "RangeEnd", server.Ranges [0].End.ToString () },
				});

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();

			var success = BuildProject (engine, project, "_XamarinBuildDownload", log);

			AssertNoMessagesOrWarnings (log, DEFAULT_IGNORE_PATTERNS);
			Assert.True (success);

			Assert.Equal ("manifest", File.ReadAllText (Path.Combine (unpackDir, "fixture-1.0.0", "manifest", "manifest.mf")));
		}


		[Fact]
		public void TestMultiplePartialZipDownload ()
		{
			using var server = new PartialZipTestServer (
				Encoding.UTF8.GetBytes ("manifest"),
				Encoding.UTF8.GetBytes ("proguard"));
			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			var unpackDir = GetTempPath ("unpacked");
			prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);
			prel.SetProperty ("XamarinBuildDownloadAllowUnsecure", "true");

			prel.AddItem (
				"XamarinBuildDownloadPartialZip", "fixture-1.0.0/manifest", new Dictionary<string, string> {
					{ "Url", server.Url },
					{ "ToFile", "manifest.mf" },
					{ "RangeStart", server.Ranges [0].Start.ToString () },
					{ "RangeEnd", server.Ranges [0].End.ToString () },
				});

			prel.AddItem (
				"XamarinBuildDownloadPartialZip", "fixture-1.0.0/proguard", new Dictionary<string, string> {
					{ "Url", server.Url },
					{ "ToFile", "gson.pro" },
					{ "RangeStart", server.Ranges [1].Start.ToString () },
					{ "RangeEnd", server.Ranges [1].End.ToString () },
				});

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();

			var success = BuildProject (engine, project, "_XamarinBuildDownload", log);

			AssertNoMessagesOrWarnings (log, DEFAULT_IGNORE_PATTERNS);
			Assert.True (success);

			Assert.Equal ("manifest", File.ReadAllText (Path.Combine (unpackDir, "fixture-1.0.0", "manifest", "manifest.mf")));
			Assert.Equal ("proguard", File.ReadAllText (Path.Combine (unpackDir, "fixture-1.0.0", "proguard", "gson.pro")));
		}



		[Fact]
		public void TestGetPartialZipItemsToDownload ()
		{
			var itemUrl = "https://dl-ssl.google.com/android/repository/android_m2repository_r40.zip";

			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			var unpackDir = GetTempPath ("unpacked");
			prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);

			prel.AddItem (
				"XamarinBuildDownloadPartialZip", "androidsupport-25.0.1/cardview.v7", new Dictionary<string, string> {
					{ "Url", itemUrl },
					{ "ToFile", "cardview.v7.aar" },
					{ "RangeStart", "196438127" },
					{ "RangeEnd", "196460160" },
				});

			prel.AddItem (
				"XamarinBuildDownloadPartialZip", "androidsupport-25.0.1/recyclerview.v7", new Dictionary<string, string> {
					{ "Url", itemUrl },
					{ "ToFile", "recyclerview.v7.aar" },
					{ "RangeStart", "199278205" },
					{ "RangeEnd", "199589731" },
				});

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();

			var success = BuildProject (engine, project, "XamarinBuildDownloadGetItemsToDownload", log);

			AssertNoMessagesOrWarnings (log, DEFAULT_IGNORE_PATTERNS);
			Assert.True (success);

			var itemToDownload = project.GetItems ("XamarinBuildDownloadItemToDownload");

			Assert.Equal (2, itemToDownload.Count);
			Assert.True (itemToDownload.First ().GetMetadata ("Url").EvaluatedValue == itemUrl);
		}

		[Fact]
		public void TestPathGreaterThan260Chars ()
		{
			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			var unpackDir = GetTempPath ("unpacked");

			for (var i = 1; unpackDir.Length < 260; i++)
				unpackDir = Path.Combine (unpackDir, $"segment{i}");

			prel.SetProperty ("XamarinBuildDownloadDir", unpackDir);

			prel.AddItem (
				"XamarinBuildDownload", "GAppM-8.8.0", new Dictionary<string, string> {
					{ "Url", "https://dl.google.com/firebase/ios/analytics/86849febfdc4ff13/GoogleAppMeasurement-8.8.0.tar.gz" },
					{ "Kind", "Tgz" }
				});

			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();

			var success = BuildProject (engine, project, "_XamarinBuildDownload", log);

			AssertNoMessagesOrWarnings (log, DEFAULT_IGNORE_PATTERNS);
			Assert.True (success);

			Assert.True (File.Exists (Path.Combine (unpackDir, "GAppM-8.8.0", "GoogleAppMeasurement-8.8.0", "dummy.txt")));
		}
	}
}