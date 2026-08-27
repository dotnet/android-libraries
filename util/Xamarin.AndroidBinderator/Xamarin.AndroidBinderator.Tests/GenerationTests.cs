using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AndroidBinderator;
using Java.Interop.Tools.Maven.Models;
using Xunit;

namespace Xamarin.AndroidBinderator.Tests
{
	public class GenerationTests : BaseTest
	{
		[Theory]
		[InlineData (false, MavenRepoType.MavenCentral, "", MavenRepoType.MavenCentral, "")]
		[InlineData (true, MavenRepoType.MavenCentral, "", MavenRepoType.Url, MavenRepositoryResolver.DotNetPublicMaven)]
		[InlineData (true, MavenRepoType.Google, "", MavenRepoType.Google, "")]
		[InlineData (true, MavenRepoType.Url, "https://example.com/maven", MavenRepoType.Url, "https://example.com/maven")]
		public void MavenCentralUsesMirrorOnlyOnCI (
			bool runningOnCI,
			MavenRepoType type,
			string location,
			MavenRepoType expectedType,
			string expectedLocation)
		{
			var actual = MavenRepositoryResolver.Resolve (type, location, runningOnCI);

			Assert.Equal (expectedType, actual.type);
			Assert.Equal (expectedLocation, actual.location);
		}

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
		public async Task DependencyOnlyArtifactCanGeneratePackageInUniqueDirectory()
		{
			var generated = Path.Combine(RootDirectory, "generated");
			var expected = Path.Combine(generated, "androidx.annotation.annotation-1.0.2", "androidx.annotation.annotation-1.0.2.csproj");

			var config = new BindingConfig
			{
				DownloadExternals = false,
				BasePath = RootDirectory,
				Templates = { new TemplateConfig(CreateTemplate(), "generated/{groupid}.{artifactid}/{groupid}.{artifactid}.csproj") },
				MavenArtifacts =
				{
					new MavenArtifactConfig
					{
						GroupId = "androidx.annotation",
						ArtifactId = "annotation",
						Version = "1.0.2",
						NugetPackageId = "Xamarin.AndroidX.Annotation",
						DependencyOnly = true,
						GeneratePackage = true,
						GeneratedArtifactId = "annotation-1.0.2",
					},
					new MavenArtifactConfig
					{
						GroupId = "androidx.arch.core",
						ArtifactId = "core-common",
						Version = "2.0.1",
						NugetPackageId = "Xamarin.AndroidX.Arch.Core.Common",
						DependencyOnly = true,
					}
				},
				Licenses = { CreateLicense () }
			};

			await Engine.BinderateAsync(config);

			Assert.True(File.Exists(expected));
			Assert.False(File.Exists(Path.Combine(generated, "androidx.arch.core.core-common", "androidx.arch.core.core-common.csproj")));
		}

		[Fact]
		public void GeneratedDependencyPackageIsProjectReference()
		{
			var dependency = new MavenArtifactConfig
			{
				GroupId = "androidx.annotation",
				ArtifactId = "annotation",
				Version = "1.0.0",
				NugetPackageId = "Xamarin.AndroidX.Annotation",
				NugetVersion = "1.0.0.1",
				DependencyOnly = true,
				GeneratePackage = true,
			};
			var config = new BindingConfig
			{
				MavenArtifacts = { dependency },
			};
			var project = new BindingProjectModel
			{
				MavenGroupId = "androidx.example",
				MavenArtifacts =
				{
					new MavenArtifactModel
					{
						MavenArtifactId = "example",
						MavenArtifactVersion = "1.0.0",
					},
				},
				MavenDependencies =
				{
					new Dependency
					{
						GroupId = dependency.GroupId,
						ArtifactId = dependency.ArtifactId,
						Version = dependency.Version,
					},
				},
			};

			BindingProjectDependencyVerifier.Verify(config, [project]);

			Assert.True(Assert.Single(project.NuGetDependencies).IsProjectReference);
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
	}
}
