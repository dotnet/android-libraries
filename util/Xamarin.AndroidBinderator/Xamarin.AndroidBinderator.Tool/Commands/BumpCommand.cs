using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AndroidBinderator;
using Newtonsoft.Json;
using NuGet.Versioning;

namespace Xamarin.AndroidBinderator.Tool;

static class BumpCommand
{
	public static Command Build ()
	{
		var update_command = new Command ("bump", "Increments the NuGet patch version of all packages in a config.json file.");

		var config_file_option = new Option<string> (["--config-file", "-c", "-config="], "JSON config file") { IsRequired = true };

		update_command.Add (config_file_option);

		update_command.SetHandler (
			RunBumpCommand,
			config_file_option
		);

		return update_command;
	}

	static async Task RunBumpCommand (string configFile)
	{
		var config = await BindingConfig.Load (configFile);

		foreach (var art in config.MavenArtifacts) {
			if (art.DependencyOnly)
				continue;

			var version = "";
			var release = "";
			var revision = 0;

			var str = art.NugetVersion ?? string.Empty;

			if (str.Contains ('-')) {
				release = str.Substring (str.IndexOf ('-'));
				str = str.Substring (0, str.LastIndexOf ('-'));
			}

			Console.WriteLine($" Artifact {art.ArtifactId}");
			Console.WriteLine($"	Version {art.Version}");
			Console.WriteLine($"	DependencyOnly {art.DependencyOnly}");

			var period_count = str.Count (c => c == '.');

			if (period_count == 2) {
				version = str;
				revision = 1;
			} else if (period_count == 3) {
				version = str.Substring (0, str.LastIndexOf ('.'));
				revision = int.Parse (str.Substring (str.LastIndexOf ('.') + 1));

				if (art.DependencyOnly == false) 
				{
					revision++;
					art.NugetVersion = $"{version}.{revision}{release}";
				} 
				else 
				{
					string package = art.NugetPackageId.ToLower ();
					string url = $"https://api.nuget.org/v3-flatcontainer/{package}/index.json";
					HttpClient? client = new HttpClient ();
					Package p;
					try 
					{
						HttpResponseMessage result = await client.GetAsync (url);
						string response_content = await result.Content.ReadAsStringAsync ();
						p = JsonConvert.DeserializeObject<Package>(response_content);
					} 
					catch (Exception e) 
					{
						Console.WriteLine (e);
						Console.WriteLine ($"url = {url}");
						Console.WriteLine ($"package = {package}");
						
						throw;
					}
					NuGetVersion latest = p.NugetVersions.FirstOrDefault ();
					art.NugetVersion = latest.ToString ();
					
					Console.WriteLine ($"		url = {url}");
				}
			}
			Console.WriteLine($"	NugetVersion {art.NugetVersion}");
		}

		config.Save (configFile);
	}
}

internal class Package
{
	public List<string> versions { get; set; }

	public List<NuGetVersion> NugetVersions
	{
		get 
		{
			return versions.Select (v => new NuGetVersion (v))
					.OrderByDescending( v => v, VersionComparer.VersionReleaseMetadata)
					.ToList();
		}
	}
}

