using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using AndroidBinderator;

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

			var period_count = str.Count (c => c == '.');

			if (period_count == 2) {
				version = str;
				revision = 1;
			} else if (period_count == 3) {
				version = str.Substring (0, str.LastIndexOf ('.'));
				revision = int.Parse (str.Substring (str.LastIndexOf ('.') + 1));
				revision++;
			}

			art.NugetVersion = $"{version}.{revision}{release}";
		}

		config.Save (configFile);
	}
}
