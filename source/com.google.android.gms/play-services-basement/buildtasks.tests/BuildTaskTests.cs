using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using NUnit.Framework;
using Xamarin.ContentPipeline.Tests;

namespace buildtasks.tests
{
	class BuildTaskTests : TestsBase
	{
		public void AddCoreTargets(ProjectRootElement el)
		{
			//var props = Path.Combine(
			//	Path.GetDirectoryName(GetType().Assembly.Location),
			//	"..", "..", "..", "buildtasks", "bin", "Debug", "Xamarin.GooglePlayServices.Basement.props"
			//);
			//el.AddImport(props);
			var targets = Path.Combine(
				Path.GetDirectoryName(GetType().Assembly.Location),
				"..", "..", "..", "buildtasks", "bin", "Debug", "Xamarin.GooglePlayServices.Basement.targets"
			);
			el.AddImport(targets);

		}

		[Test]
		public void Test_Skip_Due_To_Newer_Outputs_Than_Inputs()
		{
			var googleServicesJsonPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "google-services.json");

			var monoAndroidResDirIntermediate = Path.Combine(TempDir, "Debug");

			Directory.CreateDirectory(monoAndroidResDirIntermediate);

			File.WriteAllText(Path.Combine(monoAndroidResDirIntermediate, "ProcessGoogleServicesJson.stamp"), "STAMPS");


			var engine = new ProjectCollection();
			var prel = ProjectRootElement.Create(Path.Combine(TempDir, "project.csproj"), engine);

			Console.WriteLine("TempDir: {0}", TempDir);

			prel.AddProperty("AndroidApplication", "True");
			prel.AddProperty("IntermediateOutputPath", monoAndroidResDirIntermediate);
			prel.AddProperty("MonoAndroidResDirIntermediate", monoAndroidResDirIntermediate);
			prel.AddProperty("_AndroidPackage", "com.xamarin.sample");

			prel.AddItem("GoogleServicesJson", googleServicesJsonPath);

			AddCoreTargets(prel);

			var project = new ProjectInstance(prel);
			var log = new MSBuildTestLogger();

			var success = BuildProject(engine, project, "ProcessGoogleServicesJson", log);

			Assert.IsTrue(success);
			Assert.IsTrue(log.Events.Any(e => e.Message.Contains("ProcessGoogleServicesJson") && e.Message.Contains("skipped")));
		}

		[Test]
		public void Test_Inputs_Newer_Than_Outputs()
		{
			var googleServicesJsonPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "google-services.json");

			var monoAndroidResDirIntermediate = Path.Combine(TempDir, "Debug");

			Directory.CreateDirectory(monoAndroidResDirIntermediate);

			var engine = new ProjectCollection();
			var prel = ProjectRootElement.Create(Path.Combine(TempDir, "project.csproj"), engine);

			Console.WriteLine("TempDir: {0}", TempDir);

			prel.AddProperty("AndroidApplication", "True");
			prel.AddProperty("IntermediateOutputPath", monoAndroidResDirIntermediate);
			prel.AddProperty("MonoAndroidResDirIntermediate", monoAndroidResDirIntermediate);
			prel.AddProperty("_AndroidPackage", "com.xamarin.sample");

			prel.AddItem("GoogleServicesJson", googleServicesJsonPath);

			AddCoreTargets(prel);

			var project = new ProjectInstance(prel);
			var log = new MSBuildTestLogger();

			var success = BuildProject(engine, project, "ProcessGoogleServicesJson", log);

			Assert.IsTrue(success);
			Assert.IsFalse(log.Events.Any(e => e.Message.Contains("ProcessGoogleServicesJson") && e.Message.Contains("skipped")));
		}

		[Test]
		public void Test_Reruns_When_GoogleServicesJson_List_Changes()
		{
			// This test validates the fix for the ProcessGoogleServicesJson target being incorrectly skipped
			// when the GoogleServicesJson file list changes (e.g., switching between environments).
			// The target should rerun even if file timestamps haven't changed.

			var googleServicesJsonPath1 = Path.Combine(TempDir, "google-services-stage.json");
			var googleServicesJsonPath2 = Path.Combine(TempDir, "google-services-test.json");

			// Create two different google-services.json files with the same content structure but different values
			var json1 = @"{
  ""project_info"": {
    ""project_id"": ""stage-project""
  },
  ""client"": [
    {
      ""client_info"": {
        ""android_client_info"": {
          ""package_name"": ""com.xamarin.sample""
        }
      },
      ""api_key"": [
        {
          ""current_key"": ""STAGE_KEY""
        }
      ]
    }
  ]
}";

			var json2 = @"{
  ""project_info"": {
    ""project_id"": ""test-project""
  },
  ""client"": [
    {
      ""client_info"": {
        ""android_client_info"": {
          ""package_name"": ""com.xamarin.sample""
        }
      },
      ""api_key"": [
        {
          ""current_key"": ""TEST_KEY""
        }
      ]
    }
  ]
}";

			File.WriteAllText(googleServicesJsonPath1, json1);
			File.WriteAllText(googleServicesJsonPath2, json2);

			var monoAndroidResDirIntermediate = Path.Combine(TempDir, "Debug");
			Directory.CreateDirectory(monoAndroidResDirIntermediate);

			var engine = new ProjectCollection();
			var prel = ProjectRootElement.Create(Path.Combine(TempDir, "project.csproj"), engine);

			Console.WriteLine("TempDir: {0}", TempDir);

			prel.AddProperty("AndroidApplication", "True");
			prel.AddProperty("IntermediateOutputPath", monoAndroidResDirIntermediate + Path.DirectorySeparatorChar);
			prel.AddProperty("MonoAndroidResDirIntermediate", monoAndroidResDirIntermediate);
			prel.AddProperty("_AndroidPackage", "com.xamarin.sample");
			prel.AddProperty("MSBuildProjectFile", "project.csproj");

			// First build with stage file
			prel.AddItem("GoogleServicesJson", googleServicesJsonPath1);
			AddCoreTargets(prel);

			var project = new ProjectInstance(prel);
			var log = new MSBuildTestLogger();

			var success = BuildProject(engine, project, "ProcessGoogleServicesJson", log);
			Assert.IsTrue(success);
			Assert.IsFalse(log.Events.Any(e => e.Message.Contains("ProcessGoogleServicesJson") && e.Message.Contains("skipped")));

			// Verify the cache file was created
			var cacheFilePath = Path.Combine(monoAndroidResDirIntermediate, "project.csproj.GoogleServicesJson.cache");
			Assert.IsTrue(File.Exists(cacheFilePath), "Cache file should exist after first build");
			var firstHash = File.ReadAllText(cacheFilePath);

			// Now rebuild with the same file - should skip
			engine.UnloadAllProjects();
			engine = new ProjectCollection();
			prel = ProjectRootElement.Create(Path.Combine(TempDir, "project.csproj"), engine);
			prel.AddProperty("AndroidApplication", "True");
			prel.AddProperty("IntermediateOutputPath", monoAndroidResDirIntermediate + Path.DirectorySeparatorChar);
			prel.AddProperty("MonoAndroidResDirIntermediate", monoAndroidResDirIntermediate);
			prel.AddProperty("_AndroidPackage", "com.xamarin.sample");
			prel.AddProperty("MSBuildProjectFile", "project.csproj");
			prel.AddItem("GoogleServicesJson", googleServicesJsonPath1);
			AddCoreTargets(prel);

			project = new ProjectInstance(prel);
			log = new MSBuildTestLogger();

			success = BuildProject(engine, project, "ProcessGoogleServicesJson", log);
			Assert.IsTrue(success);
			Assert.IsTrue(log.Events.Any(e => e.Message.Contains("ProcessGoogleServicesJson") && e.Message.Contains("skipped")), 
				"Target should be skipped when inputs haven't changed");

			// Now rebuild with a different file - should NOT skip
			engine.UnloadAllProjects();
			engine = new ProjectCollection();
			prel = ProjectRootElement.Create(Path.Combine(TempDir, "project.csproj"), engine);
			prel.AddProperty("AndroidApplication", "True");
			prel.AddProperty("IntermediateOutputPath", monoAndroidResDirIntermediate + Path.DirectorySeparatorChar);
			prel.AddProperty("MonoAndroidResDirIntermediate", monoAndroidResDirIntermediate);
			prel.AddProperty("_AndroidPackage", "com.xamarin.sample");
			prel.AddProperty("MSBuildProjectFile", "project.csproj");
			prel.AddItem("GoogleServicesJson", googleServicesJsonPath2);  // Different file!
			AddCoreTargets(prel);

			project = new ProjectInstance(prel);
			log = new MSBuildTestLogger();

			success = BuildProject(engine, project, "ProcessGoogleServicesJson", log);
			Assert.IsTrue(success);
			Assert.IsFalse(log.Events.Any(e => e.Message.Contains("ProcessGoogleServicesJson") && e.Message.Contains("skipped")), 
				"Target should NOT be skipped when GoogleServicesJson list changes");

			// Verify the cache file has a different hash
			var secondHash = File.ReadAllText(cacheFilePath);
			Assert.AreNotEqual(firstHash, secondHash, "Hash should be different when input file changes");
		}
	}
}
