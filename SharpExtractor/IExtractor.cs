using SharpExtractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpExtractor
{
	interface IExtractor
	{
		void ExtractToDirectory(string outputDirectory, FileEntry fileEntry, IList<Regex> acceptFilters = null, IList<Regex> denyFilters = null, bool printNames = false);
	}
}
