
using DiscUtils;
using DiscUtils.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpExtractor
{
    /// <summary>
    /// The VHDX Extractor Implementation
    /// </summary>
    public class Extractor 
    {
        private IExtractor mExtractor;
        private static readonly Utility.Logger Logger = Utility.GetCurrentClassLogger();

        /// <summary>
        /// The constructor takes the Extractor context for recursion.
        /// </summary>
        /// <param name="context">The Extractor context.</param>
        public Extractor(Extractors type)
        {
            // poor man's type resolving
			switch (type)
			{
                case Extractors.vhdx:
                    mExtractor = new SharpExtractor.VhxdExtractor();
                    break;
                default:
                    throw new NotImplementedException($"{type} extractor not implemented");
                    break;
			}
		}

        public void Extract(
            string vhdx,
            string outputDirectory,
            IList<Regex> allowFilters = null,
            IList<Regex> denyFilters = null
            )
		{
            FileEntry fileEntry = null;

            if (!File.Exists(vhdx))
            {
                Logger.Warn("ExtractFile called, but {0} does not exist.", vhdx);
                return;
            }

            using (var fs = new FileStream(vhdx, FileMode.Open))
            {
                try
                {
                    fileEntry = new FileEntry(Path.GetFileName(vhdx), fs);
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex, "Failed to extract file {0}", vhdx);
                }

                mExtractor.ExtractToDirectory(
                    outputDirectory,
                    fileEntry,
                    allowFilters,
                    denyFilters,
                    true
                    );
            }
        }
    }
}
