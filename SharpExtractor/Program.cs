using Fclp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpExtractor
{
	class Program
	{
		class Args
		{
			public Extractors Extractor { get; set; }
			public string File { get; set; }
			public IList<string> AllowFilters { get; set; }
			public IList<string> DenyFilters { get; set; }
			public string OutDirectory { get; set;}
			public bool IgnoreFilters { get; set; }
		}

		static void Main(string[] args)
		{
			try
			{
				// handle args
				var pArgs = HandleArgs(args);

				// get regexes
				var allowRegexes = pArgs.AllowFilters != null ? pArgs.AllowFilters.Select(x => new Regex(x)).ToList() : null;
				var denyRegexes = pArgs.DenyFilters != null ? pArgs.DenyFilters.Select(x => new Regex(x)).ToList() : null;

				// initialize file systems, containers and whatever
				DiscUtils.Complete.SetupHelper.SetupComplete();

				// extract the files
				var extractor = new Extractor(pArgs.Extractor);
				extractor.Extract(pArgs.File, pArgs.OutDirectory, allowRegexes, denyRegexes );
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			//Console.ReadLine();
		}

		private static Args HandleArgs(string[] args)
		{
			var pArgs = new Args();
			var p = new FluentCommandLineParser();

			p.SetupHelp("h", "help")
				.Callback(text => Console.WriteLine(text)); 

			p.Setup<string>('f', "file")
			   .Callback(arg => pArgs.File = arg)
			   .WithDescription("\tThe file to process")
			   .Required();

			p.Setup<string>('o', "output-dir")
			   .Callback(arg => pArgs.OutDirectory = arg)
			   .WithDescription("Output directory")
			   .Required();

			p.Setup<Extractors>('e', "extractor")
			   .Callback(arg => pArgs.Extractor = arg)
			   .WithDescription("The extractor to use (vhxd)")
			   .Required();

			p.Setup<List<string>>('a', "allow-filter")
			   .WithDescription("Optional regex allow filter")
			   .Callback(arg => pArgs.AllowFilters = arg);

			p.Setup<List<string>>('d',"deny-filter")
			   .WithDescription("Optional regex deny filter")
			   .Callback(arg => pArgs.DenyFilters = arg );

			p.Setup<bool>('i', "ignore-filters")
				.WithDescription("\bForces extraction when no filters are specfied")
				.Callback(arg => pArgs.IgnoreFilters = arg);

			var result = p.Parse(args);

			if (result.HasErrors || result.HelpCalled)
			{
				if(!result.HelpCalled)
					p.HelpOption.ShowHelp(p.Options);

				Environment.Exit(0);
			}

			if (pArgs.AllowFilters == null && pArgs.DenyFilters == null && !pArgs.IgnoreFilters)
			{
				p.HelpOption.ShowHelp(p.Options);
				Console.WriteLine("No filters were specfied. If its the intended scenario, use -i flag to ignore this warning.");
				Environment.Exit(0);
			}

				return pArgs;
		}
	}
}
