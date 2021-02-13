using DiscUtils;
using DiscUtils.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpExtractor
{
	class VhxdExtractor : IExtractor
	{
        private static readonly Utility.Logger Logger = Utility.GetCurrentClassLogger();

        /// <summary>
        ///     Extracts an a VHDX file
        /// </summary>
        /// <param name="fileEntry"> </param>
        /// <returns> </returns>
        public IEnumerable<FileEntry> Extract(FileEntry fileEntry, IList<Regex> allow, IList<Regex> deny)
        {
            using (var disk = new DiscUtils.Vhdx.Disk(fileEntry.Content, Ownership.None))
            {
                LogicalVolumeInfo[] logicalVolumes = null;

                try
                {
                    var manager = new VolumeManager(disk);
                    logicalVolumes = manager.GetLogicalVolumes();
                }
                catch (Exception e)
                {
                    Logger.Debug("Error reading {0} disk at {1} ({2}:{3})", disk.GetType(), fileEntry.FullPath, e.GetType(), e.Message);
                }

                if (logicalVolumes != null)
                {
                    foreach (var volume in logicalVolumes)
                    {
                        var fsInfos = FileSystemManager.DetectFileSystems(volume);

                        foreach (var entry in DumpLogicalVolume(volume, fileEntry.FullPath, fileEntry))
                        {
                            if (Utility.FileNamePasses(entry.Name, allow, deny))
                                yield return entry;
                        }
                    }
                }
            }
        }

        public void ExtractToDirectory(string outputDirectory, FileEntry fileEntry, IList<Regex> acceptFilters = null, IList<Regex> denyFilters = null, bool printNames = false)
        {
            foreach (var entry in Extract(fileEntry, acceptFilters, denyFilters))
            {
                if (Utility.FileNamePasses(entry.FullPath, acceptFilters, denyFilters))
                {
                    var targetPath = Path.Combine(outputDirectory, Utility.GetFSSafeName(entry.FullPath));
                    try
                    {
                        targetPath = Utility.GetFileSafeName(outputDirectory, entry.Name);

                        using (var fs = new FileStream(targetPath, FileMode.Create))
                        {
                            entry.Content.CopyTo(fs);
                            if (printNames)
                            {
                                Console.WriteLine("Extracted {0} as {1}.", entry.FullPath , targetPath);
                            }
                            Logger.Trace("Extracted {0}", entry.FullPath);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Fatal(e, "Failed to create file at {0}.", targetPath);
                    }
                }
            }
        }


        public IEnumerable<FileEntry> DumpLogicalVolume(LogicalVolumeInfo volume, string parentPath, FileEntry parent = null)
        {
            DiscUtils.FileSystemInfo[] fsInfos = null;
            try
            {
                fsInfos = FileSystemManager.DetectFileSystems(volume);
            }
            catch (Exception e)
            {
                Logger.Debug("Failed to get file systems from logical volume {0} Image {1} ({2}:{3})", volume.Identity, parentPath, e.GetType(), e.Message);
            }

            foreach (var fsInfo in fsInfos ?? new DiscUtils.FileSystemInfo[] { })
            {
                using (var fs = fsInfo.Open(volume))
                {
                    var diskFiles = fs.GetFiles(fs.Root.FullName, "*.*", SearchOption.AllDirectories).ToList();

                    foreach (var file in diskFiles)
                    {
                        Stream fileStream = null;
                        DiscFileInfo fi = null;
                        try
                        {
                            fi = fs.GetFileInfo(file);
                            fileStream = fi.OpenRead();
                        }
                        catch (Exception e)
                        {
                            Logger.Debug(e, "Failed to open {0} in volume {1}", file, volume.Identity);
                        }
                        if (fileStream != null && fi != null)
                        {
                            yield return FileEntry.FromStream($"{volume.Identity}{Path.DirectorySeparatorChar}{fi.FullName}", fileStream, parent);
                        }
                    }
                }
            }
        }
    }
}
