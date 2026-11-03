using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AndroidBinderator;
using Newtonsoft.Json;

namespace Xamarin.AndroidBinderator.Tool;

static class MirrorPlanCommand
{
	public static Command Build ()
	{
		var command = new Command ("mirror-plan", "Discovers a Maven mirror audit scope without changing the configuration.");
		var config = new Option<string> ("--config-file") { IsRequired = true };
		var baseline = new Option<string?> ("--base-config-file");
		var ciLog = new Option<string?> ("--ci-log-file");
		var output = new Option<string> ("--output") { IsRequired = true };
		command.Add (config);
		command.Add (baseline);
		command.Add (ciLog);
		command.Add (output);
		command.SetHandler (async (InvocationContext context) => {
			var configFile = context.ParseResult.GetValueForOption (config)!;
			var baseFile = context.ParseResult.GetValueForOption (baseline);
			var logFile = context.ParseResult.GetValueForOption (ciLog);
			var outputFile = context.ParseResult.GetValueForOption (output)!;
			var head = await BindingConfig.Load (configFile);
			var errors = new List<string> ();
			var latest = new Dictionary<string, string> ();
			try {
				latest = await ConfigUpdater.DiscoverMirrorCandidates (head);
			} catch (Exception ex) {
				errors.Add ($"Latest stable discovery incomplete: {ex.Message}");
			}
			using var client = new HttpClient { Timeout = TimeSpan.FromSeconds (60) };
			var plan = await MavenMirrorPlan.Create (head, baseFile is null ? null : await BindingConfig.Load (baseFile),
				latest, logFile is null ? "" : File.ReadAllText (logFile), client);
			plan.Errors.AddRange (errors);
			File.WriteAllText (outputFile, JsonConvert.SerializeObject (plan, Formatting.Indented));
			context.ExitCode = plan.Complete ? 0 : 1;
		});
		return command;
	}
}
