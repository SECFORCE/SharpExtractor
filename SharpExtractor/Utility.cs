// Copyright (c) Microsoft Corporation. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SharpExtractor
{
    public class Utility
    {
        public enum LogLevel
        {
            Info,
            Verbose,
            Debug,
            
        }

        public class Logger
        {
            public static LogLevel LogLevel { get; set; }

            public void Warn(string message, params object[] args)
            {
                System.Console.WriteLine(message, args);
            }

            public void Debug(string message, params object[] args)
            {
                if(LogLevel==LogLevel.Debug)
                    System.Console.WriteLine(message, args);
            }

            public void Debug(Exception ex, string message, params object[] args)
            {
                if (LogLevel == LogLevel.Debug)
                {
                    Console.WriteLine(message, args);
                    Console.WriteLine(ex.ToString());
                }
            }

            public void Trace(string message, params object[] args)
            {
                if (LogLevel >= LogLevel.Verbose)
                    Console.WriteLine(message, args);
            }

            public void Fatal(Exception ex, string message, params object[] args)
            {
                Console.WriteLine(message, args);
                Console.WriteLine(ex.ToString());
            }

            public void Info(string message, params object[] args)
            {
                Console.WriteLine(message, args);
            }

            public void ConditionalTrace(string message, params object[] args)
            {
                if(LogLevel >= LogLevel.Verbose)
                    Console.WriteLine(message, args);
            }
        }

        static Logger mLogger;

        public static Logger GetCurrentClassLogger()
        {
            if (mLogger == null)
                mLogger = new Logger();
            return mLogger;
        }

        public static string GetFSSafeName(string targetPath)
        {
            foreach (char c in Path.GetInvalidPathChars())
            {
                targetPath = targetPath.Replace(c, '_');
            }

            // remove because its not supported in path
            targetPath = targetPath.Replace(':', '_');
            return targetPath;
        }

        public static bool FileNamePasses(string fileName, IList<Regex> acceptFilters = null, IList<Regex> denyFilters = null)
        {
            foreach (var denyRegex in denyFilters ?? new Regex[] { })
            {
                if (denyRegex.IsMatch(fileName))
                {
                    return false;
                }
            }

            foreach (var allowRegex in acceptFilters ?? new Regex[] { })
            {
                if (allowRegex.IsMatch(fileName))
                {
                    return true;
                }
            }
            
            return ( acceptFilters == null || acceptFilters.Count == 0) ;
        }

        /// <summary>
        /// Makes sure that a new filename is returned
        /// </summary>
        /// <param name="targetDir"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFileSafeName(string targetDir, string fileName)
        {
            string outFilename = fileName;
            int count = 1;
            while (File.Exists(Path.Combine(targetDir, outFilename)))
            {
                outFilename = $"{Path.GetFileNameWithoutExtension(fileName)}_{count++}{Path.GetExtension(fileName)}";
            }
            return Path.Combine(targetDir, GetFSSafeName(outFilename));
        }
    }
}