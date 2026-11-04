using System;
using System.CommandLine;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Xamarin.AndroidBinderator.Tool;

class Program
{
	static async Task<int> Main (params string [] args)
	{
		Trace.Listeners.Add (new TextWriterTraceListener (Console.Out));

		var root_command = new RootCommand ("Utilities for working with Android binding libraries.");

		root_command.AddCommand (BinderateCommand.Build ());
		root_command.AddCommand (UpdateCommand.Build ());
		root_command.AddCommand (BumpCommand.Build ());
		root_command.AddCommand (SortCommand.Build ());
		root_command.AddCommand (PublishedCommand.Build ());

		// If not using new command verbs, fallback to old legacy command line handling
		if (args.Any () && !root_command.Subcommands.Select (c => c.Name).Any (c => c == args [0])) {
			await LegacyCommandLine.Run (args);
			return 0;
		}

		return await root_command.InvokeAsync (args);
	}
}
