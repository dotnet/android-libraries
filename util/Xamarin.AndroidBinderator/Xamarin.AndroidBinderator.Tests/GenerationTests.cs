using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AndroidBinderator;
using Xunit;

namespace Xamarin.AndroidBinderator.Tests
{
	public class GenerationTests : BaseTest
	{
		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async Task ExternalsAreNotDownloadedOnlyWhenRequested(bool shouldDownload)
		{
			var externals = Path.Combine(RootDirectory, "externals");
			var generated = Path.Combine(RootDirectory, "generated");

			var config = new BindingConfig
			{
				DownloadExternals = shouldDownload,
				ExternalsDir = externals,
				BasePath = RootDirectory,
				Templates = { new TemplateConfig(CreateTemplate(), "generated/{artifactid}.csproj") },
				MavenArtifacts =
				{
					new MavenArtifactConfig
					{
						GroupId = "androidx.annotation",
						ArtifactId = "annotation",
						Version = "1.0.2",
						NugetPackageId = "Xamarin.AndroidX.Annotation",
					}
				},
				Licenses = { CreateLicense () }
			};

			await Engine.BinderateAsync(config);

			Assert.True(File.Exists(Path.Combine(generated, "annotation.csproj")));
			Assert.Equal(shouldDownload, Directory.Exists(externals));
			Assert.Equal(shouldDownload, File.Exists(Path.Combine(externals, "androidx.annotation", "annotation.jar")));
		}

		[Fact]
		public async Task TemplateIsCorrectlyGenerated()
		{
			var generated = Path.Combine(RootDirectory, "generated");

			var template = CreateTemplate(@"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFrameworks>monoandroid9.0</TargetFrameworks>
		<AssemblyName>@(Model.NuGetPackageId)</AssemblyName>
	</PropertyGroup>
	<PropertyGroup>
		<PackageId>@(Model.NuGetPackageId)</PackageId>
		<Title>Xamarin Android Support Library - @(Model.Name)</Title>
		<PackageVersion>@(Model.NuGetVersion)</PackageVersion>
	</PropertyGroup>
</Project>");

			const string expected = @"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFrameworks>monoandroid9.0</TargetFrameworks>
		<AssemblyName>Xamarin.AndroidX.Annotation</AssemblyName>
	</PropertyGroup>
	<PropertyGroup>
		<PackageId>Xamarin.AndroidX.Annotation</PackageId>
		<Title>Xamarin Android Support Library - annotation</Title>
		<PackageVersion>1.0.2</PackageVersion>
	</PropertyGroup>
</Project>";

			var config = new BindingConfig
			{
				DownloadExternals = false,
				BasePath = RootDirectory,
				Templates = { new TemplateConfig(template, "generated/{artifactid}.csproj") },
				MavenArtifacts =
				{
					new MavenArtifactConfig
					{
						GroupId = "androidx.annotation",
						ArtifactId = "annotation",
						Version = "1.0.2",
						NugetPackageId = "Xamarin.AndroidX.Annotation",
					}
				},
				Licenses = { CreateLicense () }
			};

			await Engine.BinderateAsync(config);

			var csproj = Path.Combine(generated, "annotation.csproj");

			Assert.True(File.Exists(csproj));
			Assert.Equal(expected, File.ReadAllText(csproj));
		}

		[Fact]
		public Task MetadataIsPassedAlong()
		{
			return ProcessAndAssertTemplate(@"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageVersion>@(Model.NuGetVersion)</PackageVersion>
		<RandomProperty>@(Model.Metadata[""More""])</RandomProperty>
	</PropertyGroup>
</Project>",
			new BindingConfig
			{
				DownloadExternals = false,
				MavenArtifacts =
				{
					new MavenArtifactConfig
					{
						GroupId = "androidx.annotation",
						ArtifactId = "annotation",
						Version = "1.0.2",
						NugetPackageId = "Xamarin.AndroidX.Annotation",
						NugetVersion = "1.0.2.1",
						Metadata = new Dictionary<string, string>
						{
							{ "More", "Yay!" }
						}
					}
				},
				Licenses = { CreateLicense () }
			}, @"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageVersion>1.0.2.1</PackageVersion>
		<RandomProperty>Yay!</RandomProperty>
	</PropertyGroup>
</Project>");
		}

		[Fact]
		public Task MetadataIsMergedBetweenConfigAndTemplateAndArtifact()
		{
			return ProcessAndAssertTemplate(@"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageVersion>@(Model.NuGetVersion)</PackageVersion>
		<RandomProperty>@(Model.Metadata[""More""])</RandomProperty>
		<RandomProperty>@(Model.Metadata[""Spare""])</RandomProperty>
		<RandomProperty>@(Model.Metadata[""Again""])</RandomProperty>
	</PropertyGroup>
</Project>",
			new BindingConfig
			{
				DownloadExternals = false,
				Metadata = new Dictionary<string, string>
				{
					{ "More", "Bad Value" },
					{ "Spare", "Keys" },
				},
				MavenArtifacts =
				{
					new MavenArtifactConfig
					{
						GroupId = "androidx.annotation",
						ArtifactId = "annotation",
						Version = "1.0.2",
						NugetPackageId = "Xamarin.AndroidX.Annotation",
						NugetVersion = "1.0.2.1",
						Metadata = new Dictionary<string, string>
						{
							{ "More", "Yay!" },
							{ "Again", "Good Value" },
						}
					}
				},
				Licenses = { CreateLicense () }
			}, @"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageVersion>1.0.2.1</PackageVersion>
		<RandomProperty>Yay!</RandomProperty>
		<RandomProperty>Change</RandomProperty>
		<RandomProperty>Good Value</RandomProperty>
	</PropertyGroup>
</Project>",
			new Dictionary<string, string>
			{
				{ "More", "Intermediate" },
				{ "Spare", "Change" },
			});
		}

		[Fact]
		public Task MetadataIsMergedBetweenConfigAndArtifact()
		{
			return ProcessAndAssertTemplate(@"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageVersion>@(Model.NuGetVersion)</PackageVersion>
		<RandomProperty>@(Model.Metadata[""More""])</RandomProperty>
		<RandomProperty>@(Model.Metadata[""Spare""])</RandomProperty>
		<RandomProperty>@(Model.Metadata[""Again""])</RandomProperty>
	</PropertyGroup>
</Project>",
			new BindingConfig
			{
				DownloadExternals = false,
				Metadata = new Dictionary<string, string>
				{
					{ "More", "Bad Value" },
					{ "Spare", "Keys" },
				},
				MavenArtifacts =
				{
					new MavenArtifactConfig
					{
						GroupId = "androidx.annotation",
						ArtifactId = "annotation",
						Version = "1.0.2",
						NugetPackageId = "Xamarin.AndroidX.Annotation",
						NugetVersion = "1.0.2.1",
						Metadata = new Dictionary<string, string>
						{
							{ "More", "Yay!" },
							{ "Again", "Good Value" },
						}
					}
				},
				Licenses = { CreateLicense () }
			}, @"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageVersion>1.0.2.1</PackageVersion>
		<RandomProperty>Yay!</RandomProperty>
		<RandomProperty>Keys</RandomProperty>
		<RandomProperty>Good Value</RandomProperty>
	</PropertyGroup>
</Project>");
		}

		[Fact]
		public Task NuGetVersionOverridesArtifactVersion()
		{
			return ProcessAndAssertTemplate(@"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageVersion>@(Model.NuGetVersion)</PackageVersion>
	</PropertyGroup>
</Project>",
			new BindingConfig
			{
				DownloadExternals = false,
				MavenArtifacts =
				{
					new MavenArtifactConfig
					{
						GroupId = "androidx.annotation",
						ArtifactId = "annotation",
						Version = "1.0.2",
						NugetPackageId = "Xamarin.AndroidX.Annotation",
						NugetVersion = "1.0.2.1",
					}
				},
				Licenses = { CreateLicense () }
			}, @"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageVersion>1.0.2.1</PackageVersion>
	</PropertyGroup>
</Project>");
		}

		[Fact]
		public Task NuGetVersionBaseCanAccessTheOriginalVersion()
		{
			return ProcessAndAssertTemplate(@"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageVersion>@(Model.NuGetVersionBase)</PackageVersion>
	</PropertyGroup>
</Project>",
			new BindingConfig
			{
				DownloadExternals = false,
				NugetVersionSuffix = "-preview",
				MavenArtifacts =
				{
					new MavenArtifactConfig
					{
						GroupId = "androidx.annotation",
						ArtifactId = "annotation",
						Version = "1.0.2",
						NugetPackageId = "Xamarin.AndroidX.Annotation",
					}
				},
				Licenses = { CreateLicense () }
			}, @"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageVersion>1.0.2</PackageVersion>
	</PropertyGroup>
</Project>");
		}

		[Fact]
		public Task NuGetVersionSuffixIsAppendedForImplicitVersions()
		{
			return ProcessAndAssertTemplate(@"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageVersion>@(Model.NuGetVersion)</PackageVersion>
	</PropertyGroup>
</Project>",
			new BindingConfig
			{
				DownloadExternals = false,
				NugetVersionSuffix = "-preview",
				MavenArtifacts =
				{
					new MavenArtifactConfig
					{
						GroupId = "androidx.annotation",
						ArtifactId = "annotation",
						Version = "1.0.2",
						NugetPackageId = "Xamarin.AndroidX.Annotation",
					}
				},
				Licenses = { CreateLicense () }
			}, @"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageVersion>1.0.2-preview</PackageVersion>
	</PropertyGroup>
</Project>");
		}

		[Fact]
		public Task NuGetVersionSuffixIsAppendedForOverrideVersions()
		{
			return ProcessAndAssertTemplate(@"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageVersion>@(Model.NuGetVersion)</PackageVersion>
	</PropertyGroup>
</Project>",
			new BindingConfig
			{
				DownloadExternals = false,
				NugetVersionSuffix = "-preview",
				MavenArtifacts =
				{
					new MavenArtifactConfig
					{
						GroupId = "androidx.annotation",
						ArtifactId = "annotation",
						Version = "1.0.2",
						NugetPackageId = "Xamarin.AndroidX.Annotation",
						NugetVersion = "1.0.2.1",
					}
				},
				Licenses = { CreateLicense () }
			}, @"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageVersion>1.0.2.1-preview</PackageVersion>
	</PropertyGroup>
</Project>");
		}

		[Fact]
		public Task NuGetVersionSuffixIsAppendedForDependencies()
		{
			return ProcessAndAssertTemplate(@"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageId>@(Model.NuGetPackageId)</PackageId>
		<PackageVersion>@(Model.NuGetVersion)</PackageVersion>
	</PropertyGroup>
	<ItemGroup>
	@foreach (var dep in @Model.NuGetDependencies) {
		<PackageReference Include=""@(dep.NuGetPackageId)"" Version=""@(dep.NuGetVersion)"" />
	}
	</ItemGroup>
</Project>",
			new BindingConfig
			{
				DownloadExternals = false,
				NugetVersionSuffix = "-preview",
				MavenArtifacts =
				{
					new MavenArtifactConfig
					{
						GroupId = "androidx.annotation",
						ArtifactId = "annotation",
						Version = "1.0.2",
						NugetPackageId = "Xamarin.AndroidX.Annotation",
					},
					new  MavenArtifactConfig
					{
						GroupId = "androidx.arch.core",
						ArtifactId = "core-common",
						Version = "2.0.1",
						NugetPackageId = "Xamarin.AndroidX.Arch.Core.Common",
					}
				},
				Licenses = { CreateLicense () }
			}, @"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageId>Xamarin.AndroidX.Arch.Core.Common</PackageId>
		<PackageVersion>2.0.1-preview</PackageVersion>
	</PropertyGroup>
	<ItemGroup>
		<PackageReference Include=""Xamarin.AndroidX.Annotation"" Version=""1.0.2-preview"" />
	</ItemGroup>
</Project>");
		}

		[Fact]
		public Task NuGetVersionSuffixIsNotAppendedForDependencyOnly()
		{
			return ProcessAndAssertTemplate(@"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageId>@(Model.NuGetPackageId)</PackageId>
		<PackageVersion>@(Model.NuGetVersion)</PackageVersion>
	</PropertyGroup>
	<ItemGroup>
	@foreach (var dep in @Model.NuGetDependencies) {
		<PackageReference Include=""@(dep.NuGetPackageId)"" Version=""@(dep.NuGetVersion)"" />
	}
	</ItemGroup>
</Project>",
			new BindingConfig
			{
				DownloadExternals = false,
				NugetVersionSuffix = "-preview",
				MavenArtifacts =
				{
					new MavenArtifactConfig
					{
						GroupId = "androidx.annotation",
						ArtifactId = "annotation",
						Version = "1.0.2",
						NugetPackageId = "Xamarin.AndroidX.Annotation",
						DependencyOnly = true,
					},
					new  MavenArtifactConfig
					{
						GroupId = "androidx.arch.core",
						ArtifactId = "core-common",
						Version = "2.0.1",
						NugetPackageId = "Xamarin.AndroidX.Arch.Core.Common",
					}
				},
				Licenses = { CreateLicense () }
			}, @"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageId>Xamarin.AndroidX.Arch.Core.Common</PackageId>
		<PackageVersion>2.0.1-preview</PackageVersion>
	</PropertyGroup>
	<ItemGroup>
		<PackageReference Include=""Xamarin.AndroidX.Annotation"" Version=""1.0.2"" />
	</ItemGroup>
</Project>");
		}

		[Fact]
		public Task MetadataIsAppendedToDependencies()
		{
			return ProcessAndAssertTemplate(@"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageId>@(Model.NuGetPackageId)</PackageId>
		<Metadata>@(Model.Metadata[""First""])</Metadata>
		<Metadata>@(Model.Metadata[""Second""])</Metadata>
		<Metadata>@(Model.Metadata[""Third""])</Metadata>
	</PropertyGroup>
	<ItemGroup>
	@foreach (var dep in @Model.NuGetDependencies) {
		<PackageReference Metadata1=""@(dep.Metadata[""First""])"" Metadata2=""@(dep.Metadata[""Second""])"" Metadata3=""@(dep.Metadata[""Third""])"" />
	}
	</ItemGroup>
</Project>",
			new BindingConfig
			{
				DownloadExternals = false,
				NugetVersionSuffix = "-preview",
				Metadata = new Dictionary<string, string>
				{
					{ "First", "One" },
					{ "Third", "Three" }
				},
				MavenArtifacts =
				{
					new MavenArtifactConfig
					{
						GroupId = "androidx.annotation",
						ArtifactId = "annotation",
						Version = "1.0.2",
						NugetPackageId = "Xamarin.AndroidX.Annotation",
						DependencyOnly = true,
						Metadata = new Dictionary<string, string>
						{
							{ "First", "wun" },
							{ "Second", "too" },
						},
					},
					new  MavenArtifactConfig
					{
						GroupId = "androidx.arch.core",
						ArtifactId = "core-common",
						Version = "2.0.1",
						NugetPackageId = "Xamarin.AndroidX.Arch.Core.Common",
						Metadata = new Dictionary<string, string>
						{
							{ "First", "1" },
							{ "Second", "2" },
						},
					}
				},
				Licenses = { CreateLicense () }
			}, @"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageId>Xamarin.AndroidX.Arch.Core.Common</PackageId>
		<Metadata>1</Metadata>
		<Metadata>2</Metadata>
		<Metadata>Three</Metadata>
	</PropertyGroup>
	<ItemGroup>
		<PackageReference Metadata1=""wun"" Metadata2=""too"" Metadata3=""Three"" />
	</ItemGroup>
</Project>");
		}

		async Task ProcessAndAssertTemplate(string input, BindingConfig config, string output, Dictionary<string, string> metadata = null)
		{
			var generated = Path.Combine(RootDirectory, "generated");
			var outputFile = Path.Combine(generated, "Generated.csproj");
			var templateFile = CreateTemplate(input);

			config.BasePath = RootDirectory;
			config.Templates.Add(new TemplateConfig(templateFile, outputFile) { Metadata = metadata ?? [] });

			await Engine.BinderateAsync(config);

			Assert.True(File.Exists(outputFile));

			var actual = File.ReadAllText(outputFile);
			Assert.Equal(output, actual);
		}

		LicenseConfig CreateLicense ()
		{
			var path = Path.Combine (RootDirectory, "licenses");
			Directory.CreateDirectory (path);
			path = Path.Combine (path, "LICENSE.txt");

			if (!File.Exists (path))
				File.WriteAllText (path, "Apache License 2.0 Example Text");

			return new LicenseConfig {
				Name = "The Apache Software License, Version 2.0",
				File = path
			};
		}

		[Fact]
		public async Task OkHttpRemappingWorksCorrectly()
		{
			var externals = Path.Combine(RootDirectory, "externals");
			var generated = Path.Combine(RootDirectory, "generated");

			// Simplified version of the actual Google Places POM with only the OkHttp dependency we care about
			// Original POM downloaded from: https://dl.google.com/dl/android/maven2/com/google/android/libraries/places/places/4.4.1/places-4.4.1.pom
			var pomContent = @"<?xml version='1.0' encoding='UTF-8'?>
<project xmlns=""http://maven.apache.org/POM/4.0.0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://maven.apache.org/POM/4.0.0 http://maven.apache.org/xsd/maven-4.0.0.xsd"">
  <modelVersion>4.0.0</modelVersion>
  <groupId>com.google.android.libraries.places</groupId>
  <artifactId>places</artifactId>
  <version>4.4.1</version>
  <packaging>aar</packaging>
  <dependencies>
    <dependency>
      <groupId>com.squareup.okhttp</groupId>
      <artifactId>okhttp</artifactId>
      <version>2.7.2</version>
      <scope>compile</scope>
      <type>jar</type>
    </dependency>
  </dependencies>
  <name>places</name>
  <licenses>
    <license>
      <name>Google Maps Platform Terms of Service</name>
      <url>https://cloud.google.com/maps-platform/terms/</url>
      <distribution>repo</distribution>
    </license>
  </licenses>
</project>";

			// Create the externals structure
			var placesDir = Path.Combine(externals, "com.google.android.libraries.places", "places");
			Directory.CreateDirectory(placesDir);
			File.WriteAllText(Path.Combine(placesDir, "places-4.4.1.pom"), pomContent);
			File.WriteAllText(Path.Combine(placesDir, "places-4.4.1.aar"), ""); // Empty aar file

			var config = new BindingConfig
			{
				DownloadExternals = false,
				ExternalsDir = externals,
				BasePath = RootDirectory,
				Templates = { new TemplateConfig(CreateTemplate(@"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<PackageId>@(Model.NuGetPackageId)</PackageId>
		<PackageVersion>@(Model.NuGetVersion)</PackageVersion>
	</PropertyGroup>
	<ItemGroup>
	@foreach (var dep in @Model.NuGetDependencies) {
		<PackageReference Include=""@(dep.NuGetPackageId)"" Version=""@(dep.NuGetVersion)"" />
	}
	</ItemGroup>
</Project>"), "generated/{artifactid}.csproj") },
				MavenArtifacts =
				{
					// The artifact we're testing (Google Places with OkHttp v2 dependency)
					new MavenArtifactConfig
					{
						GroupId = "com.google.android.libraries.places",
						ArtifactId = "places",
						Version = "4.4.1",
						NugetPackageId = "Xamarin.GoogleAndroid.Libraries.Places",
						NugetVersion = "4.4.1.1",
					},
					// The target artifact (OkHttp3) that the old OkHttp should be remapped to
					// Note: We intentionally do NOT include com.squareup.okhttp:okhttp in the config
					// This forces the remapping to be tested
					new MavenArtifactConfig
					{
						GroupId = "com.squareup.okhttp3",
						ArtifactId = "okhttp",
						Version = "4.12.0",
						NugetPackageId = "Square.OkHttp3",
						NugetVersion = "4.12.0.1",
						DependencyOnly = true
					},
					// Add missing dependencies for Google Places library test
					new MavenArtifactConfig
					{
						GroupId = "org.jetbrains.kotlinx",
						ArtifactId = "kotlinx-coroutines-play-services",
						Version = "1.8.0",
						NugetPackageId = "Xamarin.KotlinX.Coroutines.Play.Services",
						NugetVersion = "1.8.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "org.jetbrains.kotlinx",
						ArtifactId = "kotlinx-coroutines-android",
						Version = "1.8.0",
						NugetPackageId = "Xamarin.KotlinX.Coroutines.Android",
						NugetVersion = "1.8.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "org.jetbrains.kotlinx",
						ArtifactId = "kotlinx-coroutines-core",
						Version = "1.8.0",
						NugetPackageId = "Xamarin.KotlinX.Coroutines.Core",
						NugetVersion = "1.8.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "org.jetbrains.kotlin",
						ArtifactId = "kotlin-stdlib",
						Version = "2.1.0",
						NugetPackageId = "Xamarin.Kotlin.StdLib",
						NugetVersion = "2.1.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "com.google.auto.value",
						ArtifactId = "auto-value-annotations",
						Version = "1.6.2",
						NugetPackageId = "Xamarin.Google.AutoValue.Annotations",
						NugetVersion = "1.6.2.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "com.google.code.gson",
						ArtifactId = "gson",
						Version = "2.10",
						NugetPackageId = "Xamarin.Google.Gson",
						NugetVersion = "2.10.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "androidx.appcompat",
						ArtifactId = "appcompat",
						Version = "1.0.0",
						NugetPackageId = "Xamarin.AndroidX.AppCompat",
						NugetVersion = "1.0.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "androidx.cardview",
						ArtifactId = "cardview",
						Version = "1.0.0",
						NugetPackageId = "Xamarin.AndroidX.CardView",
						NugetVersion = "1.0.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "androidx.constraintlayout",
						ArtifactId = "constraintlayout",
						Version = "2.1.4",
						NugetPackageId = "Xamarin.AndroidX.ConstraintLayout",
						NugetVersion = "2.1.4.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "androidx.exifinterface",
						ArtifactId = "exifinterface",
						Version = "1.0.0",
						NugetPackageId = "Xamarin.AndroidX.ExifInterface",
						NugetVersion = "1.0.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "androidx.fragment",
						ArtifactId = "fragment",
						Version = "1.1.0",
						NugetPackageId = "Xamarin.AndroidX.Fragment",
						NugetVersion = "1.1.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "androidx.lifecycle",
						ArtifactId = "lifecycle-runtime-ktx",
						Version = "2.8.7",
						NugetPackageId = "Xamarin.AndroidX.Lifecycle.Runtime.Ktx",
						NugetVersion = "2.8.7.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "androidx.lifecycle",
						ArtifactId = "lifecycle-viewmodel",
						Version = "2.0.0",
						NugetPackageId = "Xamarin.AndroidX.Lifecycle.ViewModel",
						NugetVersion = "2.0.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "androidx.lifecycle",
						ArtifactId = "lifecycle-viewmodel-ktx",
						Version = "2.8.7",
						NugetPackageId = "Xamarin.AndroidX.Lifecycle.ViewModel.Ktx",
						NugetVersion = "2.8.7.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "androidx.recyclerview",
						ArtifactId = "recyclerview",
						Version = "1.3.2",
						NugetPackageId = "Xamarin.AndroidX.RecyclerView",
						NugetVersion = "1.3.2.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "androidx.viewpager2",
						ArtifactId = "viewpager2",
						Version = "1.1.0",
						NugetPackageId = "Xamarin.AndroidX.ViewPager2",
						NugetVersion = "1.1.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "com.android.volley",
						ArtifactId = "volley",
						Version = "1.2.1",
						NugetPackageId = "Xamarin.Android.Volley",
						NugetVersion = "1.2.1.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "com.github.bumptech.glide",
						ArtifactId = "glide",
						Version = "4.11.0",
						NugetPackageId = "Xamarin.Glide",
						NugetVersion = "4.11.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "com.google.android.datatransport",
						ArtifactId = "transport-api",
						Version = "3.1.0",
						NugetPackageId = "Xamarin.Google.Android.DataTransport.Api",
						NugetVersion = "3.1.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "com.google.android.datatransport",
						ArtifactId = "transport-backend-cct",
						Version = "3.2.0",
						NugetPackageId = "Xamarin.Google.Android.DataTransport.BackendCct",
						NugetVersion = "3.2.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "com.google.android.datatransport",
						ArtifactId = "transport-runtime",
						Version = "3.2.0",
						NugetPackageId = "Xamarin.Google.Android.DataTransport.Runtime",
						NugetVersion = "3.2.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "com.google.android.gms",
						ArtifactId = "play-services-base",
						Version = "18.5.0",
						NugetPackageId = "Xamarin.GooglePlayServices.Base",
						NugetVersion = "18.5.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "com.google.android.gms",
						ArtifactId = "play-services-basement",
						Version = "18.4.0",
						NugetPackageId = "Xamarin.GooglePlayServices.Basement",
						NugetVersion = "18.4.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "com.google.android.gms",
						ArtifactId = "play-services-location",
						Version = "21.0.1",
						NugetPackageId = "Xamarin.GooglePlayServices.Location",
						NugetVersion = "21.0.1.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "com.google.android.gms",
						ArtifactId = "play-services-maps",
						Version = "17.0.0",
						NugetPackageId = "Xamarin.GooglePlayServices.Maps",
						NugetVersion = "17.0.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "com.google.android.gms",
						ArtifactId = "play-services-tasks",
						Version = "18.2.0",
						NugetPackageId = "Xamarin.GooglePlayServices.Tasks",
						NugetVersion = "18.2.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "com.google.android.material",
						ArtifactId = "material",
						Version = "1.12.0",
						NugetPackageId = "Xamarin.Google.Android.Material",
						NugetVersion = "1.12.0.1",
						DependencyOnly = true
					},
					new MavenArtifactConfig
					{
						GroupId = "com.google.guava",
						ArtifactId = "guava",
						Version = "33.3.0-android",
						NugetPackageId = "Xamarin.Google.Guava",
						NugetVersion = "33.3.0.1",
						DependencyOnly = true
					}
				},
				Licenses = { 
					CreateLicense(), // Apache License for default testing
					CreateGoogleMapsLicense() // Google Maps Platform license for Places library
				}
			};

			// This should succeed without throwing an exception
			// If remapping doesn't work, it would throw because com.squareup.okhttp:okhttp is not in config
			await Engine.BinderateAsync(config);

			// Verify the project was generated
			var projectFile = Path.Combine(generated, "places.csproj");
			Assert.True(File.Exists(projectFile), "Project file should be generated when OkHttp remapping works");
			
			// Verify the project file contents have the correct remapped dependency
			var projectContent = File.ReadAllText(projectFile);
			Assert.Contains("<PackageReference Include=\"Square.OkHttp3\"", projectContent);
			Assert.DoesNotContain("<PackageReference Include=\"Square.OkHttp\"", projectContent);
		}

		LicenseConfig CreateGoogleMapsLicense ()
		{
			var path = Path.Combine (RootDirectory, "licenses");
			Directory.CreateDirectory (path);
			path = Path.Combine (path, "GOOGLE_MAPS_LICENSE.txt");

			if (!File.Exists (path))
				File.WriteAllText (path, "Google Maps Platform Terms of Service Example Text");

			return new LicenseConfig {
				Name = "Google Maps Platform Terms of Service",
				File = path
			};
		}
	}
}
